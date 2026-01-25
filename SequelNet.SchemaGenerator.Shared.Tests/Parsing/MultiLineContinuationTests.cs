using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Parsing;

public class MultiLineContinuationTests
{
    [Fact]
    public void Backslash_Continuation_Joins_Lines_For_Directives()
    {
        var script = @"
MyTable
dbo.MyTable
@ForeignKey: NAME(FK_Test); \
FOREIGNTABLE(OtherTable); COLUMNS[OtherId]; FOREIGNCOLUMNS[Id]
Id: PRIMARY KEY; INT;
OtherId: INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("schema.AddForeignKey(", result.Code);
        Assert.Contains("FK_Test", result.Code);
    }
}
