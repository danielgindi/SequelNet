using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dg.Sql.Migrations
{
    public class MigrationState
    {
        public Int64 StartVersion;
        public Int64 TargetVersion;
        public Int64 CurrentVersion;

        public bool IsRollingBack = false;
        public bool IsInIntermediateState = false;
    }
}
