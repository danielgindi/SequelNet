using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

public class DateTimeDiff : IPhrase
{
    public ValueWrapper Value1;
    public ValueWrapper Value2;
    public DateTimeUnit Unit;

    #region Constructors

    public DateTimeDiff(DateTimeUnit unit, object value1, ValueObjectType value1Type, object value2, ValueObjectType value2Type)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.Make(value1, value1Type);
        this.Value2 = ValueWrapper.Make(value2, value2Type);
    }

    public DateTimeDiff(DateTimeUnit unit, object value1, ValueObjectType value1Type, DateTime value2)
        : this(unit, value1, value1Type, value2, ValueObjectType.Value)
    {
    }

    public DateTimeDiff(DateTimeUnit unit, DateTime value1, object value2, ValueObjectType value2Type)
        : this(unit, value1, ValueObjectType.Value, value2, value2Type)
    {
    }

    public DateTimeDiff(DateTimeUnit unit, string tableName1, string columnName1, DateTime value2)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.Column(tableName1, columnName1);
        this.Value2 = ValueWrapper.From(value2);
    }

    public DateTimeDiff(DateTimeUnit unit, DateTime value1, string tableName2, string columnName2)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.From(value1);
        this.Value2 = ValueWrapper.Column(tableName2, columnName2);
    }

    public DateTimeDiff(DateTimeUnit unit, string tableName1, string columnName1, string tableName2, string columnName2)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.Column(tableName1, columnName1);
        this.Value2 = ValueWrapper.Column(tableName2, columnName2);
    }

    public DateTimeDiff(DateTimeUnit unit, string columnName1, string columnName2)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.Column(columnName1);
        this.Value2 = ValueWrapper.Column(columnName2);
    }

    public DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, IPhrase phrase2)
        : this(unit, phrase1, ValueObjectType.Value, phrase2, ValueObjectType.Value)
    {
    }

    public DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, object value2, ValueObjectType value2Type)
        : this(unit, phrase1, ValueObjectType.Value, value2, value2Type)
    {
    }

    public DateTimeDiff(DateTimeUnit unit, object value1, ValueObjectType value1Type, IPhrase phrase2)
        : this(unit, value1, value1Type, phrase2, ValueObjectType.Value)
    {
    }

    public DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, DateTime value2)
        : this(unit, phrase1, ValueObjectType.Value, value2, ValueObjectType.Value)
    {
    }

    public DateTimeDiff(DateTimeUnit unit, DateTime value1, IPhrase phrase2)
        : this(unit, value1, ValueObjectType.Value, phrase2, ValueObjectType.Value)
    {
    }

    public DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, string tableName2, string columnName2)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.From(phrase1);
        this.Value2 = ValueWrapper.Column(tableName2, columnName2);
    }

    public DateTimeDiff(DateTimeUnit unit, string tableName1, string columnName1, IPhrase phrase2)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.Column(tableName1, columnName1);
        this.Value2 = ValueWrapper.From(phrase2);
    }

    public DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, string columnName2)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.From(phrase1);
        this.Value2 = ValueWrapper.Column(columnName2);
    }

    public DateTimeDiff(DateTimeUnit unit, string columnName1, IPhrase phrase2)
    {
        this.Unit = unit;
        this.Value1 = ValueWrapper.Column(columnName1);
        this.Value2 = ValueWrapper.From(phrase2);
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        conn.Language.BuildDateTimeDiff(this, sb, conn, relatedQuery);
    }

    #region Multiply operators

    public static Phrases.Multiply operator *(DateTimeDiff a, DateTimeDiff b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(DateTimeDiff a, decimal b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(DateTimeDiff a, double b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(DateTimeDiff a, Int64 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(DateTimeDiff a, Int32 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(DateTimeDiff a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(DateTimeDiff a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(decimal a, DateTimeDiff b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(double a, DateTimeDiff b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int64 a, DateTimeDiff b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int32 a, DateTimeDiff b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt64 a, DateTimeDiff b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt32 a, DateTimeDiff b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    #endregion

    #region Divide operators

    public static Phrases.Divide operator /(DateTimeDiff a, DateTimeDiff b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(DateTimeDiff a, decimal b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(DateTimeDiff a, double b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(DateTimeDiff a, Int64 b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(DateTimeDiff a, Int32 b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(DateTimeDiff a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(DateTimeDiff a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(decimal a, DateTimeDiff b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(double a, DateTimeDiff b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int64 a, DateTimeDiff b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int32 a, DateTimeDiff b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt64 a, DateTimeDiff b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt32 a, DateTimeDiff b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    #endregion

    #region Add operators

    public static Phrases.Add operator +(DateTimeDiff a, DateTimeDiff b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(DateTimeDiff a, decimal b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(DateTimeDiff a, double b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(DateTimeDiff a, Int64 b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(DateTimeDiff a, Int32 b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(DateTimeDiff a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(DateTimeDiff a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(decimal a, DateTimeDiff b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(double a, DateTimeDiff b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int64 a, DateTimeDiff b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int32 a, DateTimeDiff b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt64 a, DateTimeDiff b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt32 a, DateTimeDiff b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    #endregion

    #region Subtract operators

    public static Phrases.Subtract operator -(DateTimeDiff a, DateTimeDiff b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(DateTimeDiff a, decimal b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(DateTimeDiff a, double b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(DateTimeDiff a, Int64 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(DateTimeDiff a, Int32 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(DateTimeDiff a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(DateTimeDiff a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(decimal a, DateTimeDiff b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(double a, DateTimeDiff b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int64 a, DateTimeDiff b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int32 a, DateTimeDiff b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt64 a, DateTimeDiff b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt32 a, DateTimeDiff b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    #endregion
}
