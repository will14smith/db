namespace SimpleDatabase.Schemas
{
    public class Index
    {
        public string Name { get; }
        public KeyStructure Structure { get; }

        // TODO should this always be 0?
        public int RootPage => 0;

        public Index(string name, KeyStructure structure)
        {
            Name = name;
            Structure = structure;
        }
    }
}
