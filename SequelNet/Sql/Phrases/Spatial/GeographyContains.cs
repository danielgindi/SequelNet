using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// This will return value in meters
    /// </summary>
    public class GeographyContains : IPhrase
    {
        public ValueWrapper Outer;
        public ValueWrapper Inner;

        #region Construcinnerrs

        public GeographyContains(
            ValueWrapper outer,
            ValueWrapper inner)
        {
            this.Outer = outer;
            this.Inner = inner;
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            object innerValue, ValueObjectType innerValueType)
        {
            this.Outer = ValueWrapper.Make(outerValue, outerValueType);
            this.Inner = ValueWrapper.Make(innerValue, innerValueType);
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerColumnName)
        {
            this.Outer = ValueWrapper.Make(outerValue, outerValueType);
            this.Inner = ValueWrapper.Column(innerColumnName);
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerTableName, string innerColumnName)
        {
            this.Outer = ValueWrapper.Make(outerValue, outerValueType);
            this.Inner = ValueWrapper.Column(innerTableName, innerColumnName);
        }

        public GeographyContains(
            Geometry outerValue,
            string innerColumnName)
        {
            this.Outer = ValueWrapper.From(outerValue);
            this.Inner = ValueWrapper.Column(innerColumnName);
        }

        public GeographyContains(
            Geometry outerValue,
            string innerTableName, string innerColumnName)
        {
            this.Outer = ValueWrapper.From(outerValue);
            this.Inner = ValueWrapper.Column(innerTableName, innerColumnName);
        }

        public GeographyContains(Geometry outerValue, Geometry innerValue)
        {
            this.Outer = ValueWrapper.From(outerValue);
            this.Inner = ValueWrapper.From(innerValue);
        }

        public GeographyContains(
            string outerColumnName,
            Geometry innerObject)
        {
            this.Outer = ValueWrapper.Column(outerColumnName);
            this.Inner = ValueWrapper.From(innerObject);
        }

        public GeographyContains(
            string outerTableName, string outerColumnName,
            Geometry innerObject)
        {
            this.Outer = ValueWrapper.Column(outerTableName, outerColumnName);
            this.Inner = ValueWrapper.From(innerObject);
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.ST_Distance_Sphere(
                Outer.Build(conn, relatedQuery),
                Inner.Build(conn, relatedQuery)));
        }
    }
}
