using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class CompositeKeyTests
{
    [Fact]
    public void CompositePrimaryKey_Disables_SingleColumnPrimaryKeyValue()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
TenantId: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // When composite key, GetPrimaryKeyValue should return null (SingleColumnPrimaryKeyName empty).
        Assert.Contains("return null;", result.Code);
    }

    [Fact]
    public void CompositePrimaryKey_Emits_FetchById_With_Multiple_Params()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
TenantId: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("FetchById(", result.Code);
        Assert.Contains("Id", result.Code);
        Assert.Contains("TenantId", result.Code);
    }

    [Fact]
    public void CompositePrimaryKey_Emits_Delete_Helper_Methods()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
TenantId: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public static int Delete(", result.Code);
        Assert.Contains("DeleteAsync(", result.Code);
    }
}
