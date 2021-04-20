using SequelNet.Sql.Spatial;
using System;
using System.Text;
using System.Linq;

namespace SequelNet.Connector
{
    public class MySqlLanguageFactory : LanguageFactory
    {
        public MySqlLanguageFactory(MySqlMode mySqlMode)
        {
            _MySqlMode = mySqlMode;
        }

        #region Versioning

        private MySqlMode _MySqlMode;

        bool? _Is5_0_3OrLater = null;
        private bool Is5_0_3OrLater()
        {
            if (_Is5_0_3OrLater == null)
            {
                _Is5_0_3OrLater = _MySqlMode.Version.CompareTo("5.0.3") >= 0;
            }
            return _Is5_0_3OrLater.Value;
        }

        bool? _Is5_7OrLater = null;
        private bool Is5_7OrLater()
        {
            if (_Is5_7OrLater == null)
            {
                _Is5_7OrLater = _MySqlMode.Version.CompareTo("5.7") >= 0;
            }
            return _Is5_7OrLater.Value;
        }

        bool? _Is8_0OrLater = null;
        private bool Is8_0OrLater()
        {
            if (_Is8_0OrLater == null)
            {
                _Is8_0OrLater = _MySqlMode.Version.CompareTo("8.0") >= 0;
            }
            return _Is8_0OrLater.Value;
        }

        bool? _Is8_0_17OrLater = null;
        private bool Is8_0_17OrLater()
        {
            if (_Is8_0_17OrLater == null)
            {
                _Is8_0_17OrLater = _MySqlMode.Version.CompareTo("8.0.17") >= 0;
            }
            return _Is8_0_17OrLater.Value;
        }

        bool? _Is8_0_21OrLater = null;
        private bool Is8_0_21OrLater()
        {
            if (_Is8_0_21OrLater == null)
            {
                _Is8_0_21OrLater = _MySqlMode.Version.CompareTo("8.0.21") >= 0;
            }
            return _Is8_0_21OrLater.Value;
        }

        #endregion

        #region Syntax

        public override Func<Query, ConnectorBase, Exception, int> OnExecuteNonQueryException => (qry, conn, ex) =>
        {
            if (ex is MySql.Data.MySqlClient.MySqlException myex &&
                myex.Code == 1054 &&
                qry.QueryMode == QueryMode.AlterTable &&
                qry.AlterTableSteps.Any(x =>
                    (x.Type == AlterTableType.AddColumn || x.Type == AlterTableType.ChangeColumn) &&
                    !x.IgnoreColumnPosition) == true)
            {
                for (int i = 0; i < qry.AlterTableSteps.Count; i++)
                {
                    var step = qry.AlterTableSteps[i];

                    if (step.Type == AlterTableType.AddColumn || step.Type == AlterTableType.ChangeColumn)
                    {
                        step.IgnoreColumnPosition = true;

                        qry.AlterTableSteps[i] = step;
                    }
                }

                return qry.Execute(conn);
            }

            throw ex;
        };

        public override Func<Query, ConnectorBase, Exception, System.Threading.Tasks.Task<int>> OnExecuteNonQueryExceptionAsync => (qry, conn, ex) =>
        {
            if (ex is MySql.Data.MySqlClient.MySqlException myex &&
                myex.Code == 1054 &&
                qry.QueryMode == QueryMode.AlterTable &&
                qry.AlterTableSteps.Any(x =>
                    (x.Type == AlterTableType.AddColumn || x.Type == AlterTableType.ChangeColumn) &&
                    !x.IgnoreColumnPosition) == true)
            {
                for (int i = 0; i < qry.AlterTableSteps.Count; i++)
                {
                    var step = qry.AlterTableSteps[i];

                    if (step.Type == AlterTableType.AddColumn || step.Type == AlterTableType.ChangeColumn)
                    {
                        step.IgnoreColumnPosition = true;

                        qry.AlterTableSteps[i] = step;
                    }
                }

                return qry.ExecuteNonQueryAsync(conn);
            }

            throw ex;
        };

        public override bool UpdateFromInsteadOfJoin => false;
        public override bool UpdateJoinRequiresFromLeftTable => false;

        public override bool GroupBySupportsOrdering => _MySqlMode.Version.CompareTo("8.0.13") < 0;

