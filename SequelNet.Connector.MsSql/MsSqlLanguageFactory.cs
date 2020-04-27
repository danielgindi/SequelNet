using SequelNet.Sql.Spatial;
using System;
using System.Text;

namespace SequelNet.Connector
{
    public class MsSqlLanguageFactory : LanguageFactory
    {
        public MsSqlLanguageFactory(MsSqlVersion sqlVersion)
        {
            _MsSqlVersion = sqlVersion;
        }

        #region Versioning

        private MsSqlVersion _MsSqlVersion;

        #endregion

        #region Syntax

        public override bool UpdateFromInsteadOfJoin => false;
        public override bool UpdateJoinRequiresFromLeftTable => true;

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
                if (index.Cluster == TableSchema.ClusterMode.Clustered) outputBuilder.Append(@"CLUSTERED ");
                else if (index.Cluster == TableSchema.ClusterMode.NonClustered) outputBuilder.Append(@"NONCLUSTERED ");

                outputBuilder.Append(@"INDEX ");
                outputBuilder.Append(WrapFieldName(index.Name));

                outputBuilder.Append(@"ON ");

                BuildTableName(qry, conn, outputBuilder, false);

                outputBuilder.Append(@"(");
                for (int i = 0; i < index.ColumnNames.Length; i++)
                {
                    if (i > 0) outputBuilder.Append(",");
                    outputBuilder.Append(WrapFieldName(index.ColumnNames[i]));
                    outputBuilder.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
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

            DataType dataType = column.ActualDataType;
            if (dataType == DataType.VarChar)
            {
                if (column.MaxLength < 0)
                {
                    sb.Append("NVARCHAR");
                    sb.Append(@"(MAX)");
                }
                else if (column.MaxLength == 0)
                    sb.Append("NTEXT");
                else if (column.MaxLength <= VarCharMaxLength)
                {
                    sb.Append("NVARCHAR");
                    sb.AppendFormat(@"({0})", column.MaxLength);
                }
                else if (column.MaxLength < 65536)
                    sb.Append("NTEXT");
                else if (column.MaxLength < 16777215)
                    sb.Append("NTEXT");
                else
                    sb.Append("NTEXT");
            }
            else if (dataType == DataType.Char)
            {
                if (column.MaxLength < 0)
                {
                    sb.Append("NVARCHAR");
                    sb.Append(@"(MAX)");
                }
                else if (column.MaxLength == 0 || column.MaxLength >= VarCharMaxLength)
                {
                    sb.Append("NVARCHAR");
                    sb.AppendFormat(@"({0})", VarCharMaxLength);
                }
                else
                {
                    sb.Append("NVARCHAR");
                    sb.AppendFormat(@"({0})", column.MaxLength);
                }
            }
            else if (dataType == DataType.Text)
                sb.Append("NTEXT");
            else if (dataType == DataType.MediumText)
                sb.Append("NTEXT");
            else if (dataType == DataType.LongText)
                sb.Append("NTEXT");
            else if (dataType == DataType.Boolean)
                sb.Append("bit");
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
                sb.Append("FLOAT(4)");
            else if (dataType == DataType.Double)
                sb.Append("FLOAT(8)");
            else if (dataType == DataType.Decimal)
            {
                if (column.NumberPrecision > 0)
                {
                    sb.Append("DECIMAL");
                    sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                }
                else
                    sb.Append("DECIMAL");
            }
            else if (dataType == DataType.Money)
            {
                if (column.NumberPrecision > 0)
                    sb.Append("MONEY");
                else
                    sb.Append("MONEY");
            }
            else if (dataType == DataType.TinyInt)
                sb.Append("TINYINT");
            else if (dataType == DataType.UnsignedTinyInt)
                sb.Append("TINYINT");
            else if (dataType == DataType.SmallInt)
                sb.Append("SMALLINT");
            else if (dataType == DataType.UnsignedSmallInt)
                sb.Append("SMALLINT");
            else if (dataType == DataType.Int)
                sb.Append("TINYINT");
            else if (dataType == DataType.UnsignedInt)
                sb.Append("INT");
            else if (dataType == DataType.BigInt)
                sb.Append("BIGINT");
            else if (dataType == DataType.UnsignedBigInt)
                sb.Append("BIGINT");
            else if (dataType == DataType.Json)
                sb.Append("TEXT");
            else if (dataType == DataType.JsonBinary)
                sb.Append("TEXT");
            else if (dataType == DataType.Blob)
                sb.Append("IMAGE");
            else if (dataType == DataType.Guid)
                sb.Append("UNIQUEIDENTIFIER");
            else if (dataType == DataType.Geometry)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.GeometryCollection)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.Point)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.LineString)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.Polygon)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.Line)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.Curve)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.Surface)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.LinearRing)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.MultiPoint)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.MultiLineString)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.MultiPolygon)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.MultiCurve)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.MultiSurface)
                sb.Append("GEOMETRY");
            else if (dataType == DataType.Geographic)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicCollection)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicPoint)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicLineString)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicPolygon)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicLine)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicCurve)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicSurface)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicLinearRing)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicMultiPoint)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicMultiLineString)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicMultiPolygon)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicMultiCurve)
                sb.Append("GEOGRAPHIC");
            else if (dataType == DataType.GeographicMultiSurface)
                sb.Append("GEOGRAPHIC");
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
            outputBuilder.Append(@"NEWID()");
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
        public override string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%").Replace(@"_", @"\_");
        }

        #endregion
    }
}
