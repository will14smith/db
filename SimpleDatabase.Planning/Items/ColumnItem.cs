using SimpleDatabase.Execution.Operations.Columns;

namespace SimpleDatabase.Planning.Items
{
    public class ColumnItem : Item
    {
        private readonly Item _cursor;
        private readonly int _index;

        public ColumnItem(Item cursor, int index)
        {
            _cursor = cursor;
            _index = index;
        }

        public override Item Load(IOperationGenerator generator)
        {
            _cursor.Load(generator);
            generator.Emit(new ColumnOperation(_index));

            // TODO type
            return new StackItem();
        }
    }
}
