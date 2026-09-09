using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

public class DateTimeAdd : IPhrase
{
    public ValueWrapper Value1;
    public ValueWrapper Value2;
    public DateTimeUnit Unit;

    #region Constructors

    public DateTimeAdd()
    {
    }

    public DateTimeAdd(ValueWrapper to, DateTimeUnit unit, ValueWrapper add)
    {
        this.Value1 = to;
        this.Unit = unit;
        this.Value2 = add;
    }

    public DateTimeAdd(IPhrase to, DateTimeUnit unit, ValueWrapper add)
        : this(ValueWrapper.From(to), unit, add)
    {
    }

    public DateTimeAdd(IPhrase to, DateTimeUnit unit, IPhrase add)
        : this(ValueWrapper.From(to), unit, ValueWrapper.From(add))
    {
    }

    public DateTimeAdd(ValueWrapper to, DateTimeUnit unit, IPhrase add)
        : this(to, unit, ValueWrapper.From(add))
    {
    }

    public DateTimeAdd(object value, ValueObjectType valueType, DateTimeUnit unit, Int64 interval)
    {
        this.Value1 = ValueWrapper.Make(value, valueType);
        this.Unit = unit;
        this.Value2 = ValueWrapper.From(interval);
    }

    public DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, Int64 interval)
    {
        this.Value1 = ValueWrapper.Column(tableName, columnName);
        this.Unit = unit;
        this.Value2 = ValueWrapper.From(interval);
    }

    public DateTimeAdd(object value, ValueObjectType valueType, DateTimeUnit unit, string addTableName, string addColumnName)
    {
        this.Value1 = ValueWrapper.Make(value, valueType);
        this.Unit = unit;
        this.Value2 = ValueWrapper.Column(addTableName, addColumnName);
    }

    public DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, string addTableName, string addColumnName)
    {
        this.Value1 = ValueWrapper.Column(tableName, columnName);
        this.Unit = unit;
        this.Value2 = ValueWrapper.Column(addTableName, addColumnName);
    }

    public DateTimeAdd(object value, ValueObjectType valueType, DateTimeUnit unit, string addColumnName)
    {
        this.Value1 = ValueWrapper.Make(value, valueType);
        this.Unit = unit;
        this.Value2 = ValueWrapper.Column(addColumnName);
    }

    public DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, string addColumnName)
    {
        this.Value1 = ValueWrapper.Column(tableName, columnName);
        this.Unit = unit;
        this.Value2 = ValueWrapper.Column(addColumnName);
    }

    public DateTimeAdd(string columnName, DateTimeUnit unit, Int64 interval)
        : this(null, columnName, unit, interval)
    {
    }

    public DateTimeAdd(IPhrase phrase, DateTimeUnit unit, Int64 interval)
        : this(phrase, ValueObjectType.Value, unit, interval)
    {
    }

    public DateTimeAdd(string columnName, DateTimeUnit unit, string addTableName, string addColumnName)
        : this(null, columnName, unit, addTableName, addColumnName)
    {
    }

    public DateTimeAdd(IPhrase phrase, DateTimeUnit unit, string addTableName, string addColumnName)
        : this(phrase, ValueObjectType.Value, unit, addTableName, addColumnName)
    {
    }

    public DateTimeAdd(string columnName, DateTimeUnit unit, string addColumnName)
        : this(null, columnName, unit, addColumnName)
    {
    }

    public DateTimeAdd(IPhrase phrase, DateTimeUnit unit, string addColumnName)
        : this(phrase, ValueObjectType.Value, unit, addColumnName)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        conn.Language.BuildDateTimeAdd(this, sb, conn, relatedQuery);
    }
}
