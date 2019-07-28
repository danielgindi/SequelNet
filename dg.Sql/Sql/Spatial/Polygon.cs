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

            public Polygon(LineString exterior, params LineString[] rings)
            {
                this._Exterior = exterior;
                _Holes = new List<LineString>(rings);
            }

            public Polygon(int holesCapacity)
            {
                _Holes = new List<LineString>(holesCapacity);
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
                var sbGeom = new StringBuilder();
                BuildValueForCollection(sbGeom, conn);

                sb.Append(IsGeographyType
                    ? conn.Language.ST_GeogFromText(sbGeom.ToString(), SRID == null ? "" : SRID.Value.ToString()) 
                    : conn.Language.ST_GeomFromText(sbGeom.ToString(), SRID == null ? "" : SRID.Value.ToString()));
            }

            public override void BuildValueForCollection(StringBuilder sb, ConnectorBase conn)
            {
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

                foreach (var ring in _Holes)
                {
                    if (firstLineString) firstLineString = false; else sb.Append(',');
                    sb.Append('(');
                    first = true;
                    foreach (var pt in ring.Points)
                    {
                        if (first) first = false; else sb.Append(',');
                        sb.Append(pt.X.ToString(formatProvider));
                        sb.Append(' ');
                        sb.Append(pt.Y.ToString(formatProvider));
                    }
                    sb.Append(')');
                }

                sb.Append(')');
            }
        }
    }
}
