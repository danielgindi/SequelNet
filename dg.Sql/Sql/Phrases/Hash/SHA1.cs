using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class SHA1 : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        public bool Binary = false;

        #region Constructors

        [Obsolete]
        public SHA1(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public SHA1(object value, ValueObjectType valueType, bool binary = false)
        {
            this.Value = value;
            this.ValueType = valueType;
            this.Binary = binary;
        }

        public SHA1(string tableName, string columnName, bool binary = false)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
            this.Binary = binary;
        }

        public SHA1(string columnName, bool binary = false)
            : this(null, columnName, binary)
        {
        }

        public SHA1(IPhrase phrase, bool binary = false)
            : this(phrase, ValueObjectType.Value, binary)
        {
        }

        public SHA1(Where where, bool binary = false)
            : this(where, ValueObjectType.Value, binary)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            if (ValueType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.WrapFieldName(TableName);
                    ret += ".";
                }
                ret += conn.WrapFieldName(Value.ToString());
            }
            else if (ValueType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Value, relatedQuery);
            }
            else ret += Value;

            return Binary ? conn.func_SHA1_Binary(ret) : conn.func_SHA1_Hex(ret);
        }
    }
}
