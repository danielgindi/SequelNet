using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Min : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        
        [Obsolete]
        public Min(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public Min()
        {
            this.Value = "*";
            this.ValueType = ValueObjectType.Literal;
        }

        public Min(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public Min(string columnName)
            : this(null, columnName)
        {
        }

        public Min(object value, ValueObjectType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public Min(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"MIN(";

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
                ret += conn.PrepareValue(Value, relatedQuery);
            }
            else ret += Value;

            ret += ")";

            return ret;
        }
    }
}
