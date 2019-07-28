using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql
{
    public abstract partial class Geometry
    {
        public class GeometryCollection<GeometryType> : Geometry
            where GeometryType : Geometry
        {
            private List<GeometryType> _Geometries;

            public GeometryCollection()
            {
                _Geometries = new List<GeometryType>();
            }

            public GeometryCollection(params GeometryType[] geometries)
            {
                _Geometries = new List<GeometryType>(geometries);
            }

            public GeometryCollection(int capacity)
            {
                _Geometries = new List<GeometryType>(capacity);
            }

            public GeometryType this[int i]
            {
                get { return _Geometries[i]; }
                set { _Geometries[i] = value; }
            }

            public List<GeometryType> Geometries
            {
                get { return _Geometries; }
                set { _Geometries = value; }
            }

            public override bool IsEmpty
            {
                get
                {
                    return _Geometries.Count == 0;
                }
            }

            public override bool IsValid
            {
                get
                {
                    foreach (Geometry g in Geometries)
                    {
                        if (!g.IsValid) return false;
                    }
                    return true;
                }
            }

            public override void BuildValue(StringBuilder sb, ConnectorBase conn)
            {
                var sbGeom = new StringBuilder();

                sbGeom.Append(@"GEOMETRYCOLLECTION(");

                bool firstGeometry = true;
                foreach (var geometry in _Geometries)
                {
                    if (firstGeometry) firstGeometry = false; else sbGeom.Append(',');
                    geometry.BuildValueForCollection(sbGeom, conn);
                }

                sbGeom.Append(')');

                sb.Append(IsGeographyType
                    ? conn.func_ST_GeogFromText(sbGeom.ToString(), SRID == null ? "" : SRID.Value.ToString())
                    : conn.func_ST_GeomFromText(sbGeom.ToString(), SRID == null ? "" : SRID.Value.ToString()));
            }

            public override void BuildValueForCollection(StringBuilder sb, ConnectorBase conn)
            {
                sb.Append(@"GEOMETRYCOLLECTION(");
                bool firstGeometry = true;

                foreach (GeometryType geometry in _Geometries)
                {
                    if (firstGeometry) firstGeometry = false; else sb.Append(',');
                    geometry.BuildValueForCollection(sb, conn);
                }

                sb.Append(@")");
            }
        }
    }
}
