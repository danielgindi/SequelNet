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
        public Query Join(JoinType joinType,
            TableSchema leftTableSchema, string leftColumn, string leftTableAlias,
            TableSchema rightTableSchema, string rightColumn, string rightTableAlias)
        {
            if (_ListJoin == null) _ListJoin = new JoinList();
            Join join = new Join(joinType, leftTableSchema, leftColumn, leftTableAlias, rightTableSchema, rightColumn, rightTableAlias);
            _ListJoin.Add(join);
            TableAliasMap[join.RightTableAlias] = join.RightTableSchema;
            return this;
        }
        public Query Join(JoinType joinType,
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            if (_ListJoin == null) _ListJoin = new JoinList();
            Join join = new Join(joinType, rightTableSchema, rightTableAlias, pairs);
            _ListJoin.Add(join);
            TableAliasMap[join.RightTableAlias] = join.RightTableSchema;
            return this;
        }
        public Query Join(JoinType joinType,
            TableSchema leftTableSchema, string leftColumn, string leftTableAlias,
            string rightTableSql, string rightColumn, string rightTableAlias)
        {
            if (_ListJoin == null) _ListJoin = new JoinList();
            Join join = new Join(joinType, leftTableSchema, leftColumn, leftTableAlias, rightTableSql, rightColumn, rightTableAlias);
            _ListJoin.Add(join);
            return this;
        }
        public Query Join(JoinType joinType,
            string rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            if (_ListJoin == null) _ListJoin = new JoinList();
            Join join = new Join(joinType, rightTableSql, rightTableAlias, pairs);
            _ListJoin.Add(join);
            return this;
        }
    }
}
