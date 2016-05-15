using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Sum : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        
        [Obsolete]
        public Sum(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public Sum()
        {
            this.Value = "*";
            this.ValueType = ValueObjectType.Literal;
        }

        public Sum(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public Sum(string columnName)
            : this(null, columnName)
        {
        }

        public Sum(object value, ValueObjectType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public Sum(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"SUM(";

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
