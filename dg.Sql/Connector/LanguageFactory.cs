using System;
using System.Globalization;
using System.Text;

namespace dg.Sql.Connector
{
    public class LanguageFactory
    {
        #region Syntax

        public virtual int varchar_MAX_VALUE
        {
            get { return 255; }
        }

        public virtual string varchar_MAX
        {
            get { return null; } // Not supported
        }

        public virtual string func_UTC_NOW()
        {
            return @"NOW()";
        }

        public virtual string func_LOWER(string value)
        {
            return @"LOWER(" + value + ")";
        }

        public virtual string func_UPPER(string value)
        {
            return @"UPPER(" + value + ")";
        }

        public virtual string func_LENGTH(string value)
        {
            return @"LENGTH(" + value + ")";
        }

        public virtual string func_YEAR(string date)
        {
            return @"YEAR(" + date + ")";
        }

        public virtual string func_MONTH(string date)
        {
            return @"MONTH(" + date + ")";
        }

        public virtual string func_DAY(string date)
        {
            return @"DAY(" + date + ")";
        }

        public virtual string func_HOUR(string date)
        {
            return @"HOUR(" + date + ")";
        }

        public virtual string func_MINUTE(string date)
        {
            return @"MINUTE(" + date + ")";
        }

        public virtual string func_SECOND(string date)
        {
            return @"SECONDS(" + date + ")";
        }

        public virtual string func_MD5_Hex(string value)
        {
            return @"MD5(" + value + ")";
        }

        public virtual string func_SHA1_Hex(string value)
        {
            return @"SHA1(" + value + ")";
        }

        public virtual string func_MD5_Binary(string value)
        {
            return @"UNHEX(MD5(" + value + "))";
        }

        public virtual string func_SHA1_Binary(string value)
        {
            return @"UNHEX(SHA1(" + value + "))";
        }

        public virtual string func_ST_X(string pt)
        {
            return "ST_X(" + pt + ")";
        }

        public virtual string func_ST_Y(string pt)
        {
            return "ST_Y(" + pt + ")";
        }

        public virtual string func_ST_Contains(string g1, string g2)
        {
            return "ST_Contains(" + g1 + ", " + g2 + ")";
        }

