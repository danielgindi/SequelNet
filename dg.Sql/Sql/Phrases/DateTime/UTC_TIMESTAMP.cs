using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class UTC_TIMESTAMP : BasePhrase
    {
        public UTC_TIMESTAMP()
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            return conn.func_UTC_NOW;
        }
    }
}
