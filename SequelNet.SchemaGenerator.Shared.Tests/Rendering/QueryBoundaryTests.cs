using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class QueryBoundaryTests
{
    [Fact]
    public void QueryBoundaryColumn_EmitsQueryBoundaryMetadata()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
TenantId: QueryBoundary; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("schema.QueryBoundaryColumns = new string[] { Columns.TenantId };", result.Code);
    }

    [Fact]
    public void QueryBoundaryColumn_IsIncludedInUpdateAndFetchByIdQueries()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
TenantId: QueryBoundary; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("qry.Where(Columns.Id, Id).AND(Columns.TenantId, TenantId);", result.Code);
        Assert.Contains("FetchByIdAsync(Int64 id, int tenantId", result.Code);
        Assert.Contains(".AND(Columns.TenantId, tenantId)", result.Code);
    }

    [Fact]
    public void QueryBoundaryColumn_ThatIsAlsoAPrimaryKey_IsNotDuplicatedInFetchById()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
TenantId: PRIMARY KEY; QueryBoundary; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("FetchByIdAsync(Int64 id, Int64 tenantId", result.Code);
        Assert.DoesNotContain("FetchByIdAsync(Int64 id, Int64 tenantId, Int64 tenantId", result.Code);
    }
}