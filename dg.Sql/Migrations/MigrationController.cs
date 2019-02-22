using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dg.Sql.Migrations
{
    public class MigrationController
    {
        public delegate void MigrationVersionEventHandler(object sender, MigrationVersionEventArgs args);
        public delegate void MigrationItemEventHandler(object sender, MigrationItemEventArgs args);
        public delegate Int64 MigrationVersionQueryDelegate();
        public delegate List<DecoratedMigration> MigrationFilterDelegate(Int64 fromVersion, Int64 toVersion, List<DecoratedMigration> migrations);

        public event MigrationVersionEventHandler MigrationVersionEvent;
        public event MigrationItemEventHandler ItemStartEvent;
        public event MigrationItemEventHandler ItemEndEvent;

        private List<DecoratedMigration> _Migrations = new List<DecoratedMigration>();
        private MigrationState _State = null;

        public MigrationController()
        {
        }

        public MigrationController(List<Migration> migrations, MigrationVersionQueryDelegate versionQueryHandler)
        {
            SetMigrations(migrations);
            this.VersionQueryHandler = versionQueryHandler;
        }

        public MigrationController(Assembly assembly, MigrationVersionQueryDelegate versionQueryHandler)
        {
            LoadMigrationsFromAssembly(assembly);
            this.VersionQueryHandler = versionQueryHandler;
        }

        public void SetMigrations(List<Migration> migrations)
        {
            _Migrations.Clear();

            foreach (var migration in migrations)
            {
                var decor = new DecoratedMigration(migration);
                if (decor.Attribute == null)
                {
                    throw new ArgumentException($"Migration type {decor.Migration.GetType().Name} does not have a MigrationAttribute");
                }

                _Migrations.Add(decor);
            }
        }

        public void SetMigrations(List<Type> migrations)
        {
            _Migrations.Clear();

            foreach (var migration in migrations)
            {
                var decor = new DecoratedMigration(migration);
                if (decor.Attribute == null)
                {
                    throw new ArgumentException($"Migration type {decor.Migration.GetType().Name} does not have a MigrationAttribute");
                }

                _Migrations.Add(decor);
            }
        }

        /// <summary>
        /// Load the available set of migrations from the specified assembly.
        /// This is probably how you want to load your migrations.
        /// </summary>
        /// <param name="assembly"></param>
        public void LoadMigrationsFromAssembly(Assembly assembly)
        {
            var migrations = new List<Type>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(MigrationAttribute), true).Length > 0)
                {
                    migrations.Add(type);
                }
            }

            SetMigrations(migrations);
        }

        /// <summary>
        /// Migrate to the latest version available
        /// </summary>
        /// <returns>The count of migrations that we ran</returns>
        public int MigrateUp()
        {
            return MigrateTo(Int64.MaxValue);
        }

        /// <summary>
        /// Run the matching set of migrations towards <paramref name="targetVersion"/>
        /// </summary>
        /// <param name="targetVersion">The version you want to get to</param>
        /// <returns>The count of migrations that we ran</returns>
        public int MigrateTo(Int64 targetVersion)
        {
            if (_State == null || !_State.IsRollingBack)
            {
                var startVersion = Version;

                _State = new MigrationState
                {
                    StartVersion = startVersion,
                    CurrentVersion = startVersion,
                    TargetVersion = targetVersion,
                };
            }

            var up = targetVersion > _State.StartVersion;

            // Try to get a predicate from the user
            var migrations = MigrationFilterHandler != null 
                ? MigrationFilterHandler(_State.StartVersion, targetVersion, new List<DecoratedMigration>(_Migrations))
                : null;

            if (migrations == null)
            {
                // Default predicate - filter by versions in range
                migrations = _Migrations
                    .FindAll(x => up
                        ? (x.Attribute.Version > _State.StartVersion && x.Attribute.Version <= targetVersion)
                        : (x.Attribute.Version <= _State.StartVersion && x.Attribute.Version > targetVersion));
            }

            migrations.Sort((a, b) => a.Attribute.Version.CompareTo(b.Attribute.Version));

            if (!up)
                migrations.Reverse();

            int counter = 0;

            foreach (var migration in migrations)
            {
                // Update the state here, so we know what to rollback in case we need
                _State.CurrentVersion = migration.Attribute.Version;
                _State.IsInIntermediateState = true;

                if (up)
                {
                    ItemStartEvent?.Invoke(this, new MigrationItemEventArgs(migration.Attribute.Version, migration.Attribute.Description, migration.Type, true));

                    migration.Migration.Up();

                    ItemEndEvent?.Invoke(this, new MigrationItemEventArgs(migration.Attribute.Version, migration.Attribute.Description, migration.Type, true));
                }
                else
                {
                    ItemStartEvent?.Invoke(this, new MigrationItemEventArgs(migration.Attribute.Version, migration.Attribute.Description, migration.Type, false));

                    migration.Migration.Down();

                    ItemEndEvent?.Invoke(this, new MigrationItemEventArgs(migration.Attribute.Version, migration.Attribute.Description, migration.Type, false));

                    _State.CurrentVersion--;
                }

                counter++;

                // Let the user know he should store the db version now
                MigrationVersionEvent?.Invoke(this, new MigrationVersionEventArgs(_State.CurrentVersion));
                _State.IsInIntermediateState = false;
            }

            return counter;
        }

        /// <summary>
        /// Try to rollback the last or current migration.
        /// You can't rollback a rollback. It will always strive to reach the rollback's destination.
        /// </summary>
        /// <returns>The count of migrations that we ran</returns>
        public int Rollback()
        {
            if (_State == null)
            {
                throw new NotSupportedException("Cannot rollback as no migration has been exeuted yet");
            }

            if (!_State.IsRollingBack)
            {
                _State.IsRollingBack = true;
                var from = _State.CurrentVersion;
                var to = _State.StartVersion;
                _State.StartVersion = from;
                _State.TargetVersion = to;
            }

            var counter = MigrateTo(_State.TargetVersion);

            MigrationVersionEvent?.Invoke(this, new MigrationVersionEventArgs(_State.TargetVersion));

            return counter;
        }

        /// <summary>
        /// Should return the current recorded version of the db.
        /// </summary>
        public MigrationVersionQueryDelegate VersionQueryHandler { get; set; }

        /// <summary>
        /// A way to supply a custom predicate for which migrations to run
        /// </summary>
        public MigrationFilterDelegate MigrationFilterHandler { get; set; }

        /// <summary>
        /// Query the current known version of the database.
        /// If you are in the process of migration, or right after a migration, it will take the locally cache value.
        /// Otherwise, it will call the <see cref="VersionQueryHandler"/>.
        /// </summary>
        public Int64 Version
        {
            get
            {
                return _State?.CurrentVersion ?? VersionQueryHandler?.Invoke() ?? 0;
            }
        }
    }
}
