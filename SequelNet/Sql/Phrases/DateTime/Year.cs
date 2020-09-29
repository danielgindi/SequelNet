using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Year : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Year(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Year(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Year(string columnName)
            : this(null, columnName)
        {
        }

        public Year(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Year(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.YearPartOfDate(ret);
        }
    }
}
