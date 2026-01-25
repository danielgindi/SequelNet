using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class MySqlEngineTests
{
    [Theory]
    [InlineData("InnoDB")]
    [InlineData("MyISAM")]
    [InlineData("ARCHIVE")]
    public void MySqlEngineDirective_Emits_SetMySqlEngine(string engine)
    {
        var script = $@"
MyTable
dbo.MyTable
@MySqlEngine: {engine}
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains($"schema.SetMySqlEngine(MySqlEngineType.{engine});", result.Code);
    }
}
