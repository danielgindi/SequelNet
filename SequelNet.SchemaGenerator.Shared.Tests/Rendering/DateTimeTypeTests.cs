using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Rendering;

public class DateTimeTypeTests
{
    [Fact]
    public void Read_For_DateTime_Uses_GetDateTime()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATETIME;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTime(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_Nullable_DateTime_Uses_GetDateTimeOrNull()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATETIME; NULLABLE;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTimeOrNull(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_DateTimeUtc_Uses_GetDateTimeUtc()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATETIME_UTC;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTimeUtc(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_Nullable_DateTimeUtc_Uses_GetDateTimeUtcOrNull()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATETIME_UTC; NULLABLE;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTimeUtcOrNull(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_DateTimeLocal_Uses_GetDateTimeLocal()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATETIME_LOCAL;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTimeLocal(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_Nullable_DateTimeLocal_Uses_GetDateTimeLocalOrNull()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATETIME_LOCAL; NULLABLE;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTimeLocalOrNull(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_DateTimeOffset_Uses_GetDateTimeOffset()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATETIMEOFFSET;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTimeOffset(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_Nullable_DateTimeOffset_Uses_GetDateTimeOffsetOrNull()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATETIMEOFFSET; NULLABLE;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTimeOffsetOrNull(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_Date_Uses_GetDateTimeUtc()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATE;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        // DATE is usually treated as DateTime read path.
        Assert.Contains("GetDateTime(Columns.When)", result.Code);
    }

    [Fact]
    public void Read_For_Nullable_Date_Uses_GetDateTimeUtcOrNull()
    {
        var script = @"
MyTable
dbo.MyTable
Id: PRIMARY KEY; INT;
When: DATE; NULLABLE;
";

        var result = SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass(script);

        Assert.Contains("GetDateTimeOrNull(Columns.When)", result.Code);
    }
}
