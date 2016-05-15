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
        public Query ClearGroupBy()
        {
            if (_ListGroupBy != null)
            {
                _ListGroupBy.Clear();
            }
            return this;
        }

        public Query GroupBy(string columnName)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(columnName));
            return this;
        }

        public Query GroupBy(string columnName, SortDirection sortDirection)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(columnName, sortDirection));
            return this;
        }

        public Query GroupBy(object value, bool valueIsLiteral)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(value, valueIsLiteral));
            return this;
        }

        public Query GroupBy(object value, bool valueIsLiteral, SortDirection sortDirection)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(value, valueIsLiteral, sortDirection));
            return this;
        }

        public Query GroupBy(string tableName, string columnName)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(tableName, columnName));
            return this;
        }

        public Query GroupBy(string tableName, string columnName, SortDirection sortDirection)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(tableName, columnName, sortDirection));
            return this;
        }

        private Query GroupBy(GroupBy groupBy)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(groupBy);
            return this;
        }

        public Query SetGroupByHint(GroupByHint groupByHint)
        {
            _GroupByHint = groupByHint;
            return this;
        }
    }
}
