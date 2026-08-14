using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet;

public class WhereList : List<Where>
{
    /// <summary>
    /// Determines whether this WHERE list, including nested lists, references a column by name.
    /// </summary>
    /// <param name="columnName">The column to find.</param>
    /// <param name="isMatchingTable">Optional predicate for limiting matches to a table or alias. It receives null for unqualified columns.</param>
    public bool IsColumnInWhere(string columnName, Func<string, bool> isMatchingTable = null)
    {
        foreach (Where where in this)
        {
            if (IsColumnMatch(where.First, where.FirstType, where.FirstTableName, columnName, isMatchingTable) ||
                IsColumnMatch(where.Second, where.SecondType, where.SecondTableName, columnName, isMatchingTable) ||
                IsColumnMatch(where.Third, where.ThirdType, where.ThirdTableName, columnName, isMatchingTable))
                return true;

            if (where.First is WhereList nestedWhereList && nestedWhereList.IsColumnInWhere(columnName, isMatchingTable))
                return true;
        }

        return false;
    }

    private static bool IsColumnMatch(
        object value,
        ValueObjectType valueType,
        string tableName,
        string columnName,
        Func<string, bool> isMatchingTable)
    {
        return valueType == ValueObjectType.ColumnName &&
            value is string candidateColumnName &&
            string.Equals(candidateColumnName, columnName, StringComparison.Ordinal) &&
            (isMatchingTable == null || isMatchingTable(tableName));
    }

    public void BuildCommand(StringBuilder outputBuilder, Where.BuildContext context)
    {
        context = context ?? new Where.BuildContext();

        bool ownsConn = context.Conn == null;
        if (ownsConn)
        {
            context.Conn = ConnectorBase.Create();
        }

        try
        {
            bool isFirst = true;
            bool isForJoinList = this is JoinColumnPair;
            foreach (Where where in this)
            {
                where.BuildCommand(outputBuilder, isFirst, context);
                if (isFirst) isFirst = false;
            }
        }
        finally
        {
            if (ownsConn)
            {
                context.Conn.Dispose();
            }
        }
    }

    public WhereList ClearWhere()
    {
        this.Clear();
        return this;
    }

    public WhereList Where(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
    {
        this.Clear();
        this.Add(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
        return this;
    }

    public WhereList Where(string columnName, object columnValue)
    {
        this.Clear();
        this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, columnValue, ValueObjectType.Value));
        return this;
    }

    public WhereList Where(string columnName, WhereComparison comparison, object columnValue)
    {
        this.Clear();
        this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
        return this;
    }

    public WhereList Where(IPhrase phrase)
    {
        this.Clear();
        this.Add(new Where(WhereCondition.AND, phrase, ValueObjectType.Value, WhereComparison.None, null, ValueObjectType.Value));
        return this;
    }

    public WhereList Where(ValueWrapper value)
    {
        this.Clear();
        this.Add(new Where(WhereCondition.AND, value, ValueObjectType.Value, WhereComparison.None, null, ValueObjectType.Value));
        return this;
    }

    public WhereList Where(WhereList whereList)
    {
        this.Clear();
        this.Add(new Where(WhereCondition.AND, whereList));
        return this;
    }

