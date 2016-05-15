using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Multiply : IPhrase
    {
        public string TableName1;
        public object Value1;
        public ValueObjectType ValueType1;
        public string TableName2;
        public object Value2;
        public ValueObjectType ValueType2;

        public Multiply(
            string tableName1, object value1, ValueObjectType valueType1,
            string tableName2, object value2, ValueObjectType valueType2
            )
        {
            this.TableName1 = tableName1;
            this.Value1 = value1;
            this.ValueType1 = valueType1;
            this.TableName2 = tableName2;
            this.Value2 = value2;
            this.ValueType2 = valueType2;
        }

        public Multiply(
            string tableName1, string column1,
            string tableName2, string column2
            )
            : this(tableName1, column1, ValueObjectType.ColumnName, tableName2, column2, ValueObjectType.ColumnName)
        {
        }

        public Multiply(
            string tableName1, string column1,
            object value2
            )
            : this(tableName1, column1, ValueObjectType.ColumnName, null, value2, ValueObjectType.Value)
        {
        }

        public Multiply(
            object value1,
            string tableName2, string column2
            )
            : this(null, value1, ValueObjectType.Value, tableName2, column2, ValueObjectType.ColumnName)
        {
        }

        public Multiply(
            object value1,
            object value2
            )
            : this(null, value1, ValueObjectType.Value, null, value2, ValueObjectType.Value)
        {
        }

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "(";
            
            if (ValueType1 == ValueObjectType.ColumnName)
            {
                if (TableName1 != null && TableName1.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName1);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Value1.ToString());
            }
            else if (ValueType1 == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Value1, relatedQuery);
            }
            else ret += Value1;

            ret += " * ";
            
            if (ValueType2 == ValueObjectType.ColumnName)
            {
                if (TableName2 != null && TableName2.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName2);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Value2.ToString());
            }
            else if (ValueType2 == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Value2, relatedQuery);
            }
            else ret += Value2;

            ret += ')';

            return ret;
        }
    }
}
