using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Round : BasePhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;
        int DecimalPlaces;

        public Round(string TableName, object Object, ValueObjectType ObjectType)
        {
            this.TableName = TableName;
            this.Object = Object;
            this.ObjectType = ObjectType;
            this.DecimalPlaces = 0;
        }
        public Round(string TableName, object Object, ValueObjectType ObjectType, int DecimalPlaces)
        {
            this.TableName = TableName;
            this.Object = Object;
            this.ObjectType = ObjectType;
            this.DecimalPlaces = DecimalPlaces;
        }
        public Round(object Object, ValueObjectType ObjectType)
            : this(null, Object, ObjectType)
        {
        }
        public Round(object Object, ValueObjectType ObjectType, int DecimalPlaces)
            : this(null, Object, ObjectType, DecimalPlaces)
        {
        }
        public Round(string ColumnName)
            : this(null, ColumnName, ValueObjectType.ColumnName)
        {
        }
        public Round(string ColumnName, int DecimalPlaces)
            : this(null, ColumnName, ValueObjectType.ColumnName, DecimalPlaces)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            string ret;

            ret = @"ROUND(";

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

            if (DecimalPlaces != 0)
            {
                ret += ',';
                ret += DecimalPlaces;
            }
            ret += ')';

            return ret;
        }
    }
}