        public virtual string func_ST_GeomFromText(string text, string srid = null)
        {
            return "ST_GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public virtual string func_ST_GeogFromText(string text, string srid = null)
        {
            return "ST_GeogFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public virtual void BuildNullSafeEqualsTo(
            Where where,
            bool negate,
            StringBuilder outputBuilder,
            Where.BuildContext context)
        {
            var wl = new WhereList();

            wl.Add(new Where
            {
                Condition = WhereCondition.OR,
                FirstTableName = where.FirstTableName,
                First = where.First,
                FirstType = where.FirstType,
                Comparison = negate ? WhereComparison.NotEqualsTo : WhereComparison.EqualsTo,
                SecondTableName = where.SecondTableName,
                Second = where.Second,
                SecondType = where.SecondType,
            });

            wl.Add(new Where(WhereCondition.OR,
                    new Where
                    {
                        FirstTableName = where.FirstTableName,
                        First = where.First,
                        FirstType = where.FirstType,
                        Comparison = WhereComparison.Is,
                        Second = null,
                        SecondType = ValueObjectType.Value,
                    },
                    ValueObjectType.Value,
                    negate ? WhereComparison.NotEqualsTo : WhereComparison.EqualsTo,
                    new Where
                    {
                        FirstTableName = where.SecondTableName,
                        First = where.Second,
                        FirstType = where.SecondType,
                        Comparison = WhereComparison.Is,
                        Second = null,
                        SecondType = ValueObjectType.Value,
                    },
                    ValueObjectType.Value
                ));

            wl.BuildCommand(outputBuilder, context);
        }

        public virtual string type_AUTOINCREMENT { get { return @"AUTOINCREMENT"; } }
        public virtual string type_AUTOINCREMENT_BIGINT { get { return @"AUTOINCREMENT"; } }

        public virtual string type_TINYINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDTINYINT { get { return @"INT"; } }
        public virtual string type_SMALLINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDSMALLINT { get { return @"INT"; } }
        public virtual string type_INT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDINT { get { return @"INT"; } }
        public virtual string type_BIGINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDBIGINT { get { return @"INT"; } }
        public virtual string type_NUMERIC { get { return @"NUMERIC"; } }
        public virtual string type_DECIMAL { get { return @"DECIMAL"; } }
        public virtual string type_MONEY { get { return @"DECIMAL"; } }
        public virtual string type_FLOAT { get { return @"FLOAT"; } }
        public virtual string type_DOUBLE { get { return @"DOUBLE"; } }
        public virtual string type_VARCHAR { get { return @"NVARCHAR"; } }
        public virtual string type_CHAR { get { return @"NCHAR"; } }
        public virtual string type_TEXT { get { return @"NTEXT"; } }
        public virtual string type_MEDIUMTEXT { get { return @"NTEXT"; } }
        public virtual string type_LONGTEXT { get { return @"NTEXT"; } }
        public virtual string type_BOOLEAN { get { return @"BOOLEAN"; } }
        public virtual string type_DATETIME { get { return @"DATETIME"; } }
        public virtual string type_BLOB { get { return @"BLOB"; } }
        public virtual string type_GUID { get { return @"GUID"; } }
        public virtual string type_JSON { get { return @"TEXT"; } }
        public virtual string type_JSON_BINARY { get { return @"TEXT"; } }

        public virtual string type_GEOMETRY { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOMETRYCOLLECTION { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_POINT { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_LINESTRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_POLYGON { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_LINE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_CURVE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_SURFACE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_LINEARRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTIPOINT { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTILINESTRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTIPOLYGON { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTICURVE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_MULTISURFACE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }

        public virtual string type_GEOGRAPHIC { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHICCOLLECTION { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_POINT { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_LINESTRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_POLYGON { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_LINE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_CURVE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_SURFACE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_LINEARRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTIPOINT { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTILINESTRING { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTIPOLYGON { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTICURVE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }
        public virtual string type_GEOGRAPHIC_MULTISURFACE { get { throw new NotImplementedException(@"Geospatial data types not supported in this database"); } }

        #endregion

        #region Preparing values for SQL

        public virtual string WrapFieldName(string fieldName)
        {
            return '`' + fieldName.Replace("`", "``") + '`';
        }

        public virtual string EscapeString(string value)
        {
            return value.Replace(@"'", @"''");
        }

        public virtual string PrepareValue(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public virtual string PrepareValue(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public virtual string PrepareValue(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public virtual string PrepareValue(bool value)
        {
            return value ? "1" : "0";
        }

        public virtual string PrepareValue(Guid value)
        {
            return '\'' + value.ToString("D") + '\'';
        }

        public virtual string PrepareValue(string value)
        {
            if (value == null) return "NULL";
            else return '\'' + EscapeString(value) + '\'';
        }

        public virtual string PrepareValue(ConnectorBase conn, object value, Query relatedQuery = null)
        {
            if (value == null || value is DBNull)
            {
                return "NULL";
            }
            else if (value is string)
            {
                return PrepareValue((string)value);
            }
            else if (value is DateTime)
            {
                return '\'' + FormatDate((DateTime)value) + '\'';
            }
            else if (value is Guid)
            {
                return PrepareValue((Guid)value);
            }
            else if (value is bool)
            {
                return PrepareValue((bool)value);
            }
            else if (value is decimal)
            {
                return PrepareValue((decimal)value);
            }
            else if (value is float)
            {
                return PrepareValue((float)value);
            }
            else if (value is double)
            {
                return PrepareValue((double)value);
            }
            else if (value is IPhrase)
            {
                return ((IPhrase)value).BuildPhrase(conn, relatedQuery);
            }
            else if (value is Geometry)
            {
                var sb = new StringBuilder();
                ((Geometry)value).BuildValue(sb, conn);
                return sb.ToString();
            }
            else if (value is Where)
            {
                var sb = new StringBuilder();
                ((Where)value).BuildCommand(sb, true, new Where.BuildContext
                {
                    Conn = conn,
                    RelatedQuery = relatedQuery
                });
                return sb.ToString();
            }
            else if (value is WhereList)
            {
                var sb = new StringBuilder();
                ((WhereList)value).BuildCommand(sb, new Where.BuildContext
                {
                    Conn = conn,
                    RelatedQuery = relatedQuery
                });
                return sb.ToString();
            }
            else if (value.GetType().BaseType.Name == "Enum")
            {
                var underlyingValue = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
                if (underlyingValue is string || underlyingValue is char)
                {
                    return PrepareValue(underlyingValue.ToString());
                }

                return underlyingValue.ToString();
            }
            else return value.ToString();
        }

        public virtual string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public virtual string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%");
        }

        public virtual string LikeEscapingStatement
        {
            get { return @"ESCAPE('\')"; }
        }

        #endregion
    }
}
