using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    public interface BasePhrase
    {
        string BuildPhrase(dg.Sql.Connector.ConnectorBase conn);
    }
}
