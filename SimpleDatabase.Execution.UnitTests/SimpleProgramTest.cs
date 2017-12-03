using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Nodes;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using Xunit;

namespace SimpleDatabase.Execution.UnitTests
{
    public class SimpleProgramTest
    {
        private static readonly StoredTable Table = new StoredTable(
            new Table("table", new[]
            {
                new Column("id", new ColumnType.Integer()),
                new Column("name", new ColumnType.String(63)),
                new Column("email", new ColumnType.String(255)),
            }),
            0);

        private static readonly FunctionLabel MainLabel = FunctionLabel.Create();
        private static readonly ProgramLabel Loop = ProgramLabel.Create();
        private static readonly ProgramLabel Finish = ProgramLabel.Create();
        private static readonly SlotLabel Cursor = SlotLabel.Create();

        private static readonly Function Main = new Function(new List<IOperation>
            {
                // cursor = first(open(RootPageNumber))
                new OpenReadOperation(Table),
                new FirstOperation(),
                new StoreOperation(Cursor),

                // cursor = next(cursor)
                Loop,
                new LoadOperation(Cursor),
                new NextOperation(Finish),
                new StoreOperation(Cursor),

                // if cursor.Key == 2 -> loop
                new LoadOperation(Cursor),
                new KeyOperation(),
                new ConstIntOperation(2),
                new ConditionalJumpOperation(Comparison.Equal, Loop),

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
            var file = Path.GetTempFileName();
            try
            {
                using (var pager = new Pager(new FilePagerStorage(file)))
                {
                    // TODO clean this up 

                    // Create root page
                    var rootPage = pager.Get(Table.RootPageNumber);
                    var rowSerializer = new RowSerializer(Table.Table, new ColumnTypeSerializerFactory());
                    var node = LeafNode.New(rowSerializer, rootPage);
                    node.IsRoot = true;
                    pager.Flush(Table.RootPageNumber);

                    // Insert some data
                    new TreeInserter(pager, rowSerializer, Table).Insert(1, new Row(new[] { new ColumnValue(1), new ColumnValue("a"), new ColumnValue("a@a.a") }));
                    new TreeInserter(pager, rowSerializer, Table).Insert(1, new Row(new[] { new ColumnValue(2), new ColumnValue("b"), new ColumnValue("b@b.b") }));

                    var result = new ProgramExecutor(Program, pager).Execute().ToList();

                    Assert.Equal(1, result.Count);
                    Assert.Equal("a", ((ObjectValue) result[0].Skip(1).First()).Value);
                }
            }
            finally
            {
                File.Delete(file);
            }
        }
    }
}
