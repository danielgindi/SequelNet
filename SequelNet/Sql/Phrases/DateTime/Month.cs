using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Month : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Month(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Month(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Month(string columnName)
            : this(null, columnName)
        {
        }

        public Month(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Month(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.MonthPartOfDate(ret);
        }
    }
}
