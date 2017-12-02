using System;
using System.Linq;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;

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

            var cursor = cursorValue.Cursor;
            if (cursor == null)
            {
                throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?");
            }

            var searcher = new TreeTraverser(_pager, CreateRowSerializer(cursorValue.Table), cursorValue.Table);
            var newCursor = searcher.AdvanceCursor(cursor);

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
            var insertResult = inserter.Insert((int)insertableRow.Values[0].Value, insertableRow);
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
    }
}
