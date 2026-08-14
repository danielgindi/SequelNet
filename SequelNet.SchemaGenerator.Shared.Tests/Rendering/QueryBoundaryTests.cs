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
}