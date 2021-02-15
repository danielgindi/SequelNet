using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Lower : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Lower(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Lower(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Lower(string columnName)
            : this(null, columnName)
        {
        }

        public Lower(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Lower(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.StringToLower(Value.Build(conn, relatedQuery)));
        }
    }
}
