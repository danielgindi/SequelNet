using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Every : BaseAggregatePhrase
    {
        #region Constructors

        public Every() : base()
        {
        }

        public Every(string tableName, string columnName) : base(tableName, columnName)
        {
        }

        public Every(string columnName) : base(columnName)
        {
        }

        public Every(object value, ValueObjectType valueType) : base(value, valueType)
        {
        }

        public Every(IPhrase phrase) : base(phrase)
        {
        }

        public Every(Where where) : base(where)
        {
        }

        public Every(WhereList where) : base(where)
        {
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.Language.Aggregate_Every(Value.Build(conn, relatedQuery));
        }
    }
}
