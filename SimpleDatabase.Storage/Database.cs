using System.Collections.Generic;

namespace SimpleDatabase.Storage
{
    public class Database
    {
        private readonly IReadOnlyDictionary<string, StoredTable> _tables;

        public Database(IReadOnlyDictionary<string, StoredTable> tables)
        {
            _tables = tables;
        }

        public StoredTable GetTable(string name) { return _tables[name]; }
    }
}
