using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

public class Concat : IPhrase
{
    public bool IgnoreNulls = false;
    public List<ValueWrapper> Values = new List<ValueWrapper>();

    #region Constructors

    public Concat(params ValueWrapper[] values)
    {
        this.Values.AddRange(values);
    }

    public Concat(bool ignoreNulls, params ValueWrapper[] values)
    {
        this.IgnoreNulls = ignoreNulls;
        this.Values.AddRange(values);
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        if (Values.Count == 0)
        {
            sb.Append(conn.Language.PrepareValue(""));
        }
        else
        {
            bool first = true;

            if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL && !IgnoreNulls)
            {
                // PostgreSQL does not ignore NULL values in || operator, like CONCAT in other sql languages

                foreach (var value in Values)
                {
                    if (first) first = false;
                    else sb.Append(" || ");

                    sb.Append(value.Build(conn, relatedQuery));
                }
            }
            else
            {
                // PostgreSQL ignores NULL values in CONCAT

                bool coalesce = IgnoreNulls && conn.TYPE != ConnectorBase.SqlServiceType.POSTGRESQL;
                
                sb.Append("CONCAT(");

                foreach (var value in Values)
                {
                    if (first) first = false;
                    else sb.Append(",");

                    if (coalesce)
                    {
                        sb.Append("COALESCE(");
                        sb.Append(value.Build(conn, relatedQuery));
                        sb.Append(",'')");
                    }
                    else
                    {
                        sb.Append(value.Build(conn, relatedQuery));
                    }
                }

                sb.Append(")");
            }
        }
    }
}
