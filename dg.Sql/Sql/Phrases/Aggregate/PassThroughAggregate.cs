using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class PassThroughAggregate : IPhrase
    {
        public ValueWrapper Value;
        public string AggregateType;

        #region Constructors

        [Obsolete]
        public PassThroughAggregate(string aggregateType, string tableName, object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(tableName, value, valueType);
            this.AggregateType = aggregateType;
        }

        public PassThroughAggregate(string aggregateType, string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.AggregateType = aggregateType;
        }

        public PassThroughAggregate(string aggregateType, string columnName)
            : this(aggregateType, columnName, ValueObjectType.ColumnName)
        {
        }

        public PassThroughAggregate(string aggregateType, object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
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

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
