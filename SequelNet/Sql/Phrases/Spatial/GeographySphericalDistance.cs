using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// This will return value in meters
    /// </summary>
    public class GeographySphericalDistance : IPhrase
    {
        public ValueWrapper From;
        public ValueWrapper To;

        #region Constructors

        public GeographySphericalDistance(
            ValueWrapper from,
            ValueWrapper to)
        {
            this.From = from;
            this.To = to;
        }

        public GeographySphericalDistance(
            object fromValue, ValueObjectType fromValueType,
            object toValue, ValueObjectType toValueType)
        {
            this.From = ValueWrapper.Make(fromValue, fromValueType);
            this.To = ValueWrapper.Make(toValue, toValueType);
        }

        public GeographySphericalDistance(
            object fromValue, ValueObjectType fromValueType,
            string toColumnName)
        {
            this.From = ValueWrapper.Make(fromValue, fromValueType);
            this.To = ValueWrapper.Column(toColumnName);
        }

        public GeographySphericalDistance(
            object fromValue, ValueObjectType fromValueType,
            string toTableName, string toColumnName)
        {
            this.From = ValueWrapper.Make(fromValue, fromValueType);
            this.To = ValueWrapper.Column(toTableName, toColumnName);
        }

        public GeographySphericalDistance(
            Geometry fromValue,
            string toColumnName)
        {
            this.From = ValueWrapper.From(fromValue);
            this.To = ValueWrapper.Column(toColumnName);
        }

        public GeographySphericalDistance(
            Geometry fromValue,
            string toTableName, string toColumnName)
        {
            this.From = ValueWrapper.From(fromValue);
            this.To = ValueWrapper.Column(toTableName, toColumnName);
        }

        public GeographySphericalDistance(Geometry fromValue, Geometry toValue)
        {
            this.From = ValueWrapper.From(fromValue);
            this.To = ValueWrapper.From(toValue);
        }

        public GeographySphericalDistance(
            string fromColumnName,
            Geometry toObject)
        {
            this.From = ValueWrapper.Column(fromColumnName);
            this.To = ValueWrapper.From(toObject);
        }

        public GeographySphericalDistance(
            string fromTableName, string fromColumnName, 
            Geometry toObject)
        {
            this.From = ValueWrapper.Column(fromTableName, fromColumnName);
            this.To = ValueWrapper.From(toObject);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            try
            {
                return conn.Language.ST_Distance_Sphere(
                    From.Build(conn, relatedQuery), 
                    To.Build(conn, relatedQuery));
            }
            catch (NotImplementedException)
            {
                return new GeographySphericalDistanceMath(
                    ValueWrapper.From(new ST_X(From)),
                    ValueWrapper.From(new ST_Y(From)),
                    ValueWrapper.From(new ST_X(To)),
                    ValueWrapper.From(new ST_Y(To))
                ).BuildPhrase(conn, relatedQuery);
            }
        }
    }
}
