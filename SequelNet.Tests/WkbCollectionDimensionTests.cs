using SequelNet;
using SequelNet.Sql.Spatial;

namespace Tests;

public class WkbCollectionDimensionTests
{
    [Test]
    public void GeometryFromWkb_UsesEachNestedGeometrysCoordinateSystemAndSrid()
    {
        var data = WkbReader.HexToBytes(
            "010400000001000000" +
            "01010000E0E6100000" +
            "000000000000F03F000000000000004000000000000008400000000000001040");

        var multiPoint = (Geometry.MultiPoint)WkbReader.GeometryFromWkb(data);
        var point = multiPoint[0];

        Assert.Multiple(() =>
        {
            Assert.That(point.X.Value, Is.EqualTo(1d));
            Assert.That(point.Y.Value, Is.EqualTo(2d));
            Assert.That(point.Z.Value, Is.EqualTo(3d));
            Assert.That(point.M.Value, Is.EqualTo(4d));
            Assert.That(point.SRID, Is.EqualTo(4326));
        });
    }
}
