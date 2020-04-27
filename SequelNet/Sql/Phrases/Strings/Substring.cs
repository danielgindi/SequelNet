using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Substring : IPhrase
    {
        public ValueWrapper Value;
        public ValueWrapper From;
        public ValueWrapper? Length;

        #region Constructors

        public Substring(object value, ValueObjectType valueType, int from, int? length = null)
        {
            this.Value = new ValueWrapper(value, valueType);
            this.From = ValueWrapper.From(from);
            this.Length = length != null ? ValueWrapper.From(length.Value) : (ValueWrapper?)null;
        }

        public Substring(object value, ValueObjectType valueType, ValueWrapper from, ValueWrapper? length = null)
        {
            this.Value = new ValueWrapper(value, valueType);
            this.From = from;
            this.Length = length;
        }

        public Substring(ValueWrapper value, ValueWrapper from, ValueWrapper? length = null)
        {
            this.Value = value;
            this.From = from;
            this.Length = length;
        }

        public Substring(string tableName, string columnName, int from, int? length = null)
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.From = ValueWrapper.From(from);
            this.Length = length != null ? ValueWrapper.From(length.Value) : (ValueWrapper?)null;
        }

        public Substring(string columnName, int from, int? length = null)
            : this(null, columnName, from, length)
        {
        }

        public Substring(IPhrase phrase, int from, int? length = null)
            : this(phrase, ValueObjectType.Value, from, length)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "SUBSTRING(";

            ret += Value.Build(conn, relatedQuery);

            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL ||
                conn.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
            {
                ret += ", " + From + ", " + Length;
            }
            else
            {
                ret += " FROM " + From + " FOR " + Length;
            }

            ret += ")";

            return ret;
        }
    }
}
