using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class UtcTimestamp : IPhrase
{
    public UtcTimestamp()
    {
    }

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.UtcNow());
    }
}
