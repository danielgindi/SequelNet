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
        public Query Where(Where where, bool clearWhereList)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            if (clearWhereList) _ListWhere.Clear();
            _ListWhere.Add(where);
            return this;
        }
        public Query Where(string columnName, object Value)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            _ListWhere.Clear();
            _ListWhere.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, Value, ValueObjectType.Value));
            return this;
        }
        public Query Where(string columnName, WhereComparision comparison, object Value)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            _ListWhere.Clear();
            _ListWhere.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, Value, ValueObjectType.Value));
            return this;
        }
        public Query Where(string tableName, string columnName, WhereComparision comparison, object Value)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            _ListWhere.Clear();
            _ListWhere.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, Value));
            return this;
        }
        public Query AddWhere(Where where)
        {
            return Where(where, false);
        }
        public Query AddWhere(WhereCondition condition, object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(condition, thisObject, thisObjectType, comparison, thatObject, thatObjectType), false);
        }
        public Query AddWhere(string columnName, object Value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, Value, ValueObjectType.Value), false);
        }
        public Query AddWhere(string columnName, WhereComparision comparison, object Value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, Value, ValueObjectType.Value), false);
        }
        public Query AddWhere(WhereCondition condition, string literalExpression)
        {
            return Where(new Where(condition, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal), false);
        }
        public Query AddWhere(WhereCondition condition, BasePhrase phrase)
        {
            return Where(new Where(condition, phrase, ValueObjectType.Value, WhereComparision.None, null, ValueObjectType.Literal), false);
        }
        public Query AddWhere(WhereCondition condition, WhereList whereList)
        {
            return Where(new Where(condition, whereList), false);
        }
        public Query AddWhere(WhereList whereList)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            foreach (Where where in whereList)
            {
                _ListWhere.Add(where);
            }
            return this;
        }
        public Query AddWhere(string tableName, string columnName, WhereComparision comparison, object Value)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            _ListWhere.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, Value));
            return this;
        }
        public Query AND(string columnName, object Value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, Value, ValueObjectType.Value), false);
        }
        public Query AND(string columnName, WhereComparision comparison, object Value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, Value, ValueObjectType.Value), false);
        }
        public Query AND(WhereList whereList)
        {
            return Where(new Where(WhereCondition.AND, whereList), false);
        }
        public Query AND(string tableName, string columnName, WhereComparision comparison, object Value)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, Value), false);
        }
        public Query AND(string TableName, string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            where.FirstTableName = TableName;
            return Where(where, false);
        }
        public Query AND(string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.AND, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            return Where(where, false);
        }
        public Query OR(string columnName, object Value)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, Value, ValueObjectType.Value), false);
        }
        public Query OR(string columnName, WhereComparision comparison, object Value)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, comparison, Value, ValueObjectType.Value), false);
        }
        public Query OR(WhereList whereList)
        {
            return Where(new Where(WhereCondition.OR, whereList), false);
        }
        public Query OR(string tableName, string columnName, WhereComparision comparison, object Value)
        {
            return Where(new Where(WhereCondition.OR, tableName, columnName, comparison, Value), false);
        }
        public Query OR(string TableName, string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.OR, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            where.FirstTableName = TableName;
            return Where(where, false);
        }
        public Query OR(string ColumnName, object BetweenValue, object AndValue)
        {
            Where where = new Where(WhereCondition.OR, ColumnName, ValueObjectType.ColumnName, BetweenValue, ValueObjectType.Value, AndValue, ValueObjectType.Value);
            return Where(where, false);
        }
    }
}
