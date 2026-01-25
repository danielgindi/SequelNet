using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class ComponentModelAttributeTests
{
    [Fact]
    public void ComponentModel_Emits_Key_For_PrimaryKey()
    {
        var script = @"
MyTable
dbo.MyTable
@ComponentModel
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("DataAnnotations.Key", result.Code);
    }

    [Fact]
    public void ComponentModel_Emits_Required_For_NonNullable()
    {
        var script = @"
MyTable
dbo.MyTable
@ComponentModel
Id: PRIMARY KEY; INT;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("DataAnnotations.Required", result.Code);
    }

    [Fact]
    public void ComponentModel_DoesNotEmit_Required_For_Nullable()
    {
        var script = @"
MyTable
dbo.MyTable
@ComponentModel
Name: STRING(50); NULLABLE;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.DoesNotContain("DataAnnotations.Required", result.Code);
    }

    [Fact]
    public void ComponentModel_Emits_MaxLength_For_String_With_MaxLength()  
    {
        var script = @"
MyTable
dbo.MyTable
@ComponentModel
Id: PRIMARY KEY; INT;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("DataAnnotations.MaxLength(50)", result.Code);
    }
}
