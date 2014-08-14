using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class DateTimeAdd : BasePhrase
    {
        string TableName;
        object Object;
        ValueObjectType ObjectType;
        DateTimeUnit Unit;
        Int64 Interval;

        public DateTimeAdd(string TableName, object Object, ValueObjectType ObjectType, DateTimeUnit unit, Int64 interval)
        {
            this.TableName = TableName;
            this.Object = Object;
            this.ObjectType = ObjectType;
            this.Unit = unit;
            this.Interval = interval;
        }
        public DateTimeAdd(object Object, ValueObjectType ObjectType, DateTimeUnit unit, Int64 interval)
            : this(null, Object, ObjectType, unit, interval)
        {
        }
        public DateTimeAdd(string ColumnName, DateTimeUnit unit, Int64 interval)
            : this(null, ColumnName, ValueObjectType.ColumnName, unit, interval)
        {
        }
        public string BuildPhrase(ConnectorBase conn)
        {
            StringBuilder sb = new StringBuilder();

            if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL)
            {
                sb.Append(@"TIMESTAMPADD(");
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
                sb.Append(Unit == DateTimeUnit.Millisecond ? Interval * 1000L : Interval);
                sb.Append(',');

                if (ObjectType == ValueObjectType.ColumnName)
                {
                    if (TableName != null && TableName.Length > 0)
                    {
                        sb.Append(conn.EncloseFieldName(TableName));
                        sb.Append(".");
                    }
                    sb.Append(conn.EncloseFieldName(Object.ToString()));
                }
                else if (ObjectType == ValueObjectType.Value)
                {
                    sb.Append(conn.PrepareValue(Object));
                }
                else sb.Append(Object);

                sb.Append(')');
            }
            else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
            {
                if (ObjectType == ValueObjectType.ColumnName)
                {
                    if (TableName != null && TableName.Length > 0)
                    {
                        sb.Append(conn.EncloseFieldName(TableName));
                        sb.Append(".");
                    }
                    sb.Append(conn.EncloseFieldName(Object.ToString()));
                }
                else if (ObjectType == ValueObjectType.Value)
                {
                    sb.Append(conn.PrepareValue(Object));
                }
                else sb.Append(Object);

                sb.Append(" + INTERVAL '");

                switch (Unit)
                {
                    case DateTimeUnit.Microsecond:
                        sb.Append(Interval / (double)1000);
                        sb.Append(@" milliseconds");
                        break;
                    case DateTimeUnit.Millisecond:
                        sb.Append(Interval);
                        sb.Append(@" milliseconds");
                        break;
                    default:
                    case DateTimeUnit.Second:
                        sb.Append(Interval);
                        sb.Append(@" seconds");
                        break;
                    case DateTimeUnit.Minute:
                        sb.Append(Interval);
                        sb.Append(@" minutes");
                        break;
                    case DateTimeUnit.Hour:
                        sb.Append(Interval);
                        sb.Append(@" hours");
                        break;
                    case DateTimeUnit.Day:
                        sb.Append(Interval);
                        sb.Append(@" days");
                        break;
                    case DateTimeUnit.Week:
                        sb.Append(Interval);
                        sb.Append(@" weeks");
                        break;
                    case DateTimeUnit.Month:
                        sb.Append(Interval);
                        sb.Append(@" months");
                        break;
                    case DateTimeUnit.QuarterYear:
                        sb.Append(Interval * 3);
                        sb.Append(@" months");
                        break;
                    case DateTimeUnit.Year:
                        sb.Append(Interval * 3);
                        sb.Append(@" years");
                        break;
                }

                sb.Append('\'');
            }
            else // if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL || conn.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
            {
                sb.Append(@"DATEADD(");
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
                sb.Append(Interval);
                sb.Append(',');

                if (ObjectType == ValueObjectType.ColumnName)
                {
                    if (TableName != null && TableName.Length > 0)
                    {
                        sb.Append(conn.EncloseFieldName(TableName));
                        sb.Append(".");
                    }
                    sb.Append(conn.EncloseFieldName(Object.ToString()));
                }
                else if (ObjectType == ValueObjectType.Value)
                {
                    sb.Append(conn.PrepareValue(Object));
                }
                else sb.Append(Object);

                sb.Append(')');
            }

            return sb.ToString();
        }
    }
}
