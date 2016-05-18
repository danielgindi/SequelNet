using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class RandWeight : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;

        #region Constructors

        [Obsolete]
        public RandWeight(string tableName, string value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public RandWeight(object value, ValueObjectType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public RandWeight(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public RandWeight(string columnName)
            : this(null, columnName)
        {
        }

        public RandWeight(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public RandWeight(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;
            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                ret = @"RAND(CAST(NEWID() AS VARBINARY)) * ";
            else if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                ret = @"RAND() * ";
            else // if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                ret = @"RANDOM() * ";

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

            return ret;
        }
    }
}
