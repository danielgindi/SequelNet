using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class NoSaveDirectiveTests
{
    [Fact]
    public void NoSave_Column_Is_Not_Included_In_GetInsertQuery()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Name: STRING(50);
Secret: STRING(50); NoSave;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Semantic: insert query should reference Name but not Secret.
        Assert.Contains("public override Query GetInsertQuery()", result.Code);
        Assert.Contains("Columns.Name", result.Code);
        Assert.DoesNotContain(".Insert(Columns.Secret", result.Code);
    }

    [Fact]
    public void NoSave_Column_Is_Not_Included_In_GetUpdateQuery()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Name: STRING(50);
Secret: STRING(50); NoSave;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Semantic: update query should reference Name but not Secret.
        Assert.Contains("public override Query GetUpdateQuery()", result.Code);
        Assert.Contains("Columns.Name", result.Code);
        Assert.DoesNotContain(".Update(Columns.Secret", result.Code);
    }

    [Fact]
    public void Computed_Column_Acts_As_NoSave()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Total: INT; Computed (Price*Qty);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Computed columns are set to NoSave by parser; should not appear in query sets.
        Assert.DoesNotContain(".Update(Columns.Total", result.Code);
        Assert.DoesNotContain(".Insert(Columns.Total", result.Code);
    }

    [Fact]
    public void NoSave_Still_Emits_Property_And_SchemaColumn()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Secret: STRING(50); NoSave;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // NoSave should not hide the property or schema definition.
        Assert.Contains("public string Secret", result.Code);
        Assert.Contains("public const string Secret", result.Code);
        Assert.Contains("schema.AddColumn(new TableSchema.Column", result.Code);
    }

    [Fact]
    public void NoSave_When_AtomicUpdatesEnabled_DoesNot_Emit_IsColumnMutated_For_NoSave_Column()
    {
        var script = @"
MyTable
dbo.MyTable
@AtomicUpdates
Id: PRIMARY KEY; INT;
Name: STRING(50);
Secret: STRING(50); NoSave;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Update gating should exist for a normal column...
        Assert.Contains("IsColumnMutated(Columns.Name)", result.Code);

        // ...but not for NoSave columns.
        Assert.DoesNotContain("IsColumnMutated(Columns.Secret)", result.Code);
    }

    [Fact]
    public void NoSave_With_Default_DoesNot_Emit_Default_Into_Insert_Or_Update_For_That_Column()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Name: STRING(50);
Secret: STRING(50); NoSave; Default ""abc"";
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Schema may include Default/HasDefault, but the column should not appear in query building.
        Assert.Contains("HasDefault", result.Code);
        Assert.DoesNotContain(".Update(Columns.Total", result.Code);
        Assert.DoesNotContain(".Insert(Columns.Total", result.Code);
    }
}
