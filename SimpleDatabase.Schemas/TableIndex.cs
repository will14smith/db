namespace SimpleDatabase.Schemas
{
    public class TableIndex
    {
        public string Name { get; }
        public KeyStructure Structure { get; }

        // TODO either immutable or store somewhere else....
        public int RootPage { get; set; }

        public TableIndex(string name, KeyStructure structure)
        {
            Name = name;
            Structure = structure;
        }
    }
}
