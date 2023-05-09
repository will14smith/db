using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.DatabaseMetaData;

public class DatabaseMetaDataWriter
{
    private readonly ISourcePager _pager;
    
    public DatabaseMetaDataWriter(ISourcePager pager)
    {
        _pager = pager;
    }

    public DatabaseMetaDataPage EnsureInitialised()
    {
        if (_pager.PageCount != 0)
        {
            var page = _pager.Get(0);
            if (page.Type != PageType.DatabaseMetaData)
            {
                throw new Exception($"heap page 0 is a {page.Type} rather than a {PageType.DatabaseMetaData}");
            }

            return DatabaseMetaDataPage.Read(page);
        }

        var root = DatabaseMetaDataPage.New(_pager.Allocate());
        var first = DatabaseMetaDataPage.New(_pager.Allocate());
        _pager.Flush(first.PageId);

        root.NextPageIndex = first.PageId.Index; 
        _pager.Flush(root.PageId);
        
        return root;
    }

    public void SetTableSchema(string name, PageId currentSchemaPageId)
    {
        // find last DatabaseMetaData, check space, add to metadata
        var nextPageIndex = 0;
        DatabaseMetaDataPage page;

        int pageOffset;
        
        do
        {
            page = DatabaseMetaDataPage.Read(_pager.Get(nextPageIndex));

            var pageData = page.Data;
            pageOffset = 0;

            for (var i = 0; i < page.TableCount; i++)
            {
                var tableName = pageData.ReadU8LengthString();
                if (tableName == name)
                {
                    pageOffset += tableName.Length + 1;

                    var pageDataUpdated = page.Data.ToArray().AsSpan();
                    pageDataUpdated[pageOffset..].Write(currentSchemaPageId.Index);
                    
                    page.Data = pageDataUpdated;
                    _pager.Flush(page.PageId);

                    return;
                }
                
                pageOffset += DatabaseMetaDataPageLayout.TableEntrySize(tableName.Length);
            }
            
            nextPageIndex = page.NextPageIndex;
        } while (nextPageIndex >= 0);
        
        var spaceRequired = DatabaseMetaDataPageLayout.TableEntrySize(name.Length);
        if (pageOffset + spaceRequired > DatabaseMetaDataPageLayout.DataSize)
        {
            throw new NotImplementedException("create new metadata page");
        }

        var pageDataNew = page.Data.ToArray().AsSpan();

        pageOffset += pageDataNew[pageOffset..].WriteU8LengthString(name);
        pageDataNew[pageOffset..].Write(currentSchemaPageId.Index);
        
        page.Data = pageDataNew;
        page.TableCount++;
        _pager.Flush(page.PageId);
    }
}