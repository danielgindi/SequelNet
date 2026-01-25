using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class ComputedAndLiteralTypeTests
{
    [Fact]
    public void LiteralType_Emits_LiteralType_Value_In_Schema()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Raw: LITERALTYPE System.Buffers.ReadOnlySequence<byte>;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("LiteralType", result.Code);
        Assert.Contains("System.Buffers.ReadOnlySequence<byte>", result.Code);
    }

    [Fact]
    public void ComputedColumn_Emits_ComputedColumn_Metadata()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Total: INT; Computed (Price*Qty);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("ComputedColumn", result.Code);
    }
}
