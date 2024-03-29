﻿using SequelNet.Sql.Spatial;
using System;
using System.Text;

namespace SequelNet.Connector
{
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
            return @"now()"; // NOT UTC
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
            return @"DATEPART(hour, " + date + ")";
        }

        public override string MinutePartOfDateOrTime(string date)
        {
            return @"DATEPART(minute, " + date + ")";
        }

        public override string SecondPartOfDateOrTime(string date)
        {
            return @"DATEPART(second, " + date + ")";
        }

        public override string ExtractUnixTimestamp(string date)
        {
            return $"DATEDIFF(second, '1970-01-01 00:00:00', {date})";
        }

        public override string NullOrDefaultValue(string expression, string defaultValue)
        {
            return $"ISNULL({expression}, {defaultValue})";
        }

        public override string DateTimeFormat(string date, Phrases.DateTimeFormat.FormatOptions format)
        {
            switch (format)
            {
                case Phrases.DateTimeFormat.FormatOptions.IsoDateTime:
                    return $"CONVERT(nvarchar(19), {date}, 126)";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeFFF:
                    return $"CONVERT(nvarchar(23), {date}, 126)";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeZ:
                    return $"CONCAT(CONVERT(nvarchar(19), {date}, 127), 'Z')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeFFFZ:
                    return $"CONCAT(CONVERT(nvarchar(23), {date}, 127), 'Z')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDate:
                    return $"CONVERT(nvarchar(10), {date}, 23)";

                case Phrases.DateTimeFormat.FormatOptions.IsoTime:
                    return $"CONVERT(nvarchar(10), {date}, 114)";

                case Phrases.DateTimeFormat.FormatOptions.IsoTimeFFF:
                    return $"CONVERT(nvarchar(14), {date}, 114)";

                case Phrases.DateTimeFormat.FormatOptions.IsoYearMonth:
                    return $"CONVERT(nvarchar(7), {date}, 127)";

                default:
                    throw new NotImplementedException($"DateTimeFormat with format {format} has not been implemented for this connector");
            }
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
            sb.Append("(");
            value.Build(sb, connection, relatedQuery);
            sb.Append(" COLLATE ");
            sb.Append(collation);

            switch (direction)
            {
                case SortDirection.ASC:
                    sb.Append(" ASC");
                    break;

                case SortDirection.DESC:
                    sb.Append(" DESC");
                    break;
            }

            sb.Append(")");
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
}
