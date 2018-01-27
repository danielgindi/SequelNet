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
        /// <param name="version">Version number for this migration</param>
        public MigrationAttribute(Int64 version)
        {
            Version = version;
        }
    }
}
