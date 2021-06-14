using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Second : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Second(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Second(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Second(string columnName)
            : this(null, columnName)
        {
        }

        public Second(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Second(ValueWrapper value)
        {
            this.Value = value;
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.SecondPartOfDate(Value.Build(conn, relatedQuery)));
        }
    }
}
