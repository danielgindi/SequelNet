using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Multiply : IPhrase
    {
        string TableName1;
        object Object1;
        ValueObjectType ObjectType1;
        string TableName2;
        object Object2;
        ValueObjectType ObjectType2;

        public Multiply(
            string tableName1, object object1, ValueObjectType objectType1,
            string tableName2, object object2, ValueObjectType objectType2
            )
        {
            this.TableName1 = tableName1;
            this.Object1 = object1;
            this.ObjectType1 = objectType1;
            this.TableName2 = tableName2;
            this.Object2 = object2;
            this.ObjectType2 = objectType2;
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

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret = "(";
            
            if (ObjectType1 == ValueObjectType.ColumnName)
            {
                if (TableName1 != null && TableName1.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName1);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Object1.ToString());
            }
            else if (ObjectType1 == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Object1);
            }
            else ret += Object1;

            ret += " * ";
            
            if (ObjectType2 == ValueObjectType.ColumnName)
            {
                if (TableName2 != null && TableName2.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName2);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Object2.ToString());
            }
            else if (ObjectType2 == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Object2);
            }
            else ret += Object2;

            ret += ')';

            return ret;
        }
    }
}