        public override bool DeleteSupportsIgnore => true;
        public override bool InsertSupportsIgnore => true;
        public override bool InsertSupportsOnConflictDoUpdate => true;

        public override bool SupportsColumnComment => true;
        public override ColumnSRIDLocationMode ColumnSRIDLocation => ColumnSRIDLocationMode.AfterNullability;

        public override string ChangeColumnCommandName => "CHANGE";
        public override string DropForeignKeyCommandName => "DROP FOREIGN KEY";
        public override string AlterTableAddIndexCommandName => "ADD";

        public override int VarCharMaxLength
        {
            get
            {
                if (Is5_0_3OrLater())
                {
                    return 21845;
                }
                else
                {
                    return 255;
                }
            }
        }

        public override string UtcNow()
        {
            return @"UTC_TIMESTAMP()";
        }

        public override string ST_X(string pt)
        {
            if (Is5_7OrLater())
            {
                return "ST_X(" + pt + ")";
            }
            else
            {
                return "X(" + pt + ")";
            }
        }

        public override string ST_Y(string pt)
        {
            if (Is5_7OrLater())
            {
                return "ST_Y(" + pt + ")";
            }
            else
            {
                return "Y(" + pt + ")";
            }
        }

        public override string ST_Contains(string g1, string g2)
        {
            if (Is5_7OrLater())
            {
                return "ST_Contains(" + g1 + ", " + g2 + ")";
            }
            else
            {
                return "MBRContains(" + g1 + ", " + g2 + ")";
            }
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

            if (Is5_7OrLater())
            {
                return "ST_GeomFromText(" + text + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
            }
            else
            {
                return "GeomFromText(" + text + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
            }
        }

        public override string ST_GeogFromText(string text, string srid = null, bool literalText = false)
        {
            return ST_GeomFromText(text, srid, literalText);
        }

        public override void BuildConflictColumnUpdate(
            StringBuilder sb, ConnectorBase conn,
            ConflictColumn column, Query relatedQuery)
        {
            sb.Append("VALUES(" + WrapFieldName(column.Column) + ")");
        }

