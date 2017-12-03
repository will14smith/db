using System;

namespace SimpleDatabase.Planning.Items
{
    public abstract class Item
    {
        public virtual Item Load(IOperationGenerator generator)
        {
            throw new NotSupportedException();
        }
        public virtual void Store(IOperationGenerator generator)
        {
            throw new NotSupportedException();
        }
    }
}
