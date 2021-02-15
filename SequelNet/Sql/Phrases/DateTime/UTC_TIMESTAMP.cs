using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class UTC_TIMESTAMP : IPhrase
    {
        public UTC_TIMESTAMP()
        {
        }

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.UtcNow());
        }
    }
}
