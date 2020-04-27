using SequelNet.Sql.Spatial;
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

        public override string HourPartOfDate(string date)
        {
            return @"DATEPART(hour, " + date + ")";
        }

        public override string MinutePartOfDate(string date)
        {
            return @"DATEPART(minute, " + date + ")";
        }

        public override string SecondPartOfDate(string date)
        {
            return @"DATEPART(second, " + date + ")";
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
            for (int i = 0; i < index.ColumnNames.Length; i++)
            {
                if (i > 0) outputBuilder.Append(",");
                outputBuilder.Append(WrapFieldName(index.ColumnNames[i]));
                outputBuilder.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
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

            DataType dataType = column.ActualDataType;
            if (dataType == DataType.VarChar)
            {
                if (column.MaxLength < 0)
                {
                    sb.Append("VARCHAR");
                    sb.Append(@"(MAX)");
                }
                else if (column.MaxLength == 0)
                    sb.Append("TEXT");
                else if (column.MaxLength <= VarCharMaxLength)
                {
                    sb.Append("VARCHAR");
                    sb.AppendFormat(@"({0})", column.MaxLength);
                }
                else if (column.MaxLength < 65536)
                    sb.Append("TEXT");
                else if (column.MaxLength < 16777215)
                    sb.Append("TEXT");
                else
                    sb.Append("TEXT");
            }
            else if (dataType == DataType.Char)
            {
                if (column.MaxLength < 0)
                {
                    sb.Append("VARCHAR");
                    sb.Append(@"(MAX)");
                }
                else if (column.MaxLength == 0 || column.MaxLength >= VarCharMaxLength)
                {
                    sb.Append("VARCHAR");
                    sb.AppendFormat(@"({0})", VarCharMaxLength);
                }
                else
                {
                    sb.Append("VARCHAR");
                    sb.AppendFormat(@"({0})", column.MaxLength);
                }
            }
            else if (dataType == DataType.Text)
                sb.Append("TEXT");
            else if (dataType == DataType.MediumText)
                sb.Append("TEXT");
            else if (dataType == DataType.LongText)
                sb.Append("TEXT");
            else if (dataType == DataType.Boolean)
                sb.Append("BIT");
            else if (dataType == DataType.DateTime)
                sb.Append("DATETIME");
            else if (dataType == DataType.Date)
                sb.Append("DATE");
            else if (dataType == DataType.Time)
                sb.Append("TIME");
            else if (dataType == DataType.Numeric)
            {
                if (column.NumberPrecision > 0)
                {
                    sb.Append("NUMERIC");
                    sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                }
                else
                    sb.Append("NUMERIC");
            }
            else if (dataType == DataType.Float)
                sb.Append("SINGLE");
            else if (dataType == DataType.Double)
                sb.Append("DOUBLE");
            else if (dataType == DataType.Decimal)
                sb.Append("DECIMAL");
            else if (dataType == DataType.Money)
                sb.Append("DECIMAL");
            else if (dataType == DataType.TinyInt)
                sb.Append("BYTE");
            else if (dataType == DataType.UnsignedTinyInt)
                sb.Append("TINYINT");
            else if (dataType == DataType.SmallInt)
                sb.Append("SHORT");
            else if (dataType == DataType.UnsignedSmallInt)
                sb.Append("SHORT");
            else if (dataType == DataType.Int)
                sb.Append("AUTOINCREMENT");
            else if (dataType == DataType.UnsignedInt)
                sb.Append("INT");
            else if (dataType == DataType.BigInt)
                sb.Append("AUTOINCREMENT");
            else if (dataType == DataType.UnsignedBigInt)
                sb.Append("INT");
            else if (dataType == DataType.Json)
                sb.Append("TEXT");
            else if (dataType == DataType.JsonBinary)
                sb.Append("TEXT");
            else if (dataType == DataType.Blob)
                sb.Append("IMAGE");
            else if (dataType == DataType.Guid)
                sb.Append("UNIQUEIDENTIFIER");
            else throw new NotImplementedException("Unsupprted data type " + dataType.ToString());

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

            if (!string.IsNullOrEmpty(column.Charset))
            {
                sb.Append(@" CHARACTER SET ");
                sb.Append(column.Charset);
            }
        }

        public override void BuildOrderByRandom(ValueWrapper seedValue, ConnectorBase conn, StringBuilder outputBuilder)
        {
            if (seedValue != null)
            {
                outputBuilder.Append(@"RND(" + seedValue.Build(conn) + @")");
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

        public override string FormatDate(DateTime dateTime)
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
