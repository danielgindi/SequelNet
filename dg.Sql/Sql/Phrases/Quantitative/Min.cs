using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Min : IPhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;
        
        [Obsolete]
        public Min(string tableName, object anObject, ValueObjectType objectType)
        {
            this.TableName = tableName;
            this.Object = anObject;
            this.ObjectType = objectType;
        }

        public Min()
        {
            this.Object = "*";
            this.ObjectType = ValueObjectType.Literal;
        }

        public Min(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Object = columnName;
            this.ObjectType = ValueObjectType.ColumnName;
        }

        public Min(string columnName)
            : this(null, columnName)
        {
        }

        public Min(object anObject, ValueObjectType objectType)
        {
            this.Object = anObject;
            this.ObjectType = objectType;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"MIN(";

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
