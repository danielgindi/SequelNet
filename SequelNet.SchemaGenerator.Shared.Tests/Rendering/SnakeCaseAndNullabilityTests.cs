using Xunit;
using SequelNet.SchemaGenerator.Shared.Tests.Fixtures;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class SnakeCaseAndNullabilityTests
{
    [Fact]
    public void SnakeColumnNames_Directive_Snakes_Column_Names_In_Columns_Struct_When_Not_CustomNamed()
    {
        var script = @"
MyTable
my_table
@SnakeColumnNames
Id: PRIMARY KEY; INT;
UserName: STRING(50);
";
    
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // ColumnName should be rewritten; property name remains UserName.
        Assert.Contains("public const string UserName = \"user_name\";", result.Code);
    }

    [Fact]
    public void SnakeColumnNames_DoesNot_Change_Custom_ColumnName()
    {
        var script = @"
MyTable
my_table
@SnakeColumnNames
UserName: STRING(50); ColumnName customUser;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public const string UserName = \"customUser\";", result.Code);
        Assert.DoesNotContain("\"user_name\"", result.Code);
    }

    [Fact]
    public void NullableEnabled_Directive_Emits_Nullability_In_Method_Signatures()
    {
        var script = @"
MyTable
dbo.MyTable
@NullableEnabled
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // GetPrimaryKeyValue is declared as object? when nullable enabled.
        Assert.Contains("public override object? GetPrimaryKeyValue()", result.Code);
    }

    [Fact]
    public void NullableEnabled_Directive_Emits_Nullable_ConnectorBase_Parameters()
    {
        var script = @"
MyTable
dbo.MyTable
@NullableEnabled
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // FetchById(..., ConnectorBase? conn = null)
        Assert.Contains("ConnectorBase? conn = null", result.Code);
    }
}
