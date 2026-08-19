namespace SequelNet;

public abstract partial class Geometry
{
    public class MultiLineString : GeometryCollection<LineString>
    {
        public static new MultiLineString Empty => new MultiLineString();

        public MultiLineString()
            : base()
        {
        }

        public MultiLineString(params LineString[] lineStrings)
            : base(lineStrings)
        {
        }

        public MultiLineString(int capacity)
            : base(capacity)
        {
        }
    }
}
