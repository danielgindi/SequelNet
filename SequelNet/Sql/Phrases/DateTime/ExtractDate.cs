using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class ExtractDate : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public ExtractDate(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public ExtractDate(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public ExtractDate(string columnName)
            : this(null, columnName)
        {
        }

        public ExtractDate(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public ExtractDate(ValueWrapper value)
        {
            this.Value = value;
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.DatePartOfDateTime(Value.Build(conn, relatedQuery)));
        }
    }
}
