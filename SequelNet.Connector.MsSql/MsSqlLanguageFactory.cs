using SequelNet.Sql.Spatial;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SequelNet.Connector
{
    public class MsSqlLanguageFactory : LanguageFactory
    {
        public MsSqlLanguageFactory(MsSqlVersion sqlVersion)
        {
            _MsSqlVersion = sqlVersion;
        }

        private static readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;

        #region Versioning

        private MsSqlVersion _MsSqlVersion;

        #endregion

        #region Syntax

        public override bool UpdateFromInsteadOfJoin => false;
        public override bool UpdateJoinRequiresFromLeftTable => true;

        public override bool InsertSupportsMerge => true;

        public override bool HasSeparateRenameColumn => true;
        public override Func<TableSchema.Index, bool> HasSeparateCreateIndex => (index) => index.Mode != TableSchema.IndexMode.PrimaryKey;
        public override Func<AlterTableType, AlterTableType, bool> IsAlterTableTypeMixCompatible => (type1, type2) =>
        {
            // Same
            if (type1 == type2)
                return true;

            // Multiple ALTER TABLE ADD ...
            if (
                (type1 == AlterTableType.AddColumn || type1 == AlterTableType.CreateIndex || type1 == AlterTableType.CreateForeignKey) ==
                (type2 == AlterTableType.AddColumn || type2 == AlterTableType.CreateIndex || type2 == AlterTableType.CreateForeignKey)
                )
                return true;

            // Can't mix
            return false;
        };
        public override bool SupportsMultipleAlterColumn => false;
        public override bool SameAlterTableCommandsAreCommaSeparated => true;
        public override string AlterTableAddCommandName => "ADD";
        public override string AlterTableAddColumnCommandName => "";
        public override string AlterTableAddIndexCommandName => "CONSTRAINT";
        public override string AlterTableAddForeignKeyCommandName => "CONSTRAINT";
        public override string ChangeColumnCommandName => "ALTER COLUMN";

        public override int VarCharMaxLength => 4000;

        public override string UtcNow()
        {
            return @"GETUTCDATE()";
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

        public override string DatePartOfDateTime(string dateTime)
        {
            return "CAST(" + dateTime + " AS DATE)";
        }

        public override string TimePartOfDateTime(string dateTime)
        {
            return "CAST(" + dateTime + " AS TIME)";
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
                    return $"FORMAT({date}, 'yyyy-MM-dd\"T\"HH:mm:ss')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeFFF:
                    return $"FORMAT({date}, 'yyyy-MM-dd\"T\"HH:mm:ss.fff')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeZ:
                    return $"FORMAT({date}, 'yyyy-MM-dd\"T\"HH:mm:ss\"Z\"')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDateTimeFFFZ:
                    return $"FORMAT({date}, 'yyyy-MM-dd\"T\"HH:mm:ss.fff\"Z\"')";

                case Phrases.DateTimeFormat.FormatOptions.IsoDate:
                    return $"FORMAT({date}, 'yyyy-MM-dd')";

                case Phrases.DateTimeFormat.FormatOptions.IsoTime:
                    return $"FORMAT({date}, 'HH:mm:ss')";

                case Phrases.DateTimeFormat.FormatOptions.IsoTimeFFF:
                    return $"FORMAT({date}, 'HH:mm:ss.fff')";

                case Phrases.DateTimeFormat.FormatOptions.IsoYearMonth:
                    return $"to_char ({date}, 'yyyy-MM";

                default:
                    throw new NotImplementedException($"DateTimeFormat with format {format} has not been implemented for this connector");
            }
        }

        public override string Md5Hex(string value)
        {
            if (_MsSqlVersion.MajorVersion < 10)
            {
                return @"SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5', " + value + ")), 3, 32)";
            }
            else
            {
                return @"CONVERT(VARCHAR(32), HASHBYTES('MD5', " + value + "), 2)";
            }
        }

        public override string Sha1Hex(string value)
        {
            if (_MsSqlVersion.MajorVersion < 10)
            {
                return @"SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('SHA1', " + value + ")), 3, 32)";
            }
            else
            {
                return @"CONVERT(VARCHAR(32), HASHBYTES('SHA1', " + value + "), 2)";
            }
        }

        public override string Md5Binary(string value)
        {
            return @"HASHBYTES('MD5', " + value + ")";
        }

        public override string Sha1Binary(string value)
        {
            return @"HASHBYTES('SHA1', " + value + ")";
        }

        public override string LengthOfString(string value)
        {
            return @"LEN(" + value + ")";
        }

        public override string ST_X(string pt)
        {
            return pt + ".STX";
        }

        public override string ST_Y(string pt)
        {
            return pt + ".STY";
        }

        public override string ST_Contains(string g1, string g2)
        {
            return g1 + ".STContains(" + g2 + ")";
        }

        public override string ST_Distance(string g1, string g2)
        {
            return g1 + ".STDistance(" + g2 + ")";
        }

        public override string ST_GeomFromText(string text, string srid = null, bool literalText = false)
        {
            if (!literalText)
                text = PrepareValue(text);

            return "geometry::STGeomFromText(" + text + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override string ST_GeogFromText(string text, string srid = null, bool literalText = false)
        {
            if (!literalText)
                text = PrepareValue(text);

            return "geography::STGeomFromText(" + text + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override void BuildConflictColumnUpdate(
            StringBuilder sb, ConnectorBase conn,
            ConflictColumn column, Query relatedQuery)
        {
            var assignemnt = relatedQuery.GetInsertUpdateList().FirstOrDefault(x => x.ColumnName == column.Column);
            if (assignemnt != null)
            {
                assignemnt.BuildSecond(sb, conn, relatedQuery);
            }
            else
            {
                sb.Append(PrepareValue(conn, relatedQuery.Schema.Columns.Find(column.Column).Default, relatedQuery));
            }
        }

        public override void BuildOnConflictSetMerge(StringBuilder sb, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
        {
            var language = conn.Language;
            var inserts = relatedQuery.GetInsertUpdateList();

            sb.Append("MERGE INTO ");
            sb.Append(language.WrapFieldName(relatedQuery.SchemaName));

            if (relatedQuery.SchemaAlias != null)
            {
                sb.Append(" AS ");
                sb.Append(language.WrapFieldName(relatedQuery.SchemaAlias));
            }

            sb.Append(" ON (");

            var schema = relatedQuery.Schema;
            bool first = false;
            var cols = new HashSet<ValueWrapper>(schema.Indexes
                .Where(x => x.Mode == TableSchema.IndexMode.PrimaryKey || x.Mode == TableSchema.IndexMode.Unique)
                .SelectMany(x => x.Columns.Select(c => c.Target))
                .Concat(schema.Columns.Where(x => x.IsPrimaryKey).Select(x => ValueWrapper.Column(x.Name)))
            );

            if (cols.Any(x => x.Type != ValueObjectType.ColumnName))
            {
                throw new NotSupportedException("MSSQL's MERGE INTO is not supported with functional indices");
            }

            foreach (var col in cols)
            {
                var assignemnt = inserts.FirstOrDefault(x => x.ColumnName == (string)col.Value);
                if (assignemnt == null) continue;

                if (first) first = false;
                else sb.Append(" AND ");

                if (relatedQuery.SchemaAlias != null)
                {
                    sb.Append(language.WrapFieldName(relatedQuery.SchemaAlias));
                }
                else
                {
                    sb.Append(language.WrapFieldName(relatedQuery.SchemaName));
                }

                sb.Append(".");
                sb.Append(language.WrapFieldName((string)col.Value));

                if (assignemnt.Second == null)
                    sb.Append(" IS ");
                else sb.Append("=");
                assignemnt.BuildSecond(sb, conn, relatedQuery);
            }

            sb.Append(") ");
            sb.Append("WHEN MATCHED THEN UPDATE SET ");

            first = true;
            foreach (var set in conflict.Updates)
            {
                if (first) first = false;
                else sb.Append(",");

                sb.Append(language.WrapFieldName(set.ColumnName));
                sb.Append("=");

                set.BuildSecond(sb, conn, relatedQuery);
            }

            sb.Append(" WHEN NOT MATCHED THEN INSERT (");
            first = true;
            foreach (AssignmentColumn ins in inserts)
            {
                if (first) first = false;
                else sb.Append(',');
                sb.Append(language.WrapFieldName(ins.ColumnName));
            }

            sb.Append(") VALUES (");
            first = true;
            foreach (AssignmentColumn ins in inserts)
            {
                if (first) first = false;
                else sb.Append(',');

                ins.BuildSecond(sb, conn, relatedQuery);
            }
            sb.Append(")");
        }

        public override void BuildLimitOffset(
            Query query,
            bool top,
            StringBuilder outputBuilder)
        {
            var withOffset = query.Offset > 0 && query.QueryMode == QueryMode.Select && _MsSqlVersion.SupportsOffset;

            if (top)
            {
                if (!withOffset && query.Limit > 0)
                {
                    outputBuilder.Append(" TOP ");
                    outputBuilder.Append(query.Limit);
                    outputBuilder.Append(' ');
                }
            }
            else
            {
                if (withOffset && query.Limit > 0)
                {
                    outputBuilder.Append(" OFFSET ");
                    outputBuilder.Append(query.Offset);
                    outputBuilder.Append(" ROWS ");

                    if (query.Limit > 0)
                    {
                        outputBuilder.Append("FETCH NEXT ");
                        outputBuilder.Append(query.Limit);
                        outputBuilder.Append(" ROWS ONLY ");
                    }
                }
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

                if (index.Cluster == TableSchema.ClusterMode.Clustered) outputBuilder.Append(@"CLUSTERED ");
                else if (index.Cluster == TableSchema.ClusterMode.NonClustered) outputBuilder.Append(@"NONCLUSTERED ");

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
                if (index.Cluster == TableSchema.ClusterMode.Clustered) outputBuilder.Append(@"CLUSTERED ");
                else if (index.Cluster == TableSchema.ClusterMode.NonClustered) outputBuilder.Append(@"NONCLUSTERED ");

                outputBuilder.Append(@"INDEX ");
                outputBuilder.Append(WrapFieldName(index.Name));

                outputBuilder.Append(@"ON ");

                BuildTableName(qry, conn, outputBuilder, false);

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
                        typeString = "NVARCHAR(MAX)";
                    else if (typeDef.MaxLength == 0)
                        typeString = "NTEXT";
                    else if (typeDef.MaxLength <= VarCharMaxLength)
                        typeString = $"NVARCHAR({typeDef.MaxLength})";
                    else if (typeDef.MaxLength < 65536)
                        typeString = "NTEXT";
                    else if (typeDef.MaxLength < 16777215)
                        typeString = "NTEXT";
                    else
                        typeString = "NTEXT";
                    break;

                case DataType.Char:
                    if (typeDef.MaxLength < 0)
                        typeString = "NVARCHAR(MAX)";
                    else if (typeDef.MaxLength == 0 || typeDef.MaxLength >= VarCharMaxLength)
                        typeString = $"NVARCHAR({VarCharMaxLength})";
                    else
                        typeString = $"NVARCHAR({typeDef.MaxLength})";
                    break;

                case DataType.Text:
                    typeString = "NTEXT";
                    break;
                case DataType.MediumText:
                    typeString = "NTEXT";
                    break;
                case DataType.LongText:
                    typeString = "NTEXT";
                    break;
                case DataType.Boolean:
                    typeString = "bit";
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
                    typeString = "FLOAT(4)";
                    break;

                case DataType.Double:
                    typeString = "FLOAT(8)";
                    break;

                case DataType.Decimal:
                    if (typeDef.Precision > 0)
                        typeString = $"DECIMAL({typeDef.Precision}, {typeDef.Scale})";
                    else
                        typeString = "DECIMAL";
                    break;

                case DataType.Money:
                    if (typeDef.Precision > 0)
                        typeString = $"MONEY({typeDef.Precision}, {typeDef.Scale})";
                    else
                        typeString = "MONEY";
                    break;
                case DataType.TinyInt:
                    typeString = "TINYINT";
                    break;
                case DataType.UnsignedTinyInt:
                    typeString = "TINYINT";
                    break;
                case DataType.SmallInt:
                    typeString = "SMALLINT";
                    break;
                case DataType.UnsignedSmallInt:
                    typeString = "SMALLINT";
                    break;
                case DataType.Int:
                    typeString = "INT";
                    break;
                case DataType.UnsignedInt:
                    typeString = "INT";
                    break;
                case DataType.BigInt:
                    typeString = "BIGINT";
                    break;
                case DataType.UnsignedBigInt:
                    typeString = "BIGINT";
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
                case DataType.Geometry:
                    typeString = "GEOMETRY";
                    break;
                case DataType.GeometryCollection:
                    typeString = "GEOMETRY";
                    break;
                case DataType.Point:
                    typeString = "GEOMETRY";
                    break;
                case DataType.LineString:
                    typeString = "GEOMETRY";
                    break;
                case DataType.Polygon:
                    typeString = "GEOMETRY";
                    break;
                case DataType.Line:
                    typeString = "GEOMETRY";
                    break;
                case DataType.Curve:
                    typeString = "GEOMETRY";
                    break;
                case DataType.Surface:
                    typeString = "GEOMETRY";
                    break;
                case DataType.LinearRing:
                    typeString = "GEOMETRY";
                    break;
                case DataType.MultiPoint:
                    typeString = "GEOMETRY";
                    break;
                case DataType.MultiLineString:
                    typeString = "GEOMETRY";
                    break;
                case DataType.MultiPolygon:
                    typeString = "GEOMETRY";
                    break;
                case DataType.MultiCurve:
                    typeString = "GEOMETRY";
                    break;
                case DataType.MultiSurface:
                    typeString = "GEOMETRY";
                    break;
                case DataType.Geographic:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicCollection:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicPoint:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicLineString:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicPolygon:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicLine:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicCurve:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicSurface:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicLinearRing:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicMultiPoint:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicMultiLineString:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicMultiPolygon:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicMultiCurve:
                    typeString = "GEOGRAPHIC";
                    break;
                case DataType.GeographicMultiSurface:
                    typeString = "GEOGRAPHIC";
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
            outputBuilder.Append(@"NEWID()");
        }

        public override void BuildJsonExtract(
            ValueWrapper value, JsonPathExpression path, bool unquote,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            sb.Append("JSON_VALUE(");
            value.Build(sb, conn, relatedQuery);
            sb.Append(", ");
            path.GetPath().Build(sb, conn, relatedQuery);
            sb.Append(")");
        }

        public override void BuildJsonExtractValue(
            ValueWrapper value, JsonPathExpression path,
            DataTypeDef returnType,
            Phrases.JsonValue.DefaultAction onEmptyAction, object onEmptyValue,
            Phrases.JsonValue.DefaultAction onErrorAction, object onErrorValue,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            if (returnType != null)
            {
                var (typeString, _) = BuildDataTypeDef(returnType);
                if (typeString != null)
                {
                    sb.Append("CAST(JSON_VALUE(");
                    value.Build(sb, conn, relatedQuery);
                    sb.Append(", ");
                    path.GetPath().Build(sb, conn, relatedQuery);
                    sb.Append($") AS {typeString})");
                    return;
                }
            }

            sb.Append("JSON_VALUE(");
            value.Build(sb, conn, relatedQuery);
            sb.Append(", ");
            path.GetPath().Build(sb, conn, relatedQuery);
            sb.Append(")");
        }

        public override void BuildSeparateRenameColumn(
            Query qry,
            ConnectorBase conn,
            AlterTableQueryData alterData,
            StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"EXEC sp_rename ");

            BuildTableName(qry, conn, outputBuilder, false);

            outputBuilder.Append('.');
            outputBuilder.Append(WrapFieldName(alterData.OldItemName));
            outputBuilder.Append(',');
            outputBuilder.Append(WrapFieldName(alterData.Column.Name));
            outputBuilder.Append(@",'COLUMN';");
        }

        public override string Aggregate_Some(string rawExpression)
        {
            return $"(SUM({rawExpression}) > 0)";
        }

        public override string Aggregate_Every(string rawExpression)
        {
            return $"(COUNT({rawExpression}) = COUNT(*))";
        }

        public override void BuildChangeColumn(AlterTableQueryData alterData, StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            // TODO: Find another way of specifying DEFAULT / removing DEFAULT value accordingly for MSSQL

            BuildColumnProperties(alterData.Column, true, sb, conn, relatedQuery);
        }

        public override string BuildFindString(
            ConnectorBase conn,
            ValueWrapper needle,
            ValueWrapper haystack,
            ValueWrapper? startAt,
            Query relatedQuery)
        {
            string ret = "CHARINDEX(";

            ret += needle.Build(conn, relatedQuery);
            ret += ",";
            ret += haystack.Build(conn, relatedQuery);

            if (startAt != null)
            {
                ret += ",";
                ret += startAt.Value.Build(conn, relatedQuery);
            }

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
            string ret = "SUBSTRING(";

            ret += value.Build(conn, relatedQuery);

            ret += ", " + from.Build(conn, relatedQuery);

            if (length != null)
            {
                ret += ", " + length.Value.Build(conn, relatedQuery);
            }
            else
            {
                throw new NotImplementedException("MSSQL does not allow omitting the `length` argument of SUBSTRING");
            }

            ret += ")";

            return ret;
        }

        public override void BuildIndexHints(
            IndexHintList hints,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            if ((hints?.Count ?? 0) == 0)
                return;
            
            sb.Append(" WITH(");
            
            var isFirstHint = true;
            foreach (var hint in hints)
            {
                if (!isFirstHint) sb.Append(",");
                else isFirstHint = false;

                sb.Append("INDEX(");
                var isFirstIndex = true;
                foreach (var index in hint.IndexNames)
                {
                    if (!isFirstIndex) sb.Append(",");
                    else isFirstIndex = false;

                    sb.Append(WrapFieldName(index));
                }
                sb.Append(")");
            }

            sb.Append(")");
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
        { // Note: For performance, ignoring enclosed [] signs
            return '[' + fieldName + ']';
        }

        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }
        
        public override string PrepareValue(string value)
        {
            return @"N'" + EscapeString(value) + '\'';
        }
        
        public override string FormatDateTime(DateTime dateTime)
        {
            return "CAST(" + dateTime.ToString(@"yyyy-MM-dd HH:mm:ss.fff") + " AS DATETIME)";
        }
        
        public override string FormatCreateDate(int year, int month, int day)
        {
            return "CAST('" +
                year.ToString(DefaultCulture).PadLeft(4, '0') + '-' +
                month.ToString(DefaultCulture).PadLeft(2, '0') + '-' +
                day.ToString(DefaultCulture).PadLeft(2, '0') + "' AS DATE)";
        }
        
        public override string FormatCreateTime(int hours, int minutes, int seconds, int milliseconds)
        {
            return "CAST('" +
                hours.ToString(DefaultCulture).PadLeft(2, '0') + ':' +
                minutes.ToString(DefaultCulture).PadLeft(2, '0') + ':' +
                seconds.ToString(DefaultCulture).PadLeft(2, '0') + '.' +
                milliseconds.ToString(DefaultCulture).PadLeft(3, '0') + "' AS TIME)";
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%").Replace(@"_", @"\_");
        }

        #endregion
    }
}
