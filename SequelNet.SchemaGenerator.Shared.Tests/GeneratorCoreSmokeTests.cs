using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests;

public class GeneratorCoreSmokeTests
{
    [Fact]
    public void GenerateDalClass_Emits_Record_And_Collection_ByDefault()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public partial class MyTable :", result.Code);
        Assert.Contains("public partial class MyTableCollection", result.Code);
    }

    [Fact]
    public void GenerateDalClass_Emits_FetchById_When_PrimaryKeyExists()
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
    public void GenerateDalClass_Emits_ModifiedOn_Update_When_ColumnExists()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
ModifiedOn: DATETIME_UTC;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("ModifiedOn = DateTime.UtcNow;", result.Code);
    }
}