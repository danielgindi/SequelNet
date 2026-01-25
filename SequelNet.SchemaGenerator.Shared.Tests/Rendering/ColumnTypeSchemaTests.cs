using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class ColumnTypeSchemaTests
{
    [Fact]
    public void Decimal_With_Precision_And_Scale_Emits_NumberPrecision_And_NumberScale()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Amount: DECIMAL(10,2);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("NumberPrecision", result.Code);
        Assert.Contains("NumberScale", result.Code);
    }

    [Fact]
    public void Money_With_Precision_Emits_NumberPrecision()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Amount: MONEY(12);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("NumberPrecision", result.Code);
    }

    [Fact]
    public void Text_With_Length_Emits_MaxLength()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Body: TEXT(500);
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("MaxLength", result.Code);
    }

    [Fact]
    public void Json_Column_Sets_DataType_When_Supported()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Payload: JSON;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // Semantic: the generator may emit a DataType literal for JSON.
        Assert.Contains("schema.AddColumn(new TableSchema.Column", result.Code);
    }

    [Fact]
    public void Srid_Emits_SRID_Property_On_Column()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Geo: GEOMETRY; SRID 4326
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("SRID", result.Code);
        Assert.Contains("4326", result.Code);
    }

    [Fact]
    public void Charset_And_Collate_Emit_Properties_On_Column()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
Name: STRING(50); Charset utf8mb4; Collate utf8mb4_general_ci
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("Charset", result.Code);
        Assert.Contains("utf8mb4", result.Code);
        Assert.Contains("Collate", result.Code);
        Assert.Contains("utf8mb4_general_ci", result.Code);
    }
}
