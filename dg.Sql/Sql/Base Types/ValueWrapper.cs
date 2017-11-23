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
        public ValueObjectType Type;

        #region Constructors

        public ValueWrapper()
        {
            this.Type = ValueObjectType.Value;
        }

        public ValueWrapper(string tableName, object value, ValueObjectType type)
        {
            this.TableName = tableName;
            this.Value = value;
            this.Type = type;
        }

        public ValueWrapper(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.Type = ValueObjectType.ColumnName;
        }

        public ValueWrapper(string column)
        {
            this.TableName = null;
            this.Value = column;
            this.Type = ValueObjectType.ColumnName;
        }

        public ValueWrapper(object value, ValueObjectType type)
        {
            this.TableName = null;
            this.Value = value;
            this.Type = type;
        }

        public ValueWrapper(IPhrase phrase)
        {
            this.TableName = null;
            this.Value = phrase;
            this.Type = ValueObjectType.Value;
        }

        public ValueWrapper(Where where)
        {
            this.TableName = null;
            this.Value = where;
            this.Type = ValueObjectType.Value;
        }

        #endregion

        #region Convenience
        
        public static ValueWrapper From(string tableName, object value, ValueObjectType type)
        {
            return new ValueWrapper(tableName, value, type);
        }

        public static ValueWrapper From(string tableName, string column)
        {
            return new ValueWrapper(tableName, column);
        }

        public static ValueWrapper From(object value, ValueObjectType type)
        {
            return new ValueWrapper(value, type);
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

            if (Type == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.WrapFieldName(TableName);
                    ret += ".";
                }

                ret += conn.WrapFieldName(Value.ToString());
            }
            else if (Type == ValueObjectType.Value)
            {
                ret += "(" + conn.PrepareValue(Value, relatedQuery) + ")";
            }
            else
            {
                ret += Value.ToString();
            }

            return ret;
        }

        public void Build(StringBuilder outputBuilder, ConnectorBase conn, Query relatedQuery = null)
        {
            if (Type == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    outputBuilder.Append(conn.WrapFieldName(TableName));
                    outputBuilder.Append(".");
                }

                outputBuilder.Append(conn.WrapFieldName(Value.ToString()));
            }
            else if (Type == ValueObjectType.Value)
            {
                if (Value is dg.Sql.Where)
                {
                    outputBuilder.Append("(");
                    ((dg.Sql.Where)Value).BuildCommand(outputBuilder, true, conn, relatedQuery);
                    outputBuilder.Append(")");
                }
                else
                {
                    outputBuilder.Append("(" + conn.PrepareValue(Value, relatedQuery) + ")");
                }
            }
            else
            {
                outputBuilder.Append(Value.ToString());
            }
        }

        #endregion
    }
}
