using SequelNet.Connector;
using System.Text;

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

        public Hour(ValueWrapper value)
        {
            this.Value = value;
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.HourPartOfDate(Value.Build(conn, relatedQuery)));
        }
    }
}
