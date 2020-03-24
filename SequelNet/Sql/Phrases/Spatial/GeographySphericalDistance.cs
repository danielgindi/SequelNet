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
        public string OuterTableName;
        public object OuterValue;
        public ValueObjectType OuterValueType;
        public string InnerTableName;
        public object InnerValue;
        public ValueObjectType InnerValueType;

        #region Constructors

        public GeographySphericalDistance(
            object outerValue, ValueObjectType outerValueType,
            object innerValue, ValueObjectType innerValueType)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerValue = innerValue;
            this.InnerValueType = innerValueType;
        }

        public GeographySphericalDistance(
            object outerValue, ValueObjectType outerValueType,
            string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographySphericalDistance(
            object outerValue, ValueObjectType outerValueType,
            string innerTableName, string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerTableName = innerTableName;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographySphericalDistance(
            Geometry outerValue,
            string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographySphericalDistance(
            Geometry outerValue,
            string innerTableName, string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerTableName = innerTableName;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographySphericalDistance(Geometry outerValue, Geometry innerValue)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerValue = innerValue;
            this.InnerValueType = ValueObjectType.Value;
        }

        public GeographySphericalDistance(
            string outerColumnName,
            Geometry innerObject)
        {
            this.OuterValue = outerColumnName;
            this.OuterValueType = ValueObjectType.ColumnName;
            this.InnerValue = innerObject;
            this.InnerValueType = ValueObjectType.Value;
        }

        public GeographySphericalDistance(
            string outerTableName, string outerColumnName, 
            Geometry innerObject)
        {
            this.OuterTableName = outerTableName;
            this.OuterValue = outerColumnName;
            this.OuterValueType = ValueObjectType.ColumnName;
            this.InnerValue = innerObject;
            this.InnerValueType = ValueObjectType.Value;
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
                        
            if (OuterValueType == ValueObjectType.ColumnName)
            {
                if (OuterTableName != null && OuterTableName.Length > 0)
                {
                    sb1.Append(conn.Language.WrapFieldName(OuterTableName));
                    sb1.Append(".");
                }
                sb1.Append(conn.Language.WrapFieldName(OuterValue.ToString()));
            }
            else if (OuterValueType == ValueObjectType.Value)
            {
                if (OuterValue is Geometry)
                {
                    ((Geometry)OuterValue).BuildValue(sb1, conn);
                }
                else
                {
                    sb1.Append(conn.Language.PrepareValue(conn, OuterValue, relatedQuery));
                }
            }
            else sb1.Append(OuterValue);
            
            if (InnerValueType == ValueObjectType.ColumnName)
            {
                if (InnerTableName != null && InnerTableName.Length > 0)
                {
                    sb2.Append(conn.Language.WrapFieldName(InnerTableName));
                    sb2.Append(".");
                }
                sb2.Append(conn.Language.WrapFieldName(InnerValue.ToString()));
            }
            else if (InnerValueType == ValueObjectType.Value)
            {
                if (InnerValue is Geometry)
                {
                    ((Geometry)InnerValue).BuildValue(sb2, conn);
                }
                else
                {
                    sb2.Append(conn.Language.PrepareValue(conn, InnerValue, relatedQuery));
                }
            }
            else sb2.Append(InnerValue);

            try
            {
                return conn.Language.ST_Distance_Sphere(sb1.ToString(), sb2.ToString());
            }
            catch (NotImplementedException)
            {
                return new GeographySphericalDistanceMath(
                    ValueWrapper.From(new ST_X(ValueWrapper.Literal(sb1.ToString()))),
                    ValueWrapper.From(new ST_Y(ValueWrapper.Literal(sb1.ToString()))),
                    ValueWrapper.From(new ST_X(ValueWrapper.Literal(sb2.ToString()))),
                    ValueWrapper.From(new ST_Y(ValueWrapper.Literal(sb2.ToString())))
                ).BuildPhrase(conn, relatedQuery);
            }
        }
    }
}
