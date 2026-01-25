using System;
using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class SchemaPrimaryKeyOutputTests
{
    [Fact]
    public void GenerateDalClass_Emits_AddColumn_With_IsPrimaryKey_For_PrimaryKeyColumn()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        var code = result.Code.Replace("\r\n", "\n");

        ContainsInOrder(code,
            "schema.AddColumn(new TableSchema.Column",
            "Name",
            "Columns.Id",
            "IsPrimaryKey",
            "true");
    }

    private static void ContainsInOrder(string text, params string[] fragments)
    {
        var index = 0;
        foreach (var f in fragments)
        {
            var next = text.IndexOf(f, index, StringComparison.Ordinal);
            Assert.True(next >= 0, $"Expected to find fragment: {f}");
            index = next + f.Length;
        }
    }
}