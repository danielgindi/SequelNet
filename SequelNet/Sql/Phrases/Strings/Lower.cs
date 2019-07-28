using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Lower : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Lower(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Lower(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Lower(string columnName)
            : this(null, columnName)
        {
        }

        public Lower(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Lower(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.StringToLower(ret);
        }
    }
}
