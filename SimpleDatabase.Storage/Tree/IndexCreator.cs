using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
{
    public class IndexCreator
    {
        private readonly SourcePager _pager;
        private readonly Index _index;

        private readonly RowSerializer _rowSerializer;

        public IndexCreator(SourcePager pager, Table table, Index index)
        {
            _pager = pager;
            _index = index;

            _rowSerializer = new RowSerializer(table, new ColumnTypeSerializerFactory());
        }

        public void Create()
        {
            var page = _pager.Allocate();

            var node = LeafNode.New(_rowSerializer, page);

            node.CellCount = 0;
            node.NextLeaf = 0;
            node.IsRoot = true;

            var index = page.Id.Index;
            _pager.Flush(index);
            _index.RootPage = index;
        }
    }
}