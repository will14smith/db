using System;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage.Nodes;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private (FunctionState, Result) Execute(FunctionState state, KeyOperation _)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var cursor = cursorValue.Cursor;
            if (cursor == null)
            {
                throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?");
            }

            var page = _pager.Get(cursor.PageNumber);
            var leaf = LeafNode.Read(CreateRowSerializer(cursorValue.Table), page);

            var key = leaf.GetCellKey(cursor.CellNumber);
            state.PushValue(new ObjectValue(key));

            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, ColumnOperation columnOperation)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var cursor = cursorValue.Cursor;
            if (cursor == null)
            {
                throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?");
            }

            var page = _pager.Get(cursor.PageNumber);
            var leaf = LeafNode.Read(CreateRowSerializer(cursorValue.Table), page);

            var value = leaf.GetCellColumn(cursor.CellNumber, columnOperation.ColumnIndex);
            state.PushValue(new ObjectValue(value));

            return (state, new Result.Next());
        }
    }
}
