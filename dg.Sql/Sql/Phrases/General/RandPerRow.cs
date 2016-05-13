using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class RandWeight : IPhrase
    {
        public string TableName;
        public string Value;
        public ValueObjectType ValueType;

        public RandWeight(
            string tableName, string value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public RandWeight(
             string Object, ValueObjectType valueType)
            : this(null, Object, valueType)
        {
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;
            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                ret = @"RAND(CAST(NEWID() AS VARBINARY)) * ";
            else if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                ret = @"RAND() * ";
            else // if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                ret = @"RANDOM() * ";

            if (ValueType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Value);
            }
            else if (ValueType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Value);
            }
            else ret += Value;

            return ret;
        }
    }
}
