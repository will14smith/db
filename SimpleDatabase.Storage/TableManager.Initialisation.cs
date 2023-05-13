using System;
using System.Linq;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.TableMetaData;

namespace SimpleDatabase.Storage;

public partial class TableManager
{
    public void EnsureInitialised()
    {
        // try get current schema
        var currentSchema = _databaseManager.TryGetTableSchema(Table.Name);
        if (!currentSchema.HasValue)
        {
            EnsureInitialisedCreate();
        }
        else
        {
            EnsureInitialisedExisting(currentSchema.Value!);
        }
    }
    
    private void EnsureInitialisedCreate()
    {
        _databaseManager.SetTableSchema(Table);

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
    
    private void EnsureInitialisedExisting(Table oldSchema)
    {
        var shouldUpdateSchema = false;
        
        // compare columns, we don't need to update the heap since the tuples track the schema version (or they will do...)
        if (oldSchema.Columns.Count == Table.Columns.Count)
        {
            for (var i = 0; i < oldSchema.Columns.Count; i++)
            {
                var oldColumn = oldSchema.Columns[i];
                var newColumn = Table.Columns[i];

                if (oldColumn.Name == newColumn.Name && oldColumn.Type == newColumn.Type)
                {
                    continue;
                }
                
                shouldUpdateSchema = true;
                break;
            }
        }
        else
        {
            shouldUpdateSchema = true;
        }

        // compare indexes, pre-condition is that the delta can either be:
        // - empty
        // - a single index is removed
        // - a single index is added to the *end* of the list
        if (oldSchema.Indexes.Count > Table.Indexes.Count)
        {
            shouldUpdateSchema = true;

            int indexToRemoveOffset;
            for (indexToRemoveOffset = 0; indexToRemoveOffset < Table.Indexes.Count; indexToRemoveOffset++)
            {
                var oldIndex = oldSchema.Indexes[indexToRemoveOffset];
                var newIndex = Table.Indexes[indexToRemoveOffset];
                if (oldIndex.Name != newIndex.Name)
                {
                    break;
                }
            }
            
            // TODO de-allocate index pages

            var root = TableMetaDataPage.Read(Pager.Get(0));

            for (var i = indexToRemoveOffset; i < oldSchema.Indexes.Count - 1; i++)
            {
                root[i] = root[i + 1];
            }
            root[oldSchema.Indexes.Count] = 0;
            Pager.Flush(root.PageId);
        }
        else if (oldSchema.Indexes.Count < Table.Indexes.Count)
        {
            shouldUpdateSchema = true;

            var root = TableMetaDataPage.Read(Pager.Get(0));

            var indexToAddOffset = Table.Indexes.Count - 1;
            var indexPage = CreateIndex(Table.Indexes[indexToAddOffset]);
            root[indexToAddOffset] = indexPage.PageId.Index;
            
            Pager.Flush(root.PageId);
        }
        
        if (shouldUpdateSchema)
        {
            _databaseManager.SetTableSchema(Table);
        }
    }
}