using System;
using System.Linq;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private (FunctionState, Result) Execute(FunctionState state, OpenReadTableOperation openReadOperation)
        {
            // TODO aquire read lock
            var table = openReadOperation.Table;
            var tableCursor = new HeapCursor(_pager, CreateRowSerializer(table), table, false);

            var cursor = new CursorValue(false);
            cursor = cursor.SetNextCursor(tableCursor);

            state = state.PushValue(cursor);
            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, OpenReadIndexOperation openReadOperation)
        {
            // TODO aquire read lock
            var table = openReadOperation.Table;
            var index = openReadOperation.Index;
            var tableCursor = new IndexCursor(_pager, CreateRowSerializer(table), table, index, false);

            var cursor = new CursorValue(false);
            cursor = cursor.SetNextCursor(tableCursor);

            state = state.PushValue(cursor);
            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, OpenWriteOperation openWriteOperation)
        {
            // TODO aquire write lock
            var table = openWriteOperation.Table;
            var heapCursor = new HeapCursor(_pager, CreateRowSerializer(table), table, true);

            var cursor = new CursorValue(true);
            cursor = cursor.SetNextCursor(heapCursor);

            state = state.PushValue(cursor);
            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, FirstOperation _)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var cursor = cursorValue.Cursor.OrElse(() =>
            {
                return cursorValue.NextCursor.OrElse(() => throw new InvalidOperationException("Something is right dodgy with this cursor..."));
            });

            var newCursor = cursor.First();
            var newCursorValue = cursorValue.SetNextCursor(newCursor);
            state = state.PushValue(newCursorValue);

            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, NextOperation nextOperation)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var newCursor = cursorValue.NextCursor.OrElse(() =>
            {
                var cursor = cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));

                return cursor.Next();
            });

            var newCursorValue = cursorValue.SetCursor(newCursor);
            state = state.PushValue(newCursorValue);

            if (!newCursor.EndOfTable)
            {
                return (state, new Result.Next());
            }

            return (state, new Result.Jump(nextOperation.DoneAddress));
        }

        private (FunctionState, Result) Execute(FunctionState state, InsertOperation insert)
        {
            RowValue row;
            Value targetValue;

            (state, targetValue) = state.PopValue();
            (state, row) = state.PopValue<RowValue>();

            IInsertTarget insertTarget;
            switch (targetValue)
            {
                case CursorValue cursorValue:
                    {
                        var cursor = cursorValue.Cursor.OrElse(() =>
                        {
                            return cursorValue.NextCursor.OrElse(() => throw new InvalidOperationException("Something is right dodgy with this cursor..."));
                        });

                        insertTarget = (IInsertTarget)cursor;
                    }
                    break;
                case IInsertTarget target:
                    insertTarget = target;
                    break;

                default:
                    throw new NotImplementedException($"Unsupported type: {targetValue.GetType()}");
            }

            var values = row.Values.Cast<ObjectValue>().Select(x => new ColumnValue(x.Value)).ToList();
            var insertableRow = new Row(values, xid, Option.None<TransactionId>());

            var insertResult = insertTarget.Insert(insertableRow);
            switch (insertResult)
            {
                case InsertTargetResult.Success _:
                    return (state, new Result.Next());

                default:
                    throw new NotImplementedException($"Unsupported type: {insertResult.GetType().Name}");
            }
        }

        private (FunctionState, Result) Execute(FunctionState state, DeleteOperation delete)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var deleteTarget = cursorValue.Cursor.As<ICursor, IDeleteTarget>().OrElse(() => throw new InvalidOperationException("Cursor is null or not an IInsertTarget"));

            var deleteResult = deleteTarget.Delete();
            switch (deleteResult)
            {
                case DeleteTargetResult.Success success:
                    state = state.PushValue(cursorValue.SetNextCursor(success.NextCursor));
                    return (state, new Result.Next());

                default:
                    throw new NotImplementedException($"Unsupported type: {deleteResult.GetType().Name}");
            }
        }
    }
}
