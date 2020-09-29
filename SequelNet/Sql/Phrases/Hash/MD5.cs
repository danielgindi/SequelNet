using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class MD5 : IPhrase
    {
        public ValueWrapper Value;
        public bool Binary = false;

        #region Constructors

        public MD5(object value, ValueObjectType valueType, bool binary = false)
        {
            this.Value = ValueWrapper.Make(value, valueType);
            this.Binary = binary;
        }

        public MD5(string tableName, string columnName, bool binary = false)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Binary = binary;
        }

        public MD5(string columnName, bool binary = false)
            : this(null, columnName, binary)
        {
        }

        public MD5(IPhrase phrase, bool binary = false)
            : this(phrase, ValueObjectType.Value, binary)
        {
        }

        public MD5(Where where, bool binary = false)
            : this(where, ValueObjectType.Value, binary)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return Binary ? conn.Language.Md5Binary(ret) : conn.Language.Md5Hex(ret);
        }
    }
}
