using SimpleDatabase.Execution.Operations.Columns;

namespace SimpleDatabase.Planning.Items
{
    public class ColumnItem : Item
    {
        private readonly Item _cursor;
        private readonly int _index;

        public ColumnItem(IOperationGenerator generator, Item cursor, int index) : base(generator)
        {
            _cursor = cursor;
            _index = index;
        }

        public override Item Load()
        {
            _cursor.Load();
            Generator.Emit(new ColumnOperation(_index));

            // TODO type
            return new StackItem(Generator);
        }
    }
}
