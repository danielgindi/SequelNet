using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class StandardDeviationOfSample : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        
        [Obsolete]
        public StandardDeviationOfSample(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public StandardDeviationOfSample()
        {
            this.Value = "*";
            this.ValueType = ValueObjectType.Literal;
        }

        public StandardDeviationOfSample(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public StandardDeviationOfSample(string columnName)
            : this(null, columnName)
        {
        }

        public StandardDeviationOfSample(object value, ValueObjectType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"STDDEV_SAMP(";

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
