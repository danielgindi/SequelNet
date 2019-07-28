namespace dg.Sql
{
    public partial class Query
    {
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
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            if (_ListJoin == null) _ListJoin = new JoinList();
            Join join = new Join(joinType, rightTableSql, rightTableAlias, pairs);
            _ListJoin.Add(join);
            return this;
        }
                
        public Query LeftJoin(
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.LeftJoin, rightTableSchema, rightTableAlias, pairs);
        }

        public Query LeftJoin(
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.LeftJoin, rightTableSql, rightTableAlias, pairs);
        }

        public Query RightJoin(
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.RightJoin, rightTableSchema, rightTableAlias, pairs);
        }

        public Query RightJoin(
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.RightJoin, rightTableSql, rightTableAlias, pairs);
        }

        public Query InnerJoin(
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.InnerJoin, rightTableSchema, rightTableAlias, pairs);
        }

        public Query InnerJoin(
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.InnerJoin, rightTableSql, rightTableAlias, pairs);
        }

        public Query LeftOuterJoin(
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.LeftOuterJoin, rightTableSchema, rightTableAlias, pairs);
        }

        public Query LeftOuterJoin(
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.LeftOuterJoin, rightTableSql, rightTableAlias, pairs);
        }

        public Query RightOuterJoin(
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.RightOuterJoin, rightTableSchema, rightTableAlias, pairs);
        }

        public Query RightOuterJoin(
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.RightOuterJoin, rightTableSql, rightTableAlias, pairs);
        }

        public Query FullOuterJoin(
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.FullOuterJoin, rightTableSchema, rightTableAlias, pairs);
        }

        public Query FullOuterJoin(
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            return Join(JoinType.FullOuterJoin, rightTableSql, rightTableAlias, pairs);
        }
    }
}
