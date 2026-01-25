using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class PropertyNameCollisionTests
{
    [Fact]
    public void ColumnName_That_Matches_ClassName_Is_Suffixed_With_X()
    {
        var script = @"
MyTable
dbo.MyTable
MyTable: INT;
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // ParseScript appends X if PropertyName collides with class name.
        Assert.Contains("MyTableX", result.Code);
    }

    [Fact]
    public void ColumnName_That_Matches_Columns_Is_Suffixed_With_X()
    {
        var script = @"
MyTable
dbo.MyTable
Columns: INT;
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("ColumnsX", result.Code);
    }
}
