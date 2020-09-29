using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class PassThroughAggregate : IPhrase
    {
        public ValueWrapper Value;
        public string AggregateType;

        #region Constructors

        public PassThroughAggregate(string aggregateType, string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.AggregateType = aggregateType;
        }

        public PassThroughAggregate(string aggregateType, string columnName)
            : this(aggregateType, columnName, ValueObjectType.ColumnName)
        {
        }

        public PassThroughAggregate(string aggregateType, object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
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
