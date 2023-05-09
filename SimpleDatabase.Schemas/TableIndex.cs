namespace SimpleDatabase.Schemas
{
    public class TableIndex
    {
        public string Name { get; }
        public KeyStructure Structure { get; }
        
        public TableIndex(string name, KeyStructure structure)
        {
            Name = name;
            Structure = structure;
        }
    }
}
