using System.Collections.Generic;
using Antlr4.Runtime;
using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Parsing.Visitors;

namespace SimpleDatabase.Parsing
{
    public class Parser
    {
        public static IReadOnlyCollection<Statement> Parse(string input)
        {
            var inputStream = new CodePointCharStream(input);
            var lexer = new SQLLexer(inputStream);
            var parser = new SQLParser(new CommonTokenStream(lexer))
            {
                ErrorHandler = new BailErrorStrategy()
            };

            var context = parser.program();

            return context.Accept(new ProgramVisitor());
        }
    }
}
