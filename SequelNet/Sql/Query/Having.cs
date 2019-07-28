namespace SequelNet
{
    public partial class Query
    {
        public Query ClearHaving()
        {
            if (_ListHaving != null)
            {
                _ListHaving.Clear();
            }
            return this;
        }


        public Query Having(Where where)
        {
            if (_ListHaving == null) _ListHaving = new WhereList();
            _ListHaving.Add(where);
            return this;
        }


        public Query Having(WhereCondition condition, object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Having(new Where(condition, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
        }

        public Query Having(WhereCondition condition, string columnName, object value)
        {
            return Having(new Where(condition, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, value, ValueObjectType.Value));
        }

        public Query Having(WhereCondition condition, string columnName, WhereComparison comparison, object value)
        {
            return Having(new Where(condition, columnName, ValueObjectType.ColumnName, comparison, value, ValueObjectType.Value));
        }

        public Query Having(WhereCondition condition, string literalExpression)
        {
            return Having(new Where(condition, literalExpression, ValueObjectType.Literal, WhereComparison.None, null, ValueObjectType.Literal));
        }

        public Query Having(WhereCondition condition, IPhrase phrase)
        {
            return Having(new Where(condition, phrase, ValueObjectType.Value, WhereComparison.None, null, ValueObjectType.Literal));
        }

        public Query Having(WhereCondition condition, WhereList whereList)
        {
            return Having(new Where(condition, whereList));
        }

        public Query Having(WhereCondition condition, string tableName, string columnName, WhereComparison comparison, object value)
        {
            Having(new Where(condition, tableName, columnName, comparison, value));
            return this;
        }

        public Query Having(WhereCondition condition, string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            Having(new Where(condition, tableName, columnName, comparison, otherTableName, otherColumnName));
            return this;
        }

        public Query Having(WhereCondition condition, string tableName, string columnName, object betweenValue, object andValue)
        {
            Where where = new Where(condition, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value);
            where.FirstTableName = tableName;
            return Having(where);
        }

        public Query Having(WhereCondition condition, string columnName, object betweenValue, object andValue)
        {
            return Having(new Where(condition, columnName, ValueObjectType.ColumnName, betweenValue, ValueObjectType.Value, andValue, ValueObjectType.Value));
        }

        public Query Having(WhereCondition condition, IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            return Having(new Where(condition, phrase, comparison, tableName, columnName));
        }

        public Query Having(WhereCondition condition, IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Having(new Where(condition, phrase, comparison, value, valueType));
        }

        public Query Having(WhereCondition condition, Query query, WhereComparison comparison, string tableName, string columnName)
        {
            return Having(new Where(condition, query, comparison, tableName, columnName));
        }

        public Query Having(WhereCondition condition, Query query, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Having(new Where(condition, query, comparison, value, valueType));
        }


        public Query Having(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Having(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType);
        }

        public Query Having(string columnName, object value)
        {
            return Having(WhereCondition.AND, columnName, value);
        }

        public Query Having(string columnName, WhereComparison comparison, object value)
        {
            return Having(WhereCondition.AND, columnName, comparison, value);
        }

        public Query Having(string literalExpression)
        {
            return Having(WhereCondition.AND, literalExpression);
        }

        public Query Having(IPhrase phrase)
        {
            return Having(WhereCondition.AND, phrase);
        }

        public Query Having(WhereList whereList)
        {
            return Having(WhereCondition.AND, whereList);
        }

        public Query Having(string tableName, string columnName, WhereComparison comparison, object value)
        {
            return Having(WhereCondition.AND, tableName, columnName, comparison, value);
        }

        public Query Having(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            return Having(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName);
        }

        public Query Having(string tableName, string columnName, object betweenValue, object andValue)
        {
            return Having(WhereCondition.AND, tableName, columnName, betweenValue, andValue);
        }

        public Query Having(string columnName, object betweenValue, object andValue)
        {
            return Having(WhereCondition.AND, columnName, betweenValue, andValue);
        }

        public Query Having(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            return Having(WhereCondition.AND, phrase, comparison, tableName, columnName);
        }

        public Query Having(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Having(WhereCondition.AND, phrase, comparison, value, valueType);
        }

        public Query Having(Query query, WhereComparison comparison, string tableName, string columnName)
        {
            return Having(WhereCondition.AND, query, comparison, tableName, columnName);
        }

        public Query Having(Query query, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            return Having(WhereCondition.AND, query, comparison, value, valueType);
        }
    }
}
