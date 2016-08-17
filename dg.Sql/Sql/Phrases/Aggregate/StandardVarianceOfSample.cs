﻿using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class StandardVarianceOfSample : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;

        #region Constructors

        [Obsolete]
        public StandardVarianceOfSample(string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
        }

        public StandardVarianceOfSample()
        {
            this.Value = "*";
            this.ValueType = ValueObjectType.Literal;
        }

        public StandardVarianceOfSample(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
        }

        public StandardVarianceOfSample(string columnName)
            : this(null, columnName)
        {
        }

        public StandardVarianceOfSample(object value, ValueObjectType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public StandardVarianceOfSample(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"VAR_SAMP(";

            if (ValueType == ValueObjectType.ColumnName)
            {
                if (TableName != null && TableName.Length > 0)
                {
                    ret += conn.WrapFieldName(TableName);
                    ret += ".";
                }
                ret += conn.WrapFieldName(Value.ToString());
            }
            else if (ValueType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Value, relatedQuery);
            }
            else ret += Value;

            ret += ")";

            return ret;
        }
    }
}