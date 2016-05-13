using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class StandardVarianceOfSample : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        
        [Obsolete]
        public StandardVarianceOfSample(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public StandardVarianceOfSample()
        {
            this.Value = "*";
            this.ValueType = ValueObjectType.Literal;
        }

        public StandardVarianceOfSample(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public StandardVarianceOfSample(string columnName)
            : this(null, columnName)
        {
        }

        public StandardVarianceOfSample(object value, ValueObjectType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"VAR_SAMP(";

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
