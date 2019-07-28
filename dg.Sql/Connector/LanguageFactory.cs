﻿using System;
using System.Globalization;
using System.Text;

namespace dg.Sql.Connector
{
    public class LanguageFactory
    {
        #region Syntax

        public virtual int VarCharMaxLength => 255;

        public virtual string VarCharMaxKeyword => null;

        public virtual string UtcNow()
        {
            return @"NOW()";
        }

        public virtual string StringToLower(string value)
        {
            return @"LOWER(" + value + ")";
        }

        public virtual string StringToUpper(string value)
        {
            return @"UPPER(" + value + ")";
        }

        public virtual string LengthOfString(string value)
        {
            return @"LENGTH(" + value + ")";
        }

        public virtual string YearPartOfDate(string date)
        {
            return @"YEAR(" + date + ")";
        }

        public virtual string MonthPartOfDate(string date)
        {
            return @"MONTH(" + date + ")";
        }

        public virtual string DayPartOfDate(string date)
        {
            return @"DAY(" + date + ")";
        }

        public virtual string HourPartOfDate(string date)
        {
            return @"HOUR(" + date + ")";
        }

        public virtual string MinutePartOfDate(string date)
        {
            return @"MINUTE(" + date + ")";
        }

        public virtual string SecondPartOfDate(string date)
        {
            return @"SECONDS(" + date + ")";
        }

        public virtual string Md5Hex(string value)
        {
            return @"MD5(" + value + ")";
        }

        public virtual string Sha1Hex(string value)
        {
            return @"SHA1(" + value + ")";
        }

        public virtual string Md5Binary(string value)
        {
            return @"UNHEX(MD5(" + value + "))";
        }

        public virtual string Sha1Binary(string value)
        {
            return @"UNHEX(SHA1(" + value + "))";
        }

        public virtual string ST_X(string pt)
        {
            return "ST_X(" + pt + ")";
        }

        public virtual string ST_Y(string pt)
        {
            return "ST_Y(" + pt + ")";
        }

        public virtual string ST_Contains(string g1, string g2)
        {
            return "ST_Contains(" + g1 + ", " + g2 + ")";
        }

        public virtual string ST_GeomFromText(string text, string srid = null)
        {
            return "ST_GeomFromText(" + PrepareValue(text) + (string.IsNullOrEmpty(srid) ? "" : "," + srid) + ")";
        }

        public virtual string ST_GeogFromText(string text, string srid = null)
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

        #endregion

        #region Types

        public virtual string AutoIncrementType => @"AUTOINCREMENT";
        public virtual string AutoIncrementBigIntType => @"AUTOINCREMENT";

        public virtual string TinyIntType => @"INT";
        public virtual string UnsignedTinyIntType => @"INT";
        public virtual string SmallIntType => @"INT";
        public virtual string UnsignedSmallIntType => @"INT";
        public virtual string IntType => @"INT";
        public virtual string UnsignedIntType => @"INT";
        public virtual string BigIntType => @"INT";
        public virtual string UnsignedBigIntType => @"INT";
        public virtual string NumericType => @"NUMERIC";
        public virtual string DecimalType => @"DECIMAL";
        public virtual string MoneyType => @"DECIMAL";
        public virtual string FloatType => @"FLOAT";
        public virtual string DoubleType => @"DOUBLE";
        public virtual string VarCharType => @"NVARCHAR";
        public virtual string CharType => @"NCHAR";
        public virtual string TextType => @"NTEXT";
        public virtual string MediumTextType => @"NTEXT";
        public virtual string LongTextType => @"NTEXT";
        public virtual string BooleanType => @"BOOLEAN";
        public virtual string DateTimeType => @"DATETIME";
        public virtual string BlobType => @"BLOB";
        public virtual string GuidType => @"GUID";
        public virtual string JsonType => @"TEXT";
        public virtual string JsonBinaryType => @"TEXT";

        public virtual string TypeGeometry => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeometryCollectionType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string PointType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string LineStringType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string PolygonType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string LineType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string CurveType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string SurfaceType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string LinearRingType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string MultiPointType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string MultiLineStringType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string MultiPolygonType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string MultiCurveType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string MultiSurfaceType => throw new NotImplementedException(@"Geospatial data types not supported in this database");

        public virtual string GeographicType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicCollectionType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicPointType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicLinestringType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicPolygonType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicLineType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicCurveType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicSurfaceType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicLinearringType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicMultipointType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicMultilinestringType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicMultipolygonType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicMulticurveType => throw new NotImplementedException(@"Geospatial data types not supported in this database");
        public virtual string GeographicMultisurfaceType => throw new NotImplementedException(@"Geospatial data types not supported in this database");

        #endregion

        #region Reading values from SQL

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column name.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="IndexOutOfRangeException">No column with the specified name was found</exception>
        public virtual Geometry ReadGeometry(object value)
        {
            throw new NotImplementedException(@"ReadGeometry not implemented for this connector");
        }

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

        public virtual string LikeEscapingStatement => @"ESCAPE('\')";

        #endregion
    }
}
