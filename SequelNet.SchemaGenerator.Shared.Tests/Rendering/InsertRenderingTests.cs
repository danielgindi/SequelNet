using System;
using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class InsertRenderingTests
{
    [Fact]
    public void Insert_When_CreatedOn_Exists_Emits_UtcNow_Assignment()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
CreatedOn: DATETIME_UTC;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public override Query GetInsertQuery()", result.Code);
        Assert.Contains("CreatedOn = DateTime.UtcNow;", result.Code);
    }

    [Fact]
    public void Insert_When_NoCreatedOnDirective_DoesNot_Emit_CreatedOn_Assignment()
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
    public void Insert_CustomBeforeInsert_Is_Emitted()
    {
        var script = @"
MyTable
my_table
@BeforeInsert: DoStuff();
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("DoStuff();", result.Code);
    }

    [Fact]
    public void Insert_CustomAfterInsertQuery_Is_Emitted()
    {
        var script = @"
MyTable
my_table
@AfterInsertQuery: qry.AND(Columns.Id, 1);
Id: PRIMARY KEY; INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("qry.AND(Columns.Id, 1);", result.Code);
    }
}