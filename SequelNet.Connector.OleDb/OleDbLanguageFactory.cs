using SequelNet.Sql.Spatial;
using System;
using System.Text;

namespace SequelNet.Connector;

public class OleDbLanguageFactory : LanguageFactory
{
    #region Syntax

    public override bool IsBooleanFalseOrderedFirst => false;

    public override bool UpdateFromInsteadOfJoin => false;
    public override bool UpdateJoinRequiresFromLeftTable => true;

    public override bool SupportsRenameColumn => false;
    public override bool SupportsMultipleAlterTable => false;
    public override string AlterTableAddCommandName => "";
    public override string AlterTableAddColumnCommandName => "ADD COLUMN";
    public override string AlterTableAddIndexCommandName => "ADD";
    public override string AlterTableAddForeignKeyCommandName => "ADD CONSTRAINT";
    public override string DropIndexCommandName => "DROP CONSTRAINT";

    public override string UtcNow()
    {
        throw new NotImplementedException(
            "UTC timestamp retrieval has not been implemented for this connector");
    }

    public override void BuildConvertUtcToTz(
        ValueWrapper value,
        ValueWrapper timeZone,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        throw new NotImplementedException(
            "UTC time zone conversion has not been implemented for this connector");
    }

    public override string StringToLower(string value)
    {
        return @"LCASE(" + value + ")";
    }

    public override string StringToUpper(string value)
    {
        return @"UCASE(" + value + ")";
    }

    public override string LengthOfString(string value)
    {
        return @"LEN(" + value + ")";
    }

    public override string HourPartOfDateOrTime(string date)
    {
        return @"HOUR(" + date + ")";
    }

    public override string MinutePartOfDateOrTime(string date)
    {
        return @"MINUTE(" + date + ")";
    }

    public override string SecondPartOfDateOrTime(string date)
    {
        return @"SECOND(" + date + ")";
    }

    public override string DatePartOfDateTime(string date)
    {
        return $"DATEVALUE({date})";
    }

    public override string TimePartOfDateTime(string date)
    {
        return $"TIMEVALUE({date})";
    }

    public override string ExtractUnixTimestamp(string date)
    {
        return $"DATEDIFF('s', #1970-01-01 00:00:00#, {date})";
    }

    public override string NullOrDefaultValue(string expression, string defaultValue)
    {
        return $"Nz({expression}, {defaultValue})";
    }

    public override string IfEqualThenNull(string value1expr, string value2expr)
    {
        return $"IIF({value1expr} = {value2expr}, NULL, {value1expr})";
    }

    public override string Md5Hex(string value)
    {
        throw new NotSupportedException("MD5 is not supported by this connector");
    }

    public override string Md5Binary(string value)
    {
        throw new NotSupportedException("MD5 is not supported by this connector");
    }

    public override string Sha1Hex(string value)
    {
        throw new NotSupportedException("SHA1 is not supported by this connector");
    }

    public override string Sha1Binary(string value)
    {
        throw new NotSupportedException("SHA1 is not supported by this connector");
    }

    public override string ST_X(string pt)
    {
        throw new NotImplementedException("ST_X has not been implemented for this connector");
    }

    public override string ST_Y(string pt)
    {
        throw new NotImplementedException("ST_Y has not been implemented for this connector");
    }

    public override void BuildCeil(
        Phrases.Ceil phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        sb.Append("(-INT(-(");
        phrase.Value.Build(sb, conn, relatedQuery);
        sb.Append(")))");
    }

    public override void BuildFloor(
        Phrases.Floor phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        sb.Append("INT(");
        phrase.Value.Build(sb, conn, relatedQuery);
        sb.Append(')');
    }

    public override void BuildGreatest(
        Phrases.Greatest phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        throw new NotImplementedException(
            "GREATEST has not been implemented for this connector");
    }

    public override void BuildLeast(
        Phrases.Least phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        throw new NotImplementedException(
            "LEAST has not been implemented for this connector");
    }

