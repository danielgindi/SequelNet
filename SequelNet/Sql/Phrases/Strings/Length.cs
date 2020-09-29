using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Length : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Length(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Length(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Length(string columnName)
            : this(null, columnName)
        {
        }

        public Length(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Length(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.Language.LengthOfString(ret);
        }
    }
}
