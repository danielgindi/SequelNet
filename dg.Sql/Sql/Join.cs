using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    internal class JoinList : List<Join> { }
    public class JoinColumnPair
    {
        private string _LeftTableNameOrAlias;
        private object _LeftColumn;
        private ValueObjectType _LeftColumnType;
        private string _RightColumn;

        public JoinColumnPair()
        {
            _LeftColumnType = ValueObjectType.ColumnName;
        }
        public JoinColumnPair(TableSchema leftTableSchema, string leftColumn, string rightColumn)
        {
            _LeftTableNameOrAlias = leftTableSchema.SchemaName;
            _LeftColumn = leftColumn;
            _LeftColumnType = ValueObjectType.ColumnName;
            _RightColumn = rightColumn;
        }
        public JoinColumnPair(string leftTableNameOrAlias, string leftColumn, string rightColumn)
        {
            _LeftTableNameOrAlias = leftTableNameOrAlias;
            _LeftColumn = leftColumn;
            _LeftColumnType = ValueObjectType.ColumnName;
            _RightColumn = rightColumn;
        }
        public JoinColumnPair(object value, string rightColumn)
        {
            _LeftTableNameOrAlias = null;
            _LeftColumn = value;
            _LeftColumnType = ValueObjectType.Value;
            _RightColumn = rightColumn;
        }
        public JoinColumnPair(object value, bool literalValue, string rightColumn)
        {
            _LeftTableNameOrAlias = null;
            _LeftColumn = value;
            _LeftColumnType = literalValue ? ValueObjectType.Literal : ValueObjectType.Value;
            _RightColumn = rightColumn;
        }

        public string LeftTableNameOrAlias
        {
            get { return _LeftTableNameOrAlias; }
            set { _LeftTableNameOrAlias = value; }
        }
        public object LeftColumn
        {
            get { return _LeftColumn; }
            set { _LeftColumn = value; }
        }
        public ValueObjectType LeftColumnType
        {
            get { return _LeftColumnType; }
            set { _LeftColumnType = value; }
        }
        public string RightColumn
        {
            get { return _RightColumn; }
            set { _RightColumn = value; }
        }
    }
    internal class Join
    {
        private JoinType _JoinType;
        private TableSchema _RightTableSchema;
        private string _RightTableSql;
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
            string rightTableSql, string rightColumn, string rightTableAlias)
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
            string rightTableSql, string rightTableAlias,
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
        public string RightTableSql
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
