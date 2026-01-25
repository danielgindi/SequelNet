using System;
using Xunit;
using SequelNet.SchemaGenerator.Shared.Tests.Fixtures;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class SchemaRenderingTests
{
    [Fact]
    public void GenerateDalClass_Emits_TableSchema_With_Basic_Flow()
    {
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(Scripts.Minimal());
        var code = CodeAssert.NormalizeNewlines(result.Code);

        CodeAssert.ContainsInOrder(code,
            "public override TableSchema GenerateTableSchema()",
            "if (null == _Schema)",
            "TableSchema schema = new TableSchema();",
            "schema.Name =",
            "schema.AddColumn(new TableSchema.Column",
            "return _Schema;");
    }

    [Fact]
    public void GenerateDalClass_When_AtomicUpdatesEnabled_Emits_MarkColumnMutated_In_Setter()
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

        Assert.Contains("static MyTable()", code);
        Assert.Contains("AtomicUpdates = true;", code);
        Assert.Contains("MarkColumnMutated(Columns.Name)", code);
    }

    [Fact]
    public void GenerateDalClass_When_AtomicUpdatesDisabled_DoesNotEmit_MarkColumnMutated_In_Setter()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        var code = CodeAssert.NormalizeNewlines(result.Code);

        Assert.DoesNotContain("static MyTable()", code);
        Assert.DoesNotContain("AtomicUpdates = true;", code);
        Assert.DoesNotContain("MarkColumnMutated(Columns.Name)", code);
    }
}