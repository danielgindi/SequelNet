using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet;

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

        private static ValueWrapper OPEN_STRING_VALUE = ValueWrapper.From("GEOMETRYCOLLECTION(");
        private static ValueWrapper CLOSE_STRING_VALUE = ValueWrapper.From(")");
        private static ValueWrapper COMMA_STRING_VALUE = ValueWrapper.From(",");

        public override void BuildValue(StringBuilder sb, ConnectorBase conn)
        {
            var geom = BuildValueText(conn);

            sb.Append(IsGeographyType
                ? conn.Language.ST_GeogFromText(geom.Build(conn), SRID == null ? "" : SRID.Value.ToString(), geom.Type != ValueObjectType.Literal)
                : conn.Language.ST_GeomFromText(geom.Build(conn), SRID == null ? "" : SRID.Value.ToString(), geom.Type != ValueObjectType.Literal));
        }

        public override ValueWrapper BuildValueText(ConnectorBase conn)
        {
            var concat = new Phrases.Concat(OPEN_STRING_VALUE);

            bool firstGeometry = true;
            foreach (var geometry in _Geometries)
            {
                if (firstGeometry) firstGeometry = false; 
                else concat.Values.Add(COMMA_STRING_VALUE);

                concat.Values.Add(geometry.BuildValueText(conn));
            }

            concat.Values.Add(CLOSE_STRING_VALUE);

            return ValueWrapper.From(concat);
        }
    }
}
