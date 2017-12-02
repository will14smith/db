using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Functions;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Nodes;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution
{
    public class FunctionExecutor
    {
        private readonly Function _function;
        private readonly IReadOnlyList<Value> _arguments;

        private readonly IPager _pager;
        private readonly Program _program;

        private readonly IReadOnlyDictionary<ProgramLabel, int> _labelAddresses;

        public FunctionExecutor(Function function, IReadOnlyList<Value> arguments, IPager pager, Program program)
        {
            _function = function;
            _arguments = arguments;
            _pager = pager;
            _program = program;

            _labelAddresses = function.Operations.Select((x, i) => (x, i)).Where(x => x.Item1 is ProgramLabel).ToDictionary(x => (ProgramLabel)x.Item1, x => x.Item2);
        }

        public IEnumerable<Value> Execute()
        {
            var state = new FunctionState(_function.Slots);

            while (true)
            {
                Value value;
                (state, value) = ExecuteStep(state);

                if (value == null)
                {
                    yield break;
                }

                yield return value;
            }
        }

        public (FunctionState, Value) ExecuteStep(FunctionState state)
        {
            while (true)
            {
                var operation = GetOperation(state);

                var (nextState, result) = Execute(state, operation);

                switch (result is Result.Yield y ? y.Inner : result)
                {
                    case Result.Next _:
                        state = nextState.SetPC(nextState.GetPC() + 1);
                        break;

                    case Result.Jump jump:
                        state = nextState.SetPC(_labelAddresses[jump.Address]);
                        break;

                    case Result.Finished _:
                        // TODO cleanup open resources
                        return (state, null);

                    default:
                        throw new NotImplementedException($"Unsupported result type: {result.GetType().Name}");
                }

                if (result is Result.Yield yield)
                {
                    return (state, yield.Value);
                }
            }
        }

        private IOperation GetOperation(FunctionState state)
        {
            var pc = state.GetPC();
            if (pc >= _function.Operations.Count)
            {
                return new FinishOperation();
            }

            return _function.Operations[pc];
        }

        private (FunctionState, Result) Execute(FunctionState state, IOperation operation)
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

                case OpenWriteOperation openWrite:
                    {
                        // TODO aquire write lock
                        var cursor = new CursorValue(openWrite.Table, false);
                        state = state.PushValue(cursor);
                        return (state, new Result.Next());
                    }

                case FirstOperation first:
                    {
                        CursorValue cursorValue;
                        (state, cursorValue) = state.PopValue<CursorValue>();

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
                        (state, cursorValue) = state.PopValue<CursorValue>();

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

                case InsertOperation _:
                    {
                        RowValue row;
                        CursorValue cursorValue;

                        (state, cursorValue) = state.PopValue<CursorValue>();
                        (state, row) = state.PopValue<RowValue>();

                        var insertableRow = new Row(row.Values.Cast<ObjectValue>().Select(x => new ColumnValue(x.Value)).ToList());

                        var inserter = new TreeInserter(_pager, CreateRowSerializer(cursorValue.Table), cursorValue.Table);
                        var insertResult = inserter.Insert((int) insertableRow.Values[0].Value, insertableRow);
                        switch (insertResult)
                        {
                            case TreeInsertResult.Success _:
                                return (state, new Result.Next());

                            case TreeInsertResult.DuplicateKey _:
                                throw new NotImplementedException("TODO error handling!");

                            default:
                                throw new NotImplementedException($"Unsupported type: {insertResult.GetType().Name}");
                        }
                    }

                // Columns
                case KeyOperation _:
                    {
                        CursorValue cursorValue;
                        (state, cursorValue) = state.PopValue<CursorValue>();

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
                        (state, cursorValue) = state.PopValue<CursorValue>();

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

                // Functions
                case ReturnOperation ret:
                    {
                        Value value;
                        (state, value) = state.PopValue();

                        return (state, new Result.Yield(new Result.Next(), value));
                    }

                case SetupCoroutineOperation setup:
                    {
                        var args = new Value[setup.ArgumentCount];

                        for (var i = setup.ArgumentCount - 1; i >= 0; i--)
                        {
                            (state, args[i]) = state.PopValue();
                        }

                        var function = _program.Functions[setup.Function];

                        var handle = new CoroutineHandle(setup.Function, args, new FunctionState(function.Slots));

                        state = state.PushValue(handle);

                        return (state, new Result.Next());
                    }
                case CallCoroutineOperation call:
                    {
                        CoroutineHandle handle;
                        (state, handle) = state.PopValue<CoroutineHandle>();

                        var function = _program.Functions[handle.Function];

                        var coroutineExecutor = new FunctionExecutor(function, handle.Args, _pager, _program);
                        var (coroutineState, coroutineValue) = coroutineExecutor.ExecuteStep(handle.State);

                        handle.SetState(coroutineState);

                        if (coroutineValue == null)
                        {
                            return (state, new Result.Jump(call.Done));
                        }

                        state = state.PushValue(coroutineValue);

                        return (state, new Result.Next());
                    }

                // Other
                case ProgramLabel _:
                    return (state, new Result.Next());

                case MakeRowOperation makeRow:
                    {
                        if (state.StackCount < makeRow.ColumnCount)
                        {
                            throw new InvalidOperationException("Stack doesn't contain enough elements for this row");
                        }

                        var row = new Value[makeRow.ColumnCount];

                        for (var i = makeRow.ColumnCount - 1; i >= 0; i--)
                        {
                            (state, row[i]) = state.PopValue();
                        }

                        state = state.PushValue(new RowValue(row));

                        return (state, new Result.Next());
                    }

                case YieldOperation _:
                    {
                        Value value;
                        (state, value) = state.PopValue();

                        return (state, new Result.Yield(new Result.Next(), value));
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
                public Value Value { get; }

                public Yield(Result inner, Value value)
                {
                    if (inner is Yield)
                    {
                        throw new InvalidOperationException("Cannot have recursive yields");
                    }

                    Inner = inner;
                    Value = value;
                }
            }
        }
    }
}