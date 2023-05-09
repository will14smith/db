using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Storage
{
    public class Database
    {
        private readonly IReadOnlyDictionary<string, Table> _tables;
            
        public Database(IEnumerable<Table> tables)
        {
            _tables = tables.ToDictionary(x => x.Name);
        }

        public IEnumerable<Table> Tables => _tables.Values;

        public Table GetTable(string name) { return _tables[name]; }
    }
}
