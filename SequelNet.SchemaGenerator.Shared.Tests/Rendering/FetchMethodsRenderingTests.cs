using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class FetchMethodsRenderingTests
{
    [Fact]
    public void FetchMethods_Emits_FetchById_And_Async_Overloads_When_PK_Exists()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("FetchById(", result.Code);
        Assert.Contains("FetchByIdAsync(", result.Code);
    }

    [Fact]
    public void FetchMethods_When_IsDeletedColumnExists_Emits_IncludeDeleted_Overload_And_Filter()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
IsDeleted: BOOL;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("bool includeDeleted = false", result.Code);
        Assert.Contains("if (!includeDeleted)", result.Code);
        Assert.Contains("qry.AND(Columns.IsDeleted, false)", result.Code);
    }

    [Fact]
    public void DeleteHelpers_When_IsDeletedColumnExists_Uses_SoftDelete_Update()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
OtherId: INT; PRIMARY KEY;
IsDeleted: BOOL;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public static int Delete(", result.Code);
        Assert.Contains(".Update(Columns.IsDeleted, true)", result.Code);
    }
}