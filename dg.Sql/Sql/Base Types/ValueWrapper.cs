using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql
{
    public class ValueWrapper
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;

        #region Constructors

        public ValueWrapper()
        {
        }

        public ValueWrapper(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public ValueWrapper(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public ValueWrapper(string column)
        {
            this.TableName = null;
            this.Value = column;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public ValueWrapper(object value, ValueObjectType valueType)
        {
            this.TableName = null;
            this.Value = value;
            this.ValueType = valueType;
        }

        public ValueWrapper(IPhrase phrase)
        {
            this.TableName = null;
            this.Value = phrase;
            this.ValueType = ValueObjectType.Value;
        }

        public ValueWrapper(Where where)
        {
            this.TableName = null;
            this.Value = where;
            this.ValueType = ValueObjectType.Value;
        }

        #endregion

        #region Convenience
        
        public static ValueWrapper From(string tableName, object value, ValueObjectType valueType)
        {
            return new ValueWrapper(tableName, value, valueType);
        }

        public static ValueWrapper From(string tableName, string column)
        {
            return new ValueWrapper(tableName, column);
        }

        public static ValueWrapper From(object value, ValueObjectType valueType)
        {
            return new ValueWrapper(value, valueType);
        }

        public static ValueWrapper From(IPhrase phrase)
        {
            return new ValueWrapper(phrase);
        }

        public static ValueWrapper From(Where where)
        {
            return new ValueWrapper(where);
        }

        public static ValueWrapper Literal(string literal)
        {
            return new ValueWrapper(null, literal, ValueObjectType.Literal);
        }

        #endregion

        #region Builders

        public string Build(ConnectorBase conn, Query relatedQuery = null)
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
            else
            {
                ret += Value;
            }

            return ret;
        }

        #endregion
    }
}
