using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class PassThroughAggregate : IPhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;
        string AggregateType;

        public PassThroughAggregate(string aggregateType, string tableName, object anObject, ValueObjectType objectType)
        {
            this.TableName = tableName;
            this.Object = anObject;
            this.ObjectType = objectType;
            this.AggregateType = aggregateType;
        }
        public PassThroughAggregate(string aggregateType, object anObject, ValueObjectType objectType)
            : this(aggregateType, null, anObject, objectType)
        {
        }
        public PassThroughAggregate(string aggregateType, string columnName)
            : this(aggregateType, null, columnName, ValueObjectType.ColumnName)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = AggregateType + @"(";

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
