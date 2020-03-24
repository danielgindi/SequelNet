using System;
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
            this.Outer = ValueWrapper.From(outerValue, outerValueType);
            this.Inner = ValueWrapper.From(innerValue, innerValueType);
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerColumnName)
        {
            this.Outer = ValueWrapper.From(outerValue, outerValueType);
            this.Inner = ValueWrapper.From(innerColumnName, ValueObjectType.ColumnName);
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerTableName, string innerColumnName)
        {
            this.Outer = ValueWrapper.From(outerValue, outerValueType);
            this.Inner = ValueWrapper.From(innerTableName, innerColumnName);
        }

        public GeographyContains(
            Geometry outerValue,
            string innerColumnName)
        {
            this.Outer = ValueWrapper.From(outerValue, ValueObjectType.Value);
            this.Inner = ValueWrapper.From(innerColumnName, ValueObjectType.ColumnName);
        }

        public GeographyContains(
            Geometry outerValue,
            string innerTableName, string innerColumnName)
        {
            this.Outer = ValueWrapper.From(outerValue, ValueObjectType.Value);
            this.Inner = ValueWrapper.From(innerTableName, innerColumnName);
        }

        public GeographyContains(Geometry outerValue, Geometry innerValue)
        {
            this.Outer = ValueWrapper.From(outerValue, ValueObjectType.Value);
            this.Inner = ValueWrapper.From(innerValue, ValueObjectType.Value);
        }

        public GeographyContains(
            string outerColumnName,
            Geometry innerObject)
        {
            this.Outer = ValueWrapper.From(outerColumnName, ValueObjectType.ColumnName);
            this.Inner = ValueWrapper.From(innerObject, ValueObjectType.Value);
        }

        public GeographyContains(
            string outerTableName, string outerColumnName,
            Geometry innerObject)
        {
            this.Outer = ValueWrapper.From(outerTableName, outerColumnName);
            this.Inner = ValueWrapper.From(innerObject, ValueObjectType.Value);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.Language.ST_Distance_Sphere(
                Outer.Build(conn, relatedQuery),
                Inner.Build(conn, relatedQuery));
        }
    }
}
