using System;
using System.Globalization;
using System.Text;

namespace SequelNet.Connector
{
    public class LanguageFactory
    {
        #region Syntax

        public virtual bool IsBooleanFalseOrderedFirst => true;

        public virtual bool UpdateFromInsteadOfJoin => false;
        public virtual bool UpdateJoinRequiresFromLeftTable => false;

        public virtual bool GroupBySupportsOrdering => false;

        public virtual bool DeleteSupportsIgnore => false;
        public virtual bool InsertSupportsIgnore => false;

        public virtual bool SupportsColumnComment => false;
        public virtual ColumnSRIDLocationMode ColumnSRIDLocation => ColumnSRIDLocationMode.InType;

        public virtual int VarCharMaxLength => 255;

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
            throw new NotImplementedException("ST_Contains has not been implemented for this connector");
        }

        public virtual string ST_Distance(string g1, string g2)
        {
            throw new NotImplementedException("ST_Distance has not been implemented for this connector");
        }

        public virtual string ST_Distance_Sphere(string g1, string g2)
        {
            throw new NotImplementedException("ST_Distance_Sphere has not been implemented for this connector");
        }

        public virtual string ST_GeomFromText(string text, string srid = null, bool literalText = false)
        {
            throw new NotImplementedException("ST_GeomFromText has not been implemented for this connector");
        }

        public virtual string ST_GeogFromText(string text, string srid = null, bool literalText = false)
        {
            throw new NotImplementedException("ST_GeogFromText has not been implemented for this connector");
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

        public virtual void BuildTableName(Query qry, ConnectorBase conn, StringBuilder sb, bool considerAlias = true)
        {
            if (qry.Schema != null)
            {
                if (qry.Schema.DatabaseOwner.Length > 0)
                {
                    sb.Append(WrapFieldName(qry.Schema.DatabaseOwner));
                    sb.Append('.');
                }
                sb.Append(WrapFieldName(qry.SchemaName));
            }
            else
            {
                sb.Append(@"(");

                if (qry.FromExpression is IPhrase)
                {
                    sb.Append(((IPhrase)qry.FromExpression).BuildPhrase(conn, qry));
                }
                else sb.Append(qry.FromExpression);

                sb.Append(@")");
            }

            if (considerAlias && !string.IsNullOrEmpty(qry.SchemaAlias))
            {
                sb.Append(" " + WrapFieldName(qry.SchemaAlias));
            }
        }

        public virtual void BuildColumnPropertiesDataType(
            StringBuilder sb,
            ConnectorBase connection, 
            TableSchema.Column column,
            Query relatedQuery,
            out bool isDefaultAllowed)
        {
            throw new NotImplementedException("BuildColumnPropertiesDataType has not been implemented for this connector");
        }

        public virtual void BuildLimitOffset(
            Query query,
            bool top,
            StringBuilder outputBuilder)
        {
            throw new NotImplementedException("Paging syntax has not been implemented for this connector");
        }

        public virtual void BuildCreateIndex(
            Query qry,
            ConnectorBase conn,
            TableSchema.Index index,
            StringBuilder outputBuilder)
        {
            throw new NotImplementedException("Index syntax has not been implemented for this connector");
        }

        public virtual void BuildOrderByRandom(ValueWrapper seedValue, ConnectorBase conn, StringBuilder outputBuilder)
        {
            outputBuilder.Append(@"RAND()");
        }

        public virtual string Aggregate_Some(string rawExpression)
        {
            throw new NotImplementedException("SOME has not been implemented for this connector");
        }

        public virtual string Aggregate_Every(string rawExpression)
        {
            throw new NotImplementedException("EVERY has not been implemented for this connector");
        }

        public virtual string GroupConcat(bool distinct, string rawExpression, string rawOrderBy, string separator)
        {
            throw new NotImplementedException("GROUP_CONCAT has not been implemented for this connector");
        }

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
            else if (value is char)
            {
                return PrepareValue(((char)value).ToString());
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

        public enum ColumnSRIDLocationMode
        {
            InType,
            AfterNullability
        }
    }
}
