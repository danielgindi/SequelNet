using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Count : IPhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;

        [Obsolete]
        public Count(string tableName, object anObject, ValueObjectType objectType)
        {
            this.TableName = tableName;
            this.Object = anObject;
            this.ObjectType = objectType;
        }

        public Count()
        {
            this.Object = "*";
            this.ObjectType = ValueObjectType.Literal;
        }

        public Count(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Object = columnName;
            this.ObjectType = ValueObjectType.ColumnName;
        }

        public Count(string columnName)
            : this(null, columnName)
        {
        }

        public Count(object anObject, ValueObjectType objectType)
        {
            this.Object = anObject;
            this.ObjectType = objectType;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"COUNT(";

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
