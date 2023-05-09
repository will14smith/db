using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Execution.Tables;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using Xunit;

namespace SimpleDatabase.Execution.UnitTests
{
    public class SimpleProgramTest
    {
        private const string Folder = "test";

        private readonly IFileSystem _fileSystem;

        public SimpleProgramTest()
        {
            _fileSystem = new MockFileSystem();
            _fileSystem.Directory.CreateDirectory(Folder);
        }
        
        private static readonly Table Table =
            new Table("table", new[]
            {
                new Column("id", new ColumnType.Integer()),
                new Column("name", new ColumnType.String(63)),
                new Column("email", new ColumnType.String(255)),
            }, Array.Empty<TableIndex>());

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
            var pageStorageFactory = new FolderPageSourceFactory(_fileSystem, Folder);
            var txm = new TransactionManager();

            using var pager = new Pager(pageStorageFactory);
            
            var databaseManager = new DatabaseManager(pager);
            databaseManager.EnsureInitialised();
            var tableManager = databaseManager.GetTableManagerFor(Table);
            tableManager.EnsureInitialised();
            
            using var tx = txm.Begin();

            // Insert some data
            new TableInserter(tableManager).Insert(new Row(new[] { new ColumnValue(1), new ColumnValue("a"), new ColumnValue("a@a.a") }, tx.Id, TransactionId.None()));
            new TableInserter(tableManager).Insert(new Row(new[] { new ColumnValue(2), new ColumnValue("b"), new ColumnValue("b@b.b") }, tx.Id, TransactionId.None()));

            var result = new ProgramExecutor(Program, databaseManager, txm).Execute().ToList();

            var resultItem = Assert.Single(result);
            Assert.Equal("a", ((ObjectValue)resultItem.Skip(1).First()).Value);
        }
    }
}
