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
        public Query ClearWhere()
        {
            _ListWhere = null;
            return this;
        }

        public Query Where(Where where)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            _ListWhere.Clear();
            _ListWhere.Add(where);
            return this;
        }

        public Query Where(Where where, bool clearWhereList)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            if (clearWhereList) _ListWhere.Clear();
            _ListWhere.Add(where);
            return this;
        }

        public Query Where(object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType), true);
        }

        public Query Where(string columnName, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, value, ValueObjectType.Value), true);
        }

        public Query Where(string columnName, WhereComparision comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value), true);
        }

        public Query Where(string literalExpression)
        {
            return Where(new Where(WhereCondition.AND, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal), true);
        }

        public Query Where(IPhrase phrase)
        {
            return Where(new Where(WhereCondition.AND, phrase, ValueObjectType.Value, WhereComparision.None, null, ValueObjectType.Literal), true);
        }

        public Query Where(WhereList whereList)
        {
            return Where(new Where(WhereCondition.AND, whereList), true);
        }

        public Query Where(string tableName, string columnName, WhereComparision comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, value), true);
        }

        public Query Where(string tableName, string columnName, WhereComparision comparison, string otherTableName, string otherColumnName)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName), true);
        }

        public Query Where(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            return Where(where, true);
        }

        public Query Where(string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            return Where(where, true);
        }

        public Query AddWhere(Where where)
        {
            return Where(where, false);
        }

        public Query AddWhere(WhereCondition condition, object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(condition, thisObject, thisObjectType, comparison, thatObject, thatObjectType), false);
        }

        public Query AddWhere(string columnName, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, value, ValueObjectType.Value), false);
        }

        public Query AddWhere(string columnName, WhereComparision comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value), false);
        }

        public Query AddWhere(WhereCondition condition, string literalExpression)
        {
            return Where(new Where(condition, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal), false);
        }

        public Query AddWhere(WhereCondition condition, IPhrase phrase)
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

        public Query AddWhere(string tableName, string columnName, WhereComparision comparison, object value)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            _ListWhere.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, value));
            return this;
        }

        public Query AddWhere(string tableName, string columnName, WhereComparision comparison, string otherTableName, string otherColumnName)
        {
            if (_ListWhere == null) _ListWhere = new WhereList();
            _ListWhere.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
            return this;
        }

        public Query AND(object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType), false);
        }

        public Query AND(string columnName, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, value, ValueObjectType.Value), false);
        }

        public Query AND(string columnName, WhereComparision comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value), false);
        }

        public Query AND(string literalExpression)
        {
            return Where(new Where(WhereCondition.AND, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal), false);
        }

        public Query AND(IPhrase phrase)
        {
            return Where(new Where(WhereCondition.AND, phrase, ValueObjectType.Value, WhereComparision.None, null, ValueObjectType.Literal), false);
        }

        public Query AND(WhereList whereList)
        {
            return Where(new Where(WhereCondition.AND, whereList), false);
        }

        public Query AND(string tableName, string columnName, WhereComparision comparison, object value)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, value), false);
        }

        public Query AND(string tableName, string columnName, WhereComparision comparison, string otherTableName, string otherColumnName)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName), false);
        }

        public Query AND(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            return Where(where, false);
        }

        public Query AND(string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            return Where(where, false);
        }

        public Query OR(object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(WhereCondition.OR, thisObject, thisObjectType, comparison, thatObject, thatObjectType), false);
        }

        public Query OR(string columnName, object value)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, value, ValueObjectType.Value), false);
        }

        public Query OR(string columnName, WhereComparision comparison, object value)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value), false);
        }

        public Query OR(string literalExpression)
        {
            return Where(new Where(WhereCondition.OR, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal), false);
        }

        public Query OR(IPhrase phrase)
        {
            return Where(new Where(WhereCondition.OR, phrase, ValueObjectType.Value, WhereComparision.None, null, ValueObjectType.Literal), false);
        }

        public Query OR(WhereList whereList)
        {
            return Where(new Where(WhereCondition.OR, whereList), false);
        }

        public Query OR(string tableName, string columnName, WhereComparision comparison, object value)
        {
            return Where(new Where(WhereCondition.OR, tableName, columnName, comparison, value), false);
        }

        public Query OR(string tableName, string columnName, WhereComparision comparison, string otherTableName, string otherColumnName)
        {
            return Where(new Where(WhereCondition.OR, tableName, columnName, comparison, otherTableName, otherColumnName), false);
        }

        public Query OR(string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            return Where(where, false);
        }

        public Query OR(string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            return Where(where, false);
        }
    }
}
