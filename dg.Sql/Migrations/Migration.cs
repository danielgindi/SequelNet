using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dg.Sql.Migrations
{
    public abstract class Migration
    {
        /// <summary>
        /// What to do when migrating up to this version?
        /// </summary>
        public abstract void Up();

        /// <summary>
        /// What to do when migrating down from this version?
        /// </summary>
        public abstract void Down();
    }
}
