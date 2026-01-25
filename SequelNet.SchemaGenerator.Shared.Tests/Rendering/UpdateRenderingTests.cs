using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class UpdateRenderingTests
{
    [Fact]
    public void Update_When_ModifiedOn_Exists_Emits_UtcNow_Assignment()
    {
        var script = @"
MyTable
my_table
@AtomicUpdates
Id: PRIMARY KEY; INT;
ModifiedOn: DATETIME_UTC;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Note: this is a semantic assertion about the *presence* of the optimization gate.
        Assert.Contains("HasMutatedColumns()", result.Code);
        Assert.Contains("ModifiedOn = DateTime.UtcNow;", result.Code);
    }

    [Fact]
    public void GenerateDalClass_When_AtomicUpdatesButNoModifiedOn_DoesNotEmit_HasMutatedColumns_Gate()
    {
        var script = @"
MyTable
my_table
@AtomicUpdates
Id: PRIMARY KEY; INT;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.DoesNotContain("HasMutatedColumns()", result.Code);
    }
}