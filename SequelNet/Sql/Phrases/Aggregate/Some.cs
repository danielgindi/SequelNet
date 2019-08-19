using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Some : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Some()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public Some(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Some(string columnName)
            : this(null, columnName)
        {
        }

        public Some(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Some(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Some(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        public Some(WhereList where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.Language.Aggregate_Some(Value.Build(conn, relatedQuery));
        }
    }
}
