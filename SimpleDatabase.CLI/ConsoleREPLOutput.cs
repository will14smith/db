using System;

namespace SimpleDatabase.CLI
{
    internal class ConsoleREPLOutput : IREPLOutput
    {
        public void Write(string str)
        {
            Console.Write(str);
        }
    }
}