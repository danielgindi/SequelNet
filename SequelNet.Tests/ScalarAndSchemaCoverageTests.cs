using SequelNet;
using SequelNet.Connector;

namespace Tests;

public class ScalarAndSchemaCoverageTests
{
    [TestCase(typeof(string), 0, DataType.Text)]
    [TestCase(typeof(string), 1, DataType.VarChar)]
    [TestCase(typeof(float), 0, DataType.Float)]
    [TestCase(typeof(double), 0, DataType.Double)]
    [TestCase(typeof(decimal), 0, DataType.Decimal)]
    [TestCase(typeof(bool), 0, DataType.Boolean)]
    [TestCase(typeof(DateTime), 0, DataType.DateTime)]
    [TestCase(typeof(DateTimeOffset), 0, DataType.DateTimeOffset)]
    [TestCase(typeof(DateOnly), 0, DataType.Date)]
    [TestCase(typeof(TimeOnly), 0, DataType.Time)]
    [TestCase(typeof(TimeSpan), 0, DataType.Time)]
    [TestCase(typeof(Guid), 0, DataType.Guid)]
    [TestCase(typeof(byte[]), 0, DataType.Blob)]
    [TestCase(typeof(byte[]), 16, DataType.Binary)]
    [TestCase(typeof(object), 0, DataType.Blob)]
    [TestCase(typeof(byte), 0, DataType.TinyInt)]
    [TestCase(typeof(sbyte), 0, DataType.TinyInt)]
    [TestCase(typeof(short), 0, DataType.SmallInt)]
    [TestCase(typeof(ushort), 0, DataType.UnsignedSmallInt)]
    [TestCase(typeof(int), 0, DataType.Int)]
    [TestCase(typeof(uint), 0, DataType.UnsignedInt)]
    [TestCase(typeof(long), 0, DataType.BigInt)]
    [TestCase(typeof(ulong), 0, DataType.UnsignedBigInt)]
    [TestCase(typeof(Geometry.Point), 0, DataType.Point)]
    [TestCase(typeof(Geometry.LineString), 0, DataType.LineString)]
    [TestCase(typeof(Geometry.Polygon), 0, DataType.Polygon)]
    [TestCase(typeof(Geometry.MultiPoint), 0, DataType.MultiPoint)]
    [TestCase(typeof(Geometry.MultiLineString), 0, DataType.MultiLineString)]
    [TestCase(typeof(Geometry.MultiPolygon), 0, DataType.MultiPolygon)]
    [TestCase(typeof(Geometry.GeometryCollection<>), 0, DataType.GeometryCollection)]
    [TestCase(typeof(Geometry), 0, DataType.Geometry)]
    [TestCase(typeof(Uri), 0, DataType.Int)]
    public void Column_InfersAutomaticDataTypesForSupportedClrTypes(Type type, int maxLength, DataType expectedDataType)
    {
        var column = new TableSchema.Column { Type = type, MaxLength = maxLength };

        Assert.That(column.ActualDataType, Is.EqualTo(expectedDataType));
    }

    [TestCase("simple", "'simple'")]
    [TestCase("O'Brian", "'O''Brian'")]
    [TestCase("", "''")]
    [TestCase(true, "1")]
    [TestCase(false, "0")]
    [TestCase(42, "42")]
    [TestCase(-42, "-42")]
    [TestCase(3.5, "3.5")]
    [TestCase('x', "'x'")]
    public void LanguageFactory_PreparesPrimitiveValues(object value, string expected)
    {
        var language = new LanguageFactory();

        Assert.That(language.PrepareValue(null, value), Is.EqualTo(expected));
    }

    [TestCase("id", "`id`")]
    [TestCase("schema.table", "`schema.table`")]
    [TestCase("with space", "`with space`")]
    [TestCase("already`quoted", "`already``quoted`")]
    [TestCase("", "``")]
    [TestCase("select", "`select`")]
    [TestCase("עברית", "`עברית`")]
    [TestCase("line\nbreak", "`line\nbreak`")]
    public void LanguageFactory_WrapsIdentifiersWithoutChangingTheirContent(string identifier, string expected)
    {
        Assert.That(new LanguageFactory().WrapFieldName(identifier), Is.EqualTo(expected));
    }

    [TestCase("", "")]
    [TestCase("00", "00")]
    [TestCase("0f", "0F")]
    [TestCase("abcdef", "ABCDEF")]
    [TestCase("0123456789", "0123456789")]
    [TestCase("deadbeef", "DEADBEEF")]
    [TestCase("ff00aa", "FF00AA")]
    [TestCase("7f80", "7F80")]
    public void ToHex_ProducesUppercaseTwoDigitBytes(string inputHex, string expectedHex)
    {
        Assert.That(StringUtils.ToHex(Convert.FromHexString(inputHex)), Is.EqualTo(expectedHex));
    }

    [Test]
    public void LanguageFactory_PreparesDateGuidAndBinaryValuesInvariantly()
    {
        var language = new LanguageFactory();
        var timestamp = new DateTime(2026, 8, 15, 13, 45, 30, DateTimeKind.Utc);
        var offsetTimestamp = new DateTimeOffset(timestamp, TimeSpan.Zero);
        var date = new DateOnly(2026, 8, 15);
        var time = new TimeOnly(13, 45, 30, 123);
        var id = Guid.Parse("4ef1df1f-4d80-4a03-9d17-e8d2c6273920");

        Assert.Multiple(() =>
        {
            Assert.That(language.PrepareValue(null, timestamp), Is.EqualTo("'2026-08-15 13:45:30'"));
            Assert.That(language.PrepareValue(null, offsetTimestamp), Does.StartWith("'2026-08-15 13:45:30"));
            Assert.That(language.PrepareValue(null, date), Is.EqualTo("DATE'2026-08-15'"));
            Assert.That(language.PrepareValue(null, time), Is.EqualTo("TIME'13:45:30.123'"));
            Assert.That(language.PrepareValue(null, id), Is.EqualTo("'4ef1df1f-4d80-4a03-9d17-e8d2c6273920'"));
            Assert.That(language.PrepareValue(null, new byte[] { 0xDE, 0xAD }), Is.EqualTo("UNHEX(DEAD)"));
        });
    }
}
