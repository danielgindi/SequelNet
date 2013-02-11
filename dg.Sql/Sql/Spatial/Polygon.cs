using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using dg.Sql.Connector;

namespace dg.Sql
{
    public abstract partial class Geometry
    {
        public class Polygon : Geometry
        {
            private LineString _Exterior;
            private List<LineString> _Holes;

            public Polygon()
            {
                _Holes = new List<LineString>();
            }
            public Polygon(LineString Exterior, params LineString[] rings)
            {
                this._Exterior = Exterior;
                _Holes = new List<LineString>(rings);
            }
            public Polygon(int HolesCapacity)
            {
                _Holes = new List<LineString>(HolesCapacity);
            }

            public LineString Exterior
            {
                get
                {
                    return _Exterior;
                }
                set
                {
                    _Exterior = value;
                }
            }
            public List<LineString> Holes
            {
                get
                {
                    return _Holes;
                }
                set
                {
                    _Holes = value;
                }
            }

            public override bool IsEmpty
            {
                get
                {
                    return Exterior == null;
                }
            }
            public override bool IsValid
            {
                get
                {
                    if (_Exterior == null) return false;
                    if (!_Exterior.IsClosedRing) return false;
                    foreach (LineString hole in _Holes)
                    {
                        if (!hole.IsClosedRing) return false;
                    }
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

                BuildValueForCollection(sb, conn);
                
                if (SRID != null)
                {
                    sb.Append(@"',");
                    sb.Append(SRID.Value);
                    sb.Append(')');
                }
                else
                {
                    sb.Append(@"')");
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
                bool firstLineString = true, first;
                sb.Append(@"POLYGON(");
                if (_Exterior != null)
                {
                    firstLineString = false; ;
                    sb.Append('(');
                    first = true;
                    foreach (Point pt in _Exterior.Points)
                    {
                        if (first) first = false; else sb.Append(',');
                        sb.Append(pt.X.ToString(formatProvider));
                        sb.Append(' ');
                        sb.Append(pt.Y.ToString(formatProvider));
                    }
                    sb.Append(')');
                }
                foreach (LineString ring in _Holes)
                {
                    if (firstLineString) firstLineString = false; else sb.Append(',');
                    sb.Append('(');
                    first = true;
                    foreach (Point pt in ring.Points)
                    {
                        if (first) first = false; else sb.Append(',');
                        sb.Append(pt.X.ToString(formatProvider));
                        sb.Append(' ');
                        sb.Append(pt.Y.ToString(formatProvider));
                    }
                    sb.Append(')');
                }

                sb.Append(')');
#else
                bool HasM = false, HasZ = false;
                if (_Exterior != null)
                {
                    if (_Exterior.Points.Count > 0)
                    {
                        Point firstPoint = _Exterior.Points[0];
                        HasM = firstPoint.M != null;
                        HasZ = firstPoint.Z != null;
                    }
                }

                bool firstLineString = true, first;

                if (!HasZ && HasM)
                {
                    sb.Append(@"POLYGONM(");
                    if (_Exterior != null)
                    {
                        firstLineString = false; ;
                        sb.Append('(');
                        first = true;
                        foreach (Point pt in _Exterior.Points)
                        {
                            if (first) first = false; else sb.Append(',');
                            sb.Append(pt.X.ToString(formatProvider));
                            sb.Append(' ');
                            sb.Append(pt.Y.ToString(formatProvider));
                            sb.Append(' ');
                            sb.Append(pt.M.Value.ToString(formatProvider));
                        }
                        sb.Append(')');
                    }
                    foreach (LineString ring in _Holes)
                    {
                        if (firstLineString) firstLineString = false; else sb.Append(',');
                        sb.Append('(');
                        first = true;
                        foreach (Point pt in ring.Points)
                        {
                            if (first) first = false; else sb.Append(',');
                            sb.Append(pt.X.ToString(formatProvider));
                            sb.Append(' ');
                            sb.Append(pt.Y.ToString(formatProvider));
                            sb.Append(' ');
                            sb.Append(pt.M.Value.ToString(formatProvider));
                        }
                        sb.Append(')');
                    }
                }
                else
                {
                    sb.Append(@"POLYGON(");
                    if (_Exterior != null)
                    {
                        firstLineString = false; ;
                        sb.Append('(');
                        first = true;
                        foreach (Point pt in _Exterior.Points)
                        {
                            if (first) first = false; else sb.Append(',');
                            sb.Append(pt.X.ToString(formatProvider));
                            sb.Append(' ');
                            sb.Append(pt.Y.ToString(formatProvider));
                            if (HasM)
                            {
                                sb.Append(' ');
                                sb.Append(pt.Z.Value.ToString(formatProvider));
                                if (HasZ)
                                {
                                    sb.Append(' ');
                                    sb.Append(pt.M.Value.ToString(formatProvider));
                                }
                            }
                        }
                        sb.Append(')');
                    }
                    foreach (LineString ring in _Holes)
                    {
                        if (firstLineString) firstLineString = false; else sb.Append(',');
                        sb.Append('(');
                        first = true;
                        foreach (Point pt in ring.Points)
                        {
                            if (first) first = false; else sb.Append(',');
                            sb.Append(pt.X.ToString(formatProvider));
                            sb.Append(' ');
                            sb.Append(pt.Y.ToString(formatProvider));
                            if (HasM)
                            {
                                sb.Append(' ');
                                sb.Append(pt.Z.Value.ToString(formatProvider));
                                if (HasZ)
                                {
                                    sb.Append(' ');
                                    sb.Append(pt.M.Value.ToString(formatProvider));
                                }
                            }
                        }
                        sb.Append(')');
                    }
                }

                sb.Append(')');
#endif
            }
        }
    }
}
