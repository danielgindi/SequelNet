using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Minute : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Minute(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Minute(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Minute(string columnName)
            : this(null, columnName)
        {
        }

        public Minute(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Minute(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.MinutePartOfDate(ret);
        }
    }
}