        public override void BuildOnConflictDoUpdate(StringBuilder outputBuilder, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
        {
            outputBuilder.Append(" ON DUPLICATE KEY UPDATE ");

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

        public override void BuildNullSafeEqualsTo(
            Where where,
            bool negate,
            StringBuilder outputBuilder,
            Where.BuildContext context)
        {
            if (negate)
                outputBuilder.Append(@" NOT ");

            where.BuildSingleValueFirst(outputBuilder, context);

            outputBuilder.Append(@" <=> ");

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

            // OFFSET is not supported without LIMIT
            if (query.Limit > 0 || withOffset)
            {
                outputBuilder.Append(" LIMIT ");
                outputBuilder.Append(query.Limit);
                outputBuilder.Append(' ');

                if (withOffset)
                {
                    outputBuilder.Append("OFFSET ");
                    outputBuilder.Append(query.Offset);
                    outputBuilder.Append(' ');
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
                outputBuilder.Append("CONSTRAINT ");

                if (!string.IsNullOrEmpty(index.Name))
                {
                    outputBuilder.Append(WrapFieldName(index.Name));
                    outputBuilder.Append(" ");
                }

                outputBuilder.Append("PRIMARY KEY ");
            }
            else
            {
                switch (index.Mode)
                {
                    case TableSchema.IndexMode.Unique:
                        outputBuilder.Append("UNIQUE ");
                        break;
                    case TableSchema.IndexMode.FullText:
                        outputBuilder.Append("FULLTEXT ");
                        break;
                    case TableSchema.IndexMode.Spatial:
                        outputBuilder.Append("SPATIAL ");
                        break;
                }

                outputBuilder.Append("INDEX ");

                if (!string.IsNullOrEmpty(index.Name))
                {
                    outputBuilder.Append(WrapFieldName(index.Name));
                    outputBuilder.Append(" ");
                }
            }

            if (index.Mode != TableSchema.IndexMode.Spatial)
            {
                switch (index.Type)
                {
                    case TableSchema.IndexType.BTREE:
                        outputBuilder.Append(@"USING BTREE ");
                        break;
                    case TableSchema.IndexType.RTREE:
                        outputBuilder.Append(@"USING RTREE ");
                        break;
                    case TableSchema.IndexType.HASH:
                        outputBuilder.Append(@"USING HASH ");
                        break;
                }
            }

            outputBuilder.Append(@"(");
            for (int i = 0; i < index.ColumnNames.Length; i++)
            {
                if (i > 0) outputBuilder.Append(",");
                outputBuilder.Append(WrapFieldName(index.ColumnNames[i]));
                if (index.ColumnLength[i] > 0) outputBuilder.AppendFormat("({0})", index.ColumnLength[i]);
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
                isDefaultAllowed = column.ActualDataType != DataType.VarChar &&
                    column.ActualDataType != DataType.Char &&
                    column.ActualDataType != DataType.Text &&
                    column.ActualDataType != DataType.MediumText &&
                    column.ActualDataType != DataType.LongText &&
                    column.Type == typeof(string);

                sb.Append(column.LiteralType);
                return;
            }

            DataType dataType = column.ActualDataType;
            DataTypeDef dataTypeDef = new DataTypeDef { Type = dataType };
            if (column.SRID != null)
            {
                dataTypeDef.SRID = column.SRID;
            }
            else if (column.MaxLength != 0)
            {
                dataTypeDef.MaxLength = column.MaxLength;
            }
            else if (column.NumberPrecision != 0 || column.NumberScale != 0)
            {
                dataTypeDef.Precision = (short)column.NumberPrecision;
                dataTypeDef.Scale = (short)column.NumberScale;
            }

            var (dataTypeString, isDefaultAllowedResult) = BuildDataTypeDef(dataTypeDef);

            if (string.IsNullOrEmpty(dataTypeString))
            {
                throw new NotImplementedException("Unsupprted data type " + dataType.ToString());
            }

            isDefaultAllowed = isDefaultAllowedResult;

            sb.Append(dataTypeString);

            if (column.AutoIncrement)
            {
                sb.Append(" AUTO_INCREMENT");
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

            if (!string.IsNullOrEmpty(column.Charset))
            {
                sb.Append(@" CHARACTER SET ");
                sb.Append(column.Charset);
            }

            if (!string.IsNullOrEmpty(column.Collate))
            {
                sb.Append(@" COLLATE ");
                sb.Append(column.Collate);
            }
        }

        public override (string typeString, bool isDefaultAllowed) BuildDataTypeDef(DataTypeDef typeDef)
        {
            string typeString = null;
            bool isDefaultAllowed = true;

            switch (typeDef.Type)
            {
                case DataType.VarChar:
                    if (typeDef.MaxLength < 0)
                    {
                        typeString = $"VARCHAR({VarCharMaxLength})";
                    }
                    else if (typeDef.MaxLength == 0)
                    {
                        typeString = "TEXT";
                        isDefaultAllowed = false;
                    }
                    else if (typeDef.MaxLength <= VarCharMaxLength)
                    {
                        typeString = $"VARCHAR({typeDef.MaxLength})";
                    }
                    else if (typeDef.MaxLength < 65536)
                    {
                        typeString = "TEXT";
                        isDefaultAllowed = false;
                    }
                    else if (typeDef.MaxLength < 16777215)
                    {
                        typeString = "MEDIUMTEXT";
                        isDefaultAllowed = false;
                    }
                    else
                    {
                        typeString = "LONGTEXT";
                        isDefaultAllowed = false;
                    }
                    break;

                case DataType.Char:
                    if (typeDef.MaxLength < 0)
                        typeString = $"CHAR({VarCharMaxLength})";
                    else if (typeDef.MaxLength == 0 || typeDef.MaxLength >= VarCharMaxLength)
                        typeString = $"CHAR({VarCharMaxLength})";
                    else
                        typeString = $"CHAR({typeDef.MaxLength})";
                    break;

                case DataType.Text:
                    typeString = "TEXT";
                    isDefaultAllowed = false;
                    break;

                case DataType.MediumText:
                    typeString = "MEDIUMTEXT";
                    isDefaultAllowed = false;
                    break;

                case DataType.LongText:
                    typeString = "LONGTEXT";
                    isDefaultAllowed = false;
                    break;

                case DataType.Boolean:
                    typeString = "BOOLEAN";
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
                    {
                        typeString = $"NUMERIC({typeDef.Precision}, {typeDef.Scale})";
                    }
                    else
                    {
                        typeString = "NUMERIC";
                    }
                    break;

                case DataType.Float:
                    if (typeDef.Precision > 0 && !Is8_0_17OrLater())
                    {
                        typeString = $"FLOAT({typeDef.Precision}, {typeDef.Scale})";
                    }
                    else
                    {
                        typeString = "FLOAT";
                    }
                    break;

                case DataType.Double:
                    if (typeDef.Precision > 0 && !Is8_0_17OrLater())
                    {
                        typeString = $"DOUBLE({typeDef.Precision}, {typeDef.Scale})";
                    }
                    else
                    {
                        typeString = "DOUBLE";
                    }
                    break;

                case DataType.Decimal:
                    if (typeDef.Precision > 0)
                    {
                        typeString = $"DECIMAL({typeDef.Precision}, {typeDef.Scale})";
                    }
                    else
                    {
                        typeString = "DECIMAL";
                    }
                    break;

                case DataType.Money:
                    if (typeDef.Precision > 0)
                    {
                        typeString = $"DECIMAL({typeDef.Precision}, {typeDef.Scale})";
                    }
                    else
                    {
                        typeString = "DECIMAL";
                    }
                    break;

                case DataType.TinyInt:
                    typeString = "TINYINT";
                    break;

                case DataType.UnsignedTinyInt:
                    typeString = "TINYINT UNSIGNED";
                    break;

                case DataType.SmallInt:
                    typeString = "SMALLINT";
                    break;

                case DataType.UnsignedSmallInt:
                    typeString = "SMALLINT UNSIGNED";
                    break;

                case DataType.Int:
                    typeString = "INT";
                    break;

                case DataType.UnsignedInt:
                    typeString = "INT UNSIGNED";
                    break;

                case DataType.BigInt:
                    typeString = "BIGINT";
                    break;

                case DataType.UnsignedBigInt:
                    typeString = "BIGINT UNSIGNED";
                    break;

                case DataType.Json:
                    typeString = "JSON";
                    break;

                case DataType.JsonBinary:
                    typeString = "JSON";
                    break;

                case DataType.Blob:
                    typeString = "BLOB";
                    break;

                case DataType.Guid:
                    typeString = "CHAR(36)";
                    break;

                case DataType.Geometry:
                    typeString = "GEOMETRY";
                    break;

                case DataType.GeometryCollection:
                    typeString = "GEOMETRYCOLLECTION";
                    break;

                case DataType.Point:
                    typeString = "POINT";
                    break;

                case DataType.LineString:
                    typeString = "LINESTRING";
                    break;

                case DataType.Polygon:
                    typeString = "POLYGON";
                    break;

                case DataType.Line:
                    typeString = "LINE";
                    break;

                case DataType.Curve:
                    typeString = "CURVE";
                    break;

                case DataType.Surface:
                    typeString = "SURFACE";
                    break;

                case DataType.LinearRing:
                    typeString = "LINEARRING";
                    break;

                case DataType.MultiPoint:
                    typeString = "MULTIPOINT";
                    break;

                case DataType.MultiLineString:
                    typeString = "MULTILINESTRING";
                    break;

                case DataType.MultiPolygon:
                    typeString = "MULTIPOLYGON";
                    break;

                case DataType.MultiCurve:
                    typeString = "MULTICURVE";
                    break;

                case DataType.MultiSurface:
                    typeString = "MULTISURFACE";
                    break;

                case DataType.Geographic:
                    typeString = "GEOMETRY";
                    break;

                case DataType.GeographicCollection:
                    typeString = "GEOMETRYCOLLECTION";
                    break;

                case DataType.GeographicPoint:
                    typeString = "POINT";
                    break;

                case DataType.GeographicLineString:
                    typeString = "LINESTRING";
                    break;

                case DataType.GeographicPolygon:
                    typeString = "POLYGON";
                    break;

                case DataType.GeographicLine:
                    typeString = "LINE";
                    break;

                case DataType.GeographicCurve:
                    typeString = "CURVE";
                    break;

                case DataType.GeographicSurface:
                    typeString = "SURFACE";
                    break;

                case DataType.GeographicLinearRing:
                    typeString = "LINEARRING";
                    break;

                case DataType.GeographicMultiPoint:
                    typeString = "MULTIPOINT";
                    break;

                case DataType.GeographicMultiLineString:
                    typeString = "MULTILINESTRING";
                    break;

                case DataType.GeographicMultiPolygon:
                    typeString = "MULTIPOLYGON";
                    break;

                case DataType.GeographicMultiCurve:
                    typeString = "MULTICURVE";
                    break;

                case DataType.GeographicMultiSurface:
                    typeString = "MULTISURFACE";
                    break;
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

            // COLLATE ASC/DESC not supported in MySQL

            sb.Append(")");
        }

        public override void BuildOrderByRandom(ValueWrapper seedValue, ConnectorBase conn, StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"RAND()");
        }

        public override void BuildJsonExtract(
            ValueWrapper value, string path, bool unquote,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            var phrase = $"JSON_EXTRACT({value.Build(conn, relatedQuery)}, {PrepareValue(path)})";

            if (unquote)
            {
                sb.Append(
                    $"(CASE WHEN JSON_TYPE({phrase}) = 'NULL' THEN NULL" +
                    $" WHEN JSON_TYPE({phrase}) = 'STRING' THEN JSON_UNQUOTE({phrase})" +
                    $" ELSE {phrase} END)"
                );
            }
            else
            {
                sb.Append(phrase);
            }
        }

        public override void BuildJsonExtractValue(
            ValueWrapper value, string path,
            DataTypeDef? returnType,
            Phrases.JsonValue.DefaultAction onEmptyAction, object onEmptyValue,
            Phrases.JsonValue.DefaultAction onErrorAction, object onErrorValue,
            StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            if (Is8_0_21OrLater())
            {
                sb.Append("JSON_VALUE(");
                value.Build(sb, conn, relatedQuery);
                sb.Append($", {PrepareValue(path)}");

                if (returnType != null)
                {
                    var (typeString, _) = BuildDataTypeDef(returnType.Value);
                    if (typeString != null)
                    {
                        sb.Append($" RETURNING {typeString}");
                    }
                }

                if (onEmptyAction == Phrases.JsonValue.DefaultAction.Error)
                {
                    sb.Append($" ERROR ON EMPTY");
                }
                else
                {
                    if (onEmptyValue == null)
                        sb.Append($" NULL ON EMPTY");
                    else sb.Append($" DEFAULT {PrepareValue(conn, onEmptyValue, relatedQuery)} ON EMPTY");
                }

                if (onErrorAction == Phrases.JsonValue.DefaultAction.Error)
                {
                    sb.Append($" ERROR ON ERROR");
                }
                else
                {
                    if (onErrorValue == null)
                        sb.Append($" NULL ON ERROR");
                    else sb.Append($" DEFAULT {PrepareValue(conn, onErrorValue, relatedQuery)} ON ERROR");
                }

                sb.Append($")");
            }
            else
            {
                var phrase = $"JSON_EXTRACT({value.Build(conn, relatedQuery)}, {PrepareValue(path)})";

                if (returnType != null)
                {
                    var (typeString, _) = BuildDataTypeDef(returnType.Value);
                    if (typeString != null)
                    {
                        if (onEmptyValue != null && onEmptyAction == Phrases.JsonValue.DefaultAction.Value)
                        {
                            sb.Append(
                                $"(CASE WHEN {phrase} IS NULL THEN {PrepareValue(conn, onEmptyValue, relatedQuery)}" +
                                $" ELSE CAST(JSON_UNQUOTE({phrase}) AS {typeString}) END)"
                            );
                        }
                        else
                        {
                            sb.Append($"CAST(JSON_UNQUOTE({phrase}) AS {typeString})");
                        }
                        return;
                    }
                }

                if (onEmptyValue != null && onEmptyAction == Phrases.JsonValue.DefaultAction.Value)
                {
                    sb.Append(
                        $"(CASE WHEN {phrase} IS NULL THEN {PrepareValue(conn, onEmptyValue, relatedQuery)}" +
                        $" WHEN JSON_TYPE({phrase}) = 'NULL' THEN NULL" +
                        $" WHEN JSON_TYPE({phrase}) = 'STRING' THEN JSON_UNQUOTE({phrase})" +
                        $" ELSE {phrase} END)"
                    );
                }
                else
                {
                    sb.Append(
                        $"(CASE WHEN JSON_TYPE({phrase}) = 'NULL' THEN NULL" +
                        $" WHEN JSON_TYPE({phrase}) = 'STRING' THEN JSON_UNQUOTE({phrase})" +
                        $" ELSE {phrase} END)"
                    );
                }
            }
        }

        public override string Aggregate_Some(string rawExpression)
        {
            return $"(SUM({rawExpression}) > 0)";
        }

        public override string Aggregate_Every(string rawExpression)
        {
            return $"(BIT_AND({rawExpression}) > 0)";
        }

        public override string GroupConcat(bool distinct, string rawExpression, string rawOrderBy, string separator)
        {
            var sb = new StringBuilder();

            sb.Append("GROUP_CONCAT(");

            if (distinct)
            {
                sb.Append("DISTINCT ");
            }

            sb.Append(rawExpression);

            if (rawOrderBy != null)
            {
                sb.Append(rawOrderBy);
            }

            if (separator != null)
            {
                sb.Append(" SEPARATOR " + PrepareValue(separator));
            }

            sb.Append(")");

            return sb.ToString();
        }

        public override void BuildChangeColumn(AlterTableQueryData alterData, StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            sb.Append(WrapFieldName(string.IsNullOrEmpty(alterData.OldItemName) ? alterData.Column.Name : alterData.OldItemName));
            sb.Append(" ");

            BuildColumnProperties(
                column: alterData.Column,
                noDefault: false,
                sb: sb,
                conn: conn, 
                relatedQuery: relatedQuery);

            int idx = relatedQuery.Schema.Columns.IndexOf(alterData.Column);
            if (idx == 0) sb.Append(@"FIRST ");
            else if (!alterData.IgnoreColumnPosition) sb.AppendFormat(@"AFTER {0} ", WrapFieldName(relatedQuery.Schema.Columns[idx - 1].Name));
        }

        public override void BuildAddColumn(AlterTableQueryData alterData, StringBuilder sb, ConnectorBase conn, Query relatedQuery)
        {
            BuildColumnProperties(
                column: alterData.Column,
                noDefault: false,
                sb: sb,
                conn: conn,
                relatedQuery: relatedQuery);

            int idx = relatedQuery.Schema.Columns.IndexOf(alterData.Column);
            if (idx == 0) sb.Append(@"FIRST ");
            else if (!alterData.IgnoreColumnPosition) sb.AppendFormat(@"AFTER {0} ", WrapFieldName(relatedQuery.Schema.Columns[idx - 1].Name));
        }

        public override string BuildFindString(
            ConnectorBase conn,
            ValueWrapper needle,
            ValueWrapper haystack,
            ValueWrapper? startAt,
            Query relatedQuery)
        {
            string ret = "LOCATE(";

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

        #endregion

        #region Reading values from SQL

        public override Geometry ReadGeometry(object value)
        {
            byte[] geometryData = value as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, true);
            }
            return null;
        }

        #endregion

        #region Preparing values for SQL

        public override string WrapFieldName(string fieldName)
        {
            return '`' + fieldName.Replace("`", "``") + '`';
        }

        private static string CharactersNeedsBackslashes = // Other special characters for escaping
            "\u005c\u00a5\u0160\u20a9\u2216\ufe68\uff3c";
        private static string CharactersNeedsDoubling = // Kinds of quotes...
            "\u0027\u0060\u00b4\u02b9\u02ba\u02bb\u02bc\u02c8\u02ca\u02cb\u02d9\u0300\u0301\u2018\u2019\u201a\u2032\u2035\u275b\u275c\uff07";

        private static string EscapeStringWithBackslashes(string value)
        {
            var sb = new StringBuilder();
            foreach (char c in value)
            {
                if (CharactersNeedsDoubling.IndexOf(c) >= 0 || CharactersNeedsBackslashes.IndexOf(c) >= 0)
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
                if (CharactersNeedsDoubling.IndexOf(c) >= 0)
                {
                    sb.Append(c);
                }
                else if (CharactersNeedsBackslashes.IndexOf(c) >= 0)
                {
                    sb.Append("\\");
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public override string EscapeString(string value)
        {
            if (_MySqlMode.NoBackSlashes)
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

        public override string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace("%", "\x10%");
        }

        public override string LikeEscapingStatement => "ESCAPE('\x10')";

        #endregion
    }
}
