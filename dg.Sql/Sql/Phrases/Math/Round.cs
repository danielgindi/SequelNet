﻿using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Round : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        public int DecimalPlaces;

        #region Constructors
        
        [Obsolete]
        public Round(string tableName, object value, ValueObjectType valueType, int decimalPlaces = 0)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
            this.DecimalPlaces = decimalPlaces;
        }

        public Round(object value, ValueObjectType valueType, int decimalPlaces = 0)
        {
            this.Value = value;
            this.ValueType = valueType;
            this.DecimalPlaces = decimalPlaces;
        }

        public Round(string tableName, string columnName, int decimalPlaces = 0)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
            this.DecimalPlaces = decimalPlaces;
        }

        public Round(string columnName, int decimalPlaces = 0)
            : this(null, columnName, decimalPlaces)
        {
        }

        public Round(IPhrase phrase, int decimalPlaces = 0)
            : this(phrase, ValueObjectType.Value, decimalPlaces)
        {
        }

        public Round(Where where)
            : this(where, ValueObjectType.Value)
        {
        }
        
        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"ROUND(";

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
