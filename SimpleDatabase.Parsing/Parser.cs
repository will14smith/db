using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Visitors;namespace SimpleDatabase.Parsing
{
    public class Parser
    {
        public static void Test()
        {
            var input = new CodePointCharStream("SELECT * FROM table WHERE name = 'a'");
            var lexer = new SQLLexer(input);
            var parser = new SQLParser(new CommonTokenStream(lexer));

            var x = parser.program();
        }
    }
}
