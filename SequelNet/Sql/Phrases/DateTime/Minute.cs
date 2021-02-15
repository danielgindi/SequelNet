using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Minute : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Minute(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Minute(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Minute(string columnName)
            : this(null, columnName)
        {
        }

        public Minute(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Minute(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.MinutePartOfDate(Value.Build(conn, relatedQuery)));
        }
    }
}
