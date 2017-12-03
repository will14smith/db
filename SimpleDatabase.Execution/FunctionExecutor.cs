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
using SimpleDatabase.Execution.Operations.Sorting;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
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

                var (nextState, result) = ExecuteOperation(state, operation);

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

        private (FunctionState, Result) ExecuteOperation(FunctionState state, IOperation operation)
        {
            switch (operation)
            {
                // Cursor
                case OpenReadOperation openRead:
                    return Execute(state, openRead);
                case OpenWriteOperation openWrite:
                    return Execute(state, openWrite);
                case FirstOperation first:
                    return Execute(state, first);
                case NextOperation next:
                    return Execute(state, next);
                case InsertOperation insert:
                    return Execute(state, insert);
                case DeleteOperation delete:
                    return Execute(state, delete);

                // Columns
                case KeyOperation key:
                    return Execute(state, key);
                case ColumnOperation column:
                    return Execute(state, column);

                // Slots
                case LoadOperation load:
                    return Execute(state, load);
                case StoreOperation store:
                    return Execute(state, store);

                // Jumps
                case ConditionalJumpOperation conditionalJump:
                    return Execute(state, conditionalJump);
                case JumpOperation jump:
                    return Execute(state, jump);

                // Constants
                case ConstIntOperation constInt:
                    return Execute(state, constInt);
                case ConstStringOperation constStr:
                    return Execute(state, constStr);

                // Functions
                case ReturnOperation ret:
                    return Execute(state, ret);
                case SetupCoroutineOperation setup:
                    return Execute(state, setup);
                case CallCoroutineOperation call:
                    return Execute(state, call);

                // Sorting
                case SorterNew op:
                    return Execute(state, op);
                case SorterSort op:
                    return Execute(state, op);
                case SorterCursor op:
                    return Execute(state, op);

                // Other
                case ProgramLabel _:
                    return (state, new Result.Next());
                case MakeRowOperation makeRow:
                    return Execute(state, makeRow);
                case YieldOperation yield:
                    return Execute(state, yield);
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