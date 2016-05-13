using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class CountDistinct : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        
        public CountDistinct(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public CountDistinct(string columnName)
            : this(null, columnName)
        {
        }

        public CountDistinct(object theObject, ValueObjectType valueType)
        {
            this.Value = theObject;
            this.ValueType = valueType;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"COUNT(DISTINCT ";

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

            ret += ")";

            return ret;
        }
    }
}
