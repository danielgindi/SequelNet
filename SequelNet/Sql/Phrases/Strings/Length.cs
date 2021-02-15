using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Length : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Length(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Length(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Length(string columnName)
            : this(null, columnName)
        {
        }

        public Length(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Length(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.LengthOfString(Value.Build(conn, relatedQuery)));
        }
    }
}
