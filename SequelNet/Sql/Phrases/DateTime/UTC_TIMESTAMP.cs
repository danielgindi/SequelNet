using SequelNet.Connector;
using System;
using System.Text;

namespace SequelNet.Phrases;

[Obsolete("Use UtcTimestamp instead")]
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
