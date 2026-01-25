using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Parsing;

public class DirectiveParsingTests
{
    [Fact]
    public void IndexDirective_Emits_AddIndex_Name_And_SortDirection()
    {
        var script = @"
MyTable
my_table
@Index: NAME(IX_MyTable_Name); UNIQUE; [Name ASC]
Id: PRIMARY KEY; INT;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("schema.AddIndex(", result.Code);
        Assert.Contains("IX_MyTable_Name", result.Code);
        Assert.Contains("SortDirection.ASC", result.Code);
    }

    [Fact]
    public void IndexDirective_Warns_When_Column_Not_Found()
    {
        var script = @"
MyTable
my_table
@Index: NAME(IX_Bad); [Nope]
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("schema.AddIndex(", result.Code);
        Assert.True(result.Warnings.Count >= 1);
    }

    [Fact]
    public void ForeignKeyDirective_Emits_AddForeignKey_And_References()
    {
        var script = @"
MyTable
my_table
@ForeignKey: NAME(FK_MyTable_OtherTable); FOREIGNTABLE(OtherTable); COLUMNS[OtherId]; FOREIGNCOLUMNS[Id]; ONDELETE(CASCADE); ONUPDATE(RESTRICT)
Id: PRIMARY KEY; INT;
OtherId: INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("schema.AddForeignKey(", result.Code);
        Assert.Contains("FK_MyTable_OtherTable", result.Code);
        Assert.Contains("TableSchema.ForeignKeyReference.Cascade", result.Code);
        Assert.Contains("TableSchema.ForeignKeyReference.Restrict", result.Code);
    }

    [Fact]
    public void ComponentModelDirective_Emits_DataAnnotations_For_Key_Required_MaxLength()
    {
        var script = @"
MyTable
my_table
@ComponentModel
Id: PRIMARY KEY; INT;
Name: STRING(50);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("DataAnnotations.Key", result.Code);
        Assert.Contains("DataAnnotations.Required", result.Code);
        Assert.Contains("DataAnnotations.MaxLength(50)", result.Code);
    }

    [Fact]
    public void NoCreatedOnDirective_Is_Recognized()
    {
        var script = @"
MyTable
my_table
@NoCreatedOn
Id: PRIMARY KEY; INT;
CreatedOn: DATETIME_UTC;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.DoesNotContain("\nCreatedOn = DateTime.UtcNow;", result.Code);
    }

    [Fact]
    public void NoModifiedOnDirective_Is_Recognized()
    {
        var script = @"
MyTable
my_table
@NoModifiedOn
Id: PRIMARY KEY; INT;
ModifiedOn: DATETIME_UTC;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.DoesNotContain("\nModifiedOn = DateTime.UtcNow;", result.Code);
    }
}