using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Floor : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Floor(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public Floor(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public Floor(string columnName)
            : this(null, columnName)
        {
        }

        public Floor(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Floor(Where where)
            : this(where, ValueObjectType.Value)
        {
        }
        
        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append("FLOOR(");
            sb.Append(Value.Build(conn, relatedQuery));
            sb.Append(')');
        }
    }
}
