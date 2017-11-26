using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleDatabase.Core;
using SimpleDatabase.Core.Paging;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Slots;
using Xunit;

namespace SimpleDatabase.Execution.UnitTests
{
    public class SimpleProgramTest
    {
        private const int RootPageNumber = 0;

        private static readonly ProgramLabel Loop = ProgramLabel.Create();
        private static readonly ProgramLabel Next = ProgramLabel.Create();
        private static readonly ProgramLabel Finish = ProgramLabel.Create();
        private static readonly SlotLabel Cursor = SlotLabel.Create();

        private static readonly Program Program = new Program(
            new List<IOperation>
            {
                // cursor = first(open(RootPageNumber))
                new OpenReadOperation(RootPageNumber),
                new FirstOperation(Finish),
                Loop,
                new StoreOperation(Cursor),

                // if cursor.Key == 2 -> jump to next
                new LoadOperation(Cursor),
                new KeyOperation(),
                new ConstIntOperation(2),
                new ConditionalJumpOperation(Comparison.Equal, Next),

                // id = cursor.Key
                new LoadOperation(Cursor),
                new KeyOperation(),
                
                // username = cursor.Column[Username]
                new LoadOperation(Cursor),
                new ColumnOperation(1),

                // email = cursor.Column[Email]
                new LoadOperation(Cursor),
                new ColumnOperation(2),

                // yield (id, username, email)
                new YieldRowOperation(3),

                // cursor = next(cursor) -> loop
                Next,
                new LoadOperation(Cursor),
                new NextOperation(Loop),

                // finish
                Finish,
                new FinishOperation(),
            },
            new Dictionary<SlotLabel, SlotDefinition>
            {
                { Cursor, new SlotDefinition() }
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

                    Assert.Equal(1, result.Count);
                }
            }
            finally
            {
                File.Delete(file);
            }
        }
    }
}
