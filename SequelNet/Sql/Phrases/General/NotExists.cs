using System.Text;
using SequelNet.Connector;

#nullable enable

namespace SequelNet.Phrases;

public class NotExists : IPhrase
{
    public Query Query;

    public NotExists(Query query)
    {
        Query = query;
    }

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        sb.Append("NOT EXISTS (");
        sb.Append(Query.BuildCommand(conn));
        sb.Append(')');
    }
}
