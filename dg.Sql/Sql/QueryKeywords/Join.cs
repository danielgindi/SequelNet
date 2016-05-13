using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    internal class JoinList : List<Join> { }
    public class JoinColumnPair : WhereList
    {
        public const string RIGHT_TABLE_PLACEHOLDER_ID = @"__RIGHT_TABLE_PLACEHOLDER_ID__";

        public JoinColumnPair()
        {

        }

        public JoinColumnPair(TableSchema leftTableSchema, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, leftTableSchema.SchemaName, leftColumn, WhereComparision.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
        }

        public JoinColumnPair(string leftTableNameOrAlias, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, leftTableNameOrAlias, leftColumn, WhereComparision.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
        }

        public JoinColumnPair(object value, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparision.EqualsTo, value));
        }

        public JoinColumnPair(object value, bool literalValue, string rightColumn)
        {
            Where w = new Where(WhereCondition.AND, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparision.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
        }

        public JoinColumnPair(string leftTableNameOrAlias, string leftColumn, object value, bool literalValue)
        {
            Where w = new Where(WhereCondition.AND, leftTableNameOrAlias, leftColumn, WhereComparision.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
        }

        public JoinColumnPair JoinAND(TableSchema leftTableSchema, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, leftTableSchema.SchemaName, leftColumn, WhereComparision.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
            return this;
        }

        public JoinColumnPair JoinAND(string leftTableNameOrAlias, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, leftTableNameOrAlias, leftColumn, WhereComparision.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
            return this;
        }

        public JoinColumnPair JoinAND(object value, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparision.EqualsTo, value));
            return this;
        }

        public JoinColumnPair JoinAND(object value, bool literalValue, string rightColumn)
        {
            Where w = new Where(WhereCondition.AND, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparision.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
            return this;
        }
        public JoinColumnPair JoinAND(string leftTableNameOrAlias, string leftColumn, object value, bool literalValue)
        {
            Where w = new Where(WhereCondition.AND, leftTableNameOrAlias, leftColumn, WhereComparision.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
            return this;
        }

        public JoinColumnPair JoinOR(TableSchema leftTableSchema, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.OR, leftTableSchema.SchemaName, leftColumn, WhereComparision.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
            return this;
        }

        public JoinColumnPair JoinOR(string leftTableNameOrAlias, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.OR, leftTableNameOrAlias, leftColumn, WhereComparision.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
            return this;
        }

        public JoinColumnPair JoinOR(object value, string rightColumn)
        {
            this.Add(new Where(WhereCondition.OR, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparision.EqualsTo, value));
            return this;
        }

        public JoinColumnPair JoinOR(object value, bool literalValue, string rightColumn)
        {
            Where w = new Where(WhereCondition.OR, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparision.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
            return this;
        }

        public JoinColumnPair JoinOR(string leftTableNameOrAlias, string leftColumn, object value, bool literalValue)
        {
            Where w = new Where(WhereCondition.OR, leftTableNameOrAlias, leftColumn, WhereComparision.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
            return this;
        }
    }
    internal class Join
    {
        private JoinType _JoinType;
        private TableSchema _RightTableSchema;
        private object _RightTableSql;
        private string _RightTableAlias;
        private List<JoinColumnPair> _Pairs = new List<JoinColumnPair>();

        public Join(JoinType joinType,
            TableSchema leftTableSchema, string leftColumn, string leftTableAlias,
            TableSchema rightTableSchema, string rightColumn, string rightTableAlias)
        {
            _JoinType = joinType;
            _Pairs.Add(new JoinColumnPair((leftTableAlias != null && leftTableAlias.Length > 0) ? leftTableAlias : leftTableSchema.SchemaName, leftColumn, rightColumn));
            _RightTableSchema = rightTableSchema;
            _RightTableAlias = rightTableAlias;
            if (_RightTableAlias == null) _RightTableAlias = _RightTableSchema.SchemaName;
        }

        public Join(JoinType joinType,
            TableSchema leftTableSchema, string leftColumn, string leftTableAlias,
            object rightTableSql, string rightColumn, string rightTableAlias)
        {
            _JoinType = joinType;
            _Pairs.Add(new JoinColumnPair((leftTableAlias != null && leftTableAlias.Length > 0) ? leftTableAlias : leftTableSchema.SchemaName, leftColumn, rightColumn));
            _RightTableSql = rightTableSql;
            _RightTableAlias = rightTableAlias;
            if (_RightTableAlias == null) _RightTableAlias = _RightTableSchema.SchemaName;
        }

        public Join(JoinType joinType,
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            _JoinType = joinType;
            _Pairs.AddRange(pairs);
            _RightTableSchema = rightTableSchema;
            _RightTableAlias = rightTableAlias;
            if (_RightTableAlias == null) _RightTableAlias = _RightTableSchema.SchemaName;
        }

        public Join(JoinType joinType,
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            _JoinType = joinType;
            _Pairs.AddRange(pairs);
            _RightTableSql = rightTableSql;
            _RightTableAlias = rightTableAlias;
            if (_RightTableAlias == null) _RightTableAlias = _RightTableSchema.SchemaName;
        }

        public JoinType JoinType
        {
            get { return _JoinType; }
            set { _JoinType = value; }
        }

        public TableSchema RightTableSchema
        {
            get { return _RightTableSchema; }
            set { _RightTableSchema = value; }
        }
        public object RightTableSql
        {
            get { return _RightTableSql; }
            set { _RightTableSql = value; }
        }

        public string RightTableAlias
        {
            get { return _RightTableAlias; }
            set { _RightTableAlias = value; }
        }

        public List<JoinColumnPair> Pairs
        {
            get { return _Pairs; }
            set { _Pairs = value; }
        }
    }
}
