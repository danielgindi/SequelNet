using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class MD5 : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;

        public MD5(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public MD5(object value, ValueObjectType valueType)
            : this(null, value, valueType)
        {
        }

        public MD5(string columnName)
            : this(null, columnName, ValueObjectType.ColumnName)
        {
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret = "";

            if (ValueType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Value.ToString());
            }
            else if (ValueType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Value);
            }
            else ret += Value;

            return conn.func_MD5(ret);
        }
    }
}
