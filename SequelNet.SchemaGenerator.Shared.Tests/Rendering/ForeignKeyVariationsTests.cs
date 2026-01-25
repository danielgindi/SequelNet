using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class ForeignKeyVariationsTests
{
    [Fact]
    public void ForeignKey_With_Multiple_Columns_Emits_StringArray_For_Columns()
    {
        var script = @"
MyTable
dbo.MyTable
@ForeignKey: NAME(FK_Multi); FOREIGNTABLE(OtherTable); COLUMNS[OtherId,TenantId]; FOREIGNCOLUMNS[Id,TenantId]
Id: PRIMARY KEY; INT;
OtherId: INT;
TenantId: INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("schema.AddForeignKey(", result.Code);
        Assert.Contains("new string[]", result.Code);
    }

    [Fact]
    public void ForeignKey_When_Reference_Self_Uses_schema_Name()
    {
        var script = @"
MyTable
dbo.MyTable
@ForeignKey: NAME(FK_Self); FOREIGNTABLE(MyTable); COLUMNS[ParentId]; FOREIGNCOLUMNS[Id]
Id: PRIMARY KEY; INT;
ParentId: INT;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // When foreign table equals current class name, generator uses schema.Name.
        Assert.Contains("schema.Name", result.Code);
    }
}
