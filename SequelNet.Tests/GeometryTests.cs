using SequelNet;

namespace Tests;

public class GeometryTests
{
    [Test]
    public void Point_ValidatesCoordinatesAndRetainsSrid()
    {
        var point = new Geometry.Point(32.08, 34.78, 4326);
        var invalid = new Geometry.Point(double.NaN, 34.78);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(point.IsEmpty, Is.False);
            Assert.That(point.IsValid, Is.True);
            Assert.That(point.SRID, Is.EqualTo(4326));
            Assert.That(invalid.IsValid, Is.False);
        }
    }

    [Test]
    public void LineString_DistinguishesClosedLinesFromValidRings()
    {
        var open = new Geometry.LineString(new Geometry.Point(0, 0), new Geometry.Point(1, 1));
        var ring = new Geometry.LineString(
            new Geometry.Point(0, 0), new Geometry.Point(1, 0),
            new Geometry.Point(1, 1), new Geometry.Point(0, 0));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(open.IsValid, Is.True);
            Assert.That(open.IsClosed, Is.False);
            Assert.That(open.IsValidRing, Is.False);
            Assert.That(ring.IsClosed, Is.True);
            Assert.That(ring.IsValidRing, Is.True);
        }
    }

    [Test]
    public void Polygon_RequiresClosedExteriorAndHoles()
    {
        var exterior = new Geometry.LineString(
            new Geometry.Point(0, 0), new Geometry.Point(2, 0),
            new Geometry.Point(2, 2), new Geometry.Point(0, 0));
        var polygon = new Geometry.Polygon(exterior);

        Assert.That(polygon.IsValid, Is.True);

        polygon.Holes.Add(new Geometry.LineString(new Geometry.Point(0.5, 0.5), new Geometry.Point(1, 1)));
        Assert.That(polygon.IsValid, Is.False);
    }

    [Test]
    public void GeometryCollection_IsValidOnlyWhenAllMembersAreValid()
    {
        var collection = new Geometry.MultiPoint(new Geometry.Point(1, 1), new Geometry.Point(2, 2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(collection.IsEmpty, Is.False);
            Assert.That(collection.IsValid, Is.True);
        }

        collection.Geometries.Add(new Geometry.Point(double.PositiveInfinity, 3));
        Assert.That(collection.IsValid, Is.False);
    }

    [Test]
    public void RectForDistanceAroundRect_NormalizesInputCorners()
    {
        var rect = Geometry.LineString.RectForDistanceAroundRect(10, 20, 5, 15, 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(rect.Points, Has.Count.EqualTo(2));
            Assert.That((double)rect[0].X.Value!, Is.GreaterThan((double)rect[1].X.Value!));
            Assert.That((double)rect[0].Y.Value!, Is.GreaterThan((double)rect[1].Y.Value!));
        }
    }

    [Test]
    public void EmptyGeometryFactories_ReturnIndependent_EmptyInstances()
    {
        var firstPoint = Geometry.Point.Empty;
        var secondPoint = Geometry.Point.Empty;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(firstPoint, Is.Not.SameAs(secondPoint));
            Assert.That(firstPoint.IsEmpty, Is.True);
            Assert.That(firstPoint.IsValid, Is.False);
            Assert.That(Geometry.LineString.Empty.IsEmpty, Is.True);
            Assert.That(Geometry.Polygon.Empty.IsEmpty, Is.True);
            Assert.That(Geometry.MultiPoint.Empty.IsEmpty, Is.True);
            Assert.That(Geometry.MultiLineString.Empty.IsEmpty, Is.True);
            Assert.That(Geometry.MultiPolygon.Empty.IsEmpty, Is.True);
            Assert.That(Geometry.GeometryCollection<Geometry>.Empty.IsEmpty, Is.True);
        }
    }
}
