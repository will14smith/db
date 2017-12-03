namespace SimpleDatabase.Planning.Items
{
    public class StackItem : Item
    {
        public override Item Load(IOperationGenerator generator)
        {
            return this;
        }
    }
}