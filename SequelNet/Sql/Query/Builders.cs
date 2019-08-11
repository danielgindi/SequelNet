using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using SequelNet.Connector;

namespace SequelNet
{
    public partial class Query
    {
        private void BuildSelectList(StringBuilder sb, ConnectorBase connection)
        {
            if (_ListSelect == null || _ListSelect.Count == 0)
            {
                sb.Append("*");
            }
            else
            {
                var language = connection.Language;

                bool bFirst = true;
                foreach (SelectColumn sel in _ListSelect)
                {
                    if (bFirst) bFirst = false;
                    else sb.Append(',');

                    if (sel.ObjectType == ValueObjectType.Value)
                    {
                        if (sel.Value is Query)
                        {
                            sb.Append('(');
                            sb.Append(((Query)sel.Value).BuildCommand(connection));
                            sb.Append(')');
                        }
                        else
                        {
                            sb.Append(language.PrepareValue(connection, sel.Value, this));
                        }

                        if (!string.IsNullOrEmpty(sel.Alias))
                        {
                            sb.Append(@" AS ");
                            sb.Append(language.WrapFieldName(sel.Alias));
                        }
                    }
                    else if (sel.ObjectType == ValueObjectType.Literal)
                    {
                        if (string.IsNullOrEmpty(sel.Alias))
                        {
                            sb.Append(sel.Value.ToString());
                        }
                        else
                        {
                            sb.Append(sel.Value.ToString());
                            sb.Append(@" AS ");
                            sb.Append(language.WrapFieldName(sel.Alias));
                        }
                    }
                    else
                    {
                        if (_ListJoin != null && _ListJoin.Count > 0 && string.IsNullOrEmpty(sel.TableName))
                        {
                            if (_SchemaAlias != null)
                            {
                                sb.Append(language.WrapFieldName(_SchemaAlias));
                            }
                            else
                            {
                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(language.WrapFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(language.WrapFieldName(_SchemaName));
                            }

                            sb.Append('.');
                            sb.Append(sel.Value == null ? "*" : language.WrapFieldName(sel.Value.ToString()));
                            if (!string.IsNullOrEmpty(sel.Alias))
                            {
                                sb.Append(@" AS ");
                                sb.Append(language.WrapFieldName(sel.Alias));
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(sel.TableName))
                            {
                                sb.Append(language.WrapFieldName(sel.TableName));
                                sb.Append('.');
                            }
                            sb.Append(sel.Value == null ? "*" : language.WrapFieldName(sel.Value.ToString()));
                            if (!string.IsNullOrEmpty(sel.Alias))
                            {
                                sb.Append(@" AS ");
                                sb.Append(language.WrapFieldName(sel.Alias));
                            }
                        }
                    }
                }
            }
        }

        private void BuildJoin(StringBuilder sb, ConnectorBase connection)
        {
            if (_ListJoin != null)
            {
                foreach (Join join in _ListJoin)
                {
                    switch (join.JoinType)
                    {
                        case JoinType.InnerJoin:
                            sb.Append(@" INNER JOIN ");
                            break;
                        case JoinType.LeftJoin:
                            sb.Append(@" LEFT JOIN ");
                            break;
                        case JoinType.RightJoin:
                            sb.Append(@" RIGHT JOIN ");
                            break;
                        case JoinType.LeftOuterJoin:
                            sb.Append(@" LEFT OUTER JOIN ");
                            break;
                        case JoinType.RightOuterJoin:
                            sb.Append(@" RIGHT OUTER JOIN ");
                            break;
                        case JoinType.FullOuterJoin:
                            sb.Append(@" FULL OUTER JOIN ");
                            break;
                    }

                    if (join.RightTableSchema != null)
                    {
                        if (join.RightTableSchema.DatabaseOwner.Length > 0)
                        {
                            sb.Append(connection.Language.WrapFieldName(join.RightTableSchema.DatabaseOwner));
                            sb.Append('.');
                        }
                        sb.Append(connection.Language.WrapFieldName(join.RightTableSchema.Name));
                    }
                    else
                    {
                        sb.Append('(');
                        if (join.RightTableSql is Query)
                        {
                            sb.Append(((Query)join.RightTableSql).BuildCommand(connection));
                        }
                        else if (join.RightTableSql is IPhrase)
                        {
                            sb.Append(((IPhrase)join.RightTableSql).BuildPhrase(connection));
                        }
                        else
                        {
                            sb.Append(join.RightTableSql.ToString());
                        }
                        sb.Append(')');
                    }

                    sb.Append(' ');

                    if (join.RightTableAlias != null)
                    {
                        sb.Append(join.RightTableAlias);
                    }
                    else
                    {
                        sb.Append(join.RightTableSchema != null ? join.RightTableSchema.Name : @"");
                    }

                    if (join.Pairs.Count > 1)
                    {
                        sb.Append(@" ON ");

                        var wl = new WhereList();

                        foreach (var joins in join.Pairs)
                            wl.AddRange(joins);

                        wl.BuildCommand(
                            sb,
                            new Where.BuildContext {
                                Conn = connection,
                                RelatedQuery = this,
                                RightTableSchema = join.RightTableSchema, 
                                RightTableName = join.RightTableAlias == null ? join.RightTableSchema.Name : join.RightTableAlias });
                    }
                    else if (join.Pairs.Count == 1)
                    {
                        sb.Append(@" ON ");
                        join.Pairs[0].BuildCommand(
                            sb,
                            new Where.BuildContext
                            {
                                Conn = connection,
                                RelatedQuery = this,
                                RightTableSchema = join.RightTableSchema,
                                RightTableName = join.RightTableAlias == null ? join.RightTableSchema.Name : join.RightTableAlias
                            });
                    }
                }
            }
        }

        private void BuildOrderBy(StringBuilder sb, ConnectorBase connection, bool invert)
        {
            if (_ListOrderBy != null && _ListOrderBy.Count > 0)
            {
                _ListOrderBy.BuildCommand(sb, connection, this, invert);
            }
        }

        private void BuildGroupBy(StringBuilder sb, ConnectorBase connection, bool invert)
        {
            if (_ListGroupBy != null && _ListGroupBy.Count > 0)
            {
                sb.Append(@" GROUP BY ");
                bool bFirst = true;
                foreach (GroupBy groupBy in _ListGroupBy)
                {
                    if (bFirst) bFirst = false;
                    else sb.Append(',');

                    if (groupBy.ColumnName is IPhrase)
                    {
                        sb.Append(((IPhrase)groupBy.ColumnName).BuildPhrase(connection, this));
                    }
                    else if (groupBy.ColumnName is Where)
                    {
                        ((Where)groupBy.ColumnName).BuildCommand(sb, true, new Where.BuildContext
                        {
                            Conn = connection,
                            RelatedQuery = this
                        });
                    }
                    else if (groupBy.IsLiteral)
                    {
                        sb.Append(groupBy.ColumnName);
                    }
                    else
                    {
                        if (groupBy.TableName != null)
                        {
                            sb.Append(connection.Language.WrapFieldName(groupBy.TableName) + @"." + connection.Language.WrapFieldName(groupBy.ColumnName.ToString()));
                        }
                        else
                        {
                            sb.Append(connection.Language.WrapFieldName(groupBy.ColumnName.ToString()));
                        }
                    }

                    if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                    {
                        switch (groupBy.SortDirection)
                        {
                            default:
                            case SortDirection.None:
                                break;
                            case SortDirection.ASC:
                                sb.Append(invert ? @" DESC" : @" ASC");
                                break;
                            case SortDirection.DESC:
                                sb.Append(invert ? @" ASC" : @" DESC");
                                break;
                        }
                    }
                }
            }
        }

        private void BuildHaving(StringBuilder sb, ConnectorBase connection)
        {
            if (_ListHaving != null && _ListHaving.Count > 0)
            {
                sb.Append(@" HAVING ");
                _ListHaving.BuildCommand(sb, new Where.BuildContext
                {
                    Conn = connection,
                    RelatedQuery = this
                });
            }
        }

        private void BuildCreateIndex(StringBuilder sb, ConnectorBase connection, object indexObj)
        {
            var language = connection.Language;

            if (indexObj is TableSchema.Index index)
            {
                language.BuildCreateIndex(this, connection, index, sb);
            }
            else if (indexObj is TableSchema.ForeignKey)
            {
                TableSchema.ForeignKey foreignKey = indexObj as TableSchema.ForeignKey;

                sb.Append(@"ALTER TABLE ");

                language.BuildTableName(this, connection, sb, false);

                sb.Append(@" ADD CONSTRAINT ");
                sb.Append(language.WrapFieldName(foreignKey.Name));
                sb.Append(@" FOREIGN KEY (");

                for (int i = 0; i < foreignKey.Columns.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(language.WrapFieldName(foreignKey.Columns[i]));
                }
                sb.AppendFormat(@") REFERENCES {0} (", foreignKey.ForeignTable);
                for (int i = 0; i < foreignKey.ForeignColumns.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(language.WrapFieldName(foreignKey.ForeignColumns[i]));
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
        }

        public void BuildColumnProperties(StringBuilder sb, ConnectorBase connection, TableSchema.Column column, bool noDefault)
        {
            sb.Append(connection.Language.WrapFieldName(column.Name));
            sb.Append(' ');

            bool isTextField;
            BuildColumnPropertiesDataType(sb, connection, column, out isTextField);

            if (!string.IsNullOrEmpty(column.Comment) && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
            {
                sb.AppendFormat(@" COMMENT {0}",
                    connection.Language.PrepareValue(column.Comment));
            }

            if (!column.Nullable)
            {
                sb.Append(@" NOT NULL");
            }

            if (column.ComputedColumn == null)
            {
                if (!noDefault && column.Default != null && (!(isTextField && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)))
                {
                    sb.Append(@" DEFAULT ");
                    Query.PrepareColumnValue(column, column.Default, sb, connection, this);
                }
            }

            sb.Append(' ');
        }

        public void BuildColumnPropertiesDataType(StringBuilder sb, ConnectorBase connection, TableSchema.Column column, out bool isTextField)
        {
            if (column.LiteralType != null && column.LiteralType.Length > 0)
            {
                isTextField = column.ActualDataType == DataType.VarChar ||
                    column.ActualDataType == DataType.Char ||
                    column.ActualDataType == DataType.Text ||
                    column.ActualDataType == DataType.MediumText ||
                    column.ActualDataType == DataType.LongText ||
                    column.Type == typeof(string);

                sb.Append(column.LiteralType);
                return;
            }

            var language = connection.Language;

            isTextField = false;
            DataType dataType = column.ActualDataType;
            if (!column.AutoIncrement || (connection.TYPE != ConnectorBase.SqlServiceType.MSACCESS && connection.TYPE != ConnectorBase.SqlServiceType.POSTGRESQL))
            {
                if (dataType == DataType.VarChar)
                {
                    if (column.MaxLength < 0)
                    {
                        if (language.VarCharMaxKeyword != null)
                        {
                            sb.Append(language.VarCharType);
                            sb.AppendFormat(@"({0})", language.VarCharMaxKeyword);
                        }
                        else
                        {
                            sb.Append(language.VarCharType);
                            sb.AppendFormat(@"({0})", language.VarCharMaxLength);
                        }
                    }
                    else if (column.MaxLength == 0)
                    {
                        sb.Append(language.TextType);
                        isTextField = true;
                    }
                    else if (column.MaxLength <= language.VarCharMaxLength)
                    {
                        sb.Append(language.VarCharType);
                        sb.AppendFormat(@"({0})", column.MaxLength);
                    }
                    else if (column.MaxLength < 65536)
                    {
                        sb.Append(language.TextType);
                        isTextField = true;
                    }
                    else if (column.MaxLength < 16777215)
                    {
                        sb.Append(language.MediumTextType);
                        isTextField = true;
                    }
                    else
                    {
                        sb.Append(language.LongTextType);
                        isTextField = true;
                    }
                }

                if (dataType == DataType.Char)
                {
                    if (column.MaxLength < 0)
                    {
                        if (language.VarCharMaxKeyword != null)
                        {
                            sb.Append(language.CharType);
                            sb.AppendFormat(@"({0})", language.VarCharMaxKeyword);
                        }
                        else
                        {
                            sb.Append(language.CharType);
                            sb.AppendFormat(@"({0})", language.VarCharMaxLength);
                        }
                    }
                    else if (column.MaxLength == 0 || column.MaxLength >= language.VarCharMaxLength)
                    {
                        sb.Append(language.CharType);
                        sb.AppendFormat(@"({0})", language.VarCharMaxLength);
                    }
                    else
                    {
                        sb.Append(language.CharType);
                        sb.AppendFormat(@"({0})", column.MaxLength);
                    }
                }
                else if (dataType == DataType.Text)
                {
                    sb.Append(language.TextType);
                    isTextField = true;
                }
                else if (dataType == DataType.MediumText)
                {
                    sb.Append(language.MediumTextType);
                    isTextField = true;
                }
                else if (dataType == DataType.LongText)
                {
                    sb.Append(language.LongTextType);
                    isTextField = true;
                }
                else if (dataType == DataType.Boolean)
                {
                    sb.Append(language.BooleanType);
                }
                else if (dataType == DataType.DateTime)
                {
                    sb.Append(language.DateTimeType);
                }
                else if (dataType == DataType.Numeric)
                {
                    if (column.NumberPrecision > 0)
                    {
                        sb.Append(language.NumericType);
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append(language.NumericType);
                    }
                }
                else if (dataType == DataType.Float)
                {
                    if (column.NumberPrecision > 0 && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                    {
                        sb.Append(language.FloatType);
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append(language.FloatType);
                    }
                }
                else if (dataType == DataType.Double)
                {
                    if (column.NumberPrecision > 0 && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                    {
                        sb.Append(language.DoubleType);
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append(language.DoubleType);
                    }
                }
                else if (dataType == DataType.Decimal)
                {
                    if (column.NumberPrecision > 0)
                    {
                        sb.Append(language.DecimalType);
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append(language.DecimalType);
                    }
                }
                else if (dataType == DataType.Money)
                {
                    if (column.NumberPrecision > 0)
                    {
                        sb.Append(language.MoneyType);
                        if (connection.TYPE != ConnectorBase.SqlServiceType.MSSQL)
                        {
                            sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                        }
                    }
                    else
                    {
                        sb.Append(language.MoneyType);
                    }
                }
                else if (dataType == DataType.TinyInt)
                {
                    sb.Append(language.TinyIntType);
                }
                else if (dataType == DataType.UnsignedTinyInt)
                {
                    sb.Append(language.UnsignedTinyIntType);
                }
                else if (dataType == DataType.SmallInt)
                {
                    sb.Append(language.SmallIntType);
                }
                else if (dataType == DataType.UnsignedSmallInt)
                {
                    sb.Append(language.UnsignedSmallIntType);
                }
                else if (dataType == DataType.Int)
                {
                    sb.Append(language.IntType);
                }
                else if (dataType == DataType.UnsignedInt)
                {
                    sb.Append(language.UnsignedIntType);
                }
                else if (dataType == DataType.BigInt)
                {
                    sb.Append(language.BigIntType);
                }
                else if (dataType == DataType.UnsignedBigInt)
                {
                    sb.Append(language.UnsignedBigIntType);
                }
                else if (dataType == DataType.Json)
                {
                    sb.Append(language.JsonType);
                }
                else if (dataType == DataType.JsonBinary)
                {
                    sb.Append(language.JsonBinaryType);
                }
                else if (dataType == DataType.Blob)
                {
                    sb.Append(language.BlobType);
                }
                else if (dataType == DataType.Guid)
                {
                    sb.Append(language.GuidType);
                }
                else if (dataType == DataType.Geometry)
                {
                    sb.Append(language.TypeGeometry);
                }
                else if (dataType == DataType.GeometryCollection)
                {
                    sb.Append(language.GeometryCollectionType);
                }
                else if (dataType == DataType.Point)
                {
                    sb.Append(language.PointType);
                }
                else if (dataType == DataType.LineString)
                {
                    sb.Append(language.LineStringType);
                }
                else if (dataType == DataType.Polygon)
                {
                    sb.Append(language.PolygonType);
                }
                else if (dataType == DataType.Line)
                {
                    sb.Append(language.LineType);
                }
                else if (dataType == DataType.Curve)
                {
                    sb.Append(language.CurveType);
                }
                else if (dataType == DataType.Surface)
                {
                    sb.Append(language.SurfaceType);
                }
                else if (dataType == DataType.LinearRing)
                {
                    sb.Append(language.LinearRingType);
                }
                else if (dataType == DataType.MultiPoint)
                {
                    sb.Append(language.MultiPointType);
                }
                else if (dataType == DataType.MultiLineString)
                {
                    sb.Append(language.MultiLineStringType);
                }
                else if (dataType == DataType.MultiPolygon)
                {
                    sb.Append(language.MultiPolygonType);
                }
                else if (dataType == DataType.MultiCurve)
                {
                    sb.Append(language.MultiCurveType);
                }
                else if (dataType == DataType.MultiSurface)
                {
                    sb.Append(language.MultiSurfaceType);
                }
                else if (dataType == DataType.Geographic)
                {
                    sb.Append(language.GeographicType);
                }
                else if (dataType == DataType.GeographicCollection)
                {
                    sb.Append(language.GeographicCollectionType);
                }
                else if (dataType == DataType.GeographicPoint)
                {
                    sb.Append(language.GeographicPointType);
                }
                else if (dataType == DataType.GeographicLineString)
                {
                    sb.Append(language.GeographicLinestringType);
                }
                else if (dataType == DataType.GeographicPolygon)
                {
                    sb.Append(language.GeographicPolygonType);
                }
                else if (dataType == DataType.GeographicLine)
                {
                    sb.Append(language.GeographicLineType);
                }
                else if (dataType == DataType.GeographicCurve)
                {
                    sb.Append(language.GeographicCurveType);
                }
                else if (dataType == DataType.GeographicSurface)
                {
                    sb.Append(language.GeographicSurfaceType);
                }
                else if (dataType == DataType.GeographicLinearRing)
                {
                    sb.Append(language.GeographicLinearringType);
                }
                else if (dataType == DataType.GeographicMultiPoint)
                {
                    sb.Append(language.GeographicMultipointType);
                }
                else if (dataType == DataType.GeographicMultiLineString)
                {
                    sb.Append(language.GeographicMultilinestringType);
                }
                else if (dataType == DataType.GeographicMultiPolygon)
                {
                    sb.Append(language.GeographicMultipolygonType);
                }
                else if (dataType == DataType.GeographicMultiCurve)
                {
                    sb.Append(language.GeographicMulticurveType);
                }
                else if (dataType == DataType.GeographicMultiSurface)
                {
                    sb.Append(language.GeographicMultisurfaceType);
                }
            }

            if (column.AutoIncrement)
            {
                sb.Append(' ');
                if (dataType == DataType.BigInt || dataType == DataType.UnsignedBigInt)
                { // Specifically for PostgreSQL
                    sb.Append(language.AutoIncrementBigIntType);
                }
                else
                {
                    sb.Append(language.AutoIncrementType);
                }
            }

            if (column.ComputedColumn != null)
            {
                sb.Append(" AS ");

                sb.Append(column.ComputedColumn.Build(connection, this));

                if (column.ComputedColumnStored)
                {
                    if (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                    {
                        sb.Append(" PERSISTED");
                    }
                    else
                    {
                        sb.Append(" STORED");
                    }
                }
            }

            if (connection.TYPE != ConnectorBase.SqlServiceType.POSTGRESQL && !string.IsNullOrEmpty(column.Charset))
            {
                sb.Append(@" COLLATE");
                sb.Append(column.Collate);
            }

            if (!string.IsNullOrEmpty(column.Charset))
            {
                sb.Append(@" CHARACTER SET");
                sb.Append(column.Charset);
            }
        }

        public string BuildCommand()
        {
            return BuildCommand(null);
        }

        public DbCommand BuildDbCommand(ConnectorBase connection)
        {
            if (this.QueryMode == QueryMode.ExecuteStoredProcedure)
            {
                var cmd = connection.Factory.NewCommand(_StoredProcedureName);
                cmd.CommandType = CommandType.StoredProcedure;

                if (CommandTimeout != null)
                    cmd.CommandTimeout = CommandTimeout.Value;

                if (_StoredProcedureParameters != null)
                {
                    foreach (var param in _StoredProcedureParameters)
                    {
                        cmd.Parameters.Add(param.Build(connection.Factory));
                    }
                }

                if (connection != null)
                {
                    cmd.Connection = connection.Connection;
                    cmd.Transaction = connection.Transaction;
                }

                return cmd;
            }
            else
            {
                var cmd = connection.Factory.NewCommand(BuildCommand(connection));
                cmd.CommandType = CommandType.Text;

                if (CommandTimeout != null)
                    cmd.CommandTimeout = CommandTimeout.Value;

                if (connection != null)
                {
                    cmd.Connection = connection.Connection;
                    cmd.Transaction = connection.Transaction;
                }

                return cmd;
            }
        }

        public string BuildCommand(ConnectorBase connection)
        {
            if (this.QueryMode == QueryMode.ExecuteStoredProcedure || this.QueryMode == QueryMode.None) return string.Empty;

            StringBuilder sb = new StringBuilder();

            bool ownsConnection = false;
            if (connection == null)
            {
                ownsConnection = true;
                connection = ConnectorBase.NewInstance();
            }
            try
            {
                var language = connection.Language;

                bool bFirst;

                if (this.QueryMode != QueryMode.None)
                {
                    switch (this.QueryMode)
                    {
                        case QueryMode.Select:
                            {
                                sb.Append(@" SELECT ");

                                language.BuildLimitOffset(this, true, sb);

                                if (IsDistinct) sb.Append(@"DISTINCT ");

                                BuildSelectList(sb, connection);

                                sb.Append(@" FROM ");

                                language.BuildTableName(this, connection, sb, true);

                                BuildJoin(sb, connection);

                                if (_ListWhere != null && _ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    _ListWhere.BuildCommand(sb, new Where.BuildContext
                                    {
                                        Conn = connection,
                                        RelatedQuery = this
                                    });
                                }

                                BuildGroupBy(sb, connection, false);
                                BuildHaving(sb, connection);
                                BuildOrderBy(sb, connection, false);

                                language.BuildLimitOffset(this, false, sb);

                                // Done with select query
                                
                                // Write out supported hints
                                switch (_QueryHint)
                                {
                                    case QueryHint.ForUpdate:
                                        if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL ||
                                            connection.TYPE == ConnectorBase.SqlServiceType.MSSQL ||
                                            connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                        {
                                            sb.Append(@" FOR UPDATE");
                                        }
                                        break;
                                    case QueryHint.LockInSharedMode:
                                        if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                        {
                                            sb.Append(@" LOCK IN SHARED MODE");
                                        }
                                        else if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                        {
                                            sb.Append(@" FOR SHARE");
                                        }
                                        break;
                                }
                            }

                            break;
                        case QueryMode.Insert:
                            {
                                sb.Append(@"INSERT INTO ");

                                language.BuildTableName(this, connection, sb, false);

                                sb.Append(@" (");
                                bFirst = true;
                                foreach (AssignmentColumn ins in _ListInsertUpdate)
                                {
                                    if (bFirst) bFirst = false;
                                    else sb.Append(',');
                                    sb.Append(language.WrapFieldName(ins.ColumnName));
                                }
                                if (InsertExpression != null)
                                {
                                    sb.Append(@") ");
                                    sb.Append(InsertExpression);
                                }
                                else
                                {
                                    sb.Append(@") VALUES (");
                                    bFirst = true;
                                    foreach (AssignmentColumn ins in _ListInsertUpdate)
                                    {
                                        if (bFirst) bFirst = false;
                                        else sb.Append(',');
                                        if (ins.SecondType == ValueObjectType.Literal)
                                        {
                                            sb.Append(ins.Second);
                                        }
                                        else if (ins.SecondType == ValueObjectType.Value)
                                        {
                                            if (ins.Second is Query)
                                            {
                                                sb.Append('(');
                                                sb.Append(((Query)ins.Second).BuildCommand(connection));
                                                sb.Append(')');
                                            }
                                            else
                                            {
                                                Query.PrepareColumnValue(Schema.Columns.Find(ins.ColumnName), ins.Second, sb, connection, this);
                                            }
                                        }
                                        else if (ins.SecondType == ValueObjectType.ColumnName)
                                        {
                                            if (ins.SecondTableName != null)
                                            {
                                                sb.Append(language.WrapFieldName(ins.SecondTableName));
                                                sb.Append(@".");
                                            }
                                            sb.Append(language.WrapFieldName(ins.Second.ToString()));
                                        }
                                    }
                                    sb.Append(@")");

                                    if (_ListWhere != null && _ListWhere.Count > 0)
                                    {
                                        sb.Append(@" WHERE ");
                                        _ListWhere.BuildCommand(sb, new Where.BuildContext
                                        {
                                            Conn = connection,
                                            RelatedQuery = this,
                                        });
                                    }
                                }
                            }

                            break;

                        case QueryMode.Update:
                            {
                                bool hasJoins = _ListJoin != null && _ListJoin.Count > 0;

                                sb.Append(@"UPDATE ");

                                if (hasJoins && 
                                    (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL || 
                                    connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS) &&
                                    !string.IsNullOrEmpty(_SchemaAlias))
                                {
                                    sb.Append(language.WrapFieldName(_SchemaAlias));
                                }
                                else
                                {
                                    language.BuildTableName(this, connection, sb, true);
                                }

                                if (hasJoins)
                                {
                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                    {
                                        BuildJoin(sb, connection);
                                    }
                                }

                                bFirst = true;
                                foreach (AssignmentColumn upd in _ListInsertUpdate)
                                {
                                    if (bFirst)
                                    {
                                        sb.Append(@" SET ");
                                        bFirst = false;
                                    }
                                    else sb.Append(',');

                                    if (_ListJoin != null && _ListJoin.Count > 0 && upd.TableName != null)
                                    {
                                        sb.Append(language.WrapFieldName(upd.TableName));
                                        sb.Append(@".");
                                    }

                                    sb.Append(language.WrapFieldName(upd.ColumnName));

                                    sb.Append('=');

                                    if (upd.SecondType == ValueObjectType.Literal)
                                    {
                                        sb.Append(upd.Second);
                                    }
                                    else if (upd.SecondType == ValueObjectType.Value)
                                    {
                                        Query.PrepareColumnValue(Schema.Columns.Find(upd.ColumnName), upd.Second, sb, connection, this);
                                    }
                                    else if (upd.SecondType == ValueObjectType.ColumnName)
                                    {
                                        if (upd.SecondTableName != null)
                                        {
                                            sb.Append(language.WrapFieldName(upd.SecondTableName));
                                            sb.Append(@".");
                                        }
                                        sb.Append(language.WrapFieldName(upd.Second.ToString()));
                                    }
                                }

                                if (hasJoins)
                                {
                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL || 
                                        connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
                                    {
                                        language.BuildTableName(this, connection, sb, true);

                                        BuildJoin(sb, connection);
                                    }
                                }

                                if (_ListWhere != null && _ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    _ListWhere.BuildCommand(sb, new Where.BuildContext
                                    {
                                        Conn = connection,
                                        RelatedQuery = this
                                    });
                                }

                                BuildOrderBy(sb, connection, false);
                            }
                            break;

                        case QueryMode.InsertOrUpdate:
                            {
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    sb.Append(@"REPLACE INTO ");

                                    language.BuildTableName(this, connection, sb, false);

                                    sb.Append(@" (");
                                    bFirst = true;
                                    foreach (AssignmentColumn ins in _ListInsertUpdate)
                                    {
                                        if (bFirst) bFirst = false;
                                        else sb.Append(',');
                                        sb.Append(language.WrapFieldName(ins.ColumnName));
                                    }
                                    if (InsertExpression != null)
                                    {
                                        sb.Append(@") ");
                                        sb.Append(InsertExpression);
                                    }
                                    else
                                    {
                                        sb.Append(@") VALUES (");
                                        bFirst = true;
                                        foreach (AssignmentColumn ins in _ListInsertUpdate)
                                        {
                                            if (bFirst) bFirst = false;
                                            else sb.Append(',');
                                            if (ins.SecondType == ValueObjectType.Literal)
                                            {
                                                sb.Append(ins.Second);
                                            }
                                            else if (ins.SecondType == ValueObjectType.Value)
                                            {
                                                if (ins.Second is Query)
                                                {
                                                    sb.Append('(');
                                                    sb.Append(((Query)ins.Second).BuildCommand(connection));
                                                    sb.Append(')');
                                                }
                                                else
                                                {
                                                    Query.PrepareColumnValue(Schema.Columns.Find(ins.ColumnName), ins.Second, sb, connection, this);
                                                }
                                            }
                                            else if (ins.SecondType == ValueObjectType.ColumnName)
                                            {
                                                if (ins.SecondTableName != null)
                                                {
                                                    sb.Append(language.WrapFieldName(ins.SecondTableName));
                                                    sb.Append(@".");
                                                }
                                                sb.Append(language.WrapFieldName(ins.Second.ToString()));
                                            }
                                        }
                                        sb.Append(@")");

                                        if (_ListWhere != null && _ListWhere.Count > 0)
                                        {
                                            sb.Append(@" WHERE ");
                                            _ListWhere.BuildCommand(sb, new Where.BuildContext
                                            {
                                                Conn = connection,
                                                RelatedQuery = this
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    _NeedTransaction = true;

                                    if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                    {
                                        throw new NotImplementedException(@"This operation is not implemented for PostgreSQL due to database limitations.");
                                    }

                                    // MSSQL
                                    QueryMode qm = this.QueryMode;
                                    this.QueryMode = QueryMode.Update;

                                    sb.Append(BuildCommand(connection));

                                    sb.Append(@"; IF @@rowcount = 0 BEGIN ");

                                    this.QueryMode = QueryMode.Insert;

                                    sb.Append(BuildCommand(connection));

                                    sb.Append(@"; END");

                                    this.QueryMode = qm;
                                }
                            }
                            break;

                        case QueryMode.Delete:
                            {
                                sb.Append(@"DELETE");

                                language.BuildLimitOffset(this, true, sb);

                                if (_ListJoin != null && _ListJoin.Count > 0)
                                {
                                    language.BuildTableName(this, connection, sb, false);
                                }

                                sb.Append(@" FROM ");

                                language.BuildTableName(this, connection, sb, false);

                                BuildJoin(sb, connection);
                                if (_ListWhere != null && _ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    _ListWhere.BuildCommand(sb, new Where.BuildContext
                                    {
                                        Conn = connection,
                                        RelatedQuery = this
                                    });
                                }

                                BuildOrderBy(sb, connection, false);

                                language.BuildLimitOffset(this, false, sb);
                            }
                            break;

                        case QueryMode.CreateTable:
                            {
                                sb.Append(@"CREATE TABLE ");

                                language.BuildTableName(this, connection, sb, false);

                                sb.Append('(');
                                int iPrimaryKeys = 0;
                                bool bSep = false;
                                foreach (TableSchema.Column col in Schema.Columns)
                                {
                                    if (col.IsPrimaryKey) iPrimaryKeys++;
                                    if (bSep) sb.Append(@", "); else bSep = true;
                                    BuildColumnProperties(sb, connection, col, false);
                                }

                                if (iPrimaryKeys > 0)
                                {
                                    if (bSep) sb.Append(@", ");

                                    sb.AppendFormat(@"CONSTRAINT {0} PRIMARY KEY(", language.WrapFieldName(@"PK_" + _SchemaName));
                                    bSep = false;
                                    foreach (TableSchema.Column col in Schema.Columns)
                                    {
                                        if (!col.IsPrimaryKey) continue;
                                        if (bSep) sb.Append(@", "); else bSep = true;
                                        if (col.IsPrimaryKey) sb.Append(language.WrapFieldName(col.Name));
                                    }
                                    bSep = true;
                                    sb.Append(')');
                                }

                                sb.Append(')');

                                if (Schema.TableOptions != null && Schema.TableOptions.Count > 0)
                                {
                                    foreach (KeyValuePair<string, string> option in Schema.TableOptions)
                                    {
                                        sb.Append(' ');
                                        sb.Append(option.Key);
                                        sb.Append('=');
                                        sb.Append(option.Value);
                                    }
                                }
                            }
                            break;

                        case QueryMode.CreateIndex:
                            BuildCreateIndex(sb, connection, _CreateIndexObject);
                            break;

                        case QueryMode.CreateIndexes:
                            {
                                if ((Schema.Indexes.Count + Schema.ForeignKeys.Count) > 1)
                                {
                                    _NeedTransaction = true;
                                }
                                foreach (TableSchema.Index index in Schema.Indexes)
                                {
                                    BuildCreateIndex(sb, connection, index);
                                    sb.Append(@";");
                                }
                                foreach (TableSchema.ForeignKey foreignKey in Schema.ForeignKeys)
                                {
                                    BuildCreateIndex(sb, connection, foreignKey);
                                    sb.Append(@";");
                                }
                            }
                            break;

                        case QueryMode.AddColumn:
                            {
                                sb.Append(@"ALTER TABLE ");

                                language.BuildTableName(this, connection, sb, false);

                                sb.Append(@" ADD ");

                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL || connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                {
                                    sb.Append(@"COLUMN ");
                                }

                                BuildColumnProperties(sb, connection, _AlterColumn, false);

                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    int idx = Schema.Columns.IndexOf(_AlterColumn);
                                    if (idx == 0) sb.Append(@"FIRST ");
                                    else sb.AppendFormat(@"AFTER {0} ", language.WrapFieldName(Schema.Columns[idx - 1].Name));
                                }
                            }
                            break;

                        case QueryMode.ChangeColumn:
                            {
                                if (_AlterColumnOldName != null && _AlterColumnOldName.Length == 0) _AlterColumnOldName = null;

                                if (_AlterColumnOldName != null)
                                {
                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                                    {
                                        sb.Append(@"EXEC sp_rename ");

                                        language.BuildTableName(this, connection, sb, false);

                                        sb.Append('.');
                                        sb.Append(language.WrapFieldName(_AlterColumnOldName));
                                        sb.Append(',');
                                        sb.Append(language.WrapFieldName(_AlterColumn.Name));
                                        sb.Append(@",'COLUMN';");
                                    }
                                    else if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                    {
                                        sb.Append(@"ALTER TABLE ");

                                        language.BuildTableName(this, connection, sb, false);

                                        sb.Append(@" RENAME COLUMN ");
                                        sb.Append(language.WrapFieldName(_AlterColumnOldName));
                                        sb.Append(@" TO ");
                                        sb.Append(language.WrapFieldName(_AlterColumn.Name));
                                        sb.Append(';');
                                    }
                                }

                                if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                {
                                    // Very limited syntax, will have to do this with several statements

                                    sb.Append(@"ALTER TABLE ");

                                    language.BuildTableName(this, connection, sb, false);

                                    string alterColumnStatement = @" ALTER COLUMN ";
                                    alterColumnStatement += language.WrapFieldName(_AlterColumn.Name);

                                    sb.Append(alterColumnStatement);
                                    sb.Append(@" TYPE ");
                                    bool isTextField; // UNUSED HERE
                                    BuildColumnPropertiesDataType(sb, connection, _AlterColumn, out isTextField);
                                    sb.Append(',');

                                    sb.Append(alterColumnStatement);
                                    sb.Append(_AlterColumn.Nullable ? @" DROP NOT NULL;" : @" SET NOT NULL;");

                                    sb.Append(alterColumnStatement);
                                    sb.Append(@" SET DEFAULT ");
                                    Query.PrepareColumnValue(_AlterColumn, _AlterColumn.Default, sb, connection, this);
                                    sb.Append(';');
                                }
                                else
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    language.BuildTableName(this, connection, sb, false);

                                    sb.Append(' ');
                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                    {
                                        sb.AppendFormat(@"CHANGE {0} ", language.WrapFieldName(_AlterColumnOldName != null ? _AlterColumnOldName : _AlterColumn.Name));
                                    }
                                    else
                                    {
                                        sb.Append(@"ALTER COLUMN ");
                                    }
                                    BuildColumnProperties(sb, connection, _AlterColumn, connection.TYPE == ConnectorBase.SqlServiceType.MSSQL);
                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                    {
                                        int idx = Schema.Columns.IndexOf(_AlterColumn);
                                        if (idx == 0) sb.Append(@"FIRST ");
                                        else sb.AppendFormat(@"AFTER {0} ", language.WrapFieldName(Schema.Columns[idx - 1].Name));
                                    }
                                }
                            }
                            break;

                        case QueryMode.DropColumn:
                            {
                                sb.Append(@"ALTER TABLE ");

                                language.BuildTableName(this, connection, sb, false);

                                sb.Append(@" DROP COLUMN ");
                                sb.Append(language.WrapFieldName(_DropColumnName));
                            }
                            break;

                        case QueryMode.DropForeignKey:
                            {
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    language.BuildTableName(this, connection, sb, false);

                                    sb.Append(@" DROP FOREIGN KEY ");
                                    sb.Append(language.WrapFieldName(_DropColumnName));
                                }
                                else if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    language.BuildTableName(this, connection, sb, false);

                                    sb.Append(@" DROP CONSTRAINT ");
                                    sb.Append(language.WrapFieldName(_DropColumnName));
                                }
                                else
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    language.BuildTableName(this, connection, sb, false);

                                    sb.Append(@" DROP CONSTRAINT ");
                                    sb.Append(language.WrapFieldName(_DropColumnName));
                                }
                            }
                            break;

                        case QueryMode.DropIndex:
                            {
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL || connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    language.BuildTableName(this, connection, sb, false);

                                    sb.Append(@" DROP INDEX ");
                                    sb.Append(language.WrapFieldName(_DropColumnName));
                                }
                                else
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    language.BuildTableName(this, connection, sb, false);

                                    sb.Append(@" DROP CONSTRAINT ");
                                    sb.Append(language.WrapFieldName(_DropColumnName));
                                }
                            }
                            break;

                        case QueryMode.DropTable:
                            {
                                sb.Append(@"DROP TABLE ");

                                language.BuildTableName(this, connection, sb, false);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }

            return sb.ToString();
        }
    }
}
