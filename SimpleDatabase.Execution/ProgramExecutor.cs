using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution
{
    public class ProgramExecutor
    {
        private readonly Program _program;
        private readonly DatabaseManager _databaseManager;
        private readonly ITransactionManager _txm;

        public ProgramExecutor(Program program, DatabaseManager databaseManager, ITransactionManager txm)
        {
            _program = program;
            _databaseManager = databaseManager;
            _txm = txm;
        }

        public IEnumerable<IReadOnlyCollection<Value>> Execute()
        {
            var entry = _program.Functions[_program.Entry];
            var executor = new FunctionExecutor(_databaseManager, _txm, _program, entry, new Value[0]);

            return executor.Execute().Cast<RowValue>().Select(x => x.Values);
        }
    }
}
