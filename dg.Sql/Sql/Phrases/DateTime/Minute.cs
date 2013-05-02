using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Minute : BasePhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;

        public Minute(string TableName, object Object, ValueObjectType ObjectType)
        {
            this.TableName = TableName;
            this.Object = Object;
            this.ObjectType = ObjectType;
        }
        public Minute(object Object, ValueObjectType ObjectType)
            : this(null, Object, ObjectType)
        {
        }
        public Minute(string ColumnName)
            : this(null, ColumnName, ValueObjectType.ColumnName)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret = "";

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

            return conn.func_MINUTE(ret);
        }
    }
}
