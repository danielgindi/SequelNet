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
        public Query OrderBy(string ColumnName, SortDirection SortDirection)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(new OrderBy(ColumnName, SortDirection));
            return this;
        }
        public Query OrderBy(string ColumnName, SortDirection SortDirection, bool ColumnNameIsLiteral)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(new OrderBy(ColumnName, SortDirection, ColumnNameIsLiteral));
            return this;
        }
        public Query OrderBy(string TableName, string ColumnName, SortDirection SortDirection)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(new OrderBy(TableName, ColumnName, SortDirection));
            return this;
        }
        private Query OrderBy(OrderBy OrderBy)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(OrderBy);
            return this;
        }
    }
}
