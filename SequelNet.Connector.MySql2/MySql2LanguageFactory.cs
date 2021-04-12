using System;
using System.Text;
using System.Linq;
using SequelNet.Sql.Spatial;

namespace SequelNet.Connector
{
    public class MySql2LanguageFactory : LanguageFactory
    {
        public MySql2LanguageFactory(MySql2Mode mySqlMode)
        {
            _MySqlMode = mySqlMode;
        }

        #region Versioning

        private MySql2Mode _MySqlMode;

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

        #endregion

        #region Syntax

        public override Func<Query, ConnectorBase, Exception, int> OnExecuteNonQueryException => (qry, conn, ex) =>
        {
            if (ex is MySqlConnector.MySqlException myex &&
                myex.Number == 1054 &&
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
            if (ex is MySqlConnector.MySqlException myex &&
                myex.Number == 1054 &&
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

            if (dataType == DataType.VarChar)
            {
                if (column.MaxLength < 0)
                {
                    sb.Append("VARCHAR");
                    sb.AppendFormat(@"({0})", VarCharMaxLength);
                }
                else if (column.MaxLength == 0)
                {
                    sb.Append("TEXT");
                    isDefaultAllowed = false;
                }
                else if (column.MaxLength <= VarCharMaxLength)
                {
                    sb.Append("VARCHAR");
                    sb.AppendFormat(@"({0})", column.MaxLength);
                }
                else if (column.MaxLength < 65536)
                {
                    sb.Append("TEXT");
                    isDefaultAllowed = false;
                }
                else if (column.MaxLength < 16777215)
                {
                    sb.Append("MEDIUMTEXT");
                    isDefaultAllowed = false;
                }
                else
                {
                    sb.Append("LONGTEXT");
                    isDefaultAllowed = false;
                }
            }
            else if (dataType == DataType.Char)
            {
                sb.Append("CHAR");

                if (column.MaxLength < 0)
                {
                    sb.AppendFormat(@"({0})", VarCharMaxLength);
                }
                else if (column.MaxLength == 0 || column.MaxLength >= VarCharMaxLength)
                {
                    sb.AppendFormat(@"({0})", VarCharMaxLength);
                }
                else
                {
                    sb.AppendFormat(@"({0})", column.MaxLength);
                }
            }
            else if (dataType == DataType.Text)
            {
                sb.Append("TEXT");
                isDefaultAllowed = false;
            }
            else if (dataType == DataType.MediumText)
            {
                sb.Append("MEDIUMTEXT");
                isDefaultAllowed = false;
            }
            else if (dataType == DataType.LongText)
            {
                sb.Append("LONGTEXT");
                isDefaultAllowed = false;
            }
            else if (dataType == DataType.Boolean)
                sb.Append("BOOLEAN");
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
                {
                    sb.Append("NUMERIC");
                }
            }
            else if (dataType == DataType.Float)
            {
                if (column.NumberPrecision > 0)
                {
                    sb.Append("FLOAT");
                    sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                }
                else
                {
                    sb.Append("FLOAT");
                }
            }
            else if (dataType == DataType.Double)
            {
                if (column.NumberPrecision > 0)
                {
                    sb.Append("DOUBLE");
                    sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                }
                else
                {
                    sb.Append("DOUBLE");
                }
            }
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
                sb.Append("TINYINT");
            else if (dataType == DataType.UnsignedTinyInt)
                sb.Append("TINYINT UNSIGNED");
            else if (dataType == DataType.SmallInt)
                sb.Append("SMALLINT");
            else if (dataType == DataType.UnsignedSmallInt)
                sb.Append("SMALLINT UNSIGNED");
            else if (dataType == DataType.Int)
                sb.Append("INT");
            else if (dataType == DataType.UnsignedInt)
                sb.Append("INT UNSIGNED");
            else if (dataType == DataType.BigInt)
                sb.Append("BIGINT");
            else if (dataType == DataType.UnsignedBigInt)
                sb.Append("BIGINT UNSIGNED");
            else if (dataType == DataType.Json)
                sb.Append("JSON");
            else if (dataType == DataType.JsonBinary)
                sb.Append("JSON");
            else if (dataType == DataType.Blob)
                sb.Append("BLOB");
            else if (dataType == DataType.Guid)
                sb.Append("CHAR(36)");
            else if (dataType == DataType.Geometry)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.GeometryCollection)
                sb.Append("GEOMETRYCOLLECTION");
            else if (dataType == DataType.Point)
                sb.Append("POINT");
            else if (dataType == DataType.LineString)
                sb.Append("LINESTRING");
            else if (dataType == DataType.Polygon)
                sb.Append("POLYGON");
            else if (dataType == DataType.Line)
                sb.Append("LINE");
            else if (dataType == DataType.Curve)
                sb.Append("CURVE");
            else if (dataType == DataType.Surface)
                sb.Append("SURFACE");
            else if (dataType == DataType.LinearRing)
                sb.Append("LINEARRING");
            else if (dataType == DataType.MultiPoint)
                sb.Append("MULTIPOINT");
            else if (dataType == DataType.MultiLineString)
                sb.Append("MULTILINESTRING");
            else if (dataType == DataType.MultiPolygon)
                sb.Append("MULTIPOLYGON");
            else if (dataType == DataType.MultiCurve)
                sb.Append("MULTICURVE");
            else if (dataType == DataType.MultiSurface)
                sb.Append("MULTISURFACE");
            else if (dataType == DataType.Geographic)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.GeographicCollection)
                sb.Append("GEOMETRYCOLLECTION");
            else if (dataType == DataType.GeographicPoint)
                sb.Append("POINT");
            else if (dataType == DataType.GeographicLineString)
                sb.Append("LINESTRING");
            else if (dataType == DataType.GeographicPolygon)
                sb.Append("POLYGON");
            else if (dataType == DataType.GeographicLine)
                sb.Append("LINE");
            else if (dataType == DataType.GeographicCurve)
                sb.Append("CURVE");
            else if (dataType == DataType.GeographicSurface)
                sb.Append("SURFACE");
            else if (dataType == DataType.GeographicLinearRing)
                sb.Append("LINEARRING");
            else if (dataType == DataType.GeographicMultiPoint)
                sb.Append("MULTIPOINT");
            else if (dataType == DataType.GeographicMultiLineString)
                sb.Append("MULTILINESTRING");
            else if (dataType == DataType.GeographicMultiPolygon)
                sb.Append("MULTIPOLYGON");
            else if (dataType == DataType.GeographicMultiCurve)
                sb.Append("MULTICURVE");
            else if (dataType == DataType.GeographicMultiSurface)
                sb.Append("MULTISURFACE");
            else throw new NotImplementedException("Unsupprted data type " + dataType.ToString());

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
