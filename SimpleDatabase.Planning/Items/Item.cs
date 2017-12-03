using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleDatabase.Planning.Items
{
    public abstract class Item
    {
        protected IOperationGenerator Generator { get; }

        protected Item(IOperationGenerator generator)
        {
            Generator = generator;
        }

        public virtual Item Load()
        {
            throw new NotSupportedException();
        }
        public virtual void Store()
        {
            throw new NotSupportedException();
        }
    }
}
