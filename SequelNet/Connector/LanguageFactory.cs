using System;
using System.Globalization;
using System.Text;

namespace SequelNet.Connector;

public class LanguageFactory
{
    #region Syntax

    public virtual Func<Query, ConnectorBase, Exception, int> OnExecuteNonQueryException => null;
    public virtual Func<Query, ConnectorBase, Exception, System.Threading.Tasks.Task<int>> OnExecuteNonQueryExceptionAsync => null;

    public virtual bool IsBooleanFalseOrderedFirst => true;

    public virtual bool UpdateFromInsteadOfJoin => false;
    public virtual bool UpdateJoinRequiresFromLeftTable => false;

    public virtual bool GroupBySupportsOrdering => false;

    public virtual bool DeleteSupportsIgnore => false;
    public virtual bool InsertSupportsIgnore => false;
    public virtual bool InsertSupportsOnConflictDoNothing => false;
    public virtual bool InsertSupportsOnConflictDoUpdate => false;
    public virtual bool InsertSupportsMerge => false;
    public virtual bool UpdateSupportsIgnore => false;

    public virtual bool SupportsColumnComment => false;
    public virtual ColumnSRIDLocationMode ColumnSRIDLocation => ColumnSRIDLocationMode.InType;

    public virtual bool SupportsRenameColumn => true;
    public virtual bool HasSeparateRenameColumn => false;
    public virtual Func<TableSchema.Index, bool> HasSeparateCreateIndex => (index) => false;
    public virtual Func<AlterTableType, AlterTableType, bool> IsAlterTableTypeMixCompatible => (type1, type2) => true;
    public virtual bool SupportsMultipleAlterTable => true;
    public virtual bool SupportsMultipleAlterColumn => true;
    public virtual bool SameAlterTableCommandsAreCommaSeparated => false;
    public virtual string AlterTableAddCommandName => "";
    public virtual string AlterTableAddColumnCommandName => "ADD COLUMN";
    public virtual string AlterTableAddIndexCommandName => "ADD CONSTRAINT";
    public virtual string AlterTableAddForeignKeyCommandName => "ADD CONSTRAINT";
    public virtual string ChangeColumnCommandName => "ALTER COLUMN";
    public virtual string DropColumnCommandName => "DROP COLUMN";
    public virtual string DropIndexCommandName => "DROP INDEX";
    public virtual string DropForeignKeyCommandName => "DROP CONSTRAINT";

    public virtual int VarCharMaxLength => 255;

    public virtual string UtcNow()
    {
        return @"NOW()";
    }

