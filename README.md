# Database

A toy database engine implemented in C# to explore how an RDBMS is implemented.
Safe to say: don't use this in production! 

## Features

- REPL (at least a basic one)
- SELECT
  - * / column as alias
  - FROM a single table
  - optional WHERE (literal, column, equality)
  - optional ORDER BY (ASC/DESC)
- INSERT INTO
  - optional list of columns
  - list of value tuples
- DELETE
  - FROM a table
  - optional WHERE
- EXPLAIN

### Unsupported

- DDL (table schemas are defined through code)
- Friendly API (just a bunch of low level ones)

## Inspired by
- https://cstack.github.io/db_tutorial/
- https://www.sqlite.org/
