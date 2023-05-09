using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.TableMetaData;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Storage;

public class TableManager : IDisposable
{
    private readonly DatabaseManager _databaseManager;
    private readonly IReadOnlyDictionary<string, int> _indexNameToOffset;

    public ISourcePager Pager { get; }
    public Table Table { get; }

    public TableManager(DatabaseManager databaseManager, ISourcePager pager, Table table)
    {
        _databaseManager = databaseManager;
        Pager = pager;
        Table = table;
        
        _indexNameToOffset = Table.Indexes.Select((x, i) => (x, i)).ToDictionary(x => x.x.Name, x => x.i);
    }

    public void EnsureInitialised()
    {
        _databaseManager.SetTableSchema(Table);

        if (Pager.PageCount != 0)
        {
            // ensure the root is a metadata page
            TableMetaDataPage.Read(Pager.Get(0));
            
            // TODO check heap and index are valid
            
            return;
        }

        var metaDataPage = TableMetaDataPage.New(Pager.Allocate());

        var heapPage = CreateHeap();
        metaDataPage.RootHeapPageIndex = heapPage.PageId.Index;

        for (var i = 0; i < Table.Indexes.Count; i++)
        {
            var indexPage = CreateIndex(Table.Indexes[i]);
            metaDataPage[i] = indexPage.PageId.Index;
        }

        Pager.Flush(metaDataPage.PageId);
    }
    
    private HeapPage CreateHeap()
    {
        var heapPage = HeapPage.New(Pager.Allocate());
        Pager.Flush(heapPage.PageId);
        return heapPage;
    }
    
    private LeafNode CreateIndex(TableIndex index)
    {
        var serializer = index.CreateSerializer();
        var indexPage = LeafNode.New(Pager.Allocate(), serializer);

        indexPage.CellCount = 0;
        indexPage.NextLeaf = 0;
        indexPage.IsRoot = true;
        Pager.Flush(indexPage.PageId);

        return indexPage;
    }

    public PageId GetIndexRootPageId(TableIndex index)
    {
        var metaData = TableMetaDataPage.Read(Pager.Get(0));
        var rootNodePageIndex = metaData[_indexNameToOffset[index.Name]]; 
        
        return new PageId(Pager.Source, rootNodePageIndex);
    }
    
    public void Dispose() { }
}