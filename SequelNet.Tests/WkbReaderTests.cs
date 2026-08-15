using System.Text;
using SequelNet;
using SequelNet.Sql.Spatial;

namespace Tests;

public class WkbReaderTests
{
    [Test]
    public void GeometryFromWkb_ParsesLittleAndBigEndianPoints()
    {
        var littleEndian = WkbReader.GeometryFromWkb(
            WkbReader.HexToBytes("0101000000000000000000F03F0000000000000040"));
        var bigEndian = WkbReader.GeometryFromWkb(
            WkbReader.HexToBytes("00000000013FF00000000000004000000000000000"));

        using (Assert.EnterMultipleScope())
        {
            AssertPoint((Geometry.Point)littleEndian, 1, 2);
            AssertPoint((Geometry.Point)bigEndian, 1, 2);
        }
    }

    [Test]
    public void GeometryFromWkb_ParsesEwkbPointWithSridAndZAndMCoordinates()
    {
        var geometry = WkbReader.GeometryFromWkb(Build(writer =>
        {
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write(0xE0000001u);
            writer.Write(4326);
            WritePoint(writer, 1, 2, 3, 4);
        }));

        var point = (Geometry.Point)geometry;
        using (Assert.EnterMultipleScope())
        {
            AssertPoint(point, 1, 2);
            Assert.That(point.SRID, Is.EqualTo(4326));
            Assert.That(point.Z.Value, Is.EqualTo(3d));
            Assert.That(point.M.Value, Is.EqualTo(4d));
        }
    }

    [Test]
    public void GeometryFromWkb_UsesPrefixedSridWhenTheGeometryHasNone()
    {
        var geometry = WkbReader.GeometryFromWkb(Build(writer =>
        {
            writer.Write(3857);
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write((uint)WkbReader.WkbGeometryTypes.WkbPoint);
            WritePoint(writer, 10, 20);
        }), beginsWithSRID: true);

        var point = (Geometry.Point)geometry;
        using (Assert.EnterMultipleScope())
        {
            AssertPoint(point, 10, 20);
            Assert.That(point.SRID, Is.EqualTo(3857));
        }
    }

    [Test]
    public void GeometryFromWkb_ParsesLineStringAndPolygonWithHoles()
    {
        var line = (Geometry.LineString)WkbReader.GeometryFromWkb(Build(writer =>
        {
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write((uint)WkbReader.WkbGeometryTypes.WkbLineString);
            writer.Write(2);
            WritePoint(writer, 1, 2);
            WritePoint(writer, 3, 4);
        }));
        var polygon = (Geometry.Polygon)WkbReader.GeometryFromWkb(Build(writer =>
        {
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write((uint)WkbReader.WkbGeometryTypes.WkbPolygon);
            writer.Write(2);
            WriteRing(writer, (0, 0), (2, 0), (2, 2), (0, 0));
            WriteRing(writer, (0.5, 0.5), (1, 0.5), (1, 1), (0.5, 0.5));
        }));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(line.Points, Has.Count.EqualTo(2));
            AssertPoint(line[1], 3, 4);
            Assert.That(polygon.Exterior.Points, Has.Count.EqualTo(4));
            Assert.That(polygon.Holes, Has.Count.EqualTo(1));
            Assert.That(polygon.IsValid, Is.True);
        }
    }

    [Test]
    public void GeometryFromWkb_ParsesMultiPointAndGeometryCollection()
    {
        var multiPoint = (Geometry.MultiPoint)WkbReader.GeometryFromWkb(Build(writer =>
        {
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write((uint)WkbReader.WkbGeometryTypes.WkbMultiPoint);
            writer.Write(2);
            WritePointGeometry(writer, 1, 2);
            WritePointGeometry(writer, 3, 4);
        }));
        var collection = (Geometry.GeometryCollection<Geometry>)WkbReader.GeometryFromWkb(Build(writer =>
        {
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write((uint)WkbReader.WkbGeometryTypes.WkbGeometryCollection);
            writer.Write(2);
            WritePointGeometry(writer, 1, 2);
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write((uint)WkbReader.WkbGeometryTypes.WkbLineString);
            writer.Write(2);
            WritePoint(writer, 0, 0);
            WritePoint(writer, 1, 1);
        }));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(multiPoint.Geometries, Has.Count.EqualTo(2));
            AssertPoint(multiPoint[1], 3, 4);
            Assert.That(collection.Geometries[0], Is.TypeOf<Geometry.Point>());
            Assert.That(collection.Geometries[1], Is.TypeOf<Geometry.LineString>());
        }
    }

    [Test]
    public void GeometryFromWkb_RejectsUnexpectedGeometryInMultiPoint()
    {
        var data = Build(writer =>
        {
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write((uint)WkbReader.WkbGeometryTypes.WkbMultiPoint);
            writer.Write(1);
            writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
            writer.Write((uint)WkbReader.WkbGeometryTypes.WkbLineString);
            writer.Write(0);
        });

        Action readGeometry = () => WkbReader.GeometryFromWkb(data);
        var exception = Assert.Throws<ArgumentException>(readGeometry);
        Assert.That(exception!.Message, Does.Contain("Point expected"));
    }

    [Test]
    public void HexToBytesAndBigEndianReader_ConvertBinaryValues()
    {
        var bytes = WkbReader.HexToBytes("00A1fe");
        using var stream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x3F, 0x80, 0x00, 0x00 });
        using var reader = new WkbReader.BigEndianBinaryReader(stream, Encoding.UTF8);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(bytes, Is.EqualTo(new byte[] { 0x00, 0xA1, 0xFE }));
            Assert.That(reader.ReadInt32(), Is.EqualTo(0x01020304));
            Assert.That(reader.ReadSingle(), Is.EqualTo(1f));
        }
    }

    private static byte[] Build(Action<BinaryWriter> write)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
            write(writer);
        return stream.ToArray();
    }

    private static void WritePointGeometry(BinaryWriter writer, double x, double y)
    {
        writer.Write((byte)WkbReader.WkbByteOrder.LittleEndian);
        writer.Write((uint)WkbReader.WkbGeometryTypes.WkbPoint);
        WritePoint(writer, x, y);
    }

    private static void WriteRing(BinaryWriter writer, params (double X, double Y)[] points)
    {
        writer.Write(points.Length);
        foreach (var point in points)
            WritePoint(writer, point.X, point.Y);
    }

    private static void WritePoint(BinaryWriter writer, double x, double y, double? z = null, double? m = null)
    {
        writer.Write(x);
        writer.Write(y);
        if (z.HasValue)
            writer.Write(z.Value);
        if (m.HasValue)
            writer.Write(m.Value);
    }

    private static void AssertPoint(Geometry.Point point, double x, double y)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(point.X.Value, Is.EqualTo(x));
            Assert.That(point.Y.Value, Is.EqualTo(y));
        }
    }
}