    public virtual void BuildConvertUtcToTz(
        ValueWrapper value, ValueWrapper timeZone, 
        StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        value.Build(sb, conn, relatedQuery);
        sb.Append(" AT TIME ZONE 'UTC' AT TIME ZONE ");
        timeZone.Build(sb, conn, relatedQuery);
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

    public virtual string YearPartOfDateTime(string date)
    {
        return @"YEAR(" + date + ")";
    }

    public virtual string MonthPartOfDateTime(string date)
    {
        return @"MONTH(" + date + ")";
    }

    public virtual string DayPartOfDateTime(string date)
    {
        return @"DAY(" + date + ")";
    }

    public virtual string HourPartOfDateOrTime(string date)
    {
        return @"HOUR(" + date + ")";
    }

    public virtual string MinutePartOfDateOrTime(string date)
    {
        return @"MINUTE(" + date + ")";
    }

    public virtual string SecondPartOfDateOrTime(string date)
    {
        return @"SECONDS(" + date + ")";
    }

    public virtual string DatePartOfDateTime(string date)
    {
        return @"DATE(" + date + ")";
    }

    public virtual string TimePartOfDateTime(string date)
    {
        return @"TIME(" + date + ")";
    }

    public virtual string ExtractUnixTimestamp(string date)
    {
        return $"UNIX_TIMESTAMP({date})";
    }

    public virtual string NullOrDefaultValue(string expression, string defaultValue)
    {
        return $"IFNULL({expression}, {defaultValue})";
    }

    public virtual string IfEqualThenNull(string value1expr, string value2expr)
    {
        return $"NULLIF({value1expr}, {value2expr})";
    }

    public virtual string DateTimeFormat(string date, Phrases.DateTimeFormat.FormatOptions format)
    {
        throw new NotImplementedException("DateTimeFormat has not been implemented for this connector");
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

    public virtual void BuildConflictColumnUpdate(
        StringBuilder sb, ConnectorBase conn,
        ConflictColumn column, Query relatedQuery)
    {
        throw new NotImplementedException("BuildConflictColumnUpdate has not been implemented for this connector");
    }

    public virtual void BuildOnConflictSetMerge(StringBuilder sb, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
    {
        throw new NotImplementedException("BuildOnConflictSetMerge has not been implemented for this connector");
    }

    public virtual void BuildOnConflictDoUpdate(StringBuilder outputBuilder, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
    {
        throw new NotImplementedException("BuildOnConflictSet has not been implemented for this connector");
    }

    public virtual void BuildOnConflictDoNothing(StringBuilder outputBuilder, ConnectorBase conn, OnConflict conflict, Query relatedQuery)
    {
        throw new NotImplementedException("BuildOnConflictIgnore has not been implemented for this connector");
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
                ((IPhrase)qry.FromExpression).Build(sb, conn, qry);
            }
            else sb.Append(qry.FromExpression);

            sb.Append(@")");
        }

        if (considerAlias && !string.IsNullOrEmpty(qry.SchemaAlias))
        {
            sb.Append(" " + WrapFieldName(qry.SchemaAlias));
        }
    }

    public virtual void BuildColumnProperties(TableSchema.Column column, bool noDefault, StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        sb.Append(WrapFieldName(column.Name));
        sb.Append(' ');

        BuildColumnPropertiesDataType(
            column: column,
            isDefaultAllowed: out var isDefaultAllowed,
            sb: sb,
            connection: conn,
            relatedQuery: relatedQuery);

        if (!string.IsNullOrEmpty(column.Comment) && SupportsColumnComment)
        {
            sb.AppendFormat(@" COMMENT {0}",
                PrepareValue(column.Comment));
        }

        if (!column.Nullable)
        {
            sb.Append(@" NOT NULL");
        }

        if (column.SRID != null && ColumnSRIDLocation == ColumnSRIDLocationMode.AfterNullability)
        {
            sb.Append(" SRID " + column.SRID.Value);
        }

        if (column.ComputedColumn == null)
        {
            if (!noDefault && (column.Default != null || column.HasDefault) && (isDefaultAllowed || column.Default == null))
            {
                if (column.Default == null)
                {
                    sb.Append(@" DEFAULT NULL");
                }
                else
                {
                    sb.Append(@" DEFAULT ");
                    Query.PrepareColumnValue(column, column.Default, sb, conn, relatedQuery);
                }
            }
        }

        sb.Append(' ');
    }

    public virtual void BuildColumnPropertiesDataType(
        TableSchema.Column column,
        out bool isDefaultAllowed,
        StringBuilder sb,
        ConnectorBase connection,
        Query relatedQuery)
    {
        throw new NotImplementedException("BuildColumnPropertiesDataType has not been implemented for this connector");
    }

    public virtual (string typeString, bool isDefaultAllowed) BuildDataTypeDef(DataTypeDef typeDef, bool forCast = false)
    {
        throw new NotImplementedException("BuildDataTypeDef has not been implemented for this connector");
    }

    public virtual void BuildCollate(
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

        // COLLATE ASC/DESC not supported in most RDBMSes

        sb.Append(")");
    }

    public virtual void BuildLimitOffset(
        Query query,
        bool top,
        StringBuilder outputBuilder)
    {
        throw new NotImplementedException("Paging syntax has not been implemented for this connector");
    }

    public virtual void BuildCreateIndex(
        TableSchema.Index index,
        StringBuilder outputBuilder,
        Query qry,
        ConnectorBase conn)
    {
        throw new NotImplementedException("Index syntax has not been implemented for this connector");
    }

    public virtual void BuildCreateForeignKey(TableSchema.ForeignKey foreignKey, StringBuilder sb, ConnectorBase connection)
    {
        sb.Append(WrapFieldName(foreignKey.Name));
        sb.Append(@" FOREIGN KEY (");

        for (int i = 0; i < foreignKey.Columns.Length; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append(WrapFieldName(foreignKey.Columns[i]));
        }
        sb.AppendFormat(@") REFERENCES {0} (", foreignKey.ForeignTable);
        for (int i = 0; i < foreignKey.ForeignColumns.Length; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append(WrapFieldName(foreignKey.ForeignColumns[i]));
        }
        sb.Append(@")");
        if (foreignKey.OnDelete != TableSchema.ForeignKeyReference.None)
        {
            switch (foreignKey.OnDelete)
            {
                case TableSchema.ForeignKeyReference.Cascade:
                    sb.Append(@" ON DELETE CASCADE");
                    break;
                case TableSchema.ForeignKeyReference.NoAction:
                    sb.Append(@" ON DELETE NO ACTION");
                    break;
                case TableSchema.ForeignKeyReference.Restrict:
                    sb.Append(@" ON DELETE RESTRICT");
                    break;
                case TableSchema.ForeignKeyReference.SetNull:
                    sb.Append(@" ON DELETE SET NULL");
                    break;
            }
        }

        if (foreignKey.OnUpdate != TableSchema.ForeignKeyReference.None)
        {
            switch (foreignKey.OnUpdate)
            {
                case TableSchema.ForeignKeyReference.Cascade:
                    sb.Append(@" ON UPDATE CASCADE");
                    break;
                case TableSchema.ForeignKeyReference.NoAction:
                    sb.Append(@" ON UPDATE NO ACTION");
                    break;
                case TableSchema.ForeignKeyReference.Restrict:
                    sb.Append(@" ON UPDATE RESTRICT");
                    break;
                case TableSchema.ForeignKeyReference.SetNull:
                    sb.Append(@" ON UPDATE SET NULL");
                    break;
            }
        }
    }

    public virtual void BuildOrderByRandom(ValueWrapper? seedValue, ConnectorBase conn, StringBuilder outputBuilder)
    {
        outputBuilder.Append(@"RAND()");
    }

    public virtual void BuildJsonExtract(
        ValueWrapper value, JsonPathExpression path, bool unquote,
        StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        throw new NotImplementedException("JSON_EXTRACT has not been implemented for this connector");
    }

    public virtual void BuildJsonContains(
        ValueWrapper target, ValueWrapper candidate, JsonPathExpression path,
        StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        throw new NotImplementedException("JSON_CONTAINS has not been implemented for this connector");
    }

    public virtual void BuildMemberOfJsonArray(
        ValueWrapper value, ValueWrapper array,
        StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        throw new NotImplementedException("MEMBER OF has not been implemented for this connector");
    }

    public virtual void BuildJsonExtractValue(
        ValueWrapper value, JsonPathExpression path, 
        DataTypeDef returnType,
        Phrases.JsonValue.DefaultAction onEmptyAction, object onEmptyValue,
        Phrases.JsonValue.DefaultAction onErrorAction, object onErrorValue,
        StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        throw new NotImplementedException("JSON_VALUE has not been implemented for this connector");
    }

    public virtual void BuildJsonArrayAggregate(
        ValueWrapper value, bool isBinary,
        StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        throw new NotImplementedException("JSON_ARRAYAGG has not been implemented for this connector");
    }

    public virtual void BuildJsonObjectAggregate(
        ValueWrapper key, ValueWrapper value, bool isBinary,
        StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        throw new NotImplementedException("JSON_OBJECTAGG has not been implemented for this connector");
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

    public virtual void BuildSeparateRenameColumn(
        Query qry,
        ConnectorBase conn,
        AlterTableQueryData alterData,
        StringBuilder outputBuilder)
    {
        throw new NotImplementedException("BuildSeparateRenameColumn has not been implemented for this connector");
    }

    public virtual void BuildChangeColumn(AlterTableQueryData alterData, StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        BuildColumnProperties(alterData.Column, false, sb, conn, relatedQuery);
    }

    public virtual void BuildAddColumn(AlterTableQueryData alterData, StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        BuildColumnProperties(alterData.Column, false, sb, conn, relatedQuery);
    }

    public virtual void BuildDropPrimaryKey(AlterTableQueryData alterData, StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        var pk = relatedQuery.Schema.Indexes.Find(x => x.Mode == TableSchema.IndexMode.PrimaryKey);
        if (pk == null)
            throw new InvalidOperationException("No primary key found in schema. This dialect requires the primary key name to be specified.");
        
        sb.Append($"DROP CONSTRAINT {WrapFieldName(pk.Name)}");
    }

    public virtual string BuildFindString(
        ConnectorBase conn,
        ValueWrapper needle,
        ValueWrapper haystack,
        ValueWrapper? startAt,
        Query relatedQuery)
    {
        throw new NotImplementedException(@"BuildFindString not implemented for this connector");
    }

    public virtual string BuildSubstring(
        ConnectorBase conn,
        ValueWrapper value,
        ValueWrapper from,
        ValueWrapper? length,
        Query relatedQuery)
    {
        string ret = "SUBSTRING(";

        ret += value.Build(conn, relatedQuery);

        ret += " FROM " + from.Build(conn, relatedQuery);

        if (length != null)
        {
            ret += " FOR " + length.Value.Build(conn, relatedQuery);
        }

        ret += ")";

        return ret;
    }

    public virtual void BuildIndexHints(
        IndexHintList hints,
        StringBuilder sb, ConnectorBase conn, Query relatedQuery)
    {
        // Not supported in most databases, do not make a fuss about it
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
            return '\'' + FormatDateTime((DateTime)value) + '\'';
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
            var sb = new StringBuilder();
            ((IPhrase)value).Build(sb, conn, relatedQuery);
            return sb.ToString();
        }
        else if (value is ValueWrapper)
        {
            return ((ValueWrapper)value).Build(conn, relatedQuery);
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
        else if (value is Enum)
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

    public virtual string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public virtual string FormatCreateDate(int year, int month, int day)
    {
        return "DATE'" +
            year.ToString().PadLeft(4, '0') + '-' +
            month.ToString().PadLeft(2, '0') + '-' +
            day.ToString().PadLeft(2, '0') + "'";
    }

    public virtual string FormatCreateTime(int hours, int minutes, int seconds, int milliseconds)
    {
        return "TIME'" +
            hours.ToString().PadLeft(2, '0') + ':' +
            minutes.ToString().PadLeft(2, '0') + ':' +
            seconds.ToString().PadLeft(2, '0') + '.' +
            milliseconds.ToString().PadLeft(3, '0') + "'";
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
