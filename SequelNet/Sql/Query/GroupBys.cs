namespace SequelNet;

public partial class Query
{
    public Query ClearGroupBy()
    {
        if (_ListGroupBy != null)
        {
            _ListGroupBy.Clear();
        }
        return this;
    }

    public Query GroupBy(string columnName)
    {
        return GroupBy(new GroupBy(columnName));
    }

    public Query GroupBy(string columnName, SortDirection sortDirection)
    {
        return GroupBy(new GroupBy(columnName, sortDirection));
    }

    public Query GroupBy(ValueWrapper value)
    {
        return GroupBy(new GroupBy(value));
    }

    public Query GroupBy(ValueWrapper value, SortDirection sortDirection)
    {
        return GroupBy(new GroupBy(value, sortDirection));
    }

    public Query GroupBy(IPhrase phrse)
    {
        return GroupBy(new GroupBy(phrse));
    }

    public Query GroupBy(IPhrase phrase, SortDirection sortDirection)
    {
        return GroupBy(new GroupBy(phrase, sortDirection));
    }

    public Query GroupBy(Where where)
    {
        return GroupBy(new GroupBy(where));
    }

    public Query GroupBy(Where where, SortDirection sortDirection)
    {
        return GroupBy(new GroupBy(where, sortDirection));
    }

    public Query GroupBy(WhereList wheres)
    {
        return GroupBy(new GroupBy(wheres));
    }

    public Query GroupBy(WhereList wheres, SortDirection sortDirection)
    {
        return GroupBy(new GroupBy(wheres, sortDirection));
    }

    public Query GroupBy(string tableName, string columnName)
    {
        return GroupBy(new GroupBy(tableName, columnName));
    }

    public Query GroupBy(string tableName, string columnName, SortDirection sortDirection)
    {
        return GroupBy(new GroupBy(tableName, columnName, sortDirection));
    }

    public Query GroupBy(GroupBy groupBy)
    {
        if (_ListGroupBy == null) _ListGroupBy = new GroupByList();
        _ListGroupBy.Add(groupBy);
        return this;
    }

    public Query SetGroupByHint(GroupByHint groupByHint)
    {
        _GroupByHint = groupByHint;
        return this;
    }
}
