using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using SequelNet.Connector;

namespace SequelNet
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

            private static ValueWrapper OPEN_STRING_VALUE = ValueWrapper.From("POLYGON(", ValueObjectType.Value);
            private static ValueWrapper CLOSE_STRING_VALUE = ValueWrapper.From(")", ValueObjectType.Value);
            private static ValueWrapper COMMA_STRING_VALUE = ValueWrapper.From(",", ValueObjectType.Value);
            private static ValueWrapper OPEN_SUB_STRING_VALUE = ValueWrapper.From("(", ValueObjectType.Value);
            private static ValueWrapper CLOSE_SUB_STRING_VALUE = ValueWrapper.From(")", ValueObjectType.Value);

            public override void BuildValue(StringBuilder sb, ConnectorBase conn)
            {
                string geom = BuildValueText(conn).Build(conn);

                sb.Append(IsGeographyType
                    ? conn.Language.ST_GeogFromText(geom, SRID == null ? "" : SRID.Value.ToString(), true)
                    : conn.Language.ST_GeomFromText(geom, SRID == null ? "" : SRID.Value.ToString(), true));
            }

            public override ValueWrapper BuildValueText(ConnectorBase conn)
            {
                var concat = new Phrases.Concat(OPEN_STRING_VALUE);

                bool firstLineString = true, first;

                if (_Exterior != null)
                {
                    firstLineString = false;

                    concat.Values.Add(OPEN_SUB_STRING_VALUE);
                    first = true;
                    foreach (Point pt in _Exterior.Points)
                    {
                        if (first) first = false;
                        else concat.Values.Add(COMMA_STRING_VALUE);

                        concat.Values.Add(pt.BuildValueText(conn));
                    }
                    concat.Values.Add(CLOSE_SUB_STRING_VALUE);
                }

                foreach (var ring in _Holes)
                {
                    if (firstLineString) firstLineString = false; 
                    else concat.Values.Add(COMMA_STRING_VALUE);

                    concat.Values.Add(OPEN_SUB_STRING_VALUE);
                    first = true;
                    foreach (var pt in ring.Points)
                    {
                        if (first) first = false;
                        else concat.Values.Add(COMMA_STRING_VALUE);

                        concat.Values.Add(pt.BuildValueText(conn));
                    }
                    concat.Values.Add(CLOSE_SUB_STRING_VALUE);
                }

                concat.Values.Add(CLOSE_STRING_VALUE);

                return ValueWrapper.From(concat);
            }
        }
    }
}
