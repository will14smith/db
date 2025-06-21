using System;
using System.Linq;
using SimpleDatabase.Parsing;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Optimisation.Algebra;
using SimpleDatabase.Planning.Queries;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using Xunit;

namespace SimpleDatabase.Planning.UnitTests;

public class AlgebraTests
{
    [Theory]
    [InlineData("SELECT * FROM person", "Projection(Get(person), person.id, person.name, person.age)")]
    [InlineData("SELECT id, name FROM person", "Projection(Get(person), person.id, person.name)")]
    [InlineData("SELECT * FROM person WHERE id = 1", "Projection(Filter(Get(person), (person.id = 1)), person.id, person.name, person.age)")]
    [InlineData("SELECT * FROM person JOIN pet ON person.id = pet.ownerid", "Projection(Join(Get(person), Get(pet), (person.id = pet.ownerid)), person.id, person.name, person.age, pet.id, pet.ownerid, pet.name)")]
    public void Test(string queryText, string expected)
    {
        var database = new Database(new[]
        {
            new Table("person", new []
            {
                new Column("id", new ColumnType.Integer()),
                new Column("name", new ColumnType.String(255)),
                new Column("age", new ColumnType.Integer()),
            }, Array.Empty<TableIndex>()),
            new Table("pet", new []
            {
                new Column("id", new ColumnType.Integer()),
                new Column("ownerid", new ColumnType.Integer()),
                new Column("name", new ColumnType.String(255)),
            }, Array.Empty<TableIndex>()),
        });
        
        var query = Parser.Parse(queryText).Cast<SelectStatement>().Single();
        var resolvedQuery = QueryResolution.Resolve(query, database);
        
        var node = QueryToAlgebra.Build(resolvedQuery);
        
        Assert.Equal(expected, node.ToString());
    }
}