using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Round : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        public int DecimalPlaces;

        public Round(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
            this.DecimalPlaces = 0;
        }

        public Round(string tableName, object value, ValueObjectType valueType, int DecimalPlaces)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
            this.DecimalPlaces = DecimalPlaces;
        }

        public Round(object value, ValueObjectType valueType)
            : this(null, value, valueType)
        {
        }

        public Round(object value, ValueObjectType valueType, int DecimalPlaces)
            : this(null, value, valueType, DecimalPlaces)
        {
        }

        public Round(string columnName)
            : this(null, columnName, ValueObjectType.ColumnName)
        {
        }

        public Round(string columnName, int DecimalPlaces)
            : this(null, columnName, ValueObjectType.ColumnName, DecimalPlaces)
        {
        }

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"ROUND(";

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

            if (DecimalPlaces != 0)
            {
                ret += ',';
                ret += DecimalPlaces;
            }
            ret += ')';

            return ret;
        }
    }
}
