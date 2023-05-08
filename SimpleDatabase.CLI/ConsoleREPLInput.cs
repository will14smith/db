using System;

namespace SimpleDatabase.CLI
{
    internal class ConsoleREPLInput : IREPLInput
    {
        public string? ReadLine()
        {
            return Console.ReadLine();
        }
    }
}