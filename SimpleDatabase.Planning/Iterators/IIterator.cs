using System;
using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Schemas.Types;

namespace SimpleDatabase.Planning.Iterators
{
    /// <summary>
    /// Used by
    ///     Init(f)
    /// s:  load Outputs
    ///     Yield [Outputs.count]
    ///     MoveNext(s)
    /// f:  Finished
    /// </summary>
    public interface IIterator
    {
        /// <summary>
        /// Defines the slots required by this iterator
        /// </summary>
        IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots { get; }
        /// <summary>
        /// Describes the outputs of the iterator (name & type)
        /// </summary>
        IReadOnlyList<IteratorOutput> Outputs { get; }

        /// <summary>
        /// Sets up the iterator
        /// When the iterator is empty it should jump to the label
        /// </summary>
        IEnumerable<IOperation> Init(ProgramLabel emptyTarget);
        /// <summary>
        /// Moves to the next item in the iterator
        /// If there are more items it should jump to the label
        /// </summary>
        IEnumerable<IOperation> MoveNext(ProgramLabel loopStartTarget);
    }

    public class IteratorOutput
    {
        public IteratorOutputName Name { get; }
        public ColumnType Type { get; }
        public IReadOnlyCollection<IOperation> LoadOperations { get; }

        public IteratorOutput(IteratorOutputName name, ColumnType type, IReadOnlyCollection<IOperation> loadOperations)
        {
            Name = name;
            Type = type;
            LoadOperations = loadOperations;
        }
    }

    public abstract class IteratorOutputName
    {
        public abstract bool Matches(string name);

        public class TableColumn : IteratorOutputName
        {
            public string Table { get; }
            public string Column { get; }

            public TableColumn(string table, string column)
            {
                Table = table;
                Column = column;
            }

            public override bool Matches(string name)
            {
                // TODO table?
                return string.Equals(Column, name, StringComparison.OrdinalIgnoreCase);
            }
        }

        public class Expression : IteratorOutputName
        {
            public Parsing.Expressions.Expression Expr { get; }

            public Expression(Parsing.Expressions.Expression expr)
            {
                Expr = expr;
            }

            public override bool Matches(string name)
            {
                switch (Expr)
                {
                    case ColumnNameExpression column:
                        return name == column.Name;

                    default: throw new ArgumentOutOfRangeException(nameof(Expr), $"Unhandled type: {Expr.GetType().FullName}");
                }
            }
        }

        public class Constant : IteratorOutputName
        {
            public string Name { get; }

            public Constant(string name)
            {
                Name = name;
            }

            public override bool Matches(string name)
            {
                return string.Equals(Name, name, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}