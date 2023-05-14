using System;
using System.Collections.Generic;
using SimpleDatabase.Execution.OperationExecutors;
using SimpleDatabase.Execution.OperationExecutors.Columns;
using SimpleDatabase.Execution.OperationExecutors.Constants;
using SimpleDatabase.Execution.OperationExecutors.Cursors;
using SimpleDatabase.Execution.OperationExecutors.Functions;
using SimpleDatabase.Execution.OperationExecutors.Jumps;
using SimpleDatabase.Execution.OperationExecutors.Slots;
using SimpleDatabase.Execution.OperationExecutors.Sorting;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution
{
    public class FunctionExecutor
    {
        private static readonly IReadOnlyDictionary<string, Func<FunctionExecutor, FunctionState, IOperation, (FunctionState, OperationResult)>> OperationExecutors = CreateOperationExecutors();

        private readonly DatabaseManager _databaseManager;
        private readonly ITransactionManager _txm;

        private readonly Program _program;
        private readonly Function _function;
        private readonly IReadOnlyList<Value> _arguments;

        private readonly IReadOnlyDictionary<ProgramLabel, int> _labelAddresses;

        public FunctionExecutor(DatabaseManager databaseManager, ITransactionManager txm, Program program, Function function, IReadOnlyList<Value> arguments)
        {
            _databaseManager = databaseManager;
            _txm = txm;

            _program = program;
            (_function, _labelAddresses) = RemoveLabels(function);
            _arguments = arguments;
        }

        private static (Function, IReadOnlyDictionary<ProgramLabel, int>) RemoveLabels(Function input)
        {
            var labels = new Dictionary<ProgramLabel, int>();
            var operations = new List<IOperation>();

            foreach (var op in input.Operations)
            {
                if (op is ProgramLabel label)
                {
                    labels.Add(label, operations.Count);
                }
                else
                {
                    operations.Add(op);
                }
            }

            return (new Function(operations, input.Slots), labels);
        }

        public IEnumerable<Value> Execute()
        {
            var state = new FunctionState(_function.Slots);

            while (true)
            {
                Value? value;
                (state, value) = ExecuteStep(state);

                if (value == null)
                {
                    yield break;
                }

                yield return value;
            }
        }

        public (FunctionState, Value?) ExecuteStep(FunctionState state)
        {
            while (true)
            {
                var operation = GetOperation(state);

                var (nextState, result) = ExecuteOperation(state, operation);

                switch (result is OperationResult.Yield y ? y.Inner : result)
                {
                    case OperationResult.Next _:
                        state = nextState.SetPC(nextState.GetPC() + 1);
                        break;

                    case OperationResult.Jump jump:
                        state = nextState.SetPC(_labelAddresses[jump.Address]);
                        break;

                    case OperationResult.Finished _:
                        // TODO cleanup open resources
                        return (state, null);

                    default:
                        throw new NotImplementedException($"Unsupported result type: {result.GetType().Name}");
                }

                if (result is OperationResult.Yield yield)
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

        private (FunctionState, OperationResult) ExecuteOperation(FunctionState state, IOperation operation)
        {
            var key = operation.GetType().FullName;

            if (!OperationExecutors.TryGetValue(key, out var executor))
            {
                throw new Exception($"No executor found for {key} - {operation}");
            }

            return executor(this, state, operation);
        }

        private static IReadOnlyDictionary<string, Func<FunctionExecutor, FunctionState, IOperation, (FunctionState, OperationResult)>> CreateOperationExecutors()
        {
            var executors = new Dictionary<string, Func<FunctionExecutor, FunctionState, IOperation, (FunctionState, OperationResult)>>();

            AddExecutor(executors, _ => new FinishOperationExecutor());
            AddExecutor(executors, _ => new MakeRowOperationExecutor());
            AddExecutor(executors, _ => new YieldOperationExecutor());
            
            // columns
            AddExecutor(executors, _ => new ColumnOperationExecutor());
            AddExecutor(executors, _ => new KeyOperationExecutor());

            // constants
            AddExecutor(executors, _ => new ConstIntOperationExecutor());
            AddExecutor(executors, _ => new ConstStringOperationExecutor());

            // cursors
            AddExecutor(executors, _ => new FirstOperationExecutor());
            AddExecutor(executors, _ => new DeleteOperationExecutor());
            AddExecutor(executors, fexec => new InsertOperationExecutor(fexec._txm));
            AddExecutor(executors, _ => new NextOperationExecutor());
            AddExecutor(executors, fexec => new OpenReadIndexOperationExecutor(fexec._databaseManager, fexec._txm));
            AddExecutor(executors, fexec => new OpenReadTableOperationExecutor(fexec._databaseManager, fexec._txm));
            AddExecutor(executors, fexec => new OpenWriteOperationExecutor(fexec._databaseManager, fexec._txm));
            AddExecutor(executors, fexec => new SeekRowIdOperationExecutor(fexec._databaseManager, fexec._txm));

            // functions
            AddExecutor(executors, fexec => new CallCoroutineOperationExecutor(fexec._databaseManager, fexec._txm, fexec._program));
            AddExecutor(executors, _ => new ReturnOperationExecutor());
            AddExecutor(executors, fexec => new SetupCoroutineOperationExecutor(fexec._program.Functions));

            // jumps
            AddExecutor(executors, _ => new ConditionalJumpOperationExecutor());
            AddExecutor(executors, _ => new JumpOperationExecutor());

            // slots
            AddExecutor(executors, _ => new LoadOperationExecutor());
            AddExecutor(executors, _ => new StoreOperationExecutor());

            // sorting
            AddExecutor(executors, _ => new SorterCursorOperationExecutor());
            AddExecutor(executors, _ => new SorterNewOperationExecutor());
            AddExecutor(executors, _ => new SorterSortOperationExecutor());

            return executors;
        }

        private static void AddExecutor<TOperation>(IDictionary<string, Func<FunctionExecutor, FunctionState, IOperation, (FunctionState, OperationResult)>> executors, Func<FunctionExecutor, IOperationExecutor<TOperation>> factory)
            where TOperation : IOperation
        {
            var key = typeof(TOperation).FullName;

            (FunctionState, OperationResult) Executor(FunctionExecutor fexec, FunctionState state, IOperation op) => factory(fexec).Execute(state, (TOperation)op);

            executors.Add(key, Executor);
        }
    }
}