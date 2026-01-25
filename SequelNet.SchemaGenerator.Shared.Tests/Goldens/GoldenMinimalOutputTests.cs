using Xunit;
using SequelNet.SchemaGenerator.Shared.Tests.Fixtures;

namespace SequelNet.SchemaGenerator.Shared.Tests.Goldens;

/// <summary>
/// A few "B" tests: intentionally brittle snapshot-ish checks on stable anchors.
/// Keep these few; don't expand unless you really want strict output stability.
/// </summary>
public class GoldenMinimalOutputTests
{
    [Fact]
    public void MinimalOutput_Contains_Standard_Regions()
    {
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(Scripts.Minimal());
        var code = CodeAssert.NormalizeNewlines(result.Code);

        // Semi-golden anchors that should be stable across refactors.
        Assert.Contains("#region Table Schema", code);
        Assert.Contains("#region Properties", code);
        Assert.Contains("#region Helpers", code);
    }

    [Fact]
    public void AtomicUpdates_Output_Contains_StaticConstructorRegion()
    {
        var script = @"
MyTable
my_table
@AtomicUpdates
Id: PRIMARY KEY; INT;
Name: STRING(50);
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        var code = CodeAssert.NormalizeNewlines(result.Code);

        Assert.Contains("#region Static Constructor", code);
        Assert.Contains("static MyTable()", code);
    }
}