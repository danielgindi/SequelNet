using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class UnionAll : IPhrase
    {
        public Query[] Queries;

        public UnionAll(params Query[] queries)
        {
            Queries = queries;
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            //sb.Append(@"(");
            foreach (Query qry in Queries)
            {
                if (first) first = false; else sb.Append(@" UNION ALL");
                sb.Append(qry.BuildCommand(conn));
            }
            //sb.Append(@")");
            return sb.ToString();
        }
    }
}
