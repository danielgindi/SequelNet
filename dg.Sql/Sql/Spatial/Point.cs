using System;
using System.Collections.Generic;
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
            public Point(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }
            public Point(double X, double Y, double Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }
            public Point(double X, double Y, double? Z, double? M)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
                this.M = M;
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
#if !USE_EWKT
                if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    sb.Append(@"geography::STGeomFromText('");
                }
                else
                {
                    sb.Append(@"GeomFromText('");
                }

                sb.Append(@"POINT(");
                sb.Append(X.ToString(formatProvider));
                sb.Append(' ');
                sb.Append(Y.ToString(formatProvider));

                if (SRID != null)
                {
                    sb.Append(@")',");
                    sb.Append(SRID.Value);
                    sb.Append(')');
                }
                else
                {
                    sb.Append(@")')");
                }

#else
                sb.Append(@"GeomFromEwkt('");
                if (SRID != null)
                {
                    sb.Append(@"SRID=");
                    sb.Append(SRID);
                    sb.Append(';');
                }

                BuildValueForCollection(sb, conn);

                sb.Append(@"')");
#endif
            }
            public override void BuildValueForCollection(StringBuilder sb, ConnectorBase conn)
            {
#if !USE_EWKT
                sb.Append(@"POINT(");
                sb.Append(X.ToString(formatProvider));
                sb.Append(' ');
                sb.Append(Y.ToString(formatProvider));
                sb.Append(@")");
#else
                if (Z == null && M != null)
                {
                    sb.Append(@"POINTM(");
                    sb.Append(X.ToString(formatProvider));
                    sb.Append(' ');
                    sb.Append(Y.ToString(formatProvider));
                    sb.Append(' ');
                    sb.Append(M.Value.ToString(formatProvider));
                }
                else
                {
                    sb.Append(@"POINT(");
                    sb.Append(X.ToString(formatProvider));
                    sb.Append(' ');
                    sb.Append(Y.ToString(formatProvider));
                    if (Z != null)
                    {
                        sb.Append(' ');
                        sb.Append(Z.Value.ToString(formatProvider));
                        if (M != null)
                        {
                            sb.Append(' ');
                            sb.Append(M.Value.ToString(formatProvider));
                        }
                    }
                }
                sb.Append(@")");
#endif
            }
        }
    }
}