    public override void BuildGeographySphericalDistanceMath(
        Phrases.GeographySphericalDistanceMath phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        throw new NotImplementedException(
            "Geography spherical distance has not been implemented for this connector");
    }

    public override string DateTimeFormat(string date, Phrases.DateTimeFormat.FormatOptions format)
    {
        switch (format)
        {
            case Phrases.DateTimeFormat.FormatOptions.IsoDateTime:
                return $@"FORMAT({date}, 'yyyy-mm-dd\Thh:nn:ss')";

            case Phrases.DateTimeFormat.FormatOptions.IsoDate:
                return $"FORMAT({date}, 'yyyy-mm-dd')";

            case Phrases.DateTimeFormat.FormatOptions.IsoTime:
                return $"FORMAT({date}, 'hh:nn:ss')";

            case Phrases.DateTimeFormat.FormatOptions.IsoYearMonth:
                return $"FORMAT({date}, 'yyyy-mm')";

            default:
                throw new NotImplementedException($"DateTimeFormat with format {format} has not been implemented for this connector");
        }
    }

    public override string FormatCreateDate(int year, int month, int day)
    {
        return $"DATESERIAL({year}, {month}, {day})";
    }

    public override string FormatCreateTime(
        int hours,
        int minutes,
        int seconds,
        int milliseconds)
    {
        var time = $"TIMESERIAL({hours}, {minutes}, {seconds})";

        if (milliseconds == 0)
            return time;

        return $"{time} + ({milliseconds} / 86400000.0)";
    }

    public override void BuildConcat(
        Phrases.Concat phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        if (phrase.Values.Count == 0)
        {
            sb.Append(PrepareValue(""));
            return;
        }

        var values = new string[phrase.Values.Count];
        for (var i = 0; i < phrase.Values.Count; i++)
            values[i] = phrase.Values[i].Build(conn, relatedQuery);

        if (phrase.IgnoreNulls)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    sb.Append(" & ");

                sb.Append("Nz(");
                sb.Append(values[i]);
                sb.Append(", '')");
            }

            return;
        }

        sb.Append("IIF(");
        for (var i = 0; i < values.Length; i++)
        {
            if (i > 0)
                sb.Append(" OR ");

            sb.Append("IsNull(");
            sb.Append(values[i]);
            sb.Append(')');
        }

        sb.Append(", NULL, ");
        for (var i = 0; i < values.Length; i++)
        {
            if (i > 0)
                sb.Append(" & ");

            sb.Append(values[i]);
        }

        sb.Append(')');
    }

    public override void BuildDateTimeAdd(
        Phrases.DateTimeAdd phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        var interval = GetDateInterval(phrase.Unit);

        sb.Append("DATEADD('");
        sb.Append(interval);
        sb.Append("', ");
        phrase.Value2.Build(sb, conn, relatedQuery);
        sb.Append(", ");
        phrase.Value1.Build(sb, conn, relatedQuery);
        sb.Append(')');
    }

    public override void BuildDateTimeDiff(
        Phrases.DateTimeDiff phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        var interval = GetDateInterval(phrase.Unit);

        sb.Append("DATEDIFF('");
        sb.Append(interval);
        sb.Append("', ");
        phrase.Value1.Build(sb, conn, relatedQuery);
        sb.Append(", ");
        phrase.Value2.Build(sb, conn, relatedQuery);
        sb.Append(')');
    }

    private static string GetDateInterval(Phrases.DateTimeUnit unit)
    {
        return unit switch
        {
            Phrases.DateTimeUnit.Microsecond => throw new NotSupportedException(
                "Access does not support microsecond date intervals"),
            Phrases.DateTimeUnit.Millisecond => throw new NotSupportedException(
                "Access does not support millisecond date intervals"),
            Phrases.DateTimeUnit.Minute => "n",
            Phrases.DateTimeUnit.Hour => "h",
            Phrases.DateTimeUnit.Day => "d",
            Phrases.DateTimeUnit.Week => "ww",
            Phrases.DateTimeUnit.Month => "m",
            Phrases.DateTimeUnit.QuarterYear => "q",
            Phrases.DateTimeUnit.Year => "yyyy",
            _ => "s",
        };
    }

    public override void BuildCase(
        Phrases.Case phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        throw new NotImplementedException(
            "CASE has not been implemented for this connector");
    }

    public override void BuildCast(
        ValueWrapper value,
        DataTypeDef typeDef,
        StringBuilder sb,
        ConnectorBase connection,
        Query relatedQuery)
    {
        throw new NotImplementedException(
            "CAST has not been implemented for this connector");
    }

    public override void BuildLimitOffset(
        Query query,
        bool top,
        StringBuilder outputBuilder)
    {
        if (!top)
            return;

        if (query.Limit > 0)
        {
            outputBuilder.Append(" TOP ");
            outputBuilder.Append(query.Limit);
            outputBuilder.Append(' ');
        }
    }

    public override void BuildCreateIndex(
        TableSchema.Index index,
        StringBuilder outputBuilder,
        Query qry,
        ConnectorBase conn)
    {
        if (index.Mode == TableSchema.IndexMode.PrimaryKey)
        {
            outputBuilder.AppendFormat(@"CONSTRAINT {0} PRIMARY KEY ", WrapFieldName(index.Name));
        }
        else
        {
            if (index.Mode == TableSchema.IndexMode.Unique) outputBuilder.Append(@"UNIQUE ");
            outputBuilder.Append(@"INDEX ");
            outputBuilder.Append(WrapFieldName(index.Name));
            outputBuilder.Append(@" ");
        }
        outputBuilder.Append(@"(");
        for (int i = 0; i < index.Columns.Length; i++)
        {
            if (i > 0) outputBuilder.Append(",");

            var column = index.Columns[i];
            column.Target.Build(outputBuilder, conn, qry);

            if (column.Sort != null)
                outputBuilder.Append(column.Sort == SortDirection.ASC ? " ASC" : " DESC");
        }
        outputBuilder.Append(@")");
    }

    public override void BuildColumnPropertiesDataType(
        TableSchema.Column column,
        out bool isDefaultAllowed,
        StringBuilder sb,
        ConnectorBase connection,
        Query relatedQuery)
    {
        isDefaultAllowed = true;

        if (column.LiteralType != null && column.LiteralType.Length > 0)
        {
            sb.Append(column.LiteralType);
            return;
        }

        var (dataTypeString, isDefaultAllowedResult) = BuildDataTypeDef(column.DataTypeDef);

        if (string.IsNullOrEmpty(dataTypeString))
        {
            throw new NotImplementedException("Unsupprted data type " + column.ActualDataType.ToString());
        }

        isDefaultAllowed = isDefaultAllowedResult;

        sb.Append(dataTypeString);

        if (column.AutoIncrement)
            sb.Append(" IDENTITY");

        if (column.ComputedColumn != null)
        {
            sb.Append(" AS ");

            sb.Append(column.ComputedColumn?.Build(connection, relatedQuery));

            if (column.ComputedColumnStored)
                sb.Append(" PERSISTED");
        }

        if (!string.IsNullOrEmpty(column.Collate))
        {
            sb.Append(@" COLLATE ");
            sb.Append(column.Collate);
        }
    }

    public override (string typeString, bool isDefaultAllowed) BuildDataTypeDef(DataTypeDef typeDef, bool forCast = false)
    {
        string typeString = null;
        bool isDefaultAllowed = true;

        switch (typeDef.Type)
        {
            case DataType.VarChar:
                if (typeDef.MaxLength < 0)
                    typeString = "VARCHAR(MAX)";
                else if (typeDef.MaxLength == 0)
                    typeString = "TEXT";
                else if (typeDef.MaxLength <= VarCharMaxLength)
                    typeString = $"VARCHAR({typeDef.MaxLength})";
                else if (typeDef.MaxLength < 65536)
                    typeString = "TEXT";
                else if (typeDef.MaxLength < 16777215)
                    typeString = "TEXT";
                else
                    typeString = "TEXT";
                break;
            case DataType.Char:
                if (typeDef.MaxLength < 0)
                    typeString = "VARCHAR(MAX)";
                else if (typeDef.MaxLength == 0 || typeDef.MaxLength >= VarCharMaxLength)
                    typeString = $"VARCHAR({VarCharMaxLength})";
                else
                    typeString = $"VARCHAR({typeDef.MaxLength})";
                break;
            case DataType.Text:
                typeString = "TEXT";
                break;
            case DataType.MediumText:
                typeString = "TEXT";
                break;
            case DataType.LongText:
                typeString = "TEXT";
                break;
            case DataType.Boolean:
                typeString = "BIT";
                break;
            case DataType.DateTime:
                typeString = "DATETIME";
                break;
            case DataType.Date:
                typeString = "DATE";
                break;
            case DataType.Time:
                typeString = "TIME";
                break;
            case DataType.Numeric:
                if (typeDef.Precision > 0)
                    typeString = $"NUMERIC({typeDef.Precision}, {typeDef.Scale})";
                else
                    typeString = "NUMERIC";
                break;
            case DataType.Float:
                typeString = "SINGLE";
                break;
            case DataType.Double:
                typeString = "DOUBLE";
                break;
            case DataType.Decimal:
                typeString = "DECIMAL";
                break;
            case DataType.Money:
                typeString = "DECIMAL";
                break;
            case DataType.TinyInt:
                typeString = "BYTE";
                break;
            case DataType.UnsignedTinyInt:
                typeString = "TINYINT";
                break;
            case DataType.SmallInt:
                typeString = "SHORT";
                break;
            case DataType.UnsignedSmallInt:
                typeString = "SHORT";
                break;
            case DataType.Int:
                typeString = "AUTOINCREMENT";
                break;
            case DataType.UnsignedInt:
                typeString = "INT";
                break;
            case DataType.BigInt:
                typeString = "AUTOINCREMENT";
                break;
            case DataType.UnsignedBigInt:
                typeString = "INT";
                break;
            case DataType.Json:
                typeString = "TEXT";
                break;
            case DataType.JsonBinary:
                typeString = "TEXT";
                break;
            case DataType.Blob:
                typeString = "IMAGE";
                break;
            case DataType.Binary:
                typeString = $"BINARY({VarCharMaxLength})";
                break;
            case DataType.VarBinary:
                typeString = $"VARBINARY({VarCharMaxLength})";
                break;
            case DataType.Guid:
                typeString = "UNIQUEIDENTIFIER";
                break;
        }

        if (!string.IsNullOrEmpty(typeDef.Charset))
        {
            typeString += $" CHARACTER SET {typeDef.Charset}";
        }

        return (typeString, isDefaultAllowed);
    }

    public override void BuildCollate(
        ValueWrapper value,
        string collation,
        SortDirection direction,
        StringBuilder sb,
        ConnectorBase connection,
        Query relatedQuery)
    {
        throw new NotImplementedException(
            "COLLATE has not been implemented for this connector");
    }

    public override void BuildOrderByRandom(ValueWrapper? seedValue, ConnectorBase conn, StringBuilder outputBuilder)
    {
        if (seedValue != null)
        {
            outputBuilder.Append(@"RND(" + seedValue.Value.Build(conn) + @")");
        }
        else
        {
            outputBuilder.Append(@"RND(NULL)");
        }
    }

    public override void BuildRandWeight(
        Phrases.RandWeight phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        sb.Append("Rnd() * ");
        phrase.Value.Build(sb, conn, relatedQuery);
    }

    public override string BuildFindString(
        ConnectorBase conn,
        ValueWrapper needle,
        ValueWrapper haystack,
        ValueWrapper? startAt,
        Query relatedQuery)
    {
        string ret = "InStr(";

        if (startAt != null)
        {
            ret += startAt.Value.Build(conn, relatedQuery);
            ret += ",";
        }

        ret += haystack.Build(conn, relatedQuery);
        ret += ",";
        ret += needle.Build(conn, relatedQuery);

        ret += ")";

        return ret;
    }

    public override string BuildSubstring(
        ConnectorBase conn,
        ValueWrapper value,
        ValueWrapper from,
        ValueWrapper? length,
        Query relatedQuery)
    {
        string ret = "MID(";

        ret += value.Build(conn, relatedQuery);

        ret += ", " + from.Build(conn, relatedQuery);

        if (length != null)
        {
            ret += ", " + length.Value.Build(conn, relatedQuery);
        }

        ret += ")";

        return ret;
    }

    public override void BuildStandardDeviationOfPopulation(
        Phrases.StandardDeviationOfPopulation phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        sb.Append("StDevP(");
        phrase.Value.Build(sb, conn, relatedQuery);
        sb.Append(')');
    }

    public override void BuildCount(
        Phrases.Count phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        ThrowIfDistinctAggregate(phrase.Distinct);
        base.BuildCount(phrase, sb, conn, relatedQuery);
    }

    public override void BuildMax(
        Phrases.Max phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        ThrowIfDistinctAggregate(phrase.Distinct);
        base.BuildMax(phrase, sb, conn, relatedQuery);
    }

    public override void BuildMin(
        Phrases.Min phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        ThrowIfDistinctAggregate(phrase.Distinct);
        base.BuildMin(phrase, sb, conn, relatedQuery);
    }

    public override void BuildSum(
        Phrases.Sum phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        ThrowIfDistinctAggregate(phrase.Distinct);
        base.BuildSum(phrase, sb, conn, relatedQuery);
    }

    private static void ThrowIfDistinctAggregate(bool distinct)
    {
        if (distinct)
            throw new NotImplementedException(
                "DISTINCT aggregate expressions have not been implemented for this connector");
    }

    public override void BuildStandardDeviationOfSample(
        Phrases.StandardDeviationOfSample phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        sb.Append("StDev(");
        phrase.Value.Build(sb, conn, relatedQuery);
        sb.Append(')');
    }

    public override void BuildStandardVarianceOfPopulation(
        Phrases.StandardVarianceOfPopulation phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        sb.Append("VarP(");
        phrase.Value.Build(sb, conn, relatedQuery);
        sb.Append(')');
    }

    public override void BuildStandardVarianceOfSample(
        Phrases.StandardVarianceOfSample phrase,
        StringBuilder sb,
        ConnectorBase conn,
        Query relatedQuery)
    {
        sb.Append("Var(");
        phrase.Value.Build(sb, conn, relatedQuery);
        sb.Append(')');
    }

    #endregion

    #region Reading values from SQL

    public override Geometry ReadGeometry(object value)
    {
        byte[] geometryData = value as byte[];
        if (geometryData != null)
        {
            return WkbReader.GeometryFromWkb(geometryData, false);
        }
        return null;
    }

    #endregion

    #region Preparing values for SQL

    public override string WrapFieldName(string fieldName)
    {
        return '[' + fieldName + ']';
    }

    public override string EscapeString(string value)
    {
        return value.Replace(@"'", @"''");
    }

    public override string PrepareValue(Guid value)
    {
        return '\'' + value.ToString(@"D") + '\'';
    }

    public override string PrepareValue(bool value)
    {
        return value ? @"true" : @"false";
    }

    public override string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
    }

    public override string EscapeLike(string expression)
    {
        return expression.Replace(@"\", @"\\").Replace(@"%", @"\%").Replace(@"_", @"\_").Replace("[", "[[]");
    }

    #endregion
}
