using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

public class Exists : IPhrase
{
    public Query Query;

    public Exists(Query query)
    {
        Query = query;
    }

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("EXISTS (");
        sb.Append(Query.BuildCommand(conn));
        sb.Append(')');
    }
}
