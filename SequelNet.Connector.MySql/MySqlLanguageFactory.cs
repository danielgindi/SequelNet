using SequelNet.Sql.Spatial;
using System;
using System.Text;

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

        #endregion

        #region Syntax

        public override bool UpdateFromInsteadOfJoin => false;
        public override bool UpdateJoinRequiresFromLeftTable => false;

        public override bool GroupBySupportsOrdering => true;

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

        public override string ST_GeomFromText(string text, string srid = null)
        {
            if (Is5_7OrLater())
            {
                return "ST_GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
            }
            else
            {
                return "GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
            }
        }

        public override string ST_GeogFromText(string text, string srid = null)
        {
            return ST_GeomFromText(text, srid);
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
                switch (index.Mode)
                {
                    case TableSchema.IndexMode.Unique:
                        outputBuilder.Append(@"UNIQUE ");
                        break;
                    case TableSchema.IndexMode.FullText:
                        outputBuilder.Append(@"FULLTEXT ");
                        break;
                    case TableSchema.IndexMode.Spatial:
                        outputBuilder.Append(@"SPATIAL ");
                        break;
                }
                outputBuilder.Append(@"INDEX ");
                outputBuilder.Append(WrapFieldName(index.Name));
                outputBuilder.Append(@" ");
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

        public override void BuildOrderByRandom(ValueWrapper seedValue, ConnectorBase conn, StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"RAND()");
        }

        #endregion

        #region Types

        public override string AutoIncrementType => @"AUTO_INCREMENT";
        public override string AutoIncrementBigIntType => @"AUTO_INCREMENT";

        public override string TinyIntType => @"TINYINT";
        public override string UnsignedTinyIntType => @"TINYINT UNSIGNED";
        public override string SmallIntType => @"SMALLINT";
        public override string UnsignedSmallIntType => @"SMALLINT UNSIGNED";
        public override string IntType => @"INT";
        public override string UnsignedIntType => @"INT UNSIGNED";
        public override string BigIntType => @"BIGINT";
        public override string UnsignedBigIntType => @"BIGINT UNSIGNED";
        public override string NumericType => @"NUMERIC";
        public override string DecimalType => @"DECIMAL";
        public override string MoneyType => @"DECIMAL";
        public override string FloatType => @"FLOAT";
        public override string DoubleType => @"DOUBLE";
        public override string VarCharType => @"NATIONAL VARCHAR";
        public override string CharType => @"NATIONAL CHAR";
        public override string TextType => @"TEXT";
        public override string MediumTextType => @"MEDIUMTEXT";
        public override string LongTextType => @"LONGTEXT";
        public override string BooleanType => @"BOOLEAN";
        public override string DateTimeType => @"DATETIME";
        public override string BlobType => @"BLOB";
        public override string GuidType => @"NATIONAL CHAR(36)";
        public override string JsonType => @"JSON";
        public override string JsonBinaryType => @"JSON";

        public override string TypeGeometry => @"GEOMETRY";
        public override string GeometryCollectionType => @"GEOMETRYCOLLECTION";
        public override string PointType => @"POINT";
        public override string LineStringType => @"LINESTRING";
        public override string PolygonType => @"POLYGON";
        public override string LineType => @"LINE";
        public override string CurveType => @"CURVE";
        public override string SurfaceType => @"SURFACE";
        public override string LinearRingType => @"LINEARRING";
        public override string MultiPointType => @"MULTIPOINT";
        public override string MultiLineStringType => @"MULTILINESTRING";
        public override string MultiPolygonType => @"MULTIPOLYGON";
        public override string MultiCurveType => @"MULTICURVE";
        public override string MultiSurfaceType => @"MULTISURFACE";

        public override string GeographicType => @"GEOMETRY";
        public override string GeographicCollectionType => @"GEOMETRYCOLLECTION";
        public override string GeographicPointType => @"POINT";
        public override string GeographicLinestringType => @"LINESTRING";
        public override string GeographicPolygonType => @"POLYGON";
        public override string GeographicLineType => @"LINE";
        public override string GeographicCurveType => @"CURVE";
        public override string GeographicSurfaceType => @"SURFACE";
        public override string GeographicLinearringType => @"LINEARRING";
        public override string GeographicMultipointType => @"MULTIPOINT";
        public override string GeographicMultilinestringType => @"MULTILINESTRING";
        public override string GeographicMultipolygonType => @"MULTIPOLYGON";
        public override string GeographicMulticurveType => @"MULTICURVE";
        public override string GeographicMultisurfaceType => @"MULTISURFACE";

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
