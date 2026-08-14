using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests;

public class GeneratorCoreSmokeTests
{
    [Fact]
    public void GenerateDalClass_Emits_Record_And_Collection_ByDefault()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public partial class MyTable :", result.Code);
        Assert.Contains("public partial class MyTableCollection", result.Code);
    }

    [Fact]
    public void GenerateDalClass_Emits_FetchById_When_PrimaryKeyExists()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("FetchById(", result.Code);
        Assert.Contains("FetchByIdAsync(", result.Code);
    }

    [Fact]
    public void GenerateDalClass_Emits_ModifiedOn_Update_When_ColumnExists()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
ModifiedOn: DATETIME_UTC;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("ModifiedOn = DateTime.UtcNow;", result.Code);
    }
    [Fact]
    public void GenerateDalClass_Ignores_A_Dash_Before_A_BlockComment_ClosingDelimiter()
    {
        var script = @"
/*
 * LegacyImportRun
 * legacy_import_run
 * Id: PRIMARY KEY; INT64;
- */";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        var first = SequelNet.SchemaGenerator.GeneratedRegion.CreateOrUpdateAfterMacro(script, script.Length, result.Code, result.Context.ClassName);
        var firstDocument = script.Substring(0, first.Start) + first.Text + script.Substring(first.Start + first.Length);
        var second = SequelNet.SchemaGenerator.GeneratedRegion.CreateOrUpdateAfterMacro(firstDocument, script.Length, result.Code, result.Context.ClassName);
        var secondDocument = firstDocument.Substring(0, second.Start) + second.Text + firstDocument.Substring(second.Start + second.Length);

        Assert.Contains("public partial class LegacyImportRun :", result.Code);
        Assert.Equal(firstDocument, secondDocument);
    }
}