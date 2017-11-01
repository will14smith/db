using System;

namespace SimpleDatabase.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new ConsoleREPLInput();
            var output = new ConsoleREPLOutput();

            var repl = new REPL(input, output, "test.db");

            var exitCode = repl.Run();
            Environment.Exit((int) exitCode);
        }

    }
}
