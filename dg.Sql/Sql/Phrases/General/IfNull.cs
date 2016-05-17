using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class IfNull : IPhrase
    {
        public string FirstTableName;
        public object FirstValue;
        public ValueObjectType FirstValueType;
        public string SecondTableName;
        public object SecondValue;
        public ValueObjectType SecondValueType;

        #region Constructors

        public IfNull(
            string firstTableName, string firstColumnName,
            string secondTableName, string secondColumnName)
        {
            this.FirstTableName = firstTableName;
            this.FirstValue = firstColumnName;
            this.FirstValueType = ValueObjectType.ColumnName;
            this.SecondTableName = secondTableName;
            this.SecondValue = secondColumnName;
            this.SecondValueType = ValueObjectType.ColumnName;
        }

        public IfNull(
             object firstValue, ValueObjectType firstValueType,
             object secondValue, ValueObjectType secondValueType)
        {
            this.FirstValue = firstValue;
            this.FirstValueType = firstValueType;
            this.SecondValue = secondValue;
            this.SecondValueType = secondValueType;
        }

        public IfNull(
             string firstTableName, string firstColumnName,
             object secondValue, ValueObjectType secondValueType)
        {
            this.FirstTableName = firstTableName;
            this.FirstValue = firstColumnName;
            this.FirstValueType = ValueObjectType.ColumnName;
            this.SecondValue = secondValue;
            this.SecondValueType = secondValueType;
        }

        public IfNull(
             object firstValue, ValueObjectType firstValueType,
             string secondTableName, string secondColumnName)
        {
            this.FirstValue = firstValue;
            this.FirstValueType = firstValueType;
            this.SecondTableName = secondTableName;
            this.SecondValue = secondColumnName;
            this.SecondValueType = ValueObjectType.ColumnName;
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;
            if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL || conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                ret = @"IFNULL(";
            else ret = @"ISNULL(";

            if (FirstValueType == ValueObjectType.ColumnName)
            {
                if (FirstTableName != null && FirstTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(FirstTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName((string)FirstValue);
            }
            else if (FirstValueType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(FirstValue, relatedQuery);
            }
            else ret += FirstValue;

            ret += ", ";

            if (SecondValueType == ValueObjectType.ColumnName)
            {
                if (SecondTableName != null && SecondTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(SecondTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName((string)SecondValue);
            }
            else if (SecondValueType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(SecondValue, relatedQuery);
            }
            else ret += SecondValue;

            ret += ")";

            return ret;
        }
    }
}
