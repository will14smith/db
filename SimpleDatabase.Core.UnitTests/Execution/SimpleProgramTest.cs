using SimpleDatabase.Core.Execution;
using SimpleDatabase.Core.Execution.Operations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleDatabase.Core.Paging;
using Xunit;

namespace SimpleDatabase.Core.UnitTests.Execution
{
    public class SimpleProgramTest
    {
        private const int RootPageNumber = 0;
        private const int CursorSlot = 0;

        private static readonly Program Program = new Program(
            new List<Operation>
            {
                // cursor = first(open(RootPageNumber))
                new OpenReadOperation(RootPageNumber),
                new FirstOperation(12),
                // loop:
                new StoreOperation(CursorSlot),

                // id = cursor.Key
                new LoadOperation(CursorSlot),
                new KeyOperation(),

                // username = cursor.Column[Username]
                new LoadOperation(CursorSlot),
                new ColumnOperation(1),

                // email = cursor.Column[Email]
                new LoadOperation(CursorSlot),
                new ColumnOperation(2),

                // yield (id, username, email)
                new YieldRowOperation(3),

                // cursor = next(cursor) -> loop
                new LoadOperation(CursorSlot),
                new NextOperation(2),

                // finish
                new FinishOperation(),
            },
            new List<SlotDefinition>
            {
                new SlotDefinition() // cursor
            });

        [Fact]
        public void RunProgram()
        {
            var file = Path.GetTempFileName();
            try
            {
                using (var pager = new Pager(new FilePagerStorage(file)))
                using (var table = new Table(pager))
                {
                    table.Insert(new InsertStatement(new Row(1, "a", "a@a.a")));
                    table.Insert(new InsertStatement(new Row(2, "b", "b@b.b")));

                    var result = new ProgramExecutor(Program, pager).Execute().ToList();

                    Assert.Equal(2, result.Count);
                }
            }
            finally
            {
                File.Delete(file);
            }
        }
    }
}
