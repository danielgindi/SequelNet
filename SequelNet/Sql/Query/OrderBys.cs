namespace SequelNet
{
    public partial class Query
    {
        public Query ClearOrderBy()
        {
            if (_ListOrderBy != null)
            {
                _ListOrderBy.Clear();
            }
            return this;
        }

        public Query OrderBy(string columnName, SortDirection sortDirection)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(new OrderBy(columnName, sortDirection));
            return this;
        }

        public Query OrderBy(string columnName, SortDirection sortDirection, bool columnNameIsLiteral)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(new OrderBy(columnName, sortDirection, columnNameIsLiteral));
            return this;
        }

        public Query OrderBy(string tableName, string columnName, SortDirection sortDirection)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(new OrderBy(tableName, columnName, sortDirection));
            return this;
        }

        public Query OrderBy(object value, ValueObjectType valueType, SortDirection sortDirection)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(new OrderBy(value, valueType, sortDirection));
            return this;
        }

        public Query OrderBy(IPhrase phrase, SortDirection sortDirection)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(new OrderBy(phrase, sortDirection));
            return this;
        }

        private Query OrderBy(OrderBy orderBy)
        {
            if (_ListOrderBy == null) _ListOrderBy = new OrderByList();
            _ListOrderBy.Add(orderBy);
            return this;
        }
    }
}
