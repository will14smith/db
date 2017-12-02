using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Nodes;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution
{
    public class ProgramExecutor
    {
        private readonly Program _program;
        private readonly IPager _pager;

        private readonly IReadOnlyDictionary<ProgramLabel, int> _labelAddresses;

        public ProgramExecutor(Program program, IPager pager)
        {
            _program = program;
            _pager = pager;

            _labelAddresses = program.Operations.Select((x, i) => (x, i)).Where(x => x.Item1 is ProgramLabel).ToDictionary(x => (ProgramLabel)x.Item1, x => x.Item2);
        }

        public IEnumerable<IReadOnlyCollection<Value>> Execute()
        {
            var state = new ProgramState(_program.Slots);

            while (true)
            {
                var operation = GetOperation(state);

                var (nextState, result) = Execute(state, operation);

                if (result is Result.Yield yield)
                {
                    yield return yield.Row;

                    result = yield.Inner;
                }

                switch (result)
                {
                    case Result.Next _:
                        state = nextState.SetPC(nextState.GetPC() + 1);
                        break;

                    case Result.Jump jump:
                        state = nextState.SetPC(_labelAddresses[jump.Address]);
                        break;

                    case Result.Finished _:
                        // TODO cleanup open resources
                        yield break;

                    default:
                        throw new NotImplementedException($"Unsupported result type: {result.GetType().Name}");
                }
            }
        }

        private IOperation GetOperation(ProgramState state)
        {
            var pc = state.GetPC();
            if (pc >= _program.Operations.Count)
            {
                return new FinishOperation();
            }

            return _program.Operations[pc];
        }

        private (ProgramState, Result) Execute(ProgramState state, IOperation operation)
        {
            switch (operation)
            {
                // Cursor
                case OpenReadOperation openRead:
                    {
                        // TODO aquire read lock
                        var cursor = new CursorValue(openRead.Table, false);
                        state = state.PushValue(cursor);
                        return (state, new Result.Next());
                    }

                case FirstOperation first:
                    {
                        CursorValue cursorValue;
                        (state, cursorValue) = state.PopCursor();

                        var searcher = new TreeTraverser(_pager, CreateRowSerializer(cursorValue.Table), cursorValue.Table);
                        var newCursor = searcher.StartCursor();

                        var newCursorValue = cursorValue.SetCursor(newCursor);
                        state = state.PushValue(newCursorValue);

                        if (!newCursor.EndOfTable)
                        {
                            return (state, new Result.Next());
                        }

                        return (state, new Result.Jump(first.EmptyAddress));
                    }

                case NextOperation next:
                    {
                        CursorValue cursorValue;
                        (state, cursorValue) = state.PopCursor();

                        var cursor = cursorValue.Cursor;
                        if (cursor == null)
                        {
                            throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?");
                        }

                        var searcher = new TreeTraverser(_pager, CreateRowSerializer(cursorValue.Table), cursorValue.Table);
                        var newCursor = searcher.AdvanceCursor(cursor);

                        var newCursorValue = cursorValue.SetCursor(newCursor);
                        state = state.PushValue(newCursorValue);

                        if (newCursor.EndOfTable)
                        {
                            return (state, new Result.Next());
                        }

                        return (state, new Result.Jump(next.SuccessAddress));
                    }

                // Columns
                case KeyOperation _:
                    {
                        CursorValue cursorValue;
                        (state, cursorValue) = state.PopCursor();

                        var cursor = cursorValue.Cursor;
                        if (cursor == null)
                        {
                            throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?");
                        }

                        var page = _pager.Get(cursor.PageNumber);
                        var leaf = LeafNode.Read(CreateRowSerializer(cursorValue.Table), page);

                        var key = leaf.GetCellKey(cursor.CellNumber);
                        state.PushValue(new ObjectValue(key));

                        return (state, new Result.Next());
                    }

                case ColumnOperation column:
                    {
                        CursorValue cursorValue;
                        (state, cursorValue) = state.PopCursor();

                        var cursor = cursorValue.Cursor;
                        if (cursor == null)
                        {
                            throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?");
                        }

                        var page = _pager.Get(cursor.PageNumber);
                        var leaf = LeafNode.Read(CreateRowSerializer(cursorValue.Table), page);

                        var value = leaf.GetCellColumn(cursor.CellNumber, column.ColumnIndex);
                        state.PushValue(new ObjectValue(value));

                        return (state, new Result.Next());
                    }

                // Slots
                case LoadOperation load:
                    {
                        var value = state.GetSlot(load.Slot);

                        state = state.PushValue(value);

                        return (state, new Result.Next());
                    }

                case StoreOperation store:
                    {
                        Value value;
                        (state, value) = state.PopValue();

                        state = state.SetSlot(store.Slot, value);

                        return (state, new Result.Next());
                    }

                // Jumps
                case ConditionalJumpOperation conditionalJump:
                    {
                        Value value1, value2;
                        (state, value1) = state.PopValue();
                        (state, value2) = state.PopValue();

                        if (Compare(conditionalJump.Comparison, value1, value2))
                        {
                            return (state, new Result.Jump(conditionalJump.Address));
                        }

                        return (state, new Result.Next());
                    }
                case JumpOperation jump:
                    {
                        return (state, new Result.Jump(jump.Address));
                    }

                // Constants
                case ConstIntOperation constInt:
                    {
                        state = state.PushValue(new ObjectValue(constInt.Value));

                        return (state, new Result.Next());
                    }
                case ConstStringOperation constStr:
                    {
                        state = state.PushValue(new ObjectValue(constStr.Value));

                        return (state, new Result.Next());
                    }

                // Other
                case ProgramLabel _:
                    return (state, new Result.Next());

                case YieldRowOperation yield:
                    {
                        if (state.StackCount < yield.ColumnCount)
                        {
                            throw new InvalidOperationException("Stack doesn't contain enough elements for this row");
                        }

                        var row = new Value[yield.ColumnCount];

                        for (var i = yield.ColumnCount - 1; i >= 0; i--)
                        {
                            (state, row[i]) = state.PopValue();
                        }

                        return (state, new Result.Yield(new Result.Next(), row));
                    }

                case FinishOperation _:
                    return (state, new Result.Finished());

                default:
                    throw new NotImplementedException($"Unsupported operation type: {operation.GetType().Name}");
            }
        }

        private IRowSerializer CreateRowSerializer(StoredTable table)
        {
            return new RowSerializer(
                table.Table,
                new ColumnTypeSerializerFactory()
            );
        }

        private bool Compare(Comparison comparison, Value value1, Value value2)
        {
            if (!(value1 is ObjectValue o1) || !(value2 is ObjectValue o2))
                throw new NotImplementedException();

            switch (comparison)
            {
                case Comparison.Equal:
                    return Equals(o1.Value, o2.Value);
                case Comparison.NotEqual:
                    return !Equals(o1.Value, o2.Value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
            }
        }

        private abstract class Result
        {
            public class Next : Result { }
            public class Jump : Result
            {
                public ProgramLabel Address { get; }

                public Jump(ProgramLabel address)
                {
                    Address = address;
                }
            }
            public class Finished : Result { }

            public class Yield : Result
            {
                public Result Inner { get; }
                public IReadOnlyCollection<Value> Row { get; }

                public Yield(Result inner, IReadOnlyCollection<Value> row)
                {
                    if (inner is Yield)
                    {
                        throw new InvalidOperationException("Cannot have recursive yields");
                    }

                    Inner = inner;
                    Row = row;
                }
            }
        }
    }
}
