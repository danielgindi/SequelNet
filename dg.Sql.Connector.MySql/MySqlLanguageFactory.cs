using System;
using System.Text;

namespace dg.Sql.Connector
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

        public override int varchar_MAX_VALUE
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

        public override string func_UTC_NOW()
        {
            return @"UTC_TIMESTAMP()";
        }

        public override string func_ST_X(string pt)
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

        public override string func_ST_Y(string pt)
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

        public override string func_ST_Contains(string g1, string g2)
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

        public override string func_ST_GeomFromText(string text, string srid = null)
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

        public override string func_ST_GeogFromText(string text, string srid = null)
        {
            return func_ST_GeomFromText(text, srid);
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

        public override string type_AUTOINCREMENT { get { return @"AUTO_INCREMENT"; } }
        public override string type_AUTOINCREMENT_BIGINT { get { return @"AUTO_INCREMENT"; } }

        public override string type_TINYINT { get { return @"TINYINT"; } }
        public override string type_UNSIGNEDTINYINT { get { return @"TINYINT UNSIGNED"; } }
        public override string type_SMALLINT { get { return @"SMALLINT"; } }
        public override string type_UNSIGNEDSMALLINT { get { return @"SMALLINT UNSIGNED"; } }
        public override string type_INT { get { return @"INT"; } }
        public override string type_UNSIGNEDINT { get { return @"INT UNSIGNED"; } }
        public override string type_BIGINT { get { return @"BIGINT"; } }
        public override string type_UNSIGNEDBIGINT { get { return @"BIGINT UNSIGNED"; } }
        public override string type_NUMERIC { get { return @"NUMERIC"; } }
        public override string type_DECIMAL { get { return @"DECIMAL"; } }
        public override string type_MONEY { get { return @"DECIMAL"; } }
        public override string type_FLOAT { get { return @"FLOAT"; } }
        public override string type_DOUBLE { get { return @"DOUBLE"; } }
        public override string type_VARCHAR { get { return @"NATIONAL VARCHAR"; } }
        public override string type_CHAR { get { return @"NATIONAL CHAR"; } }
        public override string type_TEXT { get { return @"TEXT"; } }
        public override string type_MEDIUMTEXT { get { return @"MEDIUMTEXT"; } }
        public override string type_LONGTEXT { get { return @"LONGTEXT"; } }
        public override string type_BOOLEAN { get { return @"BOOLEAN"; } }
        public override string type_DATETIME { get { return @"DATETIME"; } }
        public override string type_BLOB { get { return @"BLOB"; } }
        public override string type_GUID { get { return @"NATIONAL CHAR(36)"; } }
        public override string type_JSON { get { return @"JSON"; } }
        public override string type_JSON_BINARY { get { return @"JSON"; } }

        public override string type_GEOMETRY { get { return @"GEOMETRY"; } }
        public override string type_GEOMETRYCOLLECTION { get { return @"GEOMETRYCOLLECTION"; } }
        public override string type_POINT { get { return @"POINT"; } }
        public override string type_LINESTRING { get { return @"LINESTRING"; } }
        public override string type_POLYGON { get { return @"POLYGON"; } }
        public override string type_LINE { get { return @"LINE"; } }
        public override string type_CURVE { get { return @"CURVE"; } }
        public override string type_SURFACE { get { return @"SURFACE"; } }
        public override string type_LINEARRING { get { return @"LINEARRING"; } }
        public override string type_MULTIPOINT { get { return @"MULTIPOINT"; } }
        public override string type_MULTILINESTRING { get { return @"MULTILINESTRING"; } }
        public override string type_MULTIPOLYGON { get { return @"MULTIPOLYGON"; } }
        public override string type_MULTICURVE { get { return @"MULTICURVE"; } }
        public override string type_MULTISURFACE { get { return @"MULTISURFACE"; } }

        public override string type_GEOGRAPHIC { get { return @"GEOMETRY"; } }
        public override string type_GEOGRAPHICCOLLECTION { get { return @"GEOMETRYCOLLECTION"; } }
        public override string type_GEOGRAPHIC_POINT { get { return @"POINT"; } }
        public override string type_GEOGRAPHIC_LINESTRING { get { return @"LINESTRING"; } }
        public override string type_GEOGRAPHIC_POLYGON { get { return @"POLYGON"; } }
        public override string type_GEOGRAPHIC_LINE { get { return @"LINE"; } }
        public override string type_GEOGRAPHIC_CURVE { get { return @"CURVE"; } }
        public override string type_GEOGRAPHIC_SURFACE { get { return @"SURFACE"; } }
        public override string type_GEOGRAPHIC_LINEARRING { get { return @"LINEARRING"; } }
        public override string type_GEOGRAPHIC_MULTIPOINT { get { return @"MULTIPOINT"; } }
        public override string type_GEOGRAPHIC_MULTILINESTRING { get { return @"MULTILINESTRING"; } }
        public override string type_GEOGRAPHIC_MULTIPOLYGON { get { return @"MULTIPOLYGON"; } }
        public override string type_GEOGRAPHIC_MULTICURVE { get { return @"MULTICURVE"; } }
        public override string type_GEOGRAPHIC_MULTISURFACE { get { return @"MULTISURFACE"; } }

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

        public override string LikeEscapingStatement
        {
            get { return "ESCAPE('\x10')"; }
        }

        #endregion
    }
}
