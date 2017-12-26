using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Execution.Tables;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;
using Xunit;

namespace SimpleDatabase.Execution.UnitTests
{
    public class SimpleProgramTest : IDisposable
    {
        public SimpleProgramTest()
        {
            var folderName = "_test" + DateTime.Now.Ticks;
            _folder = Path.Combine(Path.GetTempPath(), folderName);
            Directory.CreateDirectory(_folder);
        }

        public void Dispose()
        {
            Directory.Delete(_folder, true);
        }

        private readonly string _folder;

        private static readonly Table Table =
            new Table("table", new[]
            {
                new Column("id", new ColumnType.Integer()),
                new Column("name", new ColumnType.String(63)),
                new Column("email", new ColumnType.String(255)),
            }, new Index[0]);

        private static readonly FunctionLabel MainLabel = FunctionLabel.Create();
        private static readonly ProgramLabel Loop = ProgramLabel.Create();
        private static readonly ProgramLabel Finish = ProgramLabel.Create();
        private static readonly SlotLabel Cursor = SlotLabel.Create();

        private static readonly Function Main = new Function(new List<IOperation>
            {
                // cursor = first(open(RootPageId))
                new OpenReadTableOperation(Table),
                new FirstOperation(),
                new StoreOperation(Cursor),

                // cursor = next(cursor)
                Loop,
                new LoadOperation(Cursor),
                new NextOperation(Finish),
                new StoreOperation(Cursor),

                // if cursor.Column[Id] == 2 -> loop
                new LoadOperation(Cursor),
                new ColumnOperation(0),
                new ConstIntOperation(2),
                new ConditionalJumpOperation(Comparison.Equal, Loop),

                // id = cursor.Column[Id]
                new LoadOperation(Cursor),
                new ColumnOperation(0),
                
                // username = cursor.Column[Username]
                new LoadOperation(Cursor),
                new ColumnOperation(1),

                // email = cursor.Column[Email]
                new LoadOperation(Cursor),
                new ColumnOperation(2),

                // yield (id, username, email)
                new MakeRowOperation(3),
                new YieldOperation(),

                // loop
                new JumpOperation(Loop),

                // finish
                Finish,
                new FinishOperation(),
            },
            new Dictionary<SlotLabel, SlotDefinition>
            {
                { Cursor, new SlotDefinition("cursor") }
            });

        private static readonly Program Program = new Program(MainLabel, new Dictionary<FunctionLabel, Function> { { MainLabel, Main } });

        [Fact]
        public void RunProgram()
        {
            var pageStorageFactory = new FolderPageSourceFactory(_folder);
            using (var pager = new Pager(pageStorageFactory))
            {
                new TableCreator(pager).Create(Table);

                // Insert some data
                new TableInserter(pager, Table).Insert(new Row(new[] { new ColumnValue(1), new ColumnValue("a"), new ColumnValue("a@a.a") }, new TransactionId(0), Option.None<TransactionId>()));
                new TableInserter(pager, Table).Insert(new Row(new[] { new ColumnValue(2), new ColumnValue("b"), new ColumnValue("b@b.b") }, new TransactionId(0), Option.None<TransactionId>()));

                var result = new ProgramExecutor(Program, pager).Execute().ToList();

                var resultItem = Assert.Single(result);
                Assert.Equal("a", ((ObjectValue)resultItem.Skip(1).First()).Value);
            }
        }
    }
}
