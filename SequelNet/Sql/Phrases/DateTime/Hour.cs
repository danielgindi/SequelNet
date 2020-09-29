using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Hour : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Hour(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Hour(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Hour(string columnName)
            : this(null, columnName)
        {
        }

        public Hour(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Hour(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.HourPartOfDate(ret);
        }
    }
}
