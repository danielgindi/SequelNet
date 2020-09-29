using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Upper : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Upper(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Upper(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Upper(string columnName)
            : this(null, columnName)
        {
        }

        public Upper(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Upper(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.StringToUpper(ret);
        }
    }
}
