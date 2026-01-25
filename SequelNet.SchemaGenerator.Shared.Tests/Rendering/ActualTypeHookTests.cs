using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class ActualTypeHookTests
{
    [Fact]
    public void ActualType_Directive_Changes_Generated_Property_Type()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Payload: JSON; ActualType System.Text.Json.JsonDocument;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Semantic: ActualType should be used in the property signature.
        Assert.Contains("System.Text.Json.JsonDocument Payload", result.Code);
    }

    [Fact]
    public void ActualType_Directive_Changes_Backing_Field_Type()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Payload: JSON; ActualType System.Text.Json.JsonDocument;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Backing field uses the effective type chosen by GetClrTypeName(dalColumn, context).
        Assert.Contains("internal System.Text.Json.JsonDocument _Payload", result.Code);
    }

    [Fact]
    public void ActualType_Directive_Is_Used_In_TableSchema_Typeof()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Payload: JSON; ActualType System.Text.Json.JsonDocument;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Schema column should use typeof(<effectiveType>) where effectiveType is ActualType.
        Assert.Contains("typeof(System.Text.Json.JsonDocument)", result.Code);
    }

    [Fact]
    public void ActualType_With_NullableEnabled_Can_Emit_Nullable_ActualType_For_ValueType()
    {
        var script = @"
MyTable
dbo.MyTable
@NullableEnabled
Id: PRIMARY KEY; INT;
Count: INT; NULLABLE; ActualType int;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // With NullableEnabled, nullable value types should become int? (unless the generator treats it differently).
        Assert.Contains("int? Count", result.Code);
        Assert.Contains("internal int? _Count", result.Code);
        Assert.Contains("typeof(int?)", result.Code);
    }
}