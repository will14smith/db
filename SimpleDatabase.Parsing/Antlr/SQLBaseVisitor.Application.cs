using System;
using Antlr4.Runtime.Tree;

namespace SimpleDatabase.Parsing.Antlr
{
    // This override the default implementation of VisitChild so that everything has to be explicitly supported.
    partial class SQLBaseVisitor<Result>
    {
        public override Result VisitChildren(IRuleNode node) => throw new NotSupportedException(node.GetType().FullName);
    }
}
