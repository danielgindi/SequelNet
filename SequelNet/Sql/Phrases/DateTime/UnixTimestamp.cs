using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class UnixTimestamp : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public UnixTimestamp(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public UnixTimestamp(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public UnixTimestamp(string columnName)
            : this(null, columnName)
        {
        }

        public UnixTimestamp(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public UnixTimestamp(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.ExtractUnixTimestamp(Value.Build(conn, relatedQuery)));
        }
    }
}
