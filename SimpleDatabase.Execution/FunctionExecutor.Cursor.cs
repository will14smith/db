using System;
using System.Linq;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Nodes;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private static (FunctionState, Result) Execute(FunctionState state, OpenReadOperation openReadOperation)
        {
            // TODO aquire read lock
            var cursor = new CursorValue(openReadOperation.Table, false);
            state = state.PushValue(cursor);
            return (state, new Result.Next());
        }

        private static (FunctionState, Result) Execute(FunctionState state, OpenWriteOperation openWriteOperation)
        {
            // TODO aquire write lock
            var cursor = new CursorValue(openWriteOperation.Table, false);
            state = state.PushValue(cursor);
            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, FirstOperation firstOperation)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var searcher = new TreeTraverser(_pager, CreateRowSerializer(cursorValue.Table), cursorValue.Table);
            var newCursor = searcher.StartCursor();

            var newCursorValue = cursorValue.SetCursor(newCursor);
            state = state.PushValue(newCursorValue);

            if (!newCursor.EndOfTable)
            {
                return (state, new Result.Next());
            }

            return (state, new Result.Jump(firstOperation.EmptyAddress));
        }

        private (FunctionState, Result) Execute(FunctionState state, NextOperation nextOperation)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var cursor = cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));

            var newCursor = cursorValue.NextCursor.OrElse(() => Next(cursorValue.Table, cursor));
            var newCursorValue = cursorValue.SetCursor(newCursor);
            state = state.PushValue(newCursorValue);

            if (newCursor.EndOfTable)
            {
                return (state, new Result.Next());
            }

            return (state, new Result.Jump(nextOperation.SuccessAddress));
        }

        private (FunctionState, Result) Execute(FunctionState state, InsertOperation insert)
        {
            RowValue row;
            CursorValue cursorValue;

            (state, cursorValue) = state.PopValue<CursorValue>();
            (state, row) = state.PopValue<RowValue>();

            var insertableRow = new Row(row.Values.Cast<ObjectValue>().Select(x => new ColumnValue(x.Value)).ToList());

            var inserter = new TreeInserter(_pager, CreateRowSerializer(cursorValue.Table), cursorValue.Table);
            var insertResult = inserter.Insert(GetKey(insertableRow), insertableRow);
            switch (insertResult)
            {
                case TreeInsertResult.Success _:
                    return (state, new Result.Next());

                case TreeInsertResult.DuplicateKey _:
                    throw new NotImplementedException("TODO error handling!");

                default:
                    throw new NotImplementedException($"Unsupported type: {insertResult.GetType().Name}");
            }
        }

        private (FunctionState, Result) Execute(FunctionState state, DeleteOperation delete)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var table = cursorValue.Table;
            var rowSerializer = CreateRowSerializer(table);

            var cursor = cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));
            
            // TODO could this be optimised to not retraverse the tree in TreeDeleter?
            var key = GetKey(table, cursor);

            var nextCursor = Next(table, cursor);
            var nextKey = nextCursor.EndOfTable ? Option.None<int>() : Option.Some(GetKey(table, nextCursor));

            var deleter = new TreeDeleter(_pager, rowSerializer, table);
            var deleteResult = deleter.Delete(key);

            switch (deleteResult)
            {
                case TreeDeleteResult.Success _:
                    {
                        if (nextKey.HasValue)
                        {
                            var newCursor = FindKey(table, nextKey.Value);
                            var newCursorValue = cursorValue.SetNextCursor(newCursor);
                            state = state.PushValue(newCursorValue);
                        }
                        else
                        {
                            throw new NotImplementedException("TODO handle EOT");
                        }

                        return (state, new Result.Next());
                    }

                case TreeDeleteResult.KeyNotFound _:
                    throw new NotImplementedException("TODO error handling!");

                default:
                    throw new NotImplementedException($"Unsupported type: {deleteResult.GetType().Name}");
            }
        }

        private Cursor FindKey(StoredTable table, int key)
        {
            var rowSerializer = CreateRowSerializer(table);

            var searcher = new TreeSearcher(_pager, new TreeKeySearcher(key), rowSerializer);

            return searcher.FindCursor(table.RootPageNumber);
        }

        private int GetKey(Row row)
        {
            var value0 = row.Values[0].Value;

            if (value0 is int i) return i;

            throw new NotImplementedException();
        }

        private int GetKey(StoredTable table, Cursor cursor)
        {
            var rowSerializer = CreateRowSerializer(table);

            var page = _pager.Get(cursor.PageNumber);
            var leaf = LeafNode.Read(rowSerializer, page);

            return leaf.GetCellKey(cursor.CellNumber);
        }
        private Cursor Next(StoredTable table, Cursor cursor)
        {
            var rowSerializer = CreateRowSerializer(table);

            var searcher = new TreeTraverser(_pager, rowSerializer, table);
            return searcher.AdvanceCursor(cursor);
        }
    }
}
