using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class UTC_TIMESTAMP : IPhrase
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
