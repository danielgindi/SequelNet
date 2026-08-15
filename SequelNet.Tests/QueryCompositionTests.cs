using SequelNet;

namespace Tests;

public class QueryCompositionTests
{
    [Test]
    public void WhereList_WhereResetsExistingConditionsAndAndOrAppend()
    {
        var where = new WhereList()
            .AND("discarded", true)
            .Where("id", 10)
            .AND("tenant_id", 20)
            .OR("is_admin", true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(where, Has.Count.EqualTo(3));
            Assert.That(where[0].First, Is.EqualTo("id"));
            Assert.That(where[0].Condition, Is.EqualTo(WhereCondition.AND));
            Assert.That(where[1].Condition, Is.EqualTo(WhereCondition.AND));
            Assert.That(where[2].Condition, Is.EqualTo(WhereCondition.OR));
        }
    }

    [Test]
    public void Query_TracksModesAssignmentsAndConflictOptions()
    {
        var query = new Query(new TableSchema("orders", null));

        query.Insert("id", 1).Insert("total", 10).InsertOrUpdate();
        query.SetIgnoreErrors(true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(query.QueryMode, Is.EqualTo(QueryMode.InsertOrUpdate));
            Assert.That(query.HasInsertsOrUpdates, Is.True);
            Assert.That(query.GetInsertUpdateList(), Has.Count.EqualTo(2));
            Assert.That(query.IgnoreErrors, Is.True);
            Assert.That(query.OnConflictDoNothing, Is.Not.Null);
        }

        query.SetIgnoreErrors(false);
        Assert.That(query.IgnoreErrors, Is.False);
    }

    [Test]
    public void Query_SchemaAliasDoesNotReplaceSchemaNameAndPreservesExplicitName()
    {
        var query = new Query(new TableSchema("orders", null));

        query.SetSchemaAlias("o");
        Assert.That(query.SchemaName, Is.EqualTo("orders"));

        query.SetSchemaName("archived_orders").SetSchemaAlias("a");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(query.SchemaAlias, Is.EqualTo("a"));
            Assert.That(query.SchemaName, Is.EqualTo("archived_orders"));
        }
    }
}
