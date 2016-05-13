using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Upper : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;

        #region Constructors

        public Upper(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public Upper(object value, ValueObjectType valueType)
            : this(null, value, valueType)
        {
        }

        public Upper(string columnName)
            : this(null, columnName, ValueObjectType.ColumnName)
        {
        }

        public Upper(IPhrase phrase)
            : this(null, phrase, ValueObjectType.Value)
        {
        }

        #endregion

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

            return conn.func_UPPER + @"(" + (ret) + @")";
        }
    }
}
