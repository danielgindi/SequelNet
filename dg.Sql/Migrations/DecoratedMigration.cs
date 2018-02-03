using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dg.Sql.Migrations
{
    public class DecoratedMigration
    {
        public Type Type { get; internal set; }
        internal Migration _Migration;
        public MigrationAttribute Attribute { get; internal set; }

        internal DecoratedMigration(Type migrationType)
        {
            this.Type = migrationType;

            var attrs = migrationType.GetCustomAttributes(typeof(MigrationAttribute), true);
            if (attrs.Length > 0)
            {
                this.Attribute = attrs[0] as MigrationAttribute;
            }
        }

        internal DecoratedMigration(Migration migration) : this(migration.GetType())
        {
            this.Migration = migration;
        }

        internal Migration Migration
        {
            get
            {
                if (_Migration == null)
                {
                    _Migration = Activator.CreateInstance(Type) as Migration;
                }

                return _Migration;
            }
            set
            {
                _Migration = value;
            }
        }
    }
}
