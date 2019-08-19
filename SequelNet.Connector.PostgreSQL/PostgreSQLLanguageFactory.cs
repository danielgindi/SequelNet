using SequelNet.Sql.Spatial;
using System;
using System.Text;

namespace SequelNet.Connector
{
    public class PostgreSQLLanguageFactory : LanguageFactory
    {
        public PostgreSQLLanguageFactory(PostgreSQLMode postgreSqlMode)
        {
            _PostgreSQLMode = postgreSqlMode;
        }

        #region Versioning

        private PostgreSQLMode _PostgreSQLMode;

        #endregion

        #region Syntax

        public override bool ShouldPrefixAutoIncrementWithType => false;

        public override bool UpdateFromInsteadOfJoin => true;
        public override bool UpdateJoinRequiresFromLeftTable => false;

        public override int VarCharMaxLength => 357913937;

        public override string UtcNow()
        {
            return @"now() at time zone 'utc'";
        }

        public override string YearPartOfDate(string date)
        {
            return @"EXTRACT(YEAR FROM " + date + @")";
        }

        public override string MonthPartOfDate(string date)
        {
            return @"EXTRACT(MONTH FROM " + date + @")";
        }

        public override string DayPartOfDate(string date)
        {
            return @"EXTRACT(DAY FROM " + date + @")";
        }

        public override string HourPartOfDate(string date)
        {
            return @"EXTRACT(HOUR FROM " + date + @")";
        }

        public override string MinutePartOfDate(string date)
        {
            return @"EXTRACT(MINUTE FROM " + date + @")";
        }

        public override string SecondPartOfDate(string date)
        {
            return @"EXTRACT(SECOND FROM " + date + @")";
        }

        public override string Md5Hex(string value)
        {
            return @"md5(" + value + ")";
        }

        public override string Sha1Hex(string value)
        {
            throw new NotSupportedException("SHA1 is not supported by Postgresql");
        }

        public override string Md5Binary(string value)
        {
            return @"decode(md5(" + value + "), 'hex')";
        }

        public override string Sha1Binary(string value)
        {
            throw new NotSupportedException("SHA1 is not supported by Postgresql");
        }

        public override string ST_X(string pt)
        {
            return "ST_X(" + pt + ")";
        }

        public override string ST_Y(string pt)
        {
            return "ST_Y(" + pt + ")";
        }

        public override string ST_Contains(string g1, string g2)
        {
            return "ST_Contains(" + g1 + ", " + g2 + ")";
        }

