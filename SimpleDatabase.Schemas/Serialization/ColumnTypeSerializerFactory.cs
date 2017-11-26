using System;
using SimpleDatabase.Schemas.Types;

namespace SimpleDatabase.Schemas.Serialization
{
    public class ColumnTypeSerializerFactory
    {
        public IColumnTypeSerializer GetSerializer(ColumnType type)
        {
            switch (type)
            {
                case ColumnType.Integer _:
                    return new IntegerColumnTypeSerializer();
                case ColumnType.String str:
                    return new StringColumnTypeSerializer(str);
                default:
                    throw new NotImplementedException(type.GetType().FullName);
            }
        }
    }
}
