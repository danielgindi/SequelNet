using SequelNet.Sql.Spatial;
using System;
using System.Text;

namespace SequelNet.Connector
{
    public class OleDbLanguageFactory : LanguageFactory
    {
        #region Syntax

        public override bool IsBooleanFalseOrderedFirst => false;

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
            Query qry,
            ConnectorBase conn,
            TableSchema.Index index,
            StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"ALTER TABLE ");

            BuildTableName(qry, conn, outputBuilder, false);

            outputBuilder.Append(@" ADD ");

            if (index.Mode == SequelNet.TableSchema.IndexMode.PrimaryKey)
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

        #endregion

        #region Types

        public override string AutoIncrementType => @"AUTOINCREMENT";
        public override string AutoIncrementBigIntType => @"AUTOINCREMENT";

        public override string TinyIntType => @"BYTE";
        public override string UnsignedTinyIntType => @"TINYINT";
        public override string SmallIntType => @"SHORT";
        public override string UnsignedSmallIntType => @"SHORT";
        public override string IntType => @"INT";
        public override string UnsignedIntType => @"INT";
        public override string BigIntType => @"INT";
        public override string UnsignedBigIntType => @"INT";
        public override string NumericType => @"NUMERIC";
        public override string DecimalType => @"DECIMAL";
        public override string MoneyType => @"DECIMAL";
        public override string FloatType => @"SINGLE";
        public override string DoubleType => @"DOUBLE";
        public override string VarCharType => @"VARCHAR";
        public override string CharType => @"CHAR";
        public override string TextType => @"TEXT";
        public override string MediumTextType => @"TEXT";
        public override string LongTextType => @"TEXT";
        public override string BooleanType => @"BIT";
        public override string DateTimeType => @"DATETIME";
        public override string GuidType => @"UNIQUEIDENTIFIER";
        public override string BlobType => @"IMAGE";
        public override string JsonType => @"TEXT";
        public override string JsonBinaryType => @"TEXT";

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
