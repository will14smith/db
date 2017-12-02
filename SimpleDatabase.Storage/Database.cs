using System.Collections.Generic;
using System.Linq;

namespace SimpleDatabase.Storage
{
    public class Database
    {
        private readonly IReadOnlyDictionary<string, StoredTable> _tables;

        public Database(IEnumerable<StoredTable> tables)
        {
            _tables = tables.ToDictionary(x => x.Table.Name);
        }

        public StoredTable GetTable(string name) { return _tables[name]; }
    }
}
