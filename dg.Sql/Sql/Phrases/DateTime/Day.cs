﻿using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Day : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;

        #region Constructors

        [Obsolete]
        public Day(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public Day(object value, ValueObjectType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public Day(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public Day(string columnName)
            : this(null, columnName)
        {
        }

        public Day(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Day(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            if (ValueType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(TableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Value.ToString());
            }
            else if (ValueType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Value, relatedQuery);
            }
            else ret += Value;

            return conn.func_DAY(ret);
        }
    }
}
