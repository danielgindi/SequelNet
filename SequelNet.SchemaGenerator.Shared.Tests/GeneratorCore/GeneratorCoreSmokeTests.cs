using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.GeneratorCore;

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
    public void GenerateDalClass_Warnings_Action_Is_Invoked()
    {
        var script = @"
MyTable
my_table
@Index: NAME(IX_Bad); [DoesNotExist]
Id: PRIMARY KEY; INT;
";

        var warningsSeen = 0;
        var code = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script, _ => warningsSeen++);

        Assert.NotEmpty(code);
        Assert.True(warningsSeen >= 1);
    }

    [Fact]
    public void GenerateDalClassResult_Includes_Warnings_And_Context()
    {
        var script = @"
MyTable
my_table
@Index: NAME(IX_Bad); [DoesNotExist]
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.NotNull(result.Context);
        Assert.NotNull(result.Warnings);
        Assert.True(result.Warnings.Count >= 1);
        Assert.False(string.IsNullOrWhiteSpace(result.Code));
    }
}