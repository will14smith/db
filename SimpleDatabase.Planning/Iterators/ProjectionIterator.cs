using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Planning.Iterators
{
    public class ProjectionIterator : IIterator
    {
        private readonly IIterator _input;
        private readonly IReadOnlyList<ResultColumn> _columns;

        public ProjectionIterator(IIterator input, IReadOnlyList<ResultColumn> columns)
        {
            _input = input;
            _columns = columns;

            Output = CalculateOutput();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit()
        {
            _input.GenerateInit();
        }

        public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
        {
            _input.GenerateMoveNext(loopStart, loopEnd);
        }

        private IteratorOutput CalculateOutput()
        {
            var innerOutput = (IteratorOutput.Row)_input.Output;

            var columns = new List<IteratorOutput.Named>();
            foreach (var column in _columns)
            {
                switch (column)
                {
                    case ResultColumn.Star star:
                        if (star.Table != null) { throw new NotImplementedException(); }

                        columns.AddRange(innerOutput.Columns);
                        break;

                    case ResultColumn.Expression expr:
                        var name = expr.Alias.Select(x => new IteratorOutputName.Constant(x!)) ?? (IteratorOutputName) new IteratorOutputName.Expression(expr.Value);
                        var item = Compile(innerOutput, expr.Value);

                        columns.Add(new IteratorOutput.Named(name, item));
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(column), $"Unhandled type: {column.GetType().FullName}");
                }
            }

            return new IteratorOutput.Row(innerOutput.Cursor, columns);
        }

        private Item Compile(IteratorOutput.Row input, Expression expr)
        {
            switch (expr)
            {
                case ColumnNameExpression column:
                    var result = input.Columns.Single(x => x.Name.Matches(column.Name));

                    return result.Value;

                default: throw new ArgumentOutOfRangeException(nameof(expr), $"Unhandled type: {expr.GetType().FullName}");
            }
        }
    }
}