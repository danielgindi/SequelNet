using System;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class GeographyContains : IPhrase
    {
        public string OuterTableName;
        public object OuterValue;
        public ValueObjectType OuterValueType;
        public string InnerTableName;
        public object InnerValue;
        public ValueObjectType InnerValueType;

        #region Constructors

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            object innerValue, ValueObjectType innerValueType)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerValue = innerValue;
            this.InnerValueType = innerValueType;
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerTableName, string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = outerValueType;
            this.InnerTableName = innerTableName;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographyContains(
            Geometry outerValue,
            string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographyContains(
            Geometry outerValue,
            string innerTableName, string innerColumnName)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerTableName = innerTableName;
            this.InnerValue = innerColumnName;
            this.InnerValueType = ValueObjectType.ColumnName;
        }

        public GeographyContains(Geometry outerValue, Geometry innerValue)
        {
            this.OuterValue = outerValue;
            this.OuterValueType = ValueObjectType.Value;
            this.InnerValue = innerValue;
            this.InnerValueType = ValueObjectType.Value;
        }

        public GeographyContains(
            string outerColumnName,
            Geometry innerObject)
        {
            this.OuterValue = outerColumnName;
            this.OuterValueType = ValueObjectType.ColumnName;
            this.InnerValue = innerObject;
            this.InnerValueType = ValueObjectType.Value;
        }

        public GeographyContains(
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
                    sb1.Append(conn.WrapFieldName(OuterTableName));
                    sb1.Append(".");
                }
                sb1.Append(conn.WrapFieldName(OuterValue.ToString()));
            }
            else if (OuterValueType == ValueObjectType.Value)
            {
                if (OuterValue is Geometry)
                {
                    ((Geometry)OuterValue).BuildValue(sb1, conn);
                }
                else
                {
                    sb1.Append(conn.PrepareValue(OuterValue, relatedQuery));
                }
            }
            else sb1.Append(OuterValue);
            
            if (InnerValueType == ValueObjectType.ColumnName)
            {
                if (InnerTableName != null && InnerTableName.Length > 0)
                {
                    sb2.Append(conn.WrapFieldName(InnerTableName));
                    sb2.Append(".");
                }
                sb2.Append(conn.WrapFieldName(InnerValue.ToString()));
            }
            else if (InnerValueType == ValueObjectType.Value)
            {
                if (InnerValue is Geometry)
                {
                    ((Geometry)InnerValue).BuildValue(sb2, conn);
                }
                else
                {
                    sb2.Append(conn.PrepareValue(InnerValue, relatedQuery));
                }
            }
            else sb2.Append(InnerValue);

            return conn.func_ST_Contains(sb1.ToString(), sb2.ToString());
        }
    }
}
