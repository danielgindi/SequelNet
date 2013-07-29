using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dg.Sql.Connector
{
    public class PostgreSQLMode
    {
        private bool _StandardConformingStrings = false; // Which is the default in earlier versions, for backwards compatibility
        private bool _BackslashQuote = false; // Relevant only for non-standard-conforming string literals

        public bool StandardConformingStrings
        {
            get { return _StandardConformingStrings == true; }
            set { _StandardConformingStrings = value; }
        }
        public bool BackslashQuote
        {
            get { return _BackslashQuote == true; }
            set { _BackslashQuote = value; }
        }
    }
}
