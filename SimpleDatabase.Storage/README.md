## Folder layout

```
[database]/
    database
    [table].tbl
```

## `database` file

Contains `DatabaseMetadata`, `Schema`, and `SchemaContinuation` pages.

Page 0 is always a `DatabaseMetadata` page that contains 0 tables and points to the actual first database metadata page.

## `[table].tbl` file

A file for each table, contains `TableMetaData`, `Heap`, `Leaf`, and `Internal` pages.

Page 0 is always a `TableMetaData` page that points at the starting point of the other pages types.