using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class DateTimeDiff : IPhrase
    {
        public ValueWrapper Value1;
        public ValueWrapper Value2;
        public DateTimeUnit Unit;

        #region Constructors

        public DateTimeDiff(DateTimeUnit unit, object value1, ValueObjectType value1Type, object value2, ValueObjectType value2Type)
        {
            this.Unit = unit;
            this.Value1 = new ValueWrapper(value1, value1Type);
            this.Value2 = new ValueWrapper(value2, value2Type);
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
            this.Value1 = new ValueWrapper(tableName1, columnName1, ValueObjectType.ColumnName);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public DateTimeDiff(DateTimeUnit unit, DateTime value1, string tableName2, string columnName2)
        {
            this.Unit = unit;
            this.Value1 = new ValueWrapper(value1, ValueObjectType.Value);
            this.Value2 = new ValueWrapper(tableName2, columnName2, ValueObjectType.ColumnName);
        }

        public DateTimeDiff(DateTimeUnit unit, string tableName1, string columnName1, string tableName2, string columnName2)
        {
            this.Unit = unit;
            this.Value1 = new ValueWrapper(tableName1, columnName1, ValueObjectType.ColumnName);
            this.Value2 = new ValueWrapper(tableName2, columnName2, ValueObjectType.ColumnName);
        }

        public DateTimeDiff(DateTimeUnit unit, string columnName1, string columnName2)
        {
            this.Unit = unit;
            this.Value1 = new ValueWrapper(null, columnName1, ValueObjectType.ColumnName);
            this.Value2 = new ValueWrapper(null, columnName2, ValueObjectType.ColumnName);
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
            this.Value1 = new ValueWrapper(phrase1, ValueObjectType.Value);
            this.Value2 = new ValueWrapper(tableName2, columnName2, ValueObjectType.ColumnName);
        }

        public DateTimeDiff(DateTimeUnit unit, string tableName1, string columnName1, IPhrase phrase2)
        {
            this.Unit = unit;
            this.Value1 = new ValueWrapper(tableName1, columnName1, ValueObjectType.ColumnName);
            this.Value2 = new ValueWrapper(phrase2, ValueObjectType.Value);
        }

        public DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, string columnName2)
        {
            this.Unit = unit;
            this.Value1 = new ValueWrapper(null, phrase1, ValueObjectType.Value);
            this.Value2 = new ValueWrapper(null, columnName2, ValueObjectType.ColumnName);
        }

        public DateTimeDiff(DateTimeUnit unit, string columnName1, IPhrase phrase2)
        {
            this.Unit = unit;
            this.Value1 = new ValueWrapper(null, columnName1, ValueObjectType.ColumnName);
            this.Value2 = new ValueWrapper(null, phrase2, ValueObjectType.Value);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            StringBuilder sb = new StringBuilder();

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

                sb.Append(Value2.Build(conn, relatedQuery));

                if (Unit == DateTimeUnit.Millisecond)
                {
                    sb.Append(" * 1000");
                }

                sb.Append(',');

                sb.Append(Value1.Build(conn, relatedQuery));

                sb.Append(')');
            }
            else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
            {
                switch (Unit)
                {
                    case DateTimeUnit.Microsecond:
                        sb.AppendFormat(@"((((DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))) * 60 + (DATE_PART('minute', {0}) - DATE_PART('minute', {1}))) * 60 + (DATE_PART('second', {0}) - DATE_PART('second', {1}))) * 1000000",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    case DateTimeUnit.Millisecond:
                        sb.AppendFormat(@"((((DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))) * 60 + (DATE_PART('minute', {0}) - DATE_PART('minute', {1}))) * 60 + (DATE_PART('second', {0}) - DATE_PART('second', {1}))) * 1000",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    default:
                    case DateTimeUnit.Second:
                        sb.AppendFormat(@"(((DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))) * 60 + (DATE_PART('minute', {0}) - DATE_PART('minute', {1}))) * 60 + (DATE_PART('second', {0}) - DATE_PART('second', {1}))",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    case DateTimeUnit.Minute:
                        sb.AppendFormat(@"((DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))) * 60 + (DATE_PART('minute', {0}) - DATE_PART('minute', {1}))",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    case DateTimeUnit.Hour:
                        sb.AppendFormat(@"(DATE_PART('day', {0}) - DATE_PART('day', {1})) * 24 + (DATE_PART('hour', {0}) - DATE_PART('hour', {1}))",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    case DateTimeUnit.Day:
                        sb.AppendFormat(@"DATE_PART('day', {0}) - DATE_PART('day', {1})",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    case DateTimeUnit.Week:
                        sb.AppendFormat(@"TRUNC(DATE_PART('day', {0} - {1}) / 7)",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    case DateTimeUnit.Month:
                        sb.AppendFormat(@"(DATE_PART('year', {0}) - DATE_PART('year', {1})) * 12 + (DATE_PART('month', {0}) - DATE_PART('month', {1}))",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    case DateTimeUnit.QuarterYear:
                        sb.AppendFormat(@"(DATE_PART('year', {0}) - DATE_PART('year', {1})) * 4 + TRUNC((DATE_PART('month', {0}) - DATE_PART('month', {1})) / 4)",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
                        break;
                    case DateTimeUnit.Year:
                        sb.AppendFormat(@"DATE_PART('year', {0}) - DATE_PART('year', {1})",
                            Value2.Build(conn, relatedQuery),
                            Value1.Build(conn, relatedQuery));
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
                sb.Append(Value2.Build(conn, relatedQuery));
                sb.Append(',');
                sb.Append(Value1.Build(conn, relatedQuery));
                sb.Append(')');
            }

            return sb.ToString();
        }
    }
}
