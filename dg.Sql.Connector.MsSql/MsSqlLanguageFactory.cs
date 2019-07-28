using System;

namespace dg.Sql.Connector
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

        public override int varchar_MAX_VALUE
        {
            get { return 4000; }
        }

        public override string varchar_MAX
        {
            get { return "MAX"; }
        }

        public override string func_UTC_NOW()
        {
            return @"GETUTCDATE()";
        }

        public override string func_HOUR(string date)
        {
            return @"DATEPART(hour, " + date + ")";
        }

        public override string func_MINUTE(string date)
        {
            return @"DATEPART(minute, " + date + ")";
        }

        public override string func_SECOND(string date)
        {
            return @"DATEPART(second, " + date + ")";
        }

        public override string func_MD5_Hex(string value)
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

        public override string func_SHA1_Hex(string value)
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

        public override string func_MD5_Binary(string value)
        {
            return @"HASHBYTES('MD5', " + value + ")";
        }

        public override string func_SHA1_Binary(string value)
        {
            return @"HASHBYTES('SHA1', " + value + ")";
        }

        public override string func_LENGTH(string value)
        {
            return @"LEN(" + value + ")";
        }

        public override string func_ST_X(string pt)
        {
            return pt + ".STX";
        }

        public override string func_ST_Y(string pt)
        {
            return pt + ".STY";
        }

        public override string func_ST_Contains(string g1, string g2)
        {
            return g1 + ".STContains(" + g2 + ")";
        }

        public override string func_ST_GeomFromText(string text, string srid = null)
        {
            return "geometry::STGeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override string func_ST_GeogFromText(string text, string srid = null)
        {
            return "geography::STGeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public override string type_TINYINT { get { return @"TINYINT"; } }
        public override string type_UNSIGNEDTINYINT { get { return @"TINYINT"; } }
        public override string type_SMALLINT { get { return @"SMALLINT"; } }
        public override string type_UNSIGNEDSMALLINT { get { return @"SMALLINT"; } }
        public override string type_INT { get { return @"INT"; } }
        public override string type_UNSIGNEDINT { get { return @"INT"; } }
        public override string type_BIGINT { get { return @"BIGINT"; } }
        public override string type_UNSIGNEDBIGINT { get { return @"BIGINT"; } }
        public override string type_NUMERIC { get { return @"NUMERIC"; } }
        public override string type_DECIMAL { get { return @"DECIMAL"; } }
        public override string type_MONEY { get { return @"MONEY"; } }
        public override string type_FLOAT { get { return @"FLOAT(4)"; } }
        public override string type_DOUBLE { get { return @"FLOAT(8)"; } }
        public override string type_VARCHAR { get { return @"NVARCHAR"; } }
        public override string type_CHAR { get { return @"NCHAR"; } }
        public override string type_TEXT { get { return @"NTEXT"; } }
        public override string type_MEDIUMTEXT { get { return @"NTEXT"; } }
        public override string type_LONGTEXT { get { return @"NTEXT"; } }
        public override string type_BOOLEAN { get { return @"bit"; } }
        public override string type_DATETIME { get { return @"DATETIME"; } }
        public override string type_BLOB { get { return @"IMAGE"; } }
        public override string type_GUID { get { return @"UNIQUEIDENTIFIER"; } }
        public override string type_AUTOINCREMENT { get { return @"IDENTITY"; } }
        public override string type_AUTOINCREMENT_BIGINT { get { return @"IDENTITY"; } }
        public override string type_JSON { get { return @"TEXT"; } }
        public override string type_JSON_BINARY { get { return @"TEXT"; } }

        public override string type_GEOMETRY { get { return @"GEOMETRY"; } }
        public override string type_GEOMETRYCOLLECTION { get { return @"GEOMETRY"; } }
        public override string type_POINT { get { return @"GEOMETRY"; } }
        public override string type_LINESTRING { get { return @"GEOMETRY"; } }
        public override string type_POLYGON { get { return @"GEOMETRY"; } }
        public override string type_LINE { get { return @"GEOMETRY"; } }
        public override string type_CURVE { get { return @"GEOMETRY"; } }
        public override string type_SURFACE { get { return @"GEOMETRY"; } }
        public override string type_LINEARRING { get { return @"GEOMETRY"; } }
        public override string type_MULTIPOINT { get { return @"GEOMETRY"; } }
        public override string type_MULTILINESTRING { get { return @"GEOMETRY"; } }
        public override string type_MULTIPOLYGON { get { return @"GEOMETRY"; } }
        public override string type_MULTICURVE { get { return @"GEOMETRY"; } }
        public override string type_MULTISURFACE { get { return @"GEOMETRY"; } }

        public override string type_GEOGRAPHIC { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHICCOLLECTION { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_POINT { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_LINESTRING { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_POLYGON { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_LINE { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_CURVE { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_SURFACE { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_LINEARRING { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTIPOINT { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTILINESTRING { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTIPOLYGON { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTICURVE { get { return @"GEOGRAPHIC"; } }
        public override string type_GEOGRAPHIC_MULTISURFACE { get { return @"GEOGRAPHIC"; } }

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
