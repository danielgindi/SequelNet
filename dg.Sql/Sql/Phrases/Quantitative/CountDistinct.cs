using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class CountDistinct : IPhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;
        
        public CountDistinct(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Object = columnName;
            this.ObjectType = ValueObjectType.ColumnName;
        }

        public CountDistinct(string columnName)
            : this(null, columnName)
        {
        }

        public CountDistinct(object theObject, ValueObjectType objectType)
        {
            this.Object = theObject;
            this.ObjectType = objectType;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"COUNT(DISTINCT ";

            if (ObjectType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Object.ToString());
            }
            else if (ObjectType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Object);
            }
            else ret += Object;

            ret += ")";

            return ret;
        }
    }
}
