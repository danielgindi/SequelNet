using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet
{
    public abstract partial class Geometry
    {
        public class Point : Geometry
        {
            public ValueWrapper X;
            public ValueWrapper Y;
            public ValueWrapper Z;
            public ValueWrapper M;

            public Point()
            {

            }

            public Point(double x, double y)
            {
                this.X = (ValueWrapper)x;
                this.Y = (ValueWrapper)y;
            }

            public Point(double x, double y, double? z = null, double? m = null)
            {
                this.X = (ValueWrapper)x;
                this.Y = (ValueWrapper)y;
                this.Z = (ValueWrapper)z;
                this.M = (ValueWrapper)m;
            }

            public Point(double x, double y, double? z, double? m, int srid)
            {
                this.X = (ValueWrapper)x;
                this.Y = (ValueWrapper)y;
                this.Z = (ValueWrapper)z;
                this.M = (ValueWrapper)m;
                this.SRID = srid;
            }

            public Point(ValueWrapper x, ValueWrapper y, int srid)
            {
                this.X = x;
                this.Y = y;
                this.SRID = srid;
            }

            public Point(ValueWrapper x, ValueWrapper y, ValueWrapper z, ValueWrapper m, int srid)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.M = m;
                this.SRID = srid;
            }

            public override bool IsEmpty
            {
                get
                {
                    return false;
                }
            }

            public override bool IsValid
            {
                get
                {
                    if (X.Value == null || (X.Value is Double xd && (
                        Double.IsNaN(xd) || Double.IsInfinity(xd)
                    ))) return false;
                    if (Y.Value == null || (Y.Value is Double yd && (
                        Double.IsNaN(yd) || Double.IsInfinity(yd)
                    ))) return false;
                    return true;
                }
            }

            private static ValueWrapper OPEN_STRING_VALUE = ValueWrapper.From("POINT(");
            private static ValueWrapper CLOSE_STRING_VALUE = ValueWrapper.From(")");
            private static ValueWrapper SPACE_STRING_VALUE = ValueWrapper.From(" ");

            public override void BuildValue(StringBuilder sb, ConnectorBase conn)
            {
                string geom = BuildValueText(conn).Build(conn);

                sb.Append(IsGeographyType
                    ? conn.Language.ST_GeogFromText(geom, SRID == null ? "" : SRID.Value.ToString(), true)
                    : conn.Language.ST_GeomFromText(geom, SRID == null ? "" : SRID.Value.ToString(), true));
            }

            public override ValueWrapper BuildValueText(ConnectorBase conn)
            {
                if (X.Type == ValueObjectType.Value &&
                    Y.Type == ValueObjectType.Value)
                {
                    var geom = $"POINT({X.Build(conn)} {Y.Build(conn)})";
                    return ValueWrapper.Literal(geom);
                }
                else
                {
                    var geom = new Phrases.Concat(OPEN_STRING_VALUE, X, SPACE_STRING_VALUE, Y, CLOSE_STRING_VALUE);
                    return ValueWrapper.From(geom);
                }
            }
        }
    }
}
