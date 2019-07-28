using System.Collections.Generic;

namespace SequelNet
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
            this.Add(new Where(WhereCondition.AND, leftTableSchema.Name, leftColumn, WhereComparison.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
        }

        public JoinColumnPair(string leftTableNameOrAlias, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, leftTableNameOrAlias, leftColumn, WhereComparison.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
        }

        public JoinColumnPair(object value, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparison.EqualsTo, value));
        }

        public JoinColumnPair(object value, bool literalValue, string rightColumn)
        {
            Where w = new Where(WhereCondition.AND, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparison.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
        }

        public JoinColumnPair(string leftTableNameOrAlias, string leftColumn, object value, bool literalValue)
        {
            Where w = new Where(WhereCondition.AND, leftTableNameOrAlias, leftColumn, WhereComparison.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
        }

        public JoinColumnPair AND(TableSchema leftTableSchema, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, leftTableSchema.Name, leftColumn, WhereComparison.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
            return this;
        }

        public JoinColumnPair AND(string leftTableNameOrAlias, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, leftTableNameOrAlias, leftColumn, WhereComparison.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
            return this;
        }

        public JoinColumnPair AND(object value, string rightColumn)
        {
            this.Add(new Where(WhereCondition.AND, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparison.EqualsTo, value));
            return this;
        }

        public JoinColumnPair AND(object value, bool literalValue, string rightColumn)
        {
            Where w = new Where(WhereCondition.AND, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparison.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
            return this;
        }

        public JoinColumnPair AND(string leftTableNameOrAlias, string leftColumn, object value, bool literalValue)
        {
            Where w = new Where(WhereCondition.AND, leftTableNameOrAlias, leftColumn, WhereComparison.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
            return this;
        }

        public new JoinColumnPair AND(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            base.AND(thisObject, thisObjectType, comparison, thatObject, thatObjectType);
            return this;
        }

        public new JoinColumnPair AND(IPhrase phrase)
        {
            base.AND(phrase);
            return this;
        }

        public new JoinColumnPair AND(IPhrase phrase, WhereComparison comparison, object value)
        {
            base.AND(phrase, comparison, value);
            return this;
        }

        public new JoinColumnPair AND(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
        {
            base.AND(phrase, comparison, value, valueType);
            return this;
        }

        public new JoinColumnPair AND(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            base.AND(phrase, comparison, tableName, columnName);
            return this;
        }

        public new JoinColumnPair AND(WhereList whereList)
        {
            base.AND(whereList);
            return this;
        }

        public new JoinColumnPair AND(string tableName, string columnName, WhereComparison comparison, object columnValue)
        {
            base.AND(tableName, columnName, comparison, columnValue);
            return this;
        }

        public new JoinColumnPair AND(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            base.AND(tableName, columnName, comparison, otherTableName, otherColumnName);
            return this;
        }

        public JoinColumnPair OR(TableSchema leftTableSchema, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.OR, leftTableSchema.Name, leftColumn, WhereComparison.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
            return this;
        }

        public JoinColumnPair OR(string leftTableNameOrAlias, string leftColumn, string rightColumn)
        {
            this.Add(new Where(WhereCondition.OR, leftTableNameOrAlias, leftColumn, WhereComparison.EqualsTo, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn));
            return this;
        }

        public JoinColumnPair OR(object value, string rightColumn)
        {
            this.Add(new Where(WhereCondition.OR, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparison.EqualsTo, value));
            return this;
        }

        public JoinColumnPair OR(object value, bool literalValue, string rightColumn)
        {
            Where w = new Where(WhereCondition.OR, RIGHT_TABLE_PLACEHOLDER_ID, rightColumn, WhereComparison.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
            return this;
        }

        public JoinColumnPair OR(string leftTableNameOrAlias, string leftColumn, object value, bool literalValue)
        {
            Where w = new Where(WhereCondition.OR, leftTableNameOrAlias, leftColumn, WhereComparison.EqualsTo, value);
            if (literalValue) w.SecondType = ValueObjectType.Literal;
            this.Add(w);
            return this;
        }

        public new JoinColumnPair OR(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            base.OR(thisObject, thisObjectType, comparison, thatObject, thatObjectType);
            return this;
        }

        public new JoinColumnPair OR(string columnName, WhereComparison comparison, object columnValue)
        {
            base.OR(columnName, comparison, columnValue);
            return this;
        }

        public new JoinColumnPair OR(IPhrase phrase)
        {
            base.OR(phrase);
            return this;
        }

        public new JoinColumnPair OR(IPhrase phrase, WhereComparison comparison, object value)
        {
            base.OR(phrase, comparison, value);
            return this;
        }

        public new JoinColumnPair OR(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
        {
            base.OR(phrase, comparison, value, valueType);
            return this;
        }

        public new JoinColumnPair OR(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            base.OR(phrase, comparison, tableName, columnName);
            return this;
        }

        public new JoinColumnPair OR(WhereList whereList)
        {
            base.OR(whereList);
            return this;
        }

        public new JoinColumnPair OR(string tableName, string columnName, WhereComparison comparison, object columnValue)
        {
            base.OR(tableName, columnName, comparison, columnValue);
            return this;
        }

        public new JoinColumnPair OR(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            base.OR(tableName, columnName, comparison, otherTableName, otherColumnName);
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
            _Pairs.Add(new JoinColumnPair((leftTableAlias != null && leftTableAlias.Length > 0) ? leftTableAlias : leftTableSchema.Name, leftColumn, rightColumn));
            _RightTableSchema = rightTableSchema;
            _RightTableAlias = rightTableAlias;
            if (_RightTableAlias == null) _RightTableAlias = _RightTableSchema.Name;
        }

        public Join(JoinType joinType,
            TableSchema leftTableSchema, string leftColumn, string leftTableAlias,
            object rightTableSql, string rightColumn, string rightTableAlias)
        {
            _JoinType = joinType;
            _Pairs.Add(new JoinColumnPair((leftTableAlias != null && leftTableAlias.Length > 0) ? leftTableAlias : leftTableSchema.Name, leftColumn, rightColumn));
            _RightTableSql = rightTableSql;
            _RightTableAlias = rightTableAlias;
            if (_RightTableAlias == null) _RightTableAlias = _RightTableSchema.Name;
        }

        public Join(JoinType joinType,
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            _JoinType = joinType;
            _Pairs.AddRange(pairs);
            _RightTableSchema = rightTableSchema;
            _RightTableAlias = rightTableAlias;
            if (_RightTableAlias == null) _RightTableAlias = _RightTableSchema.Name;
        }

        public Join(JoinType joinType,
            object rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            _JoinType = joinType;
            _Pairs.AddRange(pairs);
            _RightTableSql = rightTableSql;
            _RightTableAlias = rightTableAlias;
            if (_RightTableAlias == null) _RightTableAlias = _RightTableSchema.Name;
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
