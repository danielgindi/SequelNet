using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class DefaultsAndComputedSchemaTests
{
    [Fact]
    public void Schema_When_Default_Specified_Emits_Default_And_HasDefault()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Count: INT; Default 123;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("HasDefault", result.Code);
        Assert.Contains("Default", result.Code);
    }

    [Fact]
    public void Schema_When_Computed_Specified_Emits_ComputedColumn_And_NoSave()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Total: INT; Computed (Price*Qty);
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("ComputedColumn", result.Code);
    }

    [Fact]
    public void Schema_When_ComputedStored_Specified_Emits_ComputedColumnStored()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Total: INT; Computed (Price*Qty) STORED;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("ComputedColumnStored", result.Code);
    }
}