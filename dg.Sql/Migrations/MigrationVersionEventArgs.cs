using System;

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
