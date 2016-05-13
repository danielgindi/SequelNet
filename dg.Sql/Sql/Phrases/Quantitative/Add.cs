using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Add : IPhrase
    {
        public string TableName1;
        public object Value1;
        public ValueObjectType ValueType1;
        public string TableName2;
        public object Value2;
        public ValueObjectType ValueType2;

        public Add(string tableName1, object value1, ValueObjectType valueType1, 
            string tableName2, object value2, ValueObjectType valueType2)
        {
            this.TableName1 = tableName1;
            this.Value1 = value1;
            this.ValueType1 = valueType1;
            this.TableName2 = tableName2;
            this.Value2 = value2;
            this.ValueType2 = valueType2;
        }

        public Add(string tableName, string columnName, Int64 value)
            : this(tableName, columnName, ValueObjectType.ColumnName, null, value, ValueObjectType.Value)
        {
        }

        public Add(string columnName, Int64 value)
            : this(null, columnName, ValueObjectType.ColumnName, null, value, ValueObjectType.Value)
        {
        }

        public Add(string tableName1, string columnName1, string tableName2, string columnName2)
            : this(tableName1, columnName1, ValueObjectType.ColumnName, tableName2, columnName2, ValueObjectType.ColumnName)
        {
        }

        public Add(string columnName1, string columnName2)
            : this(null, columnName1, ValueObjectType.ColumnName, null, columnName2, ValueObjectType.ColumnName)
        {
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret = @"";

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
                ret += @"(" + conn.PrepareValue(Value1) + @")";
            }
            else ret += Value1;

            ret += @"+";

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
                ret += @"(" + conn.PrepareValue(Value2) + @")";
            }
            else ret += Value2;

            return ret;
        }
    }
}
