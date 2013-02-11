using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class IfNull : BasePhrase
    {
        string FirstTableName;
        string FirstObject;
        ValueObjectType FirstObjectType;
        string SecondTableName;
        string SecondObject;
        ValueObjectType SecondObjectType;

        public IfNull(
            string FirstTableName, string FirstObject, ValueObjectType FirstObjectType,
            string SecondTableName, string SecondObject, ValueObjectType SecondObjectType)
        {
            this.FirstTableName = FirstTableName;
            this.FirstObject = FirstObject;
            this.FirstObjectType = FirstObjectType;
            this.SecondTableName = SecondTableName;
            this.SecondObject = SecondObject;
            this.SecondObjectType = SecondObjectType;
        }
        public IfNull(
             string FirstObject, ValueObjectType FirstObjectType,
             string SecondObject, ValueObjectType SecondObjectType)
            : this(null, FirstObject, FirstObjectType, null, SecondObject, SecondObjectType)
        {
        }
        public IfNull(
             string FirstTableName, string FirstObject, ValueObjectType FirstObjectType,
             string SecondObject, ValueObjectType SecondObjectType)
            : this(FirstTableName, FirstObject, FirstObjectType, null, SecondObject, SecondObjectType)
        {
        }
        public IfNull(
             string FirstObject, ValueObjectType FirstObjectType,
             string SecondTableName, string SecondObject, ValueObjectType SecondObjectType)
            : this(null, FirstObject, FirstObjectType, SecondTableName, SecondObject, SecondObjectType)
        {
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
                    ret += conn.encloseFieldName(FirstTableName);
                    ret += ".";
                }
                ret += conn.encloseFieldName(FirstObject);
            }
            else if (FirstObjectType == ValueObjectType.Value)
            {
                ret += conn.prepareValue(FirstObject);
            }
            else ret += FirstObject;

            ret += ", ";

            if (SecondObjectType == ValueObjectType.ColumnName)
            {
                if (SecondTableName != null && SecondTableName.Length > 0)
                {
                    ret += conn.encloseFieldName(SecondTableName);
                    ret += ".";
                }
                ret += conn.encloseFieldName(SecondObject);
            }
            else if (SecondObjectType == ValueObjectType.Value)
            {
                ret += conn.prepareValue(SecondObject);
            }
            else ret += SecondObject;

            ret += ")";

            return ret;
        }
    }
}
