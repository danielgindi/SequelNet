using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class EnumOutputTests
{
    [Fact]
    public void EnumBlock_Emits_Public_Enum_With_Items()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Status: INT; StatusEnum:
""StatusEnum""
- Active
- Inactive
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public enum StatusEnum", result.Code);
        Assert.Contains("Active,", result.Code);
        Assert.Contains("Inactive,", result.Code);
    }

    [Fact]
    public void EnumColumn_Uses_EnumTypeName_In_Property_Type_When_ActualType_Not_Overridden()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Status: INT; StatusEnum:
""StatusEnum""
- A
- B
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Property name is Status, type likely StatusEnum.
        Assert.Contains("StatusEnum Status", result.Code);
    }
}
