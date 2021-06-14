using SequelNet.Connector;
using System.Text;

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

        public Year(ValueWrapper value)
        {
            this.Value = value;
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.YearPartOfDate(Value.Build(conn, relatedQuery)));
        }
    }
}
