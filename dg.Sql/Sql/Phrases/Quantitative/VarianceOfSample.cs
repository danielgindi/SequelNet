using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class VarianceOfSample : BasePhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;

        public VarianceOfSample(string TableName, object Object, ValueObjectType ObjectType)
        {
            this.TableName = TableName;
            this.Object = Object;
            this.ObjectType = ObjectType;
        }
        public VarianceOfSample(object Object, ValueObjectType ObjectType)
            : this(null, Object, ObjectType)
        {
        }
        public VarianceOfSample(string ColumnName)
            : this(null, ColumnName, ValueObjectType.ColumnName)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"VAR_SAMP(";

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
