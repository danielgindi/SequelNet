using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class PassThroughAggregate : IPhrase
    {
        public string TableName;
        public object Value;
        public ValueObjectType ValueType;
        public string AggregateType;

        #region Constructors

        [Obsolete]
        public PassThroughAggregate(string aggregateType, string tableName, object value, ValueObjectType valueType)
        {
            this.TableName = tableName;
            this.Value = value;
            this.ValueType = valueType;
            this.AggregateType = aggregateType;
        }

        public PassThroughAggregate(string aggregateType, string tableName, string columnName)
        {
            this.TableName = tableName;
            this.Value = columnName;
            this.ValueType = ValueObjectType.ColumnName;
            this.AggregateType = aggregateType;
        }

        public PassThroughAggregate(string aggregateType, string columnName)
            : this(aggregateType, columnName, ValueObjectType.ColumnName)
        {
        }

        public PassThroughAggregate(string aggregateType, object value, ValueObjectType valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
            this.AggregateType = aggregateType;
        }

        public PassThroughAggregate(string aggregateType, IPhrase phrase)
            : this(aggregateType, phrase, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = AggregateType + @"(";

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
