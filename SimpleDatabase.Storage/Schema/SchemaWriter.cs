using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.MetaData;

public class SchemaWriter
{
    private readonly ISourcePager _pager;

    public SchemaWriter(ISourcePager pager)
    {
        _pager = pager;
    }

    public SchemaPage Write(Table table)
    {
        var buffer = WriteToBuffer(table);
        if (buffer.Length > SchemaPageLayout.DataSize)
        {
            throw new NotImplementedException("handle schema continuation pages");
        }
        
        var page = SchemaPage.New(_pager.Allocate());
        page.ColumnCount = (byte)table.Columns.Count;
        page.IndexCount = (byte)table.Indexes.Count;
        page.Data = buffer;
        _pager.Flush(page.PageId);

        return page;
    }

    private ReadOnlySpan<byte> WriteToBuffer(Table table)
    {
        using var buffer = new MemoryStream();
        using (var writer = new BinaryWriter(buffer))
        {
            WriteTableToBuffer(writer, table);
        }
        return buffer.ToArray();
    }
    
    private void WriteTableToBuffer(BinaryWriter writer, Table table)
    {
        WriteString(writer, table.Name);

        foreach (var column in table.Columns)
        {
            WriteColumnToBuffer(writer, column);
        }

        var columnIndexes = table.Columns.Select((x, i) => (x.Name, Index: i)).ToDictionary(x => x.Name, x => x.Index);
        
        foreach (var index in table.Indexes)
        {
            WriteIndexToBuffer(writer, columnIndexes, index);
        }
    }
    
    private static void WriteColumnToBuffer(BinaryWriter writer, Column column)
    {
        WriteString(writer, column.Name);
        WriteColumnTypeToBuffer(writer, column.Type);
    }

    private static void WriteColumnTypeToBuffer(BinaryWriter writer, ColumnType type)
    {
        switch (type)
        {
            case ColumnType.Integer:
                writer.Write((byte)1);
                break;
            case ColumnType.String str:
                writer.Write((byte)2);
                writer.Write((byte)str.Length);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }

    private static void WriteIndexToBuffer(BinaryWriter writer, IReadOnlyDictionary<string, int> columns, TableIndex index)
    {
        var structure = index.Structure;
        
        writer.Write((byte)structure.Keys.Count);
        writer.Write((byte)structure.Data.Count);
        
        WriteString(writer, index.Name);

        foreach (var key in structure.Keys)
        {
            var columnIndex = columns[key.Item1.Name];
            
            writer.Write((byte)columnIndex);
            writer.Write((byte)key.Item2);
        }
        
        foreach (var data in structure.Data)
        {
            var columnIndex = columns[data.Name];
            
            writer.Write((byte)columnIndex);
        }
    }
    
    private static void WriteString(BinaryWriter writer, string str)
    {
        writer.Write((byte)str.Length);
        writer.Write(Encoding.ASCII.GetBytes(str));
    }
}