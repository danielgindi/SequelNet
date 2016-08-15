using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class DateTimeAdd : IPhrase
    {
        public string TableName1;
        public object Value1;
        public ValueObjectType ValueType1;
        public string TableName2;
        public object Value2;
        public ValueObjectType ValueType2;
        public DateTimeUnit Unit;

        #region Constructors

        [Obsolete]
        public DateTimeAdd(string tableName, object value, ValueObjectType valueType, DateTimeUnit unit, Int64 interval)
        {
            this.TableName1 = tableName;
            this.Value1 = value;
            this.ValueType1 = valueType;
            this.Unit = unit;
            this.Value2 = interval;
            this.ValueType2 = ValueObjectType.Value;
        }

        public DateTimeAdd(object value, ValueObjectType valueType, DateTimeUnit unit, Int64 interval)
        {
            this.Value1 = value;
            this.ValueType1 = valueType;
            this.Unit = unit;
            this.Value2 = interval;
            this.ValueType2 = ValueObjectType.Value;
        }

        public DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, Int64 interval)
        {
            this.TableName1 = tableName;
            this.Value1 = columnName;
            this.ValueType1 = ValueObjectType.ColumnName;
            this.Unit = unit;
            this.Value2 = interval;
            this.ValueType2 = ValueObjectType.Value;
        }

        public DateTimeAdd(object value, ValueObjectType valueType, DateTimeUnit unit, string addTableName, string addColumnName)
        {
            this.Value1 = value;
            this.ValueType1 = valueType;
            this.Unit = unit;
            this.TableName2 = addTableName;
            this.Value2 = addColumnName;
            this.ValueType2 = ValueObjectType.ColumnName;
        }

        public DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, string addTableName, string addColumnName)
        {
            this.TableName1 = tableName;
            this.Value1 = columnName;
            this.ValueType1 = ValueObjectType.ColumnName;
            this.Unit = unit;
            this.TableName2 = addTableName;
            this.Value2 = addColumnName;
            this.ValueType2 = ValueObjectType.ColumnName;
        }

        public DateTimeAdd(object value, ValueObjectType valueType, DateTimeUnit unit, string addColumnName)
        {
            this.Value1 = value;
            this.ValueType1 = valueType;
            this.Unit = unit;
            this.Value2 = addColumnName;
            this.ValueType2 = ValueObjectType.ColumnName;
        }

        public DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, string addColumnName)
        {
            this.TableName1 = tableName;
            this.Value1 = columnName;
            this.ValueType1 = ValueObjectType.ColumnName;
            this.Unit = unit;
            this.Value2 = addColumnName;
            this.ValueType2 = ValueObjectType.ColumnName;
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

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
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

                sb.Append(PhraseHelper.StringifyValue(TableName2, Value2, ValueType2, conn, relatedQuery));

                if (Unit == DateTimeUnit.Millisecond)
                {
                    sb.Append(" * 1000");
                }

                sb.Append(',');

                if (ValueType1 == ValueObjectType.ColumnName)
                {
                    if (TableName1 != null && TableName1.Length > 0)
                    {
                        sb.Append(conn.WrapFieldName(TableName1));
                        sb.Append(".");
                    }
                    sb.Append(conn.WrapFieldName(Value1.ToString()));
                }
                else if (ValueType1 == ValueObjectType.Value)
                {
                    sb.Append(conn.PrepareValue(Value1, relatedQuery));
                }
                else sb.Append(Value1);

                sb.Append(')');
            }
            else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
            {
                if (ValueType1 == ValueObjectType.ColumnName)
                {
                    if (TableName1 != null && TableName1.Length > 0)
                    {
                        sb.Append(conn.WrapFieldName(TableName1));
                        sb.Append(".");
                    }
                    sb.Append(conn.WrapFieldName(Value1.ToString()));
                }
                else if (ValueType1 == ValueObjectType.Value)
                {
                    sb.Append(conn.PrepareValue(Value1, relatedQuery));
                }
                else sb.Append(Value1);

                sb.Append(" + INTERVAL '");

                sb.Append(PhraseHelper.StringifyValue(TableName2, Value2, ValueType2, conn, relatedQuery));

                switch (Unit)
                {
                    case DateTimeUnit.Microsecond:
                        sb.Append(" / 1000 milliseconds");
                        break;
                    case DateTimeUnit.Millisecond:
                        sb.Append(" milliseconds");
                        break;
                    default:
                    case DateTimeUnit.Second:
                        sb.Append(" seconds");
                        break;
                    case DateTimeUnit.Minute:
                        sb.Append(" minutes");
                        break;
                    case DateTimeUnit.Hour:
                        sb.Append(" hours");
                        break;
                    case DateTimeUnit.Day:
                        sb.Append(" days");
                        break;
                    case DateTimeUnit.Week:
                        sb.Append(" weeks");
                        break;
                    case DateTimeUnit.Month:
                        sb.Append(" months");
                        break;
                    case DateTimeUnit.QuarterYear:
                        sb.Append(" * 3 months");
                        break;
                    case DateTimeUnit.Year:
                        sb.Append(" years");
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
                sb.Append(PhraseHelper.StringifyValue(TableName2, Value2, ValueType2, conn, relatedQuery));
                sb.Append(',');

                if (ValueType1 == ValueObjectType.ColumnName)
                {
                    if (TableName1 != null && TableName1.Length > 0)
                    {
                        sb.Append(conn.WrapFieldName(TableName1));
                        sb.Append(".");
                    }
                    sb.Append(conn.WrapFieldName(Value1.ToString()));
                }
                else if (ValueType1 == ValueObjectType.Value)
                {
                    sb.Append(conn.PrepareValue(Value1, relatedQuery));
                }
                else sb.Append(Value1);

                sb.Append(')');
            }

            return sb.ToString();
        }
    }
}
