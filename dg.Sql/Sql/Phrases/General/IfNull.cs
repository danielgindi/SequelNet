using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class IfNull : BasePhrase
    {
        string FirstTableName;
        object FirstObject;
        ValueObjectType FirstObjectType;
        string SecondTableName;
        object SecondObject;
        ValueObjectType SecondObjectType;

        public IfNull(
            string FirstTableName, string FirstColumnName,
            string SecondTableName, string SecondColumnName)
        {
            this.FirstTableName = FirstTableName;
            this.FirstObject = FirstColumnName;
            this.FirstObjectType = ValueObjectType.ColumnName;
            this.SecondTableName = SecondTableName;
            this.SecondObject = SecondColumnName;
            this.SecondObjectType = ValueObjectType.ColumnName;
        }
        public IfNull(
             object FirstObject, ValueObjectType FirstObjectType,
             object SecondObject, ValueObjectType SecondObjectType)
        {
            this.FirstObject = FirstObject;
            this.FirstObjectType = FirstObjectType;
            this.SecondObject = SecondObject;
            this.SecondObjectType = SecondObjectType;
        }
        public IfNull(
             string FirstTableName, string FirstColumnName,
             object SecondObject, ValueObjectType SecondObjectType)
        {
            this.FirstTableName = FirstTableName;
            this.FirstObject = FirstColumnName;
            this.FirstObjectType = ValueObjectType.ColumnName;
            this.SecondObject = SecondObject;
            this.SecondObjectType = SecondObjectType;
        }
        public IfNull(
             object FirstObject, ValueObjectType FirstObjectType,
             string SecondTableName, string SecondColumnName)
        {
            this.FirstObject = FirstObject;
            this.FirstObjectType = FirstObjectType;
            this.SecondTableName = SecondTableName;
            this.SecondObject = SecondColumnName;
            this.SecondObjectType = ValueObjectType.ColumnName;
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;
            if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                ret = @"IFNULL(";
            else ret = @"ISNULL(";

            if (FirstObjectType == ValueObjectType.ColumnName)
            {
                if (FirstTableName != null && FirstTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(FirstTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName((string)FirstObject);
            }
            else if (FirstObjectType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(FirstObject);
            }
            else ret += FirstObject;

            ret += ", ";

            if (SecondObjectType == ValueObjectType.ColumnName)
            {
                if (SecondTableName != null && SecondTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(SecondTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName((string)SecondObject);
            }
            else if (SecondObjectType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(SecondObject);
            }
            else ret += SecondObject;

            ret += ")";

            return ret;
        }
    }
}
