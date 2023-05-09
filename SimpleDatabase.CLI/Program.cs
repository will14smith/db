using System;

namespace SimpleDatabase.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = "test.db";
            Seed.EnsureExists(folder);
            
            var input = new ConsoleREPLInput();
            var output = new ConsoleREPLOutput();

            var repl = new REPL(input, output, folder);

            var exitCode = repl.Run();
            Environment.Exit((int) exitCode);
        }

    }
}
