using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class RandWeight : BasePhrase
    {
        string TableName;
        string Object;
        ValueObjectType ObjectType;

        public RandWeight(
            string TableName, string Object, ValueObjectType ObjectType)
        {
            this.TableName = TableName;
            this.Object = Object;
            this.ObjectType = ObjectType;
        }
        public RandWeight(
             string Object, ValueObjectType ObjectType)
            : this(null, Object, ObjectType)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;
            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                ret = @"RAND(CAST(NEWID() AS VARBINARY)) * ";
            else if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                ret = @"RAND() * ";
            else // if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                ret = @"RANDOM() * ";

            if (ObjectType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Object);
            }
            else if (ObjectType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Object);
            }
            else ret += Object;

            return ret;
        }
    }
}
