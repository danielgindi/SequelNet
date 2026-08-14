using SequelNet;

namespace Tests;

public class QueryBoundaryTests
{
    private Action<QueryBoundaryViolation>? _originalHandler;
    private Func<Query, bool>? _originalPredicate;

    [SetUp]
    public void SetUp()
    {
        _originalHandler = Query.QueryBoundaryViolationHandler;
        _originalPredicate = Query.QueryBoundaryValidationPredicate;
    }

    [TearDown]
    public void TearDown()
    {
        Query.QueryBoundaryViolationHandler = _originalHandler;
        Query.QueryBoundaryValidationPredicate = _originalPredicate;
    }

    [Test]
    public void ValidateQueryBoundary_ReportsOnlyColumnsMissingFromWhere()
    {
        QueryBoundaryViolation? violation = null;
        Query.QueryBoundaryViolationHandler = value => violation = value;

        var query = CreateQuery().Where("id", 10);
        query.ValidateQueryBoundary();

        Assert.That(violation, Is.Not.Null);
        Assert.That(violation!.MissingColumns, Is.EquivalentTo(new[] { "tenant_id" }));
        Assert.That(violation.Query, Is.SameAs(query));
    }

    [Test]
    public void ValidateQueryBoundary_AllowsScopeColumnsInNestedWhere()
    {
        Query.QueryBoundaryViolationHandler = _ => Assert.Fail("All query-boundary WHERE columns are present.");

        var nestedWhere = new WhereList().Where("tenant_id", 20);
        CreateQuery().Where("id", 10).AND(nestedWhere).ValidateQueryBoundary();
    }

    [Test]
    public void IsColumnInWhere_UsesNestedConditionsAndTableMatcher()
    {
        var where = new WhereList()
            .Where("other_people", "tenant_id", WhereComparison.EqualsTo, 30)
            .AND(new WhereList().Where("people", "tenant_id", WhereComparison.EqualsTo, 20));

        Assert.That(where.IsColumnInWhere("tenant_id", table => table == "people"), Is.True);
        Assert.That(where.IsColumnInWhere("tenant_id", table => table == "organizations"), Is.False);
    }

    [TestCase(QueryMode.Select)]
    [TestCase(QueryMode.Update)]
    [TestCase(QueryMode.Delete)]
    public void ValidateQueryBoundary_RequiresWhereColumnsForQueriesWithWhere(QueryMode queryMode)
    {
        Query.QueryBoundaryViolationHandler = _ => Assert.Fail("All query-boundary WHERE columns are present.");

        var query = CreateQuery().Where("id", 10).AND("tenant_id", 20);
        query.QueryMode = queryMode;
        query.ValidateQueryBoundary();
    }

    [Test]
    public void ValidateQueryBoundary_RequiresInsertAssignmentsForInsert()
    {
        Query.QueryBoundaryViolationHandler = _ => Assert.Fail("All query-boundary insert columns are present.");

        CreateQuery().Insert("id", 10).Insert("tenant_id", 20).ValidateQueryBoundary();
    }

    [Test]
    public void ValidateQueryBoundary_RequiresInsertAssignmentsForUpsert()
    {
        Query.QueryBoundaryViolationHandler = _ => Assert.Fail("All query-boundary insert columns are present.");

        CreateQuery().Insert("id", 10).Insert("tenant_id", 20).InsertOrUpdate().ValidateQueryBoundary();
    }

    [Test]
    public void IgnoreQueryBoundary_SkipsValidationForThisQueryOnly()
    {
        Query.QueryBoundaryViolationHandler = _ => Assert.Fail("The query explicitly opted out of validation.");

        CreateQuery().Where("id", 10).IgnoreQueryBoundary().ValidateQueryBoundary();
    }

    private static Query CreateQuery()
    {
        return new Query(new TableSchema("people", null)
        {
            QueryBoundaryColumns = new[] { "id", "tenant_id" }
        });
    }
}