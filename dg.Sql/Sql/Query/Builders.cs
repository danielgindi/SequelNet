using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using dg.Sql.Connector;

namespace dg.Sql
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
                            sb.Append(connection.PrepareValue(sel.Value, this));
                        }

                        if (!string.IsNullOrEmpty(sel.Alias))
                        {
                            sb.Append(@" AS ");
                            sb.Append(connection.WrapFieldName(sel.Alias));
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
                            sb.Append(connection.WrapFieldName(sel.Alias));
                        }
                    }
                    else
                    {
                        if (_ListJoin != null && _ListJoin.Count > 0 && string.IsNullOrEmpty(sel.TableName))
                        {
                            if (_SchemaAlias != null)
                            {
                                sb.Append(connection.WrapFieldName(_SchemaAlias));
                            }
                            else
                            {
                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.WrapFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.WrapFieldName(_SchemaName));
                            }

                            sb.Append('.');
                            sb.Append(sel.Value == null ? "*" : connection.WrapFieldName(sel.Value.ToString()));
                            if (!string.IsNullOrEmpty(sel.Alias))
                            {
                                sb.Append(@" AS ");
                                sb.Append(connection.WrapFieldName(sel.Alias));
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(sel.TableName))
                            {
                                sb.Append(connection.WrapFieldName(sel.TableName));
                                sb.Append('.');
                            }
                            sb.Append(sel.Value == null ? "*" : connection.WrapFieldName(sel.Value.ToString()));
                            if (!string.IsNullOrEmpty(sel.Alias))
                            {
                                sb.Append(@" AS ");
                                sb.Append(connection.WrapFieldName(sel.Alias));
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
                            sb.Append(connection.WrapFieldName(join.RightTableSchema.DatabaseOwner));
                            sb.Append('.');
                        }
                        sb.Append(connection.WrapFieldName(join.RightTableSchema.Name));
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
                            sb.Append(connection.WrapFieldName(groupBy.TableName) + @"." + connection.WrapFieldName(groupBy.ColumnName.ToString()));
                        }
                        else
                        {
                            sb.Append(connection.WrapFieldName(groupBy.ColumnName.ToString()));
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
            if (indexObj is TableSchema.Index)
            {
                TableSchema.Index index = indexObj as TableSchema.Index;
                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                {
                    sb.Append(@"ALTER TABLE ");

                    BuildTableName(connection, sb, false);

                    sb.Append(@" ADD ");

                    if (index.Mode == dg.Sql.TableSchema.IndexMode.PrimaryKey)
                    {
                        sb.AppendFormat(@"CONSTRAINT {0} PRIMARY KEY ", connection.WrapFieldName(index.Name));
                    }
                    else
                    {
                        switch (index.Mode)
                        {
                            case TableSchema.IndexMode.Unique:
                                sb.Append(@"UNIQUE ");
                                break;
                            case TableSchema.IndexMode.FullText:
                                sb.Append(@"FULLTEXT ");
                                break;
                            case TableSchema.IndexMode.Spatial:
                                sb.Append(@"SPATIAL ");
                                break;
                        }
                        sb.Append(@"INDEX ");
                        sb.Append(connection.WrapFieldName(index.Name));
                        sb.Append(@" ");
                    }
                    if (index.Mode != TableSchema.IndexMode.Spatial)
                    {
                        switch (index.Type)
                        {
                            case TableSchema.IndexType.BTREE:
                                sb.Append(@"USING BTREE ");
                                break;
                            case TableSchema.IndexType.RTREE:
                                sb.Append(@"USING RTREE ");
                                break;
                            case TableSchema.IndexType.HASH:
                                sb.Append(@"USING HASH ");
                                break;
                        }
                    }
                    sb.Append(@"(");
                    for (int i = 0; i < index.ColumnNames.Length; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append(connection.WrapFieldName(index.ColumnNames[i]));
                        if (index.ColumnLength[i] > 0) sb.AppendFormat("({0})", index.ColumnLength[i]);
                        sb.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                    }
                    sb.Append(@")");
                }
                else if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                {
                    if (index.Mode == dg.Sql.TableSchema.IndexMode.PrimaryKey)
                    {
                        sb.Append(@"ALTER TABLE ");

                        BuildTableName(connection, sb, false);

                        sb.Append(@" ADD CONSTRAINT ");
                        sb.Append(connection.WrapFieldName(index.Name));
                        sb.Append(@" PRIMARY KEY ");

                        sb.Append(@"(");
                        for (int i = 0; i < index.ColumnNames.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append(connection.WrapFieldName(index.ColumnNames[i]));
                            sb.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                        }
                        sb.Append(@")");
                    }
                    else
                    {
                        sb.Append(@"CREATE ");

                        if (index.Mode == TableSchema.IndexMode.Unique) sb.Append(@"UNIQUE ");

                        sb.Append(@"INDEX ");
                        sb.Append(connection.WrapFieldName(index.Name));

                        sb.Append(@"ON ");

                        BuildTableName(connection, sb, false);

                        if (index.Mode == TableSchema.IndexMode.Spatial)
                        {
                            sb.Append(@"USING GIST");
                        }

                        sb.Append(@"(");
                        for (int i = 0; i < index.ColumnNames.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append(connection.WrapFieldName(index.ColumnNames[i]));
                            sb.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                        }
                        sb.Append(@")");
                    }
                }
                else if (connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
                {
                    sb.Append(@"ALTER TABLE ");

                    BuildTableName(connection, sb, false);

                    sb.Append(@" ADD ");

                    if (index.Mode == dg.Sql.TableSchema.IndexMode.PrimaryKey)
                    {
                        sb.AppendFormat(@"CONSTRAINT {0} PRIMARY KEY ", connection.WrapFieldName(index.Name));
                    }
                    else
                    {
                        if (index.Mode == TableSchema.IndexMode.Unique) sb.Append(@"UNIQUE ");
                        sb.Append(@"INDEX ");
                        sb.Append(connection.WrapFieldName(index.Name));
                        sb.Append(@" ");
                    }
                    sb.Append(@"(");
                    for (int i = 0; i < index.ColumnNames.Length; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append(connection.WrapFieldName(index.ColumnNames[i]));
                        sb.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                    }
                    sb.Append(@")");
                }
                else if (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    if (index.Mode == dg.Sql.TableSchema.IndexMode.PrimaryKey)
                    {
                        sb.Append(@"ALTER TABLE ");

                        BuildTableName(connection, sb, false);

                        sb.Append(@" ADD CONSTRAINT ");
                        sb.Append(connection.WrapFieldName(index.Name));
                        sb.Append(@" PRIMARY KEY ");

                        if (index.Cluster == TableSchema.ClusterMode.Clustered) sb.Append(@"CLUSTERED ");
                        else if (index.Cluster == TableSchema.ClusterMode.NonClustered) sb.Append(@"NONCLUSTERED ");

                        sb.Append(@"(");
                        for (int i = 0; i < index.ColumnNames.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append(connection.WrapFieldName(index.ColumnNames[i]));
                            sb.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                        }
                        sb.Append(@")");
                    }
                    else
                    {
                        sb.Append(@"CREATE ");

                        if (index.Mode == TableSchema.IndexMode.Unique) sb.Append(@"UNIQUE ");
                        if (index.Cluster == TableSchema.ClusterMode.Clustered) sb.Append(@"CLUSTERED ");
                        else if (index.Cluster == TableSchema.ClusterMode.NonClustered) sb.Append(@"NONCLUSTERED ");

                        sb.Append(@"INDEX ");
                        sb.Append(connection.WrapFieldName(index.Name));

                        sb.Append(@"ON ");

                        BuildTableName(connection, sb, false);

                        sb.Append(@"(");
                        for (int i = 0; i < index.ColumnNames.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append(connection.WrapFieldName(index.ColumnNames[i]));
                            sb.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                        }
                        sb.Append(@")");
                    }
                }
            }
            else if (indexObj is TableSchema.ForeignKey)
            {
                TableSchema.ForeignKey foreignKey = indexObj as TableSchema.ForeignKey;

                sb.Append(@"ALTER TABLE ");

                BuildTableName(connection, sb, false);

                sb.Append(@" ADD CONSTRAINT ");
                sb.Append(connection.WrapFieldName(foreignKey.Name));
                sb.Append(@" FOREIGN KEY (");

                for (int i = 0; i < foreignKey.Columns.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(connection.WrapFieldName(foreignKey.Columns[i]));
                }
                sb.AppendFormat(@") REFERENCES {0} (", foreignKey.ForeignTable);
                for (int i = 0; i < foreignKey.ForeignColumns.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(connection.WrapFieldName(foreignKey.ForeignColumns[i]));
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

        public void BuildColumnProperties(StringBuilder sb, ConnectorBase connection, TableSchema.Column column, bool NoDefault)
        {
            sb.Append(connection.WrapFieldName(column.Name));
            sb.Append(' ');

            bool isTextField;
            BuildColumnPropertiesDataType(sb, connection, column, out isTextField);

            if (!string.IsNullOrEmpty(column.Comment) && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
            {
                sb.AppendFormat(@" COMMENT {0}",
                    connection.PrepareValue(column.Comment));
            }

            if (!column.Nullable)
            {
                sb.Append(@" NOT NULL");
            }

            if (column.ComputedColumn == null)
            {
                if (!NoDefault && column.Default != null && (!(isTextField && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)))
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

            isTextField = false;
            DataType dataType = column.ActualDataType;
            if (!column.AutoIncrement || (connection.TYPE != ConnectorBase.SqlServiceType.MSACCESS && connection.TYPE != ConnectorBase.SqlServiceType.POSTGRESQL))
            {
                if (dataType == DataType.VarChar)
                {
                    if (column.MaxLength < 0)
                    {
                        if (connection.varchar_MAX != null)
                        {
                            sb.Append(connection.type_VARCHAR);
                            sb.AppendFormat(@"({0})", connection.varchar_MAX);
                        }
                        else
                        {
                            sb.Append(connection.type_VARCHAR);
                            sb.AppendFormat(@"({0})", connection.varchar_MAX_VALUE);
                        }
                    }
                    else if (column.MaxLength == 0)
                    {
                        sb.Append(connection.type_TEXT);
                        isTextField = true;
                    }
                    else if (column.MaxLength <= connection.varchar_MAX_VALUE)
                    {
                        sb.Append(connection.type_VARCHAR);
                        sb.AppendFormat(@"({0})", column.MaxLength);
                    }
                    else if (column.MaxLength < 65536)
                    {
                        sb.Append(connection.type_TEXT);
                        isTextField = true;
                    }
                    else if (column.MaxLength < 16777215)
                    {
                        sb.Append(connection.type_MEDIUMTEXT);
                        isTextField = true;
                    }
                    else
                    {
                        sb.Append(connection.type_LONGTEXT);
                        isTextField = true;
                    }
                }

                if (dataType == DataType.Char)
                {
                    if (column.MaxLength < 0)
                    {
                        if (connection.varchar_MAX != null)
                        {
                            sb.Append(connection.type_CHAR);
                            sb.AppendFormat(@"({0})", connection.varchar_MAX);
                        }
                        else
                        {
                            sb.Append(connection.type_CHAR);
                            sb.AppendFormat(@"({0})", connection.varchar_MAX_VALUE);
                        }
                    }
                    else if (column.MaxLength == 0 || column.MaxLength >= connection.varchar_MAX_VALUE)
                    {
                        sb.Append(connection.type_CHAR);
                        sb.AppendFormat(@"({0})", connection.varchar_MAX_VALUE);
                    }
                    else
                    {
                        sb.Append(connection.type_CHAR);
                        sb.AppendFormat(@"({0})", column.MaxLength);
                    }
                }
                else if (dataType == DataType.Text)
                {
                    sb.Append(connection.type_TEXT);
                    isTextField = true;
                }
                else if (dataType == DataType.MediumText)
                {
                    sb.Append(connection.type_MEDIUMTEXT);
                    isTextField = true;
                }
                else if (dataType == DataType.LongText)
                {
                    sb.Append(connection.type_LONGTEXT);
                    isTextField = true;
                }
                else if (dataType == DataType.Boolean)
                {
                    sb.Append(connection.type_BOOLEAN);
                }
                else if (dataType == DataType.DateTime)
                {
                    sb.Append(connection.type_DATETIME);
                }
                else if (dataType == DataType.Numeric)
                {
                    if (column.NumberPrecision > 0)
                    {
                        sb.Append(connection.type_NUMERIC);
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append(connection.type_NUMERIC);
                    }
                }
                else if (dataType == DataType.Float)
                {
                    if (column.NumberPrecision > 0 && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                    {
                        sb.Append(connection.type_FLOAT);
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append(connection.type_FLOAT);
                    }
                }
                else if (dataType == DataType.Double)
                {
                    if (column.NumberPrecision > 0 && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                    {
                        sb.Append(connection.type_DOUBLE);
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append(connection.type_DOUBLE);
                    }
                }
                else if (dataType == DataType.Decimal)
                {
                    if (column.NumberPrecision > 0)
                    {
                        sb.Append(connection.type_DECIMAL);
                        sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                    }
                    else
                    {
                        sb.Append(connection.type_DECIMAL);
                    }
                }
                else if (dataType == DataType.Money)
                {
                    if (column.NumberPrecision > 0)
                    {
                        sb.Append(connection.type_MONEY);
                        if (connection.TYPE != ConnectorBase.SqlServiceType.MSSQL)
                        {
                            sb.AppendFormat(@"({0}, {1})", column.NumberPrecision, column.NumberScale);
                        }
                    }
                    else
                    {
                        sb.Append(connection.type_MONEY);
                    }
                }
                else if (dataType == DataType.TinyInt)
                {
                    sb.Append(connection.type_TINYINT);
                }
                else if (dataType == DataType.UnsignedTinyInt)
                {
                    sb.Append(connection.type_UNSIGNEDTINYINT);
                }
                else if (dataType == DataType.SmallInt)
                {
                    sb.Append(connection.type_SMALLINT);
                }
                else if (dataType == DataType.UnsignedSmallInt)
                {
                    sb.Append(connection.type_UNSIGNEDSMALLINT);
                }
                else if (dataType == DataType.Int)
                {
                    sb.Append(connection.type_INT);
                }
                else if (dataType == DataType.UnsignedInt)
                {
                    sb.Append(connection.type_UNSIGNEDINT);
                }
                else if (dataType == DataType.BigInt)
                {
                    sb.Append(connection.type_BIGINT);
                }
                else if (dataType == DataType.UnsignedBigInt)
                {
                    sb.Append(connection.type_UNSIGNEDBIGINT);
                }
                else if (dataType == DataType.Json)
                {
                    sb.Append(connection.type_JSON);
                }
                else if (dataType == DataType.JsonBinary)
                {
                    sb.Append(connection.type_JSON_BINARY);
                }
                else if (dataType == DataType.Blob)
                {
                    sb.Append(connection.type_BLOB);
                }
                else if (dataType == DataType.Guid)
                {
                    sb.Append(connection.type_GUID);
                }
                else if (dataType == DataType.Geometry)
                {
                    sb.Append(connection.type_GEOMETRY);
                }
                else if (dataType == DataType.GeometryCollection)
                {
                    sb.Append(connection.type_GEOMETRYCOLLECTION);
                }
                else if (dataType == DataType.Point)
                {
                    sb.Append(connection.type_POINT);
                }
                else if (dataType == DataType.LineString)
                {
                    sb.Append(connection.type_LINESTRING);
                }
                else if (dataType == DataType.Polygon)
                {
                    sb.Append(connection.type_POLYGON);
                }
                else if (dataType == DataType.Line)
                {
                    sb.Append(connection.type_LINE);
                }
                else if (dataType == DataType.Curve)
                {
                    sb.Append(connection.type_CURVE);
                }
                else if (dataType == DataType.Surface)
                {
                    sb.Append(connection.type_SURFACE);
                }
                else if (dataType == DataType.LinearRing)
                {
                    sb.Append(connection.type_LINEARRING);
                }
                else if (dataType == DataType.MultiPoint)
                {
                    sb.Append(connection.type_MULTIPOINT);
                }
                else if (dataType == DataType.MultiLineString)
                {
                    sb.Append(connection.type_MULTILINESTRING);
                }
                else if (dataType == DataType.MultiPolygon)
                {
                    sb.Append(connection.type_MULTIPOLYGON);
                }
                else if (dataType == DataType.MultiCurve)
                {
                    sb.Append(connection.type_MULTICURVE);
                }
                else if (dataType == DataType.MultiSurface)
                {
                    sb.Append(connection.type_MULTISURFACE);
                }
                else if (dataType == DataType.Geographic)
                {
                    sb.Append(connection.type_GEOGRAPHIC);
                }
                else if (dataType == DataType.GeographicCollection)
                {
                    sb.Append(connection.type_GEOGRAPHICCOLLECTION);
                }
                else if (dataType == DataType.GeographicPoint)
                {
                    sb.Append(connection.type_GEOGRAPHIC_POINT);
                }
                else if (dataType == DataType.GeographicLineString)
                {
                    sb.Append(connection.type_GEOGRAPHIC_LINESTRING);
                }
                else if (dataType == DataType.GeographicPolygon)
                {
                    sb.Append(connection.type_GEOGRAPHIC_POLYGON);
                }
                else if (dataType == DataType.GeographicLine)
                {
                    sb.Append(connection.type_GEOGRAPHIC_LINE);
                }
                else if (dataType == DataType.GeographicCurve)
                {
                    sb.Append(connection.type_GEOGRAPHIC_CURVE);
                }
                else if (dataType == DataType.GeographicSurface)
                {
                    sb.Append(connection.type_GEOGRAPHIC_SURFACE);
                }
                else if (dataType == DataType.GeographicLinearRing)
                {
                    sb.Append(connection.type_GEOGRAPHIC_LINEARRING);
                }
                else if (dataType == DataType.GeographicMultiPoint)
                {
                    sb.Append(connection.type_GEOGRAPHIC_MULTIPOINT);
                }
                else if (dataType == DataType.GeographicMultiLineString)
                {
                    sb.Append(connection.type_GEOGRAPHIC_MULTILINESTRING);
                }
                else if (dataType == DataType.GeographicMultiPolygon)
                {
                    sb.Append(connection.type_GEOGRAPHIC_MULTIPOLYGON);
                }
                else if (dataType == DataType.GeographicMultiCurve)
                {
                    sb.Append(connection.type_GEOGRAPHIC_MULTICURVE);
                }
                else if (dataType == DataType.GeographicMultiSurface)
                {
                    sb.Append(connection.type_GEOGRAPHIC_MULTISURFACE);
                }
            }

            if (column.AutoIncrement)
            {
                sb.Append(' ');
                if (dataType == DataType.BigInt || dataType == DataType.UnsignedBigInt)
                { // Specifically for PostgreSQL
                    sb.Append(connection.type_AUTOINCREMENT_BIGINT);
                }
                else
                {
                    sb.Append(connection.type_AUTOINCREMENT);
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

        private void BuildTableName(ConnectorBase connection, StringBuilder sb, bool considerAlias = true)
        {
            if (Schema != null)
            {
                if (Schema.DatabaseOwner.Length > 0)
                {
                    sb.Append(connection.WrapFieldName(Schema.DatabaseOwner));
                    sb.Append('.');
                }
                sb.Append(connection.WrapFieldName(_SchemaName));
            }
            else
            {
                sb.Append(@"(");
                if (_FromExpression is IPhrase)
                {
                    sb.Append(((IPhrase)_FromExpression).BuildPhrase(connection, this));
                }
                else sb.Append(_FromExpression);
                sb.Append(@")");
            }

            if (considerAlias && !string.IsNullOrEmpty(_SchemaAlias))
            {
                sb.Append(" " + connection.WrapFieldName(_SchemaAlias));
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
                bool bFirst;

                if (this.QueryMode != QueryMode.None)
                {
                    switch (this.QueryMode)
                    {
                        case QueryMode.Select:
                            {
                                if (Offset > 0 && 
                                    connection.TYPE == ConnectorBase.SqlServiceType.MSSQL &&
                                    !connection.SupportsSelectPaging())
                                {
                                    // Special case for Sql Server where in versions prior to 2012 there was no paging support
                                    BuildSelectForMsSqlPaging(sb, connection);
                                }
                                else if (Offset > 0 && Limit > 0 &&
                                    connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
                                {
                                    // Special case for Ms Access where paging is not supported so we do a complex emulation of LIMIT+OFFSET
                                    BuildSelectForMsAccessLimitOffset(sb, connection);
                                }
                                else if (Offset > 0 &&
                                    connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
                                {
                                    // Special case for Ms Access where paging is not supported so we do a complex emulation of OFFSET
                                    BuildSelectForMsAccessOffset(sb, connection);
                                }
                                else
                                {
                                    sb.Append(@" SELECT ");

                                    if (IsDistinct) sb.Append(@"DISTINCT ");

                                    if (Limit > 0 && 
                                        (connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS ||
                                        (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL && !(connection.SupportsSelectPaging() && Offset > 0))
                                        ))
                                    {
                                        sb.Append(@"TOP " + Limit);
                                        sb.Append(' ');
                                    }

                                    BuildSelectList(sb, connection);

                                    sb.Append(@" FROM ");

                                    BuildTableName(connection, sb, true);

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

                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                    {
                                        if (Limit > 0)
                                        {
                                            sb.Append(@" LIMIT ");
                                            sb.Append(Limit);

                                            // OFFSET is not supported without LIMIT
                                            if (Offset > 0)
                                            {
                                                sb.Append(@" OFFSET ");
                                                sb.Append(Offset);
                                            }
                                        }
                                    }
                                    else if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                    {
                                        if (Limit > 0)
                                        {
                                            sb.Append(@" LIMIT ");
                                            sb.Append(Limit);
                                        }
                                        if (Offset > 0)
                                        {
                                            sb.Append(@" OFFSET ");
                                            sb.Append(Offset);
                                        }
                                    }
                                    else if (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                                    {
                                        if (connection.SupportsSelectPaging() && Offset > 0)
                                        {
                                            // If we are in MsSql OFFSET/FETCH mode...

                                            sb.Append(@" OFFSET ");
                                            sb.Append(Offset);
                                            sb.Append(@" ROWS");
                                            if (Limit > 0) 
                                            {
                                                sb.Append(@" FETCH NEXT ");
                                                sb.Append(Limit);
                                                sb.Append(@" ROWS ONLY");
                                            }
                                        }
                                    }

                                    // Done with select query
                                }
                                
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

                                BuildTableName(connection, sb, false);

                                sb.Append(@" (");
                                bFirst = true;
                                foreach (AssignmentColumn ins in _ListInsertUpdate)
                                {
                                    if (bFirst) bFirst = false;
                                    else sb.Append(',');
                                    sb.Append(connection.WrapFieldName(ins.ColumnName));
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
                                                sb.Append(connection.WrapFieldName(ins.SecondTableName));
                                                sb.Append(@".");
                                            }
                                            sb.Append(connection.WrapFieldName(ins.Second.ToString()));
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
                                    sb.Append(connection.WrapFieldName(_SchemaAlias));
                                }
                                else
                                {
                                    BuildTableName(connection, sb, true);
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
                                        sb.Append(connection.WrapFieldName(upd.TableName));
                                        sb.Append(@".");
                                    }

                                    sb.Append(connection.WrapFieldName(upd.ColumnName));

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
                                            sb.Append(connection.WrapFieldName(upd.SecondTableName));
                                            sb.Append(@".");
                                        }
                                        sb.Append(connection.WrapFieldName(upd.Second.ToString()));
                                    }
                                }

                                if (hasJoins)
                                {
                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL || 
                                        connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
                                    {
                                        BuildTableName(connection, sb, true);

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

                                    BuildTableName(connection, sb, false);

                                    sb.Append(@" (");
                                    bFirst = true;
                                    foreach (AssignmentColumn ins in _ListInsertUpdate)
                                    {
                                        if (bFirst) bFirst = false;
                                        else sb.Append(',');
                                        sb.Append(connection.WrapFieldName(ins.ColumnName));
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
                                                    sb.Append(connection.WrapFieldName(ins.SecondTableName));
                                                    sb.Append(@".");
                                                }
                                                sb.Append(connection.WrapFieldName(ins.Second.ToString()));
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
                                if (_ListJoin != null && _ListJoin.Count > 0)
                                {
                                    BuildTableName(connection, sb, false);
                                }
                                sb.Append(@" FROM ");

                                BuildTableName(connection, sb, false);

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
                            }
                            break;

                        case QueryMode.CreateTable:
                            {
                                sb.Append(@"CREATE TABLE ");

                                BuildTableName(connection, sb, false);

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

                                    sb.AppendFormat(@"CONSTRAINT {0} PRIMARY KEY(", connection.WrapFieldName(@"PK_" + _SchemaName));
                                    bSep = false;
                                    foreach (TableSchema.Column col in Schema.Columns)
                                    {
                                        if (!col.IsPrimaryKey) continue;
                                        if (bSep) sb.Append(@", "); else bSep = true;
                                        if (col.IsPrimaryKey) sb.Append(connection.WrapFieldName(col.Name));
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

                                BuildTableName(connection, sb, false);

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
                                    else sb.AppendFormat(@"AFTER {0} ", connection.WrapFieldName(Schema.Columns[idx - 1].Name));
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

                                        BuildTableName(connection, sb, false);

                                        sb.Append('.');
                                        sb.Append(connection.WrapFieldName(_AlterColumnOldName));
                                        sb.Append(',');
                                        sb.Append(connection.WrapFieldName(_AlterColumn.Name));
                                        sb.Append(@",'COLUMN';");
                                    }
                                    else if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                    {
                                        sb.Append(@"ALTER TABLE ");

                                        BuildTableName(connection, sb, false);

                                        sb.Append(@" RENAME COLUMN ");
                                        sb.Append(connection.WrapFieldName(_AlterColumnOldName));
                                        sb.Append(@" TO ");
                                        sb.Append(connection.WrapFieldName(_AlterColumn.Name));
                                        sb.Append(';');
                                    }
                                }

                                if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                {
                                    // Very limited syntax, will have to do this with several statements

                                    sb.Append(@"ALTER TABLE ");

                                    BuildTableName(connection, sb, false);

                                    string alterColumnStatement = @" ALTER COLUMN ";
                                    alterColumnStatement += connection.WrapFieldName(_AlterColumn.Name);

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

                                    BuildTableName(connection, sb, false);

                                    sb.Append(' ');
                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                    {
                                        sb.AppendFormat(@"CHANGE {0} ", connection.WrapFieldName(_AlterColumnOldName != null ? _AlterColumnOldName : _AlterColumn.Name));
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
                                        else sb.AppendFormat(@"AFTER {0} ", connection.WrapFieldName(Schema.Columns[idx - 1].Name));
                                    }
                                }
                            }
                            break;

                        case QueryMode.DropColumn:
                            {
                                sb.Append(@"ALTER TABLE ");

                                BuildTableName(connection, sb, false);

                                sb.Append(@" DROP COLUMN ");
                                sb.Append(connection.WrapFieldName(_DropColumnName));
                            }
                            break;

                        case QueryMode.DropForeignKey:
                            {
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    BuildTableName(connection, sb, false);

                                    sb.Append(@" DROP FOREIGN KEY ");
                                    sb.Append(connection.WrapFieldName(_DropColumnName));
                                }
                                else if (connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    BuildTableName(connection, sb, false);

                                    sb.Append(@" DROP CONSTRAINT ");
                                    sb.Append(connection.WrapFieldName(_DropColumnName));
                                }
                                else
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    BuildTableName(connection, sb, false);

                                    sb.Append(@" DROP CONSTRAINT ");
                                    sb.Append(connection.WrapFieldName(_DropColumnName));
                                }
                            }
                            break;

                        case QueryMode.DropIndex:
                            {
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL || connection.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    BuildTableName(connection, sb, false);

                                    sb.Append(@" DROP INDEX ");
                                    sb.Append(connection.WrapFieldName(_DropColumnName));
                                }
                                else
                                {
                                    sb.Append(@"ALTER TABLE ");

                                    BuildTableName(connection, sb, false);

                                    sb.Append(@" DROP CONSTRAINT ");
                                    sb.Append(connection.WrapFieldName(_DropColumnName));
                                }
                            }
                            break;

                        case QueryMode.DropTable:
                            {
                                sb.Append(@"DROP TABLE ");

                                BuildTableName(connection, sb, false);
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

        private void BuildSelectForMsSqlPaging(StringBuilder sb, ConnectorBase connection)
        {
            // MSSQL: 
            //   WITH [table] AS 
            //   (
            //     SELECT [selects],ROW_NUMBER() OVER([orders]) AS __ROWID__ 
            //     FROM [tables, joins] WHERE [wheres]
            //   ) 
            //   SELECT * FROM [table] 
            //   WHERE __ROWID__ BETWEEN [offset+1] AND [offset+1+limit]
            // --WHERE __ROWID__ > offset

            sb.Append(@"WITH [Ordered Table] AS ( SELECT ");

            if (IsDistinct) sb.Append(@"DISTINCT ");

            BuildSelectList(sb, connection);

            sb.Append(@",ROW_NUMBER() OVER(");
            BuildOrderBy(sb, connection, false);
            sb.Append(@") AS __ROWID__");

            sb.Append(@" FROM ");

            BuildTableName(connection, sb, true);

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

            sb.Append(@") SELECT * FROM [Ordered Table]");
            if (Limit > 0) sb.AppendFormat(@" WHERE __ROWID__ BETWEEN {0} AND {1}", Offset + 1, Offset + 1 + Limit);
            else sb.AppendFormat(@" WHERE __ROWID__ > {0}", Offset);
        }

        private void BuildSelectForMsAccessOffset(StringBuilder sb, ConnectorBase connection)
        {
            // MSACCESS - OFFSET:
            // SELECT * FROM
            //  (
            //    SELECT TOP 
            //     (SELECT COUNT(*) FROM [tables, joins] WHERE [wheres]) - [offset]
            //     [selects] FROM [tables, joins] 
            //     WHERE [wheres] 
            //     ORDER BY [inverted orders]
            //  ) p ORDER BY [orders]

            sb.Append(@"SELECT * FROM ( SELECT TOP ( SELECT COUNT(*) FROM ");

            BuildTableName(connection, sb, true);

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
            sb.AppendFormat(@") - {0} ", Offset);

            if (IsDistinct) sb.Append(@"DISTINCT ");

            BuildSelectList(sb, connection);

            sb.Append(@" FROM ");

            BuildTableName(connection, sb, true);

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

            BuildGroupBy(sb, connection, true);
            BuildOrderBy(sb, connection, true);
            sb.Append(@") p ");
            BuildGroupBy(sb, connection, false);
            BuildOrderBy(sb, connection, false);
        }

        private void BuildSelectForMsAccessLimitOffset(StringBuilder sb, ConnectorBase connection)
        {
            // MSACCESS - LIMIT,OFFSET:
            // SELECT * FROM
            //  (
            //    SELECT TOP [limit] * FROM 
            //     (
            //       SELECT TOP [offset+limit] 
            //       [selects] FROM [tables, joins] 
            //       WHERE [wheres] 
            //       ORDER BY [orders]
            //     ) pp ORDER BY [inverted orders]
            //  ) p ORDER BY [orders]

            sb.AppendFormat(@"SELECT * FROM ( SELECT TOP {0} * FROM ( SELECT TOP {1} ", Limit, Offset + Limit);

            if (IsDistinct) sb.Append(@"DISTINCT ");

            BuildSelectList(sb, connection);

            sb.Append(@" FROM ");

            BuildTableName(connection, sb, true);

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

            StringBuilder sbOrderBy = new StringBuilder();
            StringBuilder sbGroupBy = new StringBuilder();
            BuildGroupBy(sbGroupBy, connection, false);
            BuildOrderBy(sbOrderBy, connection, false);
            sb.Append(sbOrderBy.ToString());
            sb.Append(sbGroupBy.ToString());
            sb.Append(@") pp ");
            BuildGroupBy(sb, connection, true);
            BuildOrderBy(sb, connection, true);
            sb.Append(@") p ");
            sb.Append(sbOrderBy.ToString());
            sb.Append(sbGroupBy.ToString());
        }
    }
}
