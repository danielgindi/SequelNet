using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Max : IPhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;
        
        [Obsolete]
        public Max(string tableName, object anObject, ValueObjectType objectType)
        {
            this.TableName = tableName;
            this.Object = anObject;
            this.ObjectType = objectType;
        }

        public Max()
        {
            this.Object = "*";
            this.ObjectType = ValueObjectType.Literal;
        }

        public Max(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Object = columnName;
            this.ObjectType = ValueObjectType.ColumnName;
        }

        public Max(string columnName)
            : this(null, columnName)
        {
        }

        public Max(object anObject, ValueObjectType objectType)
        {
            this.Object = anObject;
            this.ObjectType = objectType;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"MAX(";

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
