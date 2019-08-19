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

        public override int VarCharMaxLength => 4000;

        public override string VarCharMaxKeyword => "MAX";

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

        public override string ST_GeomFromText(string text, string srid = null)
        {
            return "geometry::STGeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override string ST_GeogFromText(string text, string srid = null)
        {
            return "geography::STGeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
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
                outputBuilder.Append(@")");
            }
        }
        
        public override void BuildOrderByRandom(ValueWrapper seedValue, ConnectorBase conn, StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"NEWID()");
        }

        public override string Aggregate_Some(string rawExpression)
        {
            return $"(SUM({rawExpression}) > 0)";
        }

        public override string Aggregate_Every(string rawExpression)
        {
            return $"(COUNT({rawExpression}) = COUNT(*))";
        }

        #endregion

        #region Types

        public override string AutoIncrementType => @"IDENTITY";
        public override string AutoIncrementBigIntType => @"IDENTITY";

        public override string TinyIntType => @"TINYINT";
        public override string UnsignedTinyIntType => @"TINYINT";
        public override string SmallIntType => @"SMALLINT";
        public override string UnsignedSmallIntType => @"SMALLINT";
        public override string IntType => @"INT";
        public override string UnsignedIntType => @"INT";
        public override string BigIntType => @"BIGINT";
        public override string UnsignedBigIntType => @"BIGINT";
        public override string NumericType => @"NUMERIC";
        public override string DecimalType => @"DECIMAL";
        public override string MoneyType => @"MONEY";
        public override string FloatType => @"FLOAT(4)";
        public override string DoubleType => @"FLOAT(8)";
        public override string VarCharType => @"NVARCHAR";
        public override string CharType => @"NCHAR";
        public override string TextType => @"NTEXT";
        public override string MediumTextType => @"NTEXT";
        public override string LongTextType => @"NTEXT";
        public override string BooleanType => @"bit";
        public override string DateTimeType => @"DATETIME";
        public override string BlobType => @"IMAGE";
        public override string GuidType => @"UNIQUEIDENTIFIER";
        public override string JsonType => @"TEXT";
        public override string JsonBinaryType => @"TEXT";

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
