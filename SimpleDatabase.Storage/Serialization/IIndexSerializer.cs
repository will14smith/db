using System;

namespace SimpleDatabase.Storage.Serialization
{
    public interface IIndexSerializer
    {
        int GetKeySize();
        int GetDataSize();

        IndexKey ReadKey(Span<byte> data);
        void WriteKey(Span<byte> data, IndexKey row);

        IndexData ReadData(Span<byte> data);
        void WriteData(Span<byte> data, IndexData row);
    }
}