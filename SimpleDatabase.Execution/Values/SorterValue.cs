﻿using System;
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

        public InsertResult Insert(Row row)
        {
            if (_frozen)
            {
                throw new InvalidOperationException("Sorter has been sorted and is now frozen.");
            }

            // TODO check row matches key structure
            // TODO allow duplicates?
            _rows.Add(row);
            return new InsertResult.Success();
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
            private readonly SorterValue _sorter;

            public Cursor(SorterValue sorter)
            {
                _sorter = sorter;
            }

            public bool EndOfTable { get; }
            public ICursor First()
            {
                throw new NotImplementedException();
            }

            public ICursor Next()
            {
                throw new NotImplementedException();
            }

            public int Key()
            {
                throw new NotImplementedException();
            }

            public ColumnValue Column(int index)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class KeyComparer : IComparer<Row>
    {
        private readonly KeyStructure _key;

        public KeyComparer(KeyStructure key)
        {
            _key = key;
        }

        public int Compare(Row x, Row y)
        {
            var xKeys = GetKey(x);
            var yKeys = GetKey(y);

            for (var i = 0; i < _key.KeyColumns; i++)
            {
                var comparison = Comparer<object>.Default.Compare(xKeys[i], yKeys[i]);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return 0;
        }

        private IReadOnlyList<object> GetKey(Row row)
        {
            return row.Values.Take(_key.KeyColumns).Select(x => x.Value).ToList();
        }
    }
}
