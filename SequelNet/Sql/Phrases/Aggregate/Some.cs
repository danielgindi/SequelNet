using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Some : BaseAggregatePhrase
    {
        #region Constructors

        public Some() : base()
        {
        }

        public Some(string tableName, string columnName) : base(tableName, columnName)
        {
        }

        public Some(string columnName) : base(columnName)
        {
        }

        public Some(object value, ValueObjectType valueType) : base(value, valueType)
        {
        }

        public Some(IPhrase phrase) : base(phrase)
        {
        }

        public Some(Where where) : base(where)
        {
        }

        public Some(WhereList where) : base(where)
        {
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.Language.Aggregate_Some(Value.Build(conn, relatedQuery));
        }
    }
}
