namespace dg.Sql
{
    public abstract partial class Geometry
    {
        public class MultiPoint : GeometryCollection<Point>
        {
            public MultiPoint()
                : base()
            {
            }

            public MultiPoint(params Point[] points)
                : base(points)
            {
            }

            public MultiPoint(int capacity)
                : base(capacity)
            {
            }
        }
    }
}