    public WhereList Where(string tableName, string columnName, WhereComparison comparison, object columnValue)
    {
        this.Clear();
        this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue));
        return this;
    }

    public WhereList Where(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
    {
        this.Clear();
        this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
        return this;
    }

    public WhereList Where(
        object aValue, ValueObjectType aType,
        object betweenValue, ValueObjectType betweenType,
        object andValue, ValueObjectType andType)
    {
        this.Add(new Where(WhereCondition.AND, aValue, aType, betweenValue, betweenType, andValue, andType));
        return this;
    }

    public WhereList Where(
        string aSchema, object aValue, ValueObjectType aType,
        string betweenSchema, object betweenValue, ValueObjectType betweenType,
        string andSchema, object andValue, ValueObjectType andType)
    {
        this.Add(new Where(WhereCondition.AND,
            aSchema, aValue, aType,
            betweenSchema, betweenValue, betweenType,
            andSchema, andValue, andType));
        return this;
    }

    public WhereList AND(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
    {
        this.Add(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
        return this;
    }

    public WhereList AND(string columnName, object columnValue)
    {
        this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, columnValue, ValueObjectType.Value));
        return this;
    }

    public WhereList AND(string columnName, WhereComparison comparison, object columnValue)
    {
        this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
        return this;
    }

    public WhereList AND(IPhrase phrase)
    {
        this.Add(new Where(WhereCondition.AND, phrase));
        return this;
    }

    public WhereList AND(IPhrase phrase, WhereComparison comparison, object value)
    {
        this.Add(new Where(WhereCondition.AND, phrase, comparison, value));
        return this;
    }

    public WhereList AND(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
    {
        this.Add(new Where(WhereCondition.AND, phrase, comparison, value, valueType));
        return this;
    }

    public WhereList AND(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
    {
        var w = new Where(WhereCondition.AND, phrase, comparison, tableName, columnName);
        w.SecondTableName = tableName;
        this.Add(w);
        return this;
    }

    public WhereList AND(ValueWrapper value)
    {
        this.Add(new Where(WhereCondition.AND, value));
        return this;
    }

    public WhereList AND(ValueWrapper value, WhereComparison comparison, object otherValue)
    {
        this.Add(new Where(WhereCondition.AND, value, comparison, otherValue));
        return this;
    }

    public WhereList AND(ValueWrapper value, WhereComparison comparison, object otherValue, ValueObjectType valueType)
    {
        this.Add(new Where(WhereCondition.AND, value, comparison, otherValue, valueType));
        return this;
    }

    public WhereList AND(ValueWrapper value, WhereComparison comparison, string tableName, string columnName)
    {
        var w = new Where(WhereCondition.AND, value, comparison, tableName, columnName);
        w.SecondTableName = tableName;
        this.Add(w);
        return this;
    }

    public WhereList AND(WhereList whereList)
    {
        this.Add(new Where(WhereCondition.AND, whereList));
        return this;
    }

    public WhereList AND(string tableName, string columnName, WhereComparison comparison, object columnValue)
    {
        this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue));
        return this;
    }

    public WhereList AND(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
    {
        this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
        return this;
    }

    public WhereList AND(
        object aValue, ValueObjectType aType,
        object betweenValue, ValueObjectType betweenType,
        object andValue, ValueObjectType andType)
    {
        this.Add(new Where(WhereCondition.AND, aValue, aType, betweenValue, betweenType, andValue, andType));
        return this;
    }

    public WhereList AND(
        string aSchema, object aValue, ValueObjectType aType,
        string betweenSchema, object betweenValue, ValueObjectType betweenType,
        string andSchema, object andValue, ValueObjectType andType)
    {
        this.Add(new Where(WhereCondition.AND,
            aSchema, aValue, aType,
            betweenSchema, betweenValue, betweenType,
            andSchema, andValue, andType));
        return this;
    }

    public WhereList OR(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
    {
        this.Add(new Where(WhereCondition.OR, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
        return this;
    }

    public WhereList OR(string columnName, object columnValue)
    {
        this.Add(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, columnValue, ValueObjectType.Value));
        return this;
    }

    public WhereList OR(string columnName, WhereComparison comparison, object columnValue)
    {
        this.Add(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
        return this;
    }

    public WhereList OR(IPhrase phrase)
    {
        this.Add(new Where(WhereCondition.OR, phrase));
        return this;
    }

    public WhereList OR(IPhrase phrase, WhereComparison comparison, object value)
    {
        this.Add(new Where(WhereCondition.OR, phrase, comparison, value));
        return this;
    }

    public WhereList OR(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
    {
        this.Add(new Where(WhereCondition.OR, phrase, comparison, value, valueType));
        return this;
    }

    public WhereList OR(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
    {
        var w = new Where(WhereCondition.OR, phrase, comparison, tableName, columnName);
        w.SecondTableName = tableName;
        this.Add(w);
        return this;
    }

    public WhereList OR(ValueWrapper value)
    {
        this.Add(new Where(WhereCondition.OR, value));
        return this;
    }

    public WhereList OR(ValueWrapper value, WhereComparison comparison, object otherValue)
    {
        this.Add(new Where(WhereCondition.OR, value, comparison, otherValue));
        return this;
    }

    public WhereList OR(ValueWrapper value, WhereComparison comparison, object otherValue, ValueObjectType valueType)
    {
        this.Add(new Where(WhereCondition.OR, value, comparison, otherValue, valueType));
        return this;
    }

    public WhereList OR(ValueWrapper value, WhereComparison comparison, string tableName, string columnName)
    {
        var w = new Where(WhereCondition.OR, value, comparison, tableName, columnName);
        w.SecondTableName = tableName;
        this.Add(w);
        return this;
    }

    public WhereList OR(WhereList whereList)
    {
        this.Add(new Where(WhereCondition.OR, whereList));
        return this;
    }

    public WhereList OR(string tableName, string columnName, WhereComparison comparison, object columnValue)
    {
        this.Add(new Where(WhereCondition.OR, tableName, columnName, comparison, columnValue));
        return this;
    }

    public WhereList OR(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
    {
        this.Add(new Where(WhereCondition.OR, tableName, columnName, comparison, otherTableName, otherColumnName));
        return this;
    }

    public WhereList OR(
        object aValue, ValueObjectType aType,
        object betweenValue, ValueObjectType betweenType,
        object andValue, ValueObjectType andType)
    {
        this.Add(new Where(WhereCondition.OR, aValue, aType, betweenValue, betweenType, andValue, andType));
        return this;
    }

    public WhereList OR(
        string aSchema, object aValue, ValueObjectType aType,
        string betweenSchema, object betweenValue, ValueObjectType betweenType,
        string andSchema, object andValue, ValueObjectType andType)
    {
        this.Add(new Where(WhereCondition.OR, 
            aSchema, aValue, aType, 
            betweenSchema, betweenValue, betweenType,
            andSchema, andValue, andType));
        return this;
    }

    public WhereList AddFromList(WhereList whereList)
    {
        foreach (Where where in whereList)
        {
            this.Add(where);
        }
        return this;
    }
}
