namespace SimpleDatabase.Schemas
{
    public class Index
    {
        public string Name { get; }
        public KeyStructure Structure { get; }

        // TODO either immutable or store somewhere else....
        public int RootPage { get; set; }

        public Index(string name, KeyStructure structure)
        {
            Name = name;
            Structure = structure;
        }
    }
}
