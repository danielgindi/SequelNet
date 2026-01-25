using Xunit;
using SequelNet.SchemaGenerator.Shared.Tests.Fixtures;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class SchemaOutputTests
{
    [Fact]
    public void Schema_Emits_GenerateTableSchema_Method()
    {
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(Scripts.Minimal());
        Assert.Contains("public override TableSchema GenerateTableSchema()", result.Code);
    }

    [Fact]
    public void Schema_Emits_SchemaName()
    {
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(Scripts.Minimal(schema: "dto.my_table"));
        Assert.Contains("schema.Name =", result.Code);
    }

    [Fact]
    public void Schema_Emits_DatabaseOwner_When_Dotted_Schema()
    {
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(Scripts.Minimal(schema: "dto.my_table"));
        Assert.Contains("schema.DatabaseOwner =", result.Code);
    }

    [Fact]
    public void Schema_DoesNotEmit_DatabaseOwner_When_Not_Dotted_Schema()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.DoesNotContain("schema.DatabaseOwner =", result.Code);
    }

    [Fact]
    public void Schema_Emits_ReturnSchema_Cache()
    {
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(Scripts.Minimal());
        Assert.Contains("return _Schema;", result.Code);
    }
}