using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class IndexRenderingTests
{
    [Fact]
    public void GenerateDalClass_When_IndexDirectivePresent_Emits_AddIndex_In_Schema()
    {
        var script = @"
MyTable
my_table
@Index: NAME(IX_MyTable_Name); [Name ASC]
Id: PRIMARY KEY; INT;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("schema.AddIndex(", result.Code);
        Assert.Contains("IX_MyTable_Name", result.Code);
        Assert.Contains("SortDirection.ASC", result.Code);
    }
}