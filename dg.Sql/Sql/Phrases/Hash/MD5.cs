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
        public bool Binary = false;

        #region Constructors

        [Obsolete]
        public MD5(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public MD5(object value, ValueObjectType valueType, bool binary = false)
        {
            this.Value = value;
            this.ValueType = valueType;
            this.Binary = binary;
        }

        public MD5(string tableName, string columnName, bool binary = false)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
            this.Binary = binary;
        }

        public MD5(string columnName, bool binary = false)
            : this(null, columnName, binary)
        {
        }

        public MD5(IPhrase phrase, bool binary = false)
            : this(phrase, ValueObjectType.Value, binary)
        {
        }

        public MD5(Where where, bool binary = false)
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

            return Binary ? conn.func_MD5_Binary(ret) : conn.func_MD5_Hex(ret);
        }
    }
}
