using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class ReadRenderingTests
{
    [Fact]
    public void Read_For_Nullable_String_Uses_GetStringOrNull()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Name: STRING(50); NULLABLE;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("GetStringOrNull(Columns.Name)", result.Code);
    }

    [Fact]
    public void Read_For_NonNullable_String_Uses_StringCast()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Name: STRING(50);
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("(string)reader[Columns.Name]", result.Code);
    }

    [Fact]
    public void Read_For_DateTimeUtc_Uses_GetDateTimeUtc()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
ModifiedOn: DATETIME_UTC;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("GetDateTimeUtc(Columns.ModifiedOn)", result.Code);
    }

    [Fact]
    public void Read_For_Nullable_DateTimeUtc_Uses_GetDateTimeUtcOrNull()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
ModifiedOn: DATETIME_UTC; NULLABLE;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("GetDateTimeUtcOrNull(Columns.ModifiedOn)", result.Code);
    }

    [Fact]
    public void Read_For_Time_Uses_GetTimeSpan()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Duration: TIME;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("GetTimeSpan(Columns.Duration)", result.Code);
    }

    [Fact]
    public void Read_For_Nullable_Time_Uses_GetTimeSpanOrNull()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Duration: TIME; NULLABLE;
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);
        Assert.Contains("GetTimeSpanOrNull(Columns.Duration)", result.Code);
    }

    [Fact]
    public void Read_For_Nullable_Enum_Emits_IsDBNull_Pattern()
    {
        var script = @"
MyTable
my_table
Id: PRIMARY KEY; INT;
Status: INT; NULLABLE; StatusEnum:
""StatusEnum""
- Active
- Inactive
";
        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("public enum StatusEnum", result.Code);
        Assert.Contains("reader.IsDBNull(Columns.Status)", result.Code);
        Assert.Contains("(StatusEnum?)null", result.Code);
        Assert.Contains("(StatusEnum)", result.Code);
    }
}