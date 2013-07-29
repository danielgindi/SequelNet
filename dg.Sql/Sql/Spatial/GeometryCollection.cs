using System;
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
            public GeometryCollection(int Capacity)
            {
                _Geometries = new List<GeometryType>(Capacity);
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
                if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    if (this.IsGeographyType)
                    {
                        sb.Append(@"geography::STGeomFromText('");
                    }
                    else
                    {
                        sb.Append(@"geometry::STGeomFromText('");
                    }
                }
                else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                {
                    if (this.IsGeographyType)
                    {
                        sb.Append(@"ST_GeogFromText('");
                    }
                    else
                    {
                        sb.Append(@"ST_GeomFromText('");
                    }
                }
                else
                {
                    sb.Append(@"GeomFromText('");
                }

                sb.Append(@"GEOMETRYCOLLECTION(");

                bool firstGeometry = true;
                foreach (GeometryType geometry in _Geometries)
                {
                    if (firstGeometry) firstGeometry = false; else sb.Append(',');
                    geometry.BuildValueForCollection(sb, conn);
                }

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
