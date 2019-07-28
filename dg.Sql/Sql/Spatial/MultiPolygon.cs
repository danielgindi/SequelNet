namespace dg.Sql
{
    public abstract partial class Geometry
    {
        public class MultiPolygon : GeometryCollection<Polygon>
        {
            public MultiPolygon()
                : base()
            {
            }

            public MultiPolygon(params Polygon[] polygons)
                : base(polygons)
            {
            }

            public MultiPolygon(int capacity)
                : base(capacity)
            {
            }
        }
    }
}
