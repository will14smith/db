﻿using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution
{
    public class ProgramExecutor
    {
        private readonly Program _program;
        private readonly IPager _pager;
        private readonly ITransactionManager _txm;

        public ProgramExecutor(Program program, IPager pager, ITransactionManager txm)
        {
            _program = program;
            _pager = pager;
            _txm = txm;
        }

        public IEnumerable<IReadOnlyCollection<Value>> Execute()
        {
            var entry = _program.Functions[_program.Entry];
            var executor = new FunctionExecutor(entry, new Value[0], _pager, _txm, _program);

            return executor.Execute().Cast<RowValue>().Select(x => x.Values);
        }
    }
}
