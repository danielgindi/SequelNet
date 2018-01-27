using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dg.Sql.Migrations
{
    public class MigrationVersionEventArgs : EventArgs
    {
        public Int64 Version;

        public MigrationVersionEventArgs(Int64 version)
        {
            this.Version = version;
        }
    }
}
