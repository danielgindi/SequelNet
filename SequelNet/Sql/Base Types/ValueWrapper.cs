using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet
{
    public struct ValueWrapper : IEquatable<ValueWrapper>
    {
        public string TableName;
        public object Value;
        public ValueObjectType Type;

        #region Constructors

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
            return new ValueWrapper(tableName, column, ValueObjectType.ColumnName);
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

        public static ValueWrapper From(byte value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static ValueWrapper From(sbyte value)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static ValueWrapper From(Int16 value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static ValueWrapper From(UInt16 value)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static ValueWrapper From(Int32 value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static ValueWrapper From(UInt32 value)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static ValueWrapper From(Int64 value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static ValueWrapper From(UInt64 value)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static ValueWrapper From(float value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static ValueWrapper From(double value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static ValueWrapper From(decimal value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static ValueWrapper Column(string tableName, string column)
        {
            return new ValueWrapper(tableName, column, ValueObjectType.ColumnName);
        }

        public static ValueWrapper Column(string column)
        {
            return new ValueWrapper(column, ValueObjectType.ColumnName);
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
                    ret += conn.Language.WrapFieldName(TableName);
                    ret += ".";
                }

                ret += conn.Language.WrapFieldName(Value.ToString());
            }
            else if (Type == ValueObjectType.Value)
            {
                ret += "(" + conn.Language.PrepareValue(conn, Value, relatedQuery) + ")";
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
                    outputBuilder.Append(conn.Language.WrapFieldName(TableName));
                    outputBuilder.Append(".");
                }

                outputBuilder.Append(conn.Language.WrapFieldName(Value.ToString()));
            }
            else if (Type == ValueObjectType.Value)
            {
                if (Value is Where)
                {
                    outputBuilder.Append("(");
                    ((Where)Value).BuildCommand(outputBuilder, true, new Where.BuildContext { Conn = conn, RelatedQuery = relatedQuery });
                    outputBuilder.Append(")");
                }
                else
                {
                    outputBuilder.Append("(" + conn.Language.PrepareValue(conn, Value, relatedQuery) + ")");
                }
            }
            else
            {
                outputBuilder.Append(Value.ToString());
            }
        }

        #endregion

        #region Casts

        public static explicit operator ValueWrapper(byte value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }
        
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static explicit operator ValueWrapper(sbyte value)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(Int16 value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static explicit operator ValueWrapper(UInt16 value)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(Int32 value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static explicit operator ValueWrapper(UInt32 value)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(Int64 value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(UInt64 value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(float value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(double value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(decimal value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(Where value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public static explicit operator ValueWrapper(WhereList value)
        {
            return new ValueWrapper(value, ValueObjectType.Value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ValueWrapper other))
                return false;

            if (TableName != other.TableName)
                return false;

            if (Type != other.Type)
                return false;

            if (!Object.Equals(Value, other.Value))
                return false;

            return true;
        }

        public bool Equals(ValueWrapper other)
        {
            if (TableName != other.TableName)
                return false;

            if (Type != other.Type)
                return false;

            if (!Object.Equals(Value, other.Value))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 1551432323;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TableName);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ValueWrapper lhs, ValueWrapper rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ValueWrapper lhs, ValueWrapper rhs)
        {
            return !lhs.Equals(rhs);
        }

        #endregion
    }
}
