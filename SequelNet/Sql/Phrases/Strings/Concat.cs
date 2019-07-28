using System.Collections.Generic;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
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

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            if (Values.Count == 0)
            {
                return conn.Language.PrepareValue("");
            }
            else
            {
                bool first = true;

                if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL && !IgnoreNulls)
                {
                    // PostgreSQL does not ignore NULL values in || operator, like CONCAT in other sql languages

                    string ret = "";

                    foreach (var value in Values)
                    {
                        if (first) first = false;
                        else ret += " || ";

                        ret += value.Build(conn, relatedQuery);
                    }

                    return ret;
                }
                else
                {
                    // PostgreSQL ignores NULL values in CONCAT

                    bool coalesce = IgnoreNulls && conn.TYPE != ConnectorBase.SqlServiceType.POSTGRESQL;
                    
                    string ret = "CONCAT(";

                    foreach (var value in Values)
                    {
                        if (first) first = false;
                        else ret += ",";

                        if (coalesce)
                        {
                            ret += "COALESCE(";
                            ret += value.Build(conn, relatedQuery);
                            ret += ",'')";
                        }
                        else
                        {
                            ret += value.Build(conn, relatedQuery);
                        }
                    }

                    ret += ")";

                    return ret;
                }
            }
        }
    }
}
