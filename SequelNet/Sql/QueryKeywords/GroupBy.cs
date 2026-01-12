using System.Collections.Generic;

namespace SequelNet;

internal class GroupByList : List<GroupBy> { }

public class GroupBy
{
    public ValueWrapper Value = new ValueWrapper();
    public SortDirection SortDirection;

    public GroupBy(ValueWrapper value)
    {
        Value = value;
    }

    public GroupBy(ValueWrapper value, SortDirection sortDirection)
    {
        Value = value;
        SortDirection = sortDirection;
    }

    public GroupBy(IPhrase phrase)
    {
        Value.Value = phrase;
        Value.Type = ValueObjectType.Value;
    }

    public GroupBy(IPhrase phrase, SortDirection sortDirection)
    {
        Value.Value = phrase;
        Value.Type = ValueObjectType.Value;
        SortDirection = sortDirection;
    }

    public GroupBy(Where where)
    {
        Value.Value = where;
        Value.Type = ValueObjectType.Value;
    }

    public GroupBy(Where where, SortDirection sortDirection)
    {
        Value.Value = where;
        Value.Type = ValueObjectType.Value;
        SortDirection = sortDirection;
    }

    public GroupBy(WhereList wheres)
    {
        Value.Value = wheres;
        Value.Type = ValueObjectType.Value;
    }

    public GroupBy(WhereList wheres, SortDirection sortDirection)
    {
        Value.Value = wheres;
        Value.Type = ValueObjectType.Value;
        SortDirection = sortDirection;
    }

    public GroupBy(string columnName)
    {
        Value.Value = columnName;
        Value.Type = ValueObjectType.ColumnName;
    }

    public GroupBy(string columnName, SortDirection sortDirection)
    {
        Value.Value = columnName;
        Value.Type = ValueObjectType.ColumnName;
        SortDirection = sortDirection;
    }

    public GroupBy(string tableName, string columnName)
    {
        Value.TableName = tableName;
        Value.Value = columnName;
        Value.Type = ValueObjectType.ColumnName;
    }

    public GroupBy(string tableName, string columnName, SortDirection sortDirection)
    {
        Value.TableName = tableName;
        Value.Value = columnName;
        Value.Type = ValueObjectType.ColumnName;
        SortDirection = sortDirection;
    }
}