        public override string ST_GeomFromText(string text, string srid = null)
        {
            return "ST_GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override string ST_GeogFromText(string text, string srid = null)
        {
            return "ST_GeogFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override void BuildNullSafeEqualsTo(
            Where where,
            bool negate,
            StringBuilder outputBuilder,
            Where.BuildContext context)
        {
            where.BuildSingleValueFirst(outputBuilder, context);

            if (negate)
                outputBuilder.Append(@" IS DISTINCT FROM ");
            else outputBuilder.Append(@" IS NOT DISTINCT FROM ");

            where.BuildSingleValueSecond(outputBuilder, context);
        }

        public override void BuildLimitOffset(
            Query query,
            bool top,
            StringBuilder outputBuilder)
        {
            if (top)
                return;

            var withOffset = query.Offset > 0 && query.QueryMode == QueryMode.Select;

            if (query.Limit > 0)
            {
                outputBuilder.Append(" LIMIT ");
                outputBuilder.Append(query.Limit);
                outputBuilder.Append(' ');
            }

            if (withOffset && query.Offset > 0)
            {
                outputBuilder.Append(" OFFSET ");
                outputBuilder.Append(query.Offset);
                outputBuilder.Append(' ');
            }
        }

        public override void BuildCreateIndex(
            Query qry,
            ConnectorBase conn,
            TableSchema.Index index,
            StringBuilder outputBuilder)
        {
            if (index.Mode == SequelNet.TableSchema.IndexMode.PrimaryKey)
            {
                outputBuilder.Append(@"ALTER TABLE ");

                BuildTableName(qry, conn, outputBuilder, false);

                outputBuilder.Append(@" ADD CONSTRAINT ");
                outputBuilder.Append(WrapFieldName(index.Name));
                outputBuilder.Append(@" PRIMARY KEY ");

                outputBuilder.Append(@"(");
                for (int i = 0; i < index.ColumnNames.Length; i++)
                {
                    if (i > 0) outputBuilder.Append(",");
                    outputBuilder.Append(WrapFieldName(index.ColumnNames[i]));
                    outputBuilder.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                }
                outputBuilder.Append(@")");
            }
            else
            {
                outputBuilder.Append(@"CREATE ");

                if (index.Mode == TableSchema.IndexMode.Unique) outputBuilder.Append(@"UNIQUE ");

                outputBuilder.Append(@"INDEX ");
                outputBuilder.Append(WrapFieldName(index.Name));

                outputBuilder.Append(@"ON ");

                BuildTableName(qry, conn, outputBuilder, false);

                if (index.Mode == TableSchema.IndexMode.Spatial)
                {
                    outputBuilder.Append(@"USING GIST");
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
        }

        public override void BuildOrderByRandom(ValueWrapper seedValue, ConnectorBase conn, StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"RANDOM()");
        }

        public override string Aggregate_Some(string rawExpression)
        {
            return "bool_or(" + rawExpression + ")";
        }

        public override string Aggregate_Every(string rawExpression)
        {
            return "bool_and(" + rawExpression + ")";
        }

        public override string GroupConcat(bool distinct, string rawExpression, string rawOrderBy, string separator)
        {
            var sb = new StringBuilder();

            sb.Append("string_agg(");

            if (distinct)
            {
                sb.Append("DISTINCT ");
            }

            sb.Append(rawExpression);

            if (separator != null)
            {
                sb.Append("," + PrepareValue(separator));
            }
            else
            {
                sb.Append(",','");
            }

            if (rawOrderBy != null)
            {
                sb.Append(rawOrderBy);
            }

            sb.Append(")");

            return sb.ToString();
        }

        #endregion

        #region Types

        public override string AutoIncrementType => @"SERIAL";
        public override string AutoIncrementBigIntType => @"BIGSERIAL";

        public override string TinyIntType => @"SMALLINT";
        public override string UnsignedTinyIntType => @"SMALLINT";
        public override string SmallIntType => @"SMALLINT";
        public override string UnsignedSmallIntType => @"SMALLINT";
        public override string IntType => @"INTEGER";
        public override string UnsignedIntType => @"INTEGER";
        public override string BigIntType => @"BIGINT";
        public override string UnsignedBigIntType => @"BIGINT";
        public override string NumericType => @"NUMERIC";
        public override string DecimalType => @"DECIMAL";
        public override string MoneyType => @"DECIMAL";
        public override string FloatType => @"FLOAT4";
        public override string DoubleType => @"FLOAT8";
        public override string VarCharType => @"VARCHAR";
        public override string CharType => @"CHAR";
        public override string TextType => @"TEXT";
        public override string MediumTextType => @"TEXT";
        public override string LongTextType => @"TEXT";
        public override string BooleanType => @"BOOLEAN";
        public override string DateTimeType => @"TIMESTAMP";
        public override string BlobType => @"BYTEA";
        public override string GuidType => @"UUID";
        public override string JsonType => @"JSON";
        public override string JsonBinaryType => @"JSONB";

        public override string TypeGeometry => @"GEOMETRY";
        public override string GeometryCollectionType => @"GEOMETRY";
        public override string PointType => @"GEOMETRY";
        public override string LineStringType => @"GEOMETRY";
        public override string PolygonType => @"GEOMETRY";
        public override string LineType => @"GEOMETRY";
        public override string CurveType => @"GEOMETRY";
        public override string SurfaceType => @"GEOMETRY";
        public override string LinearRingType => @"GEOMETRY";
        public override string MultiPointType => @"GEOMETRY";
        public override string MultiLineStringType => @"GEOMETRY";
        public override string MultiPolygonType => @"GEOMETRY";
        public override string MultiCurveType => @"GEOMETRY";
        public override string MultiSurfaceType => @"GEOMETRY";

        public override string GeographicType => @"GEOGRAPHIC";
        public override string GeographicCollectionType => @"GEOGRAPHIC";
        public override string GeographicPointType => @"GEOGRAPHIC";
        public override string GeographicLinestringType => @"GEOGRAPHIC";
        public override string GeographicPolygonType => @"GEOGRAPHIC";
        public override string GeographicLineType => @"GEOGRAPHIC";
        public override string GeographicCurveType => @"GEOGRAPHIC";
        public override string GeographicSurfaceType => @"GEOGRAPHIC";
        public override string GeographicLinearringType => @"GEOGRAPHIC";
        public override string GeographicMultipointType => @"GEOGRAPHIC";
        public override string GeographicMultilinestringType => @"GEOGRAPHIC";
        public override string GeographicMultipolygonType => @"GEOGRAPHIC";
        public override string GeographicMulticurveType => @"GEOGRAPHIC";
        public override string GeographicMultisurfaceType => @"GEOGRAPHIC";

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
        { // Note: For performance, ignoring enclosed " signs
            return '"' + fieldName + '"';
        }

        private static string EscapeStringWithBackslashes(string value)
        {
            var sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c == '\'' || c == '\\')
                {
                    sb.Append("\\");
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static string EscapeStringWithoutBackslashes(string value)
        {
            var sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c == '\'')
                {
                    sb.Append(c);
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public override string EscapeString(string value)
        {
            if (_PostgreSQLMode.StandardConformingStrings)
            {
                return EscapeStringWithoutBackslashes(value);
            }
            else
            {
                return EscapeStringWithBackslashes(value);
            }
        }

        public override string PrepareValue(bool value)
        {
            return value ? @"TRUE" : @"FALSE";
        }

        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }

        public override string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"_", @"\x10_").Replace(@"%", @"\x10%");
        }

        public override string LikeEscapingStatement => "ESCAPE('\x10')";

        #endregion
    }
}
