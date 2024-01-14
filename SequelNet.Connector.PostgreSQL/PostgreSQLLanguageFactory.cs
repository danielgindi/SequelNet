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

        public override bool InsertSupportsOnConflictDoNothing => true;
        public override bool InsertSupportsOnConflictDoUpdate => true;

        public override bool HasSeparateRenameColumn => true;
        public override Func<TableSchema.Index, bool> HasSeparateCreateIndex => (index) => index.Mode != TableSchema.IndexMode.PrimaryKey;

        public override int VarCharMaxLength => 357913937;

        public override string UtcNow()
        {
            return @"now() at time zone 'utc'";
        }

        public override void BuildConvertUtcToTz(
            ValueWrapper value, ValueWrapper timeZone,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            value.Build(sb, conn, relatedQuery);
            sb.Append(" AT TIME ZONE ");
            timeZone.Build(sb, conn, relatedQuery);
        }

        public override string YearPartOfDateTime(string date)
        {
            return $"EXTRACT(YEAR FROM {date})";
        }

        public override string MonthPartOfDateTime(string date)
        {
            return $"EXTRACT(MONTH FROM {date})";
        }

        public override string DayPartOfDateTime(string date)
        {
            return $"EXTRACT(DAY FROM {date})";
        }

        public override string HourPartOfDateOrTime(string date)
        {
            return $"EXTRACT(HOUR FROM {date})";
        }

        public override string MinutePartOfDateOrTime(string date)
        {
            return $"EXTRACT(MINUTE FROM {date})";
        }

        public override string SecondPartOfDateOrTime(string date)
        {
            return $"EXTRACT(SECOND FROM {date})";
        }

        public override string DatePartOfDateTime(string dateTime)
        {
            return dateTime + "::DATE";
        }

        public override string TimePartOfDateTime(string dateTime)
        {
            return dateTime + "::TIME";
        }

        public override string ExtractUnixTimestamp(string date)
        {
            return $"EXTRACT(epoch FROM {date})";
        }

        public override string DateTimeFormat(string date, Phrases.DateTimeFormat.FormatOptions format)
        {
            switch (format)
            {
                case Phrases.DateTimeFormat.FormatOptions.IsoDateTime:
                    return $"to_char ({date}, 'YYYY-MM-DD\"T\"HH24:MI:SS')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeFFF:
                    return $"to_char ({date}, 'YYYY-MM-DD\"T\"HH24:MI:SS.MS')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeZ:
                    return $"to_char ({date}::timestamptz at time zone 'UTC', 'YYYY-MM-DD\"T\"HH24:MI:SS\"Z\"')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeFFFZ:
                    return $"to_char ({date}::timestamptz at time zone 'UTC', 'YYYY-MM-DD\"T\"HH24:MI:SS.MS\"Z\"')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDate:
                    return $"to_char ({date}, 'YYYY-MM-DD";

                case Phrases.DateTimeFormat.FormatOptions.IsoTime:
                    return $"to_char ({date}, 'HH24:MI:SS";

                case Phrases.DateTimeFormat.FormatOptions.IsoTimeFFF:
                    return $"to_char ({date}, 'HH24:MI:SS.MS";

                case Phrases.DateTimeFormat.FormatOptions.IsoYearMonth:
                    return $"to_char ({date}, 'YYYY-MM";

                default:
                    throw new NotImplementedException($"DateTimeFormat with format {format} has not been implemented for this connector");
            }
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

        public override string ST_Distance_Sphere(string g1, string g2)
        {
            return "ST_Distance_Sphere(" + g1 + ", " + g2 + ")";
        }

        public override string ST_GeomFromText(string text, string srid = null, bool literalText = false)
        {
            if (!literalText)
                text = PrepareValue(text);

            return "ST_GeomFromText(" + text + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override string ST_GeogFromText(string text, string srid = null, bool literalText = false)
        {
            if (!literalText)
                text = PrepareValue(text);

            return "ST_GeogFromText(" + text + ")";
        }

        public override void BuildConflictColumnUpdate(
            StringBuilder sb, ConnectorBase conn,
            ConflictColumn column, Query relatedQuery)
        {
            sb.Append("EXCLUDED." + WrapFieldName(column.Column));
        }

        public override void BuildOnConflictDoUpdate(StringBuilder outputBuilder, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
        {
            outputBuilder.Append(" ON CONFLICT ");

            if (conflict.OnColumn != null)
            {
                outputBuilder.Append($" ({conn.Language.WrapFieldName(conflict.OnColumn)}) ");
            }
            else if (conflict.OnConstraint != null)
            {
                outputBuilder.Append($" ON CONSTRAINT ({conn.Language.WrapFieldName(conflict.OnConstraint)}) ");
            }
            else
            {
                var idx = relatedQuery.Schema.Indexes.Find(x => x.Mode == TableSchema.IndexMode.PrimaryKey);
                if (idx != null)
                {
                    outputBuilder.Append($" ON CONSTRAINT ({conn.Language.WrapFieldName(idx.Name)}) ");
                }
                else
                {
                    outputBuilder.Append($" ON CONSTRAINT ({conn.Language.WrapFieldName(relatedQuery.SchemaName + "_pkey")}) ");
                }
            }

            outputBuilder.Append(") DO UPDATE SET ");

            bool first = true;
            foreach (var set in conflict.Updates)
            {
                if (first) first = false;
                else outputBuilder.Append(",");

                outputBuilder.Append(conn.Language.WrapFieldName(set.ColumnName));
                outputBuilder.Append("=");

                set.BuildSecond(outputBuilder, conn, relatedQuery);
            }
        }

        public override void BuildOnConflictDoNothing(StringBuilder outputBuilder, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
        {
            outputBuilder.Append(" ON CONFLICT ");

            if (conflict.OnColumn != null)
            {
                outputBuilder.Append($" ({conn.Language.WrapFieldName(conflict.OnColumn)}) ");
            }
            else if (conflict.OnConstraint != null)
            {
                outputBuilder.Append($" ON CONSTRAINT ({conn.Language.WrapFieldName(conflict.OnConstraint)}) ");
            }
            else
            {
                var idx = relatedQuery.Schema.Indexes.Find(x => x.Mode == TableSchema.IndexMode.PrimaryKey);
                if (idx != null)
                {
                    outputBuilder.Append($" ON CONSTRAINT ({conn.Language.WrapFieldName(idx.Name)}) ");
                }
                else
                {
                    outputBuilder.Append($" ON CONSTRAINT ({conn.Language.WrapFieldName(relatedQuery.SchemaName + "_pkey")}) ");
                }
            }

            outputBuilder.Append(") DO NOTHING");
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
            if (top || query.QueryMode == QueryMode.Update)
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
            TableSchema.Index index,
            StringBuilder outputBuilder,
            Query qry,
            ConnectorBase conn)
        {
            if (index.Mode == TableSchema.IndexMode.PrimaryKey)
            {
                outputBuilder.Append(WrapFieldName(index.Name));
                outputBuilder.Append(@" PRIMARY KEY ");

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
                for (int i = 0; i < index.Columns.Length; i++)
                {
                    if (i > 0) outputBuilder.Append(",");

                    var column = index.Columns[i];
                    column.Target.Build(outputBuilder, conn, qry);

                    if (column.Sort != null)
                        outputBuilder.Append(column.Sort == SortDirection.ASC ? " ASC" : " DESC");
                }
                outputBuilder.Append(@");");
            }
        }

        private string GeometryType(string type, int? srid = null)
        {
            return "GEOMETRY(" + type + (srid == null ? "" : (", " + srid.Value)) + ")";
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

            if (!column.AutoIncrement)
            {
                var (dataTypeString, isDefaultAllowedResult) = BuildDataTypeDef(column.DataTypeDef);

                if (string.IsNullOrEmpty(dataTypeString))
                {
                    throw new NotImplementedException("Unsupprted data type " + column.ActualDataType.ToString());
                }

                isDefaultAllowed = isDefaultAllowedResult;

                sb.Append(dataTypeString);
            }
            else
            {
                sb.Append(' ');

                var dataType = column.ActualDataType;
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

                sb.Append(column.ComputedColumn?.Build(connection, relatedQuery));

                if (column.ComputedColumnStored)
                {
                    sb.Append(" STORED");
                }
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
                        typeString = $"VARCHAR({VarCharMaxLength})";
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
                        typeString = $"VARCHAR({VarCharMaxLength})";
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
                    typeString = "BOOLEAN";
                    break;
                case DataType.DateTime:
                    typeString = "TIMESTAMP";
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
                    typeString = "FLOAT4";
                    break;
                case DataType.Double:
                    typeString = "FLOAT8";
                    break;
                case DataType.Decimal:
                    if (typeDef.Precision > 0)
                        typeString = $"DECIMAL({typeDef.Precision}, {typeDef.Scale})";
                    else
                        typeString = "DECIMAL";
                    break;
                case DataType.Money:
                    typeString = "MONEY";
                    break;
                case DataType.TinyInt:
                    typeString = "SMALLINT";
                    break;
                case DataType.UnsignedTinyInt:
                    typeString = "SMALLINT";
                    break;
                case DataType.SmallInt:
                    typeString = "SMALLINT";
                    break;
                case DataType.UnsignedSmallInt:
                    typeString = "SMALLINT";
                    break;
                case DataType.Int:
                    typeString = "INTEGER";
                    break;
                case DataType.UnsignedInt:
                    typeString = "INTEGER";
                    break;
                case DataType.BigInt:
                    typeString = "BIGINT";
                    break;
                case DataType.UnsignedBigInt:
                    typeString = "BIGINT";
                    break;
                case DataType.Json:
                    typeString = "JSON";
                    break;
                case DataType.JsonBinary:
                    typeString = "JSONB";
                    break;
                case DataType.Blob:
                    typeString = "BYTEA";
                    break;
                case DataType.Guid:
                    typeString = "UUID";
                    break;
                case DataType.Geometry:
                    typeString = GeometryType("GEOMETRY", typeDef.SRID);
                    break;
                case DataType.GeometryCollection:
                    typeString = GeometryType("GEOMETRYCOLLECTION", typeDef.SRID);
                    break;
                case DataType.Point:
                    typeString = GeometryType("POINT", typeDef.SRID);
                    break;
                case DataType.LineString:
                    typeString = GeometryType("LINESTRING", typeDef.SRID);
                    break;
                case DataType.Polygon:
                    typeString = GeometryType("POLYGON", typeDef.SRID);
                    break;
                case DataType.Line:
                    typeString = GeometryType("LINE", typeDef.SRID);
                    break;
                case DataType.Curve:
                    typeString = GeometryType("CURVE", typeDef.SRID);
                    break;
                case DataType.Surface:
                    typeString = GeometryType("SURFACE", typeDef.SRID);
                    break;
                case DataType.LinearRing:
                    typeString = GeometryType("LINEARRING", typeDef.SRID);
                    break;
                case DataType.MultiPoint:
                    typeString = GeometryType("MULTIPOINT", typeDef.SRID);
                    break;
                case DataType.MultiLineString:
                    typeString = GeometryType("MULTILINESTRING", typeDef.SRID);
                    break;
                case DataType.MultiPolygon:
                    typeString = GeometryType("MULTIPOLYGON", typeDef.SRID);
                    break;
                case DataType.MultiCurve:
                    typeString = GeometryType("MULTICURVE", typeDef.SRID);
                    break;
                case DataType.MultiSurface:
                    typeString = GeometryType("MULTISURFACE", typeDef.SRID);
                    break;
                case DataType.Geographic:
                    typeString = GeometryType("GEOMETRY");
                    break;
                case DataType.GeographicCollection:
                    typeString = GeometryType("GEOMETRYCOLLECTION");
                    break;
                case DataType.GeographicPoint:
                    typeString = GeometryType("POINT");
                    break;
                case DataType.GeographicLineString:
                    typeString = GeometryType("LINESTRING");
                    break;
                case DataType.GeographicPolygon:
                    typeString = GeometryType("POLYGON");
                    break;
                case DataType.GeographicLine:
                    typeString = GeometryType("LINE");
                    break;
                case DataType.GeographicCurve:
                    typeString = GeometryType("CURVE");
                    break;
                case DataType.GeographicSurface:
                    typeString = GeometryType("SURFACE");
                    break;
                case DataType.GeographicLinearRing:
                    typeString = GeometryType("LINEARRING");
                    break;
                case DataType.GeographicMultiPoint:
                    typeString = GeometryType("MULTIPOINT");
                    break;
                case DataType.GeographicMultiLineString:
                    typeString = GeometryType("MULTILINESTRING");
                    break;
                case DataType.GeographicMultiPolygon:
                    typeString = GeometryType("MULTIPOLYGON");
                    break;
                case DataType.GeographicMultiCurve:
                    typeString = GeometryType("MULTICURVE");
                    break;
                case DataType.GeographicMultiSurface:
                    typeString = GeometryType("MULTISURFACE");
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
            sb.Append(PrepareValue(collation));

            // COLLATE ASC/DESC not supported in PG

            sb.Append(")");
        }

        public override void BuildOrderByRandom(ValueWrapper? seedValue, ConnectorBase conn, StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"RANDOM()");
        }

        public override void BuildJsonExtract(
            ValueWrapper value, string path, bool unquote,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            // No support for returning "self". Postgres works with actual json Objects.
            var parts = JsonPathValue.GetPathParts(path);
            if (parts.Count > 0 && parts[0] == "$")
            {
                parts.RemoveAt(0);
            }

            var pgPath = "";
            foreach (var part in parts)
            {
                if (pgPath.Length > 0)
                    pgPath += $", {PrepareValue(part)}";
                else pgPath += PrepareValue(part);
            }

            sb.Append("json_extract_path_text(");
            value.Build(sb, conn, relatedQuery);
            sb.Append($", {pgPath}");
            sb.Append(")");
        }

        public override void BuildJsonContains(
            ValueWrapper target, ValueWrapper candidate, string path,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            sb.Append("(");

            bool hasPath = false;

            if (!string.IsNullOrEmpty(path))
            {
                var parts = JsonPathValue.GetPathParts(path);
                if (parts.Count > 0 && parts[0] == "$")
                {
                    parts.RemoveAt(0);

                    var pgPath = "";
                    foreach (var part in parts)
                    {
                        if (pgPath.Length > 0)
                            pgPath += $", {PrepareValue(part)}";
                        else pgPath += PrepareValue(part);
                    }

                    sb.Append("json_extract_path(");
                    target.Build(sb, conn, relatedQuery);
                    sb.Append($", {pgPath}");
                    sb.Append(")");

                    hasPath = true;
                }
            }

            if (!hasPath)
                target.Build(sb, conn, relatedQuery);

            sb.Append(" @> ");

            candidate.Build(sb, conn, relatedQuery);
            sb.Append(")");
        }

        public override void BuildJsonExtractValue(
            ValueWrapper value, string path,
            DataTypeDef returnType,
            Phrases.JsonValue.DefaultAction onEmptyAction, object onEmptyValue,
            Phrases.JsonValue.DefaultAction onErrorAction, object onErrorValue,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            // No support for returning "self". Postgres works with actual json Objects.
            var parts = JsonPathValue.GetPathParts(path);
            if (parts.Count > 0 && parts[0] == "$")
            {
                parts.RemoveAt(0);
            }

            var pgPath = "";
            foreach (var part in parts)
            {
                if (pgPath.Length > 0)
                    pgPath += ", " + PrepareValue(part);
                else pgPath += PrepareValue(part);
            }

            if (returnType != null)
            {
                var (typeString, _) = BuildDataTypeDef(returnType);
                if (typeString != null)
                {
                    sb.Append("json_extract_path_text(");
                    sb.Append(value.Build(conn, relatedQuery));
                    sb.Append($", {pgPath}");
                    sb.Append($")::{typeString}");
                    return;
                }
            }

            sb.Append("json_extract_path_text(");
            sb.Append(value.Build(conn, relatedQuery));
            sb.Append($", {pgPath}");
            sb.Append($")");
        }

        public override void BuildJsonArrayAggregate(
            ValueWrapper value, bool isBinary,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            if (isBinary)
                sb.Append("jsonb_agg(");
            else sb.Append("json_agg(");
            sb.Append(value.Build(conn, relatedQuery));
            sb.Append(")");
        }

        public override void BuildJsonObjectAggregate(
            ValueWrapper key, ValueWrapper value, bool isBinary,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            if (isBinary)
                sb.Append("jsonb_object_agg(");
            else sb.Append("json_object_agg(");
            sb.Append(key.Build(conn, relatedQuery));
            sb.Append(",");
            sb.Append(value.Build(conn, relatedQuery));
            sb.Append(")");
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

        public override void BuildSeparateRenameColumn(
            Query qry,
            ConnectorBase conn,
            AlterTableQueryData alterData,
            StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"ALTER TABLE ");

            BuildTableName(qry, conn, outputBuilder, false);

            outputBuilder.Append(@" RENAME COLUMN ");
            outputBuilder.Append(WrapFieldName(alterData.OldItemName));
            outputBuilder.Append(@" TO ");
            outputBuilder.Append(WrapFieldName(alterData.Column.Name));
            outputBuilder.Append(';');
        }

        public override void BuildChangeColumn(AlterTableQueryData alterData, StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            // Alter type
            sb.Append(WrapFieldName(alterData.Column.Name));
            sb.Append(@" TYPE ");
            BuildColumnPropertiesDataType(
                column: alterData.Column,
                isDefaultAllowed: out _,
                sb: sb,
                connection: conn,
                relatedQuery: relatedQuery);
            sb.Append(',');

            // Alter nullability
            sb.Append(ChangeColumnCommandName + " " + WrapFieldName(alterData.Column.Name));
            sb.Append(alterData.Column.Nullable ? " DROP NOT NULL," : " SET NOT NULL,");

            // Alter default
            if (alterData.Column.Default != null || alterData.Column.HasDefault)
            {
                sb.Append(ChangeColumnCommandName + " " + WrapFieldName(alterData.Column.Name));
                sb.Append(" SET DEFAULT ");

                if (alterData.Column.Default == null)
                {
                    sb.Append(@" DEFAULT NULL");
                }
                else
                {
                    sb.Append(@" DEFAULT ");
                    Query.PrepareColumnValue(alterData.Column, alterData.Column.Default, sb, conn, relatedQuery);
                }
            }
            else
            {
                sb.Append(" DROP DEFAULT ");
            }
        }

        public override string BuildFindString(
            ConnectorBase conn,
            ValueWrapper needle,
            ValueWrapper haystack,
            ValueWrapper? startAt,
            Query relatedQuery)
        {
            string ret = "POSITION(";

            ret += needle.Build(conn, relatedQuery);
            ret += " IN ";
            ret += haystack.Build(conn, relatedQuery);

            if (startAt != null)
            {
                throw new NotImplementedException("FindString (POSITION) with `startAt` argument is not supported in postgre");
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
            return value ? "TRUE" : "FALSE";
        }

        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }

        public override string FormatDateTime(DateTime dateTime)
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
