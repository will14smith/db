﻿namespace SimpleDatabase.Execution
{
    public class SlotDefinition
    {
        // TODO type
        public string Name { get; }
        
        public SlotDefinition(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}