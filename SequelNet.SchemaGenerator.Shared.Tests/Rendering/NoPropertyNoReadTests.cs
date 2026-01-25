using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class NoPropertyNoReadTests
{
    [Fact]
    public void NoProperty_DoesNot_Emit_Public_Property_For_Column()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Secret: STRING(50); NoProperty;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Property should not be generated.
        Assert.DoesNotContain("public string Secret", result.Code);

        // But column is still part of Columns struct / schema.
        Assert.Contains("public const string Secret", result.Code);
    }

    [Fact]
    public void NoProperty_DoesNot_Emit_Backing_Field_For_Column()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Secret: STRING(50); NoProperty;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Backing field naming convention is _Secret.
        Assert.DoesNotContain("_Secret", result.Code);
    }

    [Fact]
    public void NoRead_DoesNot_Assign_Property_In_Read_Method()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Secret: STRING(50); NoRead;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Read(...) should not assign Secret.
        Assert.DoesNotContain("\nSecret =", result.Code);

        // But the property should still exist (since it's not NoProperty).
        Assert.Contains("public string Secret", result.Code);
    }

    [Fact]
    public void NoRead_DoesNot_Exclude_Column_From_Schema()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Secret: STRING(50); NoRead;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public const string Secret", result.Code);
        Assert.Contains("schema.AddColumn(new TableSchema.Column", result.Code);
    }

    [Fact]
    public void NoProperty_And_NoRead_Together_Still_Emits_Column_In_Schema_But_No_Property_Or_Read_Assignment()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Secret: STRING(50); NoProperty; NoRead;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.DoesNotContain("public string Secret", result.Code);
        Assert.Contains("public const string Secret", result.Code);
    }
}
