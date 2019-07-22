using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    public interface IPhrase
    {
        string BuildPhrase(Connector.ConnectorBase connection, Query relatedQuery = null);
    }
}
