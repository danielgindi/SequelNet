using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dg.Sql.Migrations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MigrationAttribute : Attribute
    {
        /// <summary>
        /// Version number for this migration
        /// </summary>
        public Int64 Version { get; }

        /// <summary>
        /// Description for this migration
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationAttribute"/> class.
        /// </summary>
        /// <param name="version">Version number for this migration</param>
        /// <param name="description">Description for this migration</param>
        public MigrationAttribute(Int64 version, string description)
        {
            Version = version;
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationAttribute"/> class.
        /// </summary>
        /// <param name="year">Year used in automatic timestamp generation</param>
        /// <param name="month">Month used in automatic timestamp generation</param>
        /// <param name="day">Day used in automatic timestamp generation</param>
        /// <param name="hour">Hour used in automatic timestamp generation</param>
        /// <param name="minutes">Minutes used in automatic timestamp generation</param>
        /// <param name="seconds">Seconds used in automatic timestamp generation</param>
        /// <param name="version">Version number for this migration</param>
        /// <param name="description">Description for this migration</param>
        public MigrationAttribute(
            int year, int month, int day,
            int hour, int minutes, int seconds,
            string description)
        {
            Version = Math.Min(year, 9999) * 10000000000L +
                Math.Min(month, 99) * 100000000L +
                Math.Min(day, 99) * 1000000L +
                Math.Min(hour, 99) * 10000L +
                Math.Min(minutes, 99) * 100L +
                Math.Min(seconds, 99);
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationAttribute"/> class.
        /// </summary>
        /// <param name="version">Version number for this migration</param>
        public MigrationAttribute(Int64 version)
        {
            Version = version;
        }
    }
}
