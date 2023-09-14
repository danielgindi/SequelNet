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
        if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL)
        {
            sb.Append(@"TIMESTAMPDIFF(");
            switch (Unit)
            {
                case DateTimeUnit.Microsecond:
                    sb.Append(@"MICROSECOND");
                    break;
                case DateTimeUnit.Millisecond:
                    sb.Append(@"MICROSECOND"); // Will multiply by 1000
                    break;
                default:
                case DateTimeUnit.Second:
                    sb.Append(@"SECOND");
                    break;
                case DateTimeUnit.Minute:
                    sb.Append(@"MINUTE");
                    break;
                case DateTimeUnit.Hour:
                    sb.Append(@"HOUR");
                    break;
                case DateTimeUnit.Day:
                    sb.Append(@"DAY");
                    break;
                case DateTimeUnit.Week:
                    sb.Append(@"WEEK");
                    break;
                case DateTimeUnit.Month:
                    sb.Append(@"MONTH");
                    break;
                case DateTimeUnit.QuarterYear:
                    sb.Append(@"QUARTER");
                    break;
                case DateTimeUnit.Year:
                    sb.Append(@"YEAR");
                    break;
            }
            sb.Append(',');

            sb.Append(Value1.Build(conn, relatedQuery));

            sb.Append(',');

            sb.Append(Value2.Build(conn, relatedQuery));

            sb.Append(')');

            if (Unit == DateTimeUnit.Millisecond)
            {
                sb.Append(" * 1000");
            }
        }
        else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
        {
            switch (Unit)
            {
                case DateTimeUnit.Microsecond:
                    sb.AppendFormat(@"((((DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))) * 60 + (DATE_PART('minute', {0}) - DATE_PART('minute', {1}))) * 60 + (DATE_PART('second', {0}) - DATE_PART('second', {1}))) * 1000000",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                case DateTimeUnit.Millisecond:
                    sb.AppendFormat(@"((((DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))) * 60 + (DATE_PART('minute', {0}) - DATE_PART('minute', {1}))) * 60 + (DATE_PART('second', {0}) - DATE_PART('second', {1}))) * 1000",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                default:
                case DateTimeUnit.Second:
                    sb.AppendFormat(@"(((DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))) * 60 + (DATE_PART('minute', {0}) - DATE_PART('minute', {1}))) * 60 + (DATE_PART('second', {0}) - DATE_PART('second', {1}))",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                case DateTimeUnit.Minute:
                    sb.AppendFormat(@"((DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))) * 60 + (DATE_PART('minute', {0}) - DATE_PART('minute', {1}))",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                case DateTimeUnit.Hour:
                    sb.AppendFormat(@"(DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                case DateTimeUnit.Day:
                    sb.AppendFormat(@"DATE_PART('day', {0}) - DATE_PART('day', {1})",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                case DateTimeUnit.Week:
                    sb.AppendFormat(@"TRUNC(DATE_PART('day', {0} - {1}) / 7)",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                case DateTimeUnit.Month:
                    sb.AppendFormat(@"(DATE_PART('year', {0}) - DATE_PART('year', {1})) * 12 + (DATE_PART('month', {0}) - DATE_PART('month', {1}))",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                case DateTimeUnit.QuarterYear:
                    sb.AppendFormat(@"(DATE_PART('year', {0}) - DATE_PART('year', {1})) * 4 + TRUNC((DATE_PART('month', {0}) - DATE_PART('month', {1})) / 4)",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
                case DateTimeUnit.Year:
                    sb.AppendFormat(@"DATE_PART('year', {0}) - DATE_PART('year', {1})",
                        Value1.Build(conn, relatedQuery),
                        Value2.Build(conn, relatedQuery));
                    break;
            }
        }
        else // if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL || conn.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
        {
            sb.Append(@"DATEDIFF(");
            switch (Unit)
            {
                case DateTimeUnit.Microsecond:
                    sb.Append(@"microsecond");
                    break;
                case DateTimeUnit.Millisecond:
                    sb.Append(@"millisecond");
                    break;
                default:
                case DateTimeUnit.Second:
                    sb.Append(@"second");
                    break;
                case DateTimeUnit.Minute:
                    sb.Append(@"minute");
                    break;
                case DateTimeUnit.Hour:
                    sb.Append(@"hour");
                    break;
                case DateTimeUnit.Day:
                    sb.Append(@"day");
                    break;
                case DateTimeUnit.Week:
                    sb.Append(@"week");
                    break;
                case DateTimeUnit.Month:
                    sb.Append(@"month");
                    break;
                case DateTimeUnit.QuarterYear:
                    sb.Append(@"quarter");
                    break;
                case DateTimeUnit.Year:
                    sb.Append(@"year");
                    break;
            }
            sb.Append(',');
            sb.Append(Value1.Build(conn, relatedQuery));
            sb.Append(',');
            sb.Append(Value2.Build(conn, relatedQuery));
            sb.Append(')');
        }
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
