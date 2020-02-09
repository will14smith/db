using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Values
{
    public class SorterValue : Value, IInsertTarget
    {
        private readonly KeyStructure _key;
        private readonly List<Row> _rows = new List<Row>();

        private bool _frozen;

        public SorterValue(KeyStructure key)
        {
            _key = key;
        }

        public InsertTargetResult Insert(Row row)
        {
            if (_frozen)
            {
                throw new InvalidOperationException("Sorter has been sorted and is now frozen.");
            }

            // TODO check row matches key structure
            // TODO allow duplicates?
            _rows.Add(row);
            return new InsertTargetResult.Success();
        }

        public void Sort()
        {
            if (_frozen)
            {
                throw new InvalidOperationException("Sorter has been sorted and is now frozen.");
            }

            _frozen = true;

            var sortedRows = _rows.OrderBy(x => x, new KeyComparer(_key)).ToList();
            _rows.Clear();
            _rows.AddRange(sortedRows);
        }

        public ICursor NewCursor()
        {
            if (!_frozen)
            {
                throw new InvalidOperationException("Can only get cursor after sorting.");
            }


            return new Cursor(this);
        }

        private class Cursor : ICursor
        {
            private readonly int _index;
            private readonly SorterValue _sorter;

            public Cursor(SorterValue sorter)
            {
                _sorter = sorter;
            }
            private Cursor(Cursor cursor, int index)
                : this(cursor._sorter)
            {
                _index = index;
            }

            public bool EndOfTable => _index >= _sorter._rows.Count;

            public ICursor First()
            {
                return new Cursor(this, 0);
            }

            public ICursor Next()
            {
                return new Cursor(this, _index + 1);
            }

            public int Key()
            {
                throw new NotImplementedException();
            }

            public ColumnValue Column(int index)
            {
                var row = _sorter._rows[_index];
                var offset = index;

                return row.Values[offset];
            }
        }
    }

    public class KeyComparer : IComparer<Row>
    {
        private readonly KeyStructure _structure;

        public KeyComparer(KeyStructure structure)
        {
            _structure = structure;
        }

        public int Compare(Row x, Row y)
        {
            var xKeys = GetKey(x);
            var yKeys = GetKey(y);

            for (var i = 0; i < _structure.Keys.Count; i++)
            {
                var comparison = Comparer<object?>.Default.Compare(xKeys[i], yKeys[i]);
                if (comparison != 0)
                {
                    var (_, ordering) = _structure.Keys[i];

                    return ordering == KeyOrdering.Ascending ? comparison : -comparison;
                }
            }

            return 0;
        }

        private IReadOnlyList<object?> GetKey(Row row)
        {
            var keyColumnCount = _structure.Keys.Count;

            return row.Values.Take(keyColumnCount).Select(x => x.Value).ToList();
        }
    }
}
