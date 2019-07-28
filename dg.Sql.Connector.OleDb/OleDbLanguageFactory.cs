using System;

namespace dg.Sql.Connector
{
    public class OleDbLanguageFactory : LanguageFactory
    {
        #region Syntax

        public override string func_UTC_NOW()
        {
            return @"now()"; // NOT UTC
        }

        public override string func_LOWER(string value)
        {
            return @"LCASE(" + value + ")";
        }

        public override string func_UPPER(string value)
        {
            return @"UCASE(" + value + ")";
        }

        public override string func_LENGTH(string value)
        {
            return @"LEN(" + value + ")";
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

        public override string type_TINYINT { get { return @"BYTE"; } }
        public override string type_UNSIGNEDTINYINT { get { return @"TINYINT"; } }
        public override string type_SMALLINT { get { return @"SHORT"; } }
        public override string type_UNSIGNEDSMALLINT { get { return @"SHORT"; } }
        public override string type_INT { get { return @"INT"; } }
        public override string type_UNSIGNEDINT { get { return @"INT"; } }
        public override string type_BIGINT { get { return @"INT"; } }
        public override string type_UNSIGNEDBIGINT { get { return @"INT"; } }
        public override string type_NUMERIC { get { return @"NUMERIC"; } }
        public override string type_DECIMAL { get { return @"DECIMAL"; } }
        public override string type_MONEY { get { return @"DECIMAL"; } }
        public override string type_FLOAT { get { return @"SINGLE"; } }
        public override string type_DOUBLE { get { return @"DOUBLE"; } }
        public override string type_VARCHAR { get { return @"VARCHAR"; } }
        public override string type_CHAR { get { return @"CHAR"; } }
        public override string type_TEXT { get { return @"TEXT"; } }
        public override string type_MEDIUMTEXT { get { return @"TEXT"; } }
        public override string type_LONGTEXT { get { return @"TEXT"; } }
        public override string type_BOOLEAN { get { return @"BIT"; } }
        public override string type_DATETIME { get { return @"DATETIME"; } }
        public override string type_GUID { get { return @"UNIQUEIDENTIFIER"; } }
        public override string type_BLOB { get { return @"IMAGE"; } }
        public override string type_AUTOINCREMENT { get { return @"AUTOINCREMENT"; } }
        public override string type_AUTOINCREMENT_BIGINT { get { return @"AUTOINCREMENT"; } }
        public override string type_JSON { get { return @"TEXT"; } }
        public override string type_JSON_BINARY { get { return @"TEXT"; } }

        #endregion

        #region Preparing values for SQL

        public override string WrapFieldName(string fieldName)
        {
            return '[' + fieldName + ']';
        }

        public override string EscapeString(string value)
        {
            return value.Replace(@"'", @"''");
        }

        public override string PrepareValue(Guid value)
        {
            return '\'' + value.ToString(@"D") + '\'';
        }

        public override string PrepareValue(bool value)
        {
            return value ? @"true" : @"false";
        }

        public override string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public override string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%").Replace(@"_", @"\_").Replace("[", "[[]");
        }

        #endregion
    }
}
