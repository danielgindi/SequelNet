using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class ST_Y : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors
        
        public ST_Y(IPhrase phrase)
        {
            this.Value = ValueWrapper.From(phrase);
        }

        public ST_Y(string tableName, string column)
        {
            this.Value = ValueWrapper.Column(tableName, column);
        }

        public ST_Y(ValueWrapper value)
        {
            this.Value = value;
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.ST_Y(Value.Build(conn, relatedQuery)));
        }
    }
}
