using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Utils;
using Table = SimpleDatabase.Parsing.Statements.Table;

namespace SimpleDatabase.Planning
{
    public class Planner
    {
        private readonly Database _database;

        public Planner(Database database)
        {
            _database = database;
        }

        public Plan Plan(Statement statement)
        {
            switch (statement)
            {
                case SelectStatement select:
                    {
                        Node root;

                        var index = TryFindIndex(select.Table, select.Ordering);
                        if (index.HasValue)
                        {
                            root = new ScanIndexNode(((Table.TableName)select.Table).Name, index.Value!.Name);
                        }
                        else
                        {
                            root = new ScanTableNode(((Table.TableName)select.Table).Name);
                        }

                        root = select.Where.Map(
                            pred => new FilterNode(root, pred),
                            () => root
                        );

                        root = new ProjectionNode(root, select.Columns);

                        if (!index.HasValue && select.Ordering.Any())
                        {
                            root = new SortNode(root, select.Ordering);
                        }

                        return new Plan(root);
                    }

                case InsertStatement insert:
                    {
                        var columns = insert.Columns;
                        if (!columns.Any())
                        {
                            var table = _database.GetTable(insert.Table);
                            columns = table.Columns.Select(x => x.Name).ToList();
                        }

                        var input = new ConstantNode(columns, insert.Values);

                        var root = new InsertNode(insert.Table, input);

                        return new Plan(root);
                    }

                case DeleteStatement delete:
                    {
                        Node root = new ScanTableNode(delete.Table);
                        root = delete.Where.Map(
                            pred => new FilterNode(root, pred),
                            () => root
                        );
                        root = new DeleteNode(root);

                        return new Plan(root);
                    }

                default: throw new ArgumentOutOfRangeException(nameof(statement), $"Unhandled type: {statement.GetType().FullName}");
            }
        }

        private Option<TableIndex> TryFindIndex(Table selectTable, IReadOnlyList<OrderExpression> ordering)
        {
            if (!ordering.Any())
            {
                return Option.None<TableIndex>();
            }

            var tableName = ((Table.TableName)selectTable).Name;
            var table = _database.GetTable(tableName);

            foreach (var index in table.Indexes)
            {
                if (IndexMatch(index, ordering))
                {
                    return Option.Some(index);
                }
            }

            return Option.None<TableIndex>();
        }

        private bool IndexMatch(TableIndex index, IReadOnlyList<OrderExpression> ordering)
        {
            var oi = 0;
            foreach (var key in index.Structure.Keys)
            {
                if (!KeyMatch(key, ordering[oi]))
                {
                    continue;
                }

                if (++oi == ordering.Count) return true;
            }

            return oi == ordering.Count;
        }

        private bool KeyMatch((Column, KeyOrdering) key, OrderExpression ordering)
        {
            if (key.Item2 == KeyOrdering.Ascending && ordering.Order == Order.Descending) return false;
            if (key.Item2 == KeyOrdering.Descending && ordering.Order == Order.Ascending) return false;

            var column = (ColumnNameExpression) ordering.Expression;

            return key.Item1.Name == column.Name;
        }
    }
}
