using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using dg.Sql.Connector;

namespace dg.Sql
{
    public partial class Query
    {
        [Obsolete("DISTINCT is deprecated, please use Distinct instead.")]
        [CLSCompliant(false)]
        public Query DISTINCT()
        {
            IsDistinct = true;
            return this;
        }

        [Obsolete("GROUP_BY is deprecated, please use GroupBy instead.")]
        [CLSCompliant(false)]
        public Query GROUP_BY(string columnName)
        {
            return GroupBy(columnName);
        }

        [Obsolete("GROUP_BY is deprecated, please use GroupBy instead.")]
        [CLSCompliant(false)]
        public Query GROUP_BY(string tableName, string columnName)
        {
            return GroupBy(tableName, columnName);
        }

        [Obsolete("ORDER_BY is deprecated, please use OrderBy instead.")]
        [CLSCompliant(false)]
        public Query ORDER_BY(string columnName, SortDirection sortDirection)
        {
            return OrderBy(columnName, sortDirection);
        }

        [Obsolete("ORDER_BY is deprecated, please use OrderBy instead.")]
        [CLSCompliant(false)]
        public Query ORDER_BY(string tableName, string columnName, SortDirection sortDirection)
        {
            return OrderBy(tableName, columnName, sortDirection);
        }
    }
}
