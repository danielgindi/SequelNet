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

        public override string ST_Distance(string g1, string g2)
        {
            return "ST_Distance(" + g1 + ", " + g2 + ")";
        }

        public override string ST_GeomFromText(string text, string srid = null)
        {
            return "ST_GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override string ST_GeogFromText(string text, string srid = null)
        {
            return "ST_GeogFromText(" + PrepareValue(text) + ")";
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

        private string GeometryType(string type, int? srid = null)
        {
            return "GEOMETRY(" + type + (srid == null ? "" : (", " + srid.Value)) + ")";
        }

        public override void BuildColumnPropertiesDataType(
            StringBuilder sb,
            ConnectorBase connection,
            TableSchema.Column column,
            Query relatedQuery,
            out bool isDefaultAllowed)
        {
            isDefaultAllowed = true;

            if (column.LiteralType != null && column.LiteralType.Length > 0)
            {
                sb.Append(column.LiteralType);
                return;
            }

            DataType dataType = column.ActualDataType;
            if (!column.AutoIncrement)
            {
                if (dataType == DataType.VarChar)
                {
                    if (column.MaxLength < 0)
                    {
                        sb.Append("VARCHAR");
                        sb.AppendFormat(@"({0})", VarCharMaxLength);
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
                        sb.AppendFormat(@"({0})", VarCharMaxLength);
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
                    sb.Append("BOOLEAN");
                else if (dataType == DataType.DateTime)
                    sb.Append("TIMESTAMP");
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
                    {
                        sb.Append("NUMERIC");
                    }
                }
                else if (dataType == DataType.Float)
                    sb.Append("FLOAT4");
                else if (dataType == DataType.Double)
                    sb.Append("FLOAT8");
                else if (dataType == DataType.Decimal)
                {
                    if (column.NumberPrecision > 0)
                    {
                        sb.Append("DECIMAL");
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append("DECIMAL");
                    }
                }
                else if (dataType == DataType.Money)
                {
                    if (column.NumberPrecision > 0)
                    {
                        sb.Append("DECIMAL");
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append("DECIMAL");
                    }
                }
                else if (dataType == DataType.TinyInt)
                    sb.Append("SMALLINT");
                else if (dataType == DataType.UnsignedTinyInt)
                    sb.Append("SMALLINT");
                else if (dataType == DataType.SmallInt)
                    sb.Append("SMALLINT");
                else if (dataType == DataType.UnsignedSmallInt)
                    sb.Append("SMALLINT");
                else if (dataType == DataType.Int)
                    sb.Append("SMALLINT");
                else if (dataType == DataType.UnsignedInt)
                    sb.Append("INTEGER");
                else if (dataType == DataType.BigInt)
                    sb.Append("BIGINT");
                else if (dataType == DataType.UnsignedBigInt)
                    sb.Append("BIGINT");
                else if (dataType == DataType.Json)
                    sb.Append("JSON");
                else if (dataType == DataType.JsonBinary)
                    sb.Append("JSONB");
                else if (dataType == DataType.Blob)
                    sb.Append("BYTEA");
                else if (dataType == DataType.Guid)
                    sb.Append("UUID");
                else if (dataType == DataType.Geometry)
                    sb.Append(GeometryType("GEOMETRY", column.SRID));
                else if (dataType == DataType.GeometryCollection)
                    sb.Append(GeometryType("GEOMETRYCOLLECTION", column.SRID));
                else if (dataType == DataType.Point)
                    sb.Append(GeometryType("POINT", column.SRID));
                else if (dataType == DataType.LineString)
                    sb.Append(GeometryType("LINESTRING", column.SRID));
                else if (dataType == DataType.Polygon)
                    sb.Append(GeometryType("POLYGON", column.SRID));
                else if (dataType == DataType.Line)
                    sb.Append(GeometryType("LINE", column.SRID));
                else if (dataType == DataType.Curve)
                    sb.Append(GeometryType("CURVE", column.SRID));
                else if (dataType == DataType.Surface)
                    sb.Append(GeometryType("SURFACE", column.SRID));
                else if (dataType == DataType.LinearRing)
                    sb.Append(GeometryType("LINEARRING", column.SRID));
                else if (dataType == DataType.MultiPoint)
                    sb.Append(GeometryType("MULTIPOINT", column.SRID));
                else if (dataType == DataType.MultiLineString)
                    sb.Append(GeometryType("MULTILINESTRING", column.SRID));
                else if (dataType == DataType.MultiPolygon)
                    sb.Append(GeometryType("MULTIPOLYGON", column.SRID));
                else if (dataType == DataType.MultiCurve)
                    sb.Append(GeometryType("MULTICURVE", column.SRID));
                else if (dataType == DataType.MultiSurface)
                    sb.Append(GeometryType("MULTISURFACE", column.SRID));
                else if (dataType == DataType.Geographic)
                    sb.Append(GeometryType("GEOMETRY"));
                else if (dataType == DataType.GeographicCollection)
                    sb.Append(GeometryType("GEOMETRYCOLLECTION"));
                else if (dataType == DataType.GeographicPoint)
                    sb.Append(GeometryType("POINT"));
                else if (dataType == DataType.GeographicLineString)
                    sb.Append(GeometryType("LINESTRING"));
                else if (dataType == DataType.GeographicPolygon)
                    sb.Append(GeometryType("POLYGON"));
                else if (dataType == DataType.GeographicLine)
                    sb.Append(GeometryType("LINE"));
                else if (dataType == DataType.GeographicCurve)
                    sb.Append(GeometryType("CURVE"));
                else if (dataType == DataType.GeographicSurface)
                    sb.Append(GeometryType("SURFACE"));
                else if (dataType == DataType.GeographicLinearRing)
                    sb.Append(GeometryType("LINEARRING"));
                else if (dataType == DataType.GeographicMultiPoint)
                    sb.Append(GeometryType("MULTIPOINT"));
                else if (dataType == DataType.GeographicMultiLineString)
                    sb.Append(GeometryType("MULTILINESTRING"));
                else if (dataType == DataType.GeographicMultiPolygon)
                    sb.Append(GeometryType("MULTIPOLYGON"));
                else if (dataType == DataType.GeographicMultiCurve)
                    sb.Append(GeometryType("MULTICURVE"));
                else if (dataType == DataType.GeographicMultiSurface)
                    sb.Append(GeometryType("MULTISURFACE"));
                else throw new NotImplementedException("Unsupprted data type " + dataType.ToString());
            }
            else
            {
                sb.Append(' ');

                if (dataType == DataType.BigInt || dataType == DataType.UnsignedBigInt)
                {
                    sb.Append("BIGSERIAL");
                }
                else
                {
                    sb.Append("SERIAL");
                }
            }

            if (column.ComputedColumn != null)
            {
                sb.Append(" AS ");

                sb.Append(column.ComputedColumn.Build(connection, relatedQuery));

                if (column.ComputedColumnStored)
                {
                    sb.Append(" STORED");
                }
            }

            if (!string.IsNullOrEmpty(column.Charset))
            {
                sb.Append(@" CHARACTER SET");
                sb.Append(column.Charset);
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
