using System.Text;
using SequelNet.Connector;

#nullable enable

namespace SequelNet.Phrases;

public class Union : IPhrase
{
    public Query[] Queries;
    public bool All = false;

    public Union(params Query[] queries)
    {
        Queries = queries;
    }

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        if (Queries.Length == 0)
            throw new System.InvalidOperationException("Union requires at least one query");

        bool first = true;
        sb.Append("(");
        foreach (Query qry in Queries)
        {
            if (first) first = false; else sb.Append(All ? " UNION ALL" : " UNION");
            sb.Append(qry.BuildCommand(conn));
        }
        sb.Append(")");
    }
}
