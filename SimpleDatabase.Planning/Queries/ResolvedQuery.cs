using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Planning.Queries;

public class ResolvedQuery
{
    public SelectStatement Query { get; }
    public TableAliasLookup Tables { get; }

    public ResolvedQuery(SelectStatement query, TableAliasLookup tables)
    {
        Query = query;
        Tables = tables;
    }
}