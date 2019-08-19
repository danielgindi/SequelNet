using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Every : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Every()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public Every(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Every(string columnName)
            : this(null, columnName)
        {
        }

        public Every(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Every(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Every(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        public Every(WhereList where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.Language.Aggregate_Every(Value.Build(conn, relatedQuery));
        }
    }
}
