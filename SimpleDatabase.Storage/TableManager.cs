using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.TableMetaData;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Storage;

public partial class TableManager : IDisposable
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