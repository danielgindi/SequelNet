using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Count : IPhrase
    {
        public bool Distinct = false;
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;

        [Obsolete]
        public Count(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public Count(bool distinct = false)
        {
            this.Distinct = distinct;
            this.Value = "*";
            this.ValueType = ValueObjectType.Literal;
        }

        public Count(string tableName, string columnName, bool distinct = false)
        {
            this.Distinct = distinct;
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public Count(string columnName, bool distinct = false)
            : this(null, columnName, distinct)
        {
        }

        public Count(object value, ValueObjectType valueType, bool distinct = false)
        {
            this.Distinct = distinct;
            this.Value = value;
            this.ValueType = valueType;
        }

        public Count(IPhrase phrase, bool distinct = false)
            : this(phrase, ValueObjectType.Value, distinct)
        {
        }

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            if (Distinct)
            {
                ret = @"COUNT(DISTINCT ";
            }
            else
            {
                ret = @"COUNT(";
            }

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
