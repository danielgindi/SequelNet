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
        public Query GroupBy(string ColumnName)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(ColumnName));
            return this;
        }
        public Query GroupBy(string ColumnName, SortDirection SortDirection)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(ColumnName, SortDirection));
            return this;
        }
        public Query GroupBy(object ColumnName, bool ColumnNameIsLiteral)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(ColumnName, ColumnNameIsLiteral));
            return this;
        }
        public Query GroupBy(object ColumnName, bool ColumnNameIsLiteral, SortDirection SortDirection)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(ColumnName, ColumnNameIsLiteral, SortDirection));
            return this;
        }
        public Query GroupBy(string TableName, string ColumnName)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(TableName, ColumnName));
            return this;
        }
        public Query GroupBy(string TableName, string ColumnName, SortDirection SortDirection)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(new GroupBy(TableName, ColumnName, SortDirection));
            return this;
        }
        private Query GroupBy(GroupBy GroupBy)
        {
            if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
            _ListGroupBy.Add(GroupBy);
            return this;
        }

        public Query SetGroupByHint(GroupByHint GroupByHint)
        {
            _GroupByHint = GroupByHint;
            return this;
        }
    }
}
