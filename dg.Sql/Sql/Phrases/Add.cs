using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Add : BasePhrase
    {
        string TableName1;
        object Object1;
        ValueObjectType ObjectType1;
        string TableName2;
        object Object2;
        ValueObjectType ObjectType2;

        public Add(string TableName1, object Object1, ValueObjectType ObjectType1, 
            string TableName2, object Object2, ValueObjectType ObjectType2)
        {
            this.TableName1 = TableName1;
            this.Object1 = Object1;
            this.ObjectType1 = ObjectType1;
            this.TableName2 = TableName2;
            this.Object2 = Object2;
            this.ObjectType2 = ObjectType2;
        }
        public Add(string TableName, string ColumnName, Int64 Value)
            : this(TableName, ColumnName, ValueObjectType.ColumnName, null, Value, ValueObjectType.Value)
        {
        }
        public Add(string ColumnName, Int64 Value)
            : this(null, ColumnName, ValueObjectType.ColumnName, null, Value, ValueObjectType.Value)
        {
        }
        public Add(string TableName1, string ColumnName1, string TableName2, string ColumnName2)
            : this(TableName1, ColumnName1, ValueObjectType.ColumnName, TableName2, ColumnName2, ValueObjectType.ColumnName)
        {
        }
        public Add(string ColumnName1, string ColumnName2)
            : this(null, ColumnName1, ValueObjectType.ColumnName, null, ColumnName2, ValueObjectType.ColumnName)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret = @"";

            if (ObjectType1 == ValueObjectType.ColumnName)
            {
                if (TableName1 != null && TableName1.Length > 0)
                {
                    ret += conn.encloseFieldName(TableName1);
                    ret += ".";
                }
                ret += conn.encloseFieldName(Object1.ToString());
            }
            else if (ObjectType1 == ValueObjectType.Value)
            {
                ret += @"(" + conn.prepareValue(Object1) + @")";
            }
            else ret += Object1;

            ret += @"+";

            if (ObjectType2 == ValueObjectType.ColumnName)
            {
                if (TableName2 != null && TableName2.Length > 0)
                {
                    ret += conn.encloseFieldName(TableName2);
                    ret += ".";
                }
                ret += conn.encloseFieldName(Object2.ToString());
            }
            else if (ObjectType2 == ValueObjectType.Value)
            {
                ret += @"(" + conn.prepareValue(Object2) + @")";
            }
            else ret += Object2;

            return ret;
        }
    }
}
