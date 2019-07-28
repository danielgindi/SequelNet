using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Second : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Second(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Second(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Second(string columnName)
            : this(null, columnName)
        {
        }

        public Second(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Second(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.SecondPartOfDate(ret);
        }
    }
}
