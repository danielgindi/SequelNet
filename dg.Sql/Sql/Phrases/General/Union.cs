using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Union : IPhrase
    {
        public Query[] Queries;
        public bool All = false;

        public Union(params Query[] queries)
        {
            Queries = queries;
        }

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            //sb.Append(@"(");
            foreach (Query qry in Queries)
            {
                if (first) first = false; else sb.Append(All ? @" UNION ALL" : @" UNION");
                sb.Append(qry.BuildCommand(conn));
            }
            //sb.Append(@")");
            return sb.ToString();
        }
    }
}
