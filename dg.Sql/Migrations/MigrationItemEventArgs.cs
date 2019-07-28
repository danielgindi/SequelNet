using System;

namespace dg.Sql.Migrations
{
    public class MigrationItemEventArgs : EventArgs
    {
        public Int64 Version;
        public string Description;
        public Type Type;
        public bool Up;

        public MigrationItemEventArgs(Int64 version, string description, Type type, bool up)
        {
            this.Version = version;
            this.Description = description;
            this.Type = type;
            this.Up = up;
        }
    }
}
