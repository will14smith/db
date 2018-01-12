using System;
using System.Collections.Generic;
using System.Text;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution
{
    public interface IRowSerializerFactory
    {
        IRowSerializer Create(Table table);
    }

    internal class RowSerializerFactory : IRowSerializerFactory
    {
        public IRowSerializer Create(Table table)
        {
            return new RowSerializer(
                table,
                new ColumnTypeSerializerFactory()
            );
        }
    }
}
