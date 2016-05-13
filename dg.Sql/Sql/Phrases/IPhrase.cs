using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    public interface IPhrase
    {
        string BuildPhrase(dg.Sql.Connector.ConnectorBase connection);
    }
}
