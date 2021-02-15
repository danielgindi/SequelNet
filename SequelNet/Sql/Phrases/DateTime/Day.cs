using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Day : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Day(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Day(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Day(string columnName)
            : this(null, columnName)
        {
        }

        public Day(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Day(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.DayPartOfDate(Value.Build(conn, relatedQuery)));
        }
    }
}
