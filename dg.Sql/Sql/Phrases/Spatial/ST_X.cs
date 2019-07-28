using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class ST_X : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors
        
        public ST_X(IPhrase phrase)
        {
            this.Value = new ValueWrapper(phrase);
        }

        public ST_X(string tableName, string column)
        {
            this.Value = new ValueWrapper(tableName, column);
        }

        public ST_X(ValueWrapper value)
        {
            this.Value = value;
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.func_ST_X(Value.Build(conn, relatedQuery));
        }
    }
}
