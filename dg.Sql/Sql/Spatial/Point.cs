using System;
using System.Text;
using dg.Sql.Connector;
using System.Globalization;

namespace dg.Sql
{
    public abstract partial class Geometry
    {
        public class Point : Geometry
        {
            public double X;
            public double Y;
            public double? Z;
            public double? M;

            public Point()
            {

            }

            public Point(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }

            public Point(double x, double y, double z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            public Point(double x, double y, double? z, double? m)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.M = m;
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
                    if (Double.IsNaN(X)) return false;
                    if (Double.IsInfinity(X)) return false;
                    if (Double.IsNaN(Y)) return false;
                    if (Double.IsInfinity(Y)) return false;
                    return true;
                }
            }

            static IFormatProvider formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            public override void BuildValue(StringBuilder sb, ConnectorBase conn)
            {
                var geom = "POINT(" + X.ToString(formatProvider) + " " + Y.ToString(formatProvider) + ")";

                sb.Append(IsGeographyType
                    ? conn.Language.func_ST_GeogFromText(geom, SRID == null ? "" : SRID.Value.ToString()) 
                    : conn.Language.func_ST_GeomFromText(geom, SRID == null ? "" : SRID.Value.ToString()));
            }

            public override void BuildValueForCollection(StringBuilder sb, ConnectorBase conn)
            {
                sb.Append(@"POINT(");
                sb.Append(X.ToString(formatProvider));
                sb.Append(' ');
                sb.Append(Y.ToString(formatProvider));
                sb.Append(@")");
            }
        }
    }
}
