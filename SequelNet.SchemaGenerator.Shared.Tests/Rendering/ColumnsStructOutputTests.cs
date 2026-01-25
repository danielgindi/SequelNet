using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class ColumnsStructOutputTests
{
    [Fact]
    public void GenerateDalClass_Emits_ColumnsStruct_With_Const_Column_Names_ByDefault()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
UserName: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public struct Columns", result.Code);
        Assert.Contains("public const string Id = \"Id\";", result.Code);
        Assert.Contains("public const string UserName = \"UserName\";", result.Code);
    }

    [Fact]
    public void GenerateDalClass_When_StaticColumnsDirectivePresent_Emits_Static_Columns()
    {
        var script = @"
MyTable
my_table
@StaticColumns
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public static string Id = \"Id\";", result.Code);
        Assert.DoesNotContain("public const string Id", result.Code);
    }

    [Fact]
    public void ColumnsStruct_Maps_PropertyName_To_ColumnName_When_Custom_ColumnName_Given()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT; ColumnName my_id;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public const string Id = \"my_id\";", result.Code);
    }
}