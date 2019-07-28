using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class UTC_TIMESTAMP : IPhrase
    {
        public UTC_TIMESTAMP()
        {
        }

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.Language.func_UTC_NOW();
        }
    }
}
