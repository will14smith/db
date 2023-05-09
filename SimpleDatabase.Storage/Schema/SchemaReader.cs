using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.MetaData;

public class SchemaReader
{
    private readonly ISourcePager _pager;

    public SchemaReader(ISourcePager pager)
    {
        _pager = pager;
    }

    public Table Read(PageId pageId)
    {
        var page = SchemaPage.Read(_pager.Get(pageId));
        var data = ReadAllPages(page);
        var offset = 0;

        var tableName = ReadString(data, ref offset);

        var columnCount = page.ColumnCount;
        var columns = new Column[columnCount];
        for (var i = 0; i < columnCount; i++)
        {
            columns[i] = ReadColumn(data, ref offset);
        }
        
        var indexCount = page.IndexCount;
        var indexes = new TableIndex[indexCount];
        for (var i = 0; i < indexCount; i++)
        {
            indexes[i] = ReadIndex(columns, data, ref offset);
        }

        return new Table(tableName, columns, indexes);
    }
    
    private Column ReadColumn(ReadOnlySpan<byte> data, ref int offset)
    {
        var columnName = ReadString(data, ref offset);
        var columnType = ReadColumnType(data, ref offset);

        return new Column(columnName, columnType);
    }

    private static ColumnType ReadColumnType(ReadOnlySpan<byte> data, ref int offset)
    {
        var type = ReadByte(data, ref offset);

        // TODO move these type ids somewhere common with writer
        return type switch
        {
            1 => new ColumnType.Integer(),
            2 => new ColumnType.String(ReadByte(data, ref offset)),
            _ => throw new Exception($"unknown column type with id: {type}")
        };
    }

    private static TableIndex ReadIndex(IReadOnlyList<Column> columns, ReadOnlySpan<byte> data, ref int offset)
    {
        var keyCount = ReadByte(data, ref offset);
        var dataColumnCount = ReadByte(data, ref offset);
        
        var indexName = ReadString(data, ref offset);

        var keys = new (Column, KeyOrdering)[keyCount];
        for (var i = 0; i < keyCount; i++)
        {
            var keyColumn = columns[ReadByte(data, ref offset)];
            var keyOrder = (KeyOrdering)ReadByte(data, ref offset);
            
            keys[i] = (keyColumn, keyOrder);
        }

        var dataColumns = new Column[dataColumnCount];
        for (var i = 0; i < dataColumnCount; i++)
        {
            dataColumns[i] = columns[ReadByte(data, ref offset)];
        }

        var structure = new KeyStructure(keys, dataColumns);
        return new TableIndex(indexName, structure);
    }

    private ReadOnlySpan<byte> ReadAllPages(SchemaPage page)
    {
        using var buffer = new MemoryStream();

        buffer.Write(page.Data);

        var nextPage = page.SchemaContinuationPageIndex;
        while (nextPage >= 0)
        {
            var continuationPage = SchemaContinuationPage.Read(_pager.Get(nextPage));

            buffer.Write(continuationPage.Data);
            
            nextPage = continuationPage.SchemaContinuationPageIndex;
        }
        
        return buffer.ToArray();
    }
    
    private static byte ReadByte(in ReadOnlySpan<byte> data, ref int offset) => data[offset++];

    private static string ReadString(in ReadOnlySpan<byte> data, ref int offset)
    {
        var str = data[offset..].ReadU8LengthString();
        offset += str.Length + 1;
        return str;
    }
}