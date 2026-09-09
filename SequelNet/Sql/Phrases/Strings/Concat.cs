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
        conn.Language.BuildConcat(this, sb, conn, relatedQuery);
    }
}
