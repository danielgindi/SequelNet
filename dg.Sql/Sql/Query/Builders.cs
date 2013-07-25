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
                            sb.Append(connection.EncloseFieldName(join.RightTableSchema.DatabaseOwner));
                            sb.Append('.');
                        }
                        sb.Append(connection.EncloseFieldName(join.RightTableSchema.SchemaName));
                    }
                    else
                    {
                        sb.Append('(');
                        sb.Append(join.RightTableSql);
                        sb.Append(')');
                    }

                    sb.Append(' ');
                    sb.Append((join.RightTableAlias != null && join.RightTableAlias.Length > 0) ? join.RightTableAlias : (join.RightTableSchema != null ? join.RightTableSchema.SchemaName : @""));

                    if (join.Pairs.Count > 1)
                    {
                        sb.Append(@" ON ");
                        WhereList wl = new WhereList();
                        foreach (WhereList joins in join.Pairs)
                        {
                            wl.AddRange(joins);
                        }
                        wl.BuildCommand(sb, connection, this, join.RightTableSchema, join.RightTableAlias == null ? join.RightTableSchema.SchemaName : join.RightTableAlias);
                    }
                    else if (join.Pairs.Count == 1)
                    {
                        sb.Append(@" ON ");
                        join.Pairs[0].BuildCommand(sb, connection, this, join.RightTableSchema, join.RightTableAlias == null ? join.RightTableSchema.SchemaName : join.RightTableAlias);
                    }
                }
            }
        }

        private void BuildOrderBy(StringBuilder sb, ConnectorBase connection, bool invert)
        {
            if (_ListOrderBy != null && _ListOrderBy.Count > 0)
            {
                sb.Append(@" ORDER BY ");
                bool bFirst = true;
                foreach (OrderBy orderBy in _ListOrderBy)
                {
                    if (bFirst) bFirst = false;
                    else sb.Append(',');
                    if (orderBy.Randomize)
                    {
                        switch (connection.TYPE)
                        {
                            case ConnectorBase.SqlServiceType.MSSQL:
                                sb.Append(@"NEWID()");
                                break;
                            default:
                            case ConnectorBase.SqlServiceType.MYSQL:
                                sb.Append(@"RAND()");
                                break;
                            case ConnectorBase.SqlServiceType.MSACCESS:
                                if (orderBy.TableName != null) sb.Append(@"RND(" + connection.EncloseFieldName(orderBy.TableName) + @"." + connection.EncloseFieldName(orderBy.ColumnName.ToString()) + @")");
                                else sb.Append(@"RND(" + connection.EncloseFieldName(orderBy.ColumnName.ToString()) + @")");
                                break;
                        }
                    }
                    else
                    {
                        if (connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS && Schema != null && !orderBy.IsLiteral)
                        {
                            TableSchema.Column col = Schema.Columns.Find(orderBy.ColumnName.ToString());
                            if (col != null && col.ActualDataType == DataType.Boolean) sb.Append(@" NOT ");
                        }
                        if (orderBy.IsLiteral)
                        {
                            if (orderBy.ColumnName is dg.Sql.BasePhrase)
                            {
                                sb.Append(((dg.Sql.BasePhrase)orderBy.ColumnName).BuildPhrase(connection));
                            }
                            else sb.Append(orderBy.ColumnName);
                        }
                        else
                        {
                            if (orderBy.TableName != null) sb.Append(connection.EncloseFieldName(orderBy.TableName) + @"." + connection.EncloseFieldName(orderBy.ColumnName.ToString()));
                            else sb.Append(connection.EncloseFieldName(orderBy.ColumnName.ToString()));
                        }
                        switch (orderBy.SortDirection)
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

                    if (groupBy.IsLiteral)
                    {
                        if (groupBy.ColumnName is dg.Sql.BasePhrase)
                        {
                            sb.Append(((dg.Sql.BasePhrase)groupBy.ColumnName).BuildPhrase(connection));
                        }
                        else sb.Append(groupBy.ColumnName);
                    }
                    else
                    {
                        if (groupBy.TableName != null) sb.Append(connection.EncloseFieldName(groupBy.TableName) + @"." + connection.EncloseFieldName(groupBy.ColumnName.ToString()));
                        else sb.Append(connection.EncloseFieldName(groupBy.ColumnName.ToString()));
                    }
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

        private void BuildCreateIndex(StringBuilder sb, ConnectorBase connection, object indexObj)
        {
            if (indexObj is TableSchema.Index)
            {
                TableSchema.Index index = indexObj as TableSchema.Index;
                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                {
                    sb.Append(@"ALTER TABLE ");

                    if (Schema.DatabaseOwner.Length > 0)
                    {
                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                        sb.Append('.');
                    }
                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                    sb.Append(@" ADD ");

                    if (index.Mode == dg.Sql.TableSchema.IndexMode.PrimaryKey)
                    {
                        sb.AppendFormat(@"CONSTRAINT {0} PRIMARY KEY ", connection.EncloseFieldName(index.Name));
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
                        sb.Append(connection.EncloseFieldName(index.Name));
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
                        sb.Append(connection.EncloseFieldName(index.ColumnNames[i]));
                        if (index.ColumnLength[i] > 0) sb.AppendFormat("({0})", index.ColumnLength[i]);
                        sb.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                    }
                    sb.Append(@");");
                }
                else if (connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
                {
                    sb.Append(@"ALTER TABLE ");

                    if (Schema.DatabaseOwner.Length > 0)
                    {
                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                        sb.Append('.');
                    }
                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                    sb.Append(@" ADD ");

                    if (index.Mode == dg.Sql.TableSchema.IndexMode.PrimaryKey)
                    {
                        sb.AppendFormat(@"CONSTRAINT {0} PRIMARY KEY ", connection.EncloseFieldName(index.Name));
                    }
                    else
                    {
                        if (index.Mode == TableSchema.IndexMode.Unique) sb.Append(@"UNIQUE ");
                        sb.Append(@"INDEX ");
                        sb.Append(connection.EncloseFieldName(index.Name));
                        sb.Append(@" ");
                    }
                    sb.Append(@"(");
                    for (int i = 0; i < index.ColumnNames.Length; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append(connection.EncloseFieldName(index.ColumnNames[i]));
                        sb.Append(index.ColumnSort[i] == SortDirection.ASC ? @" ASC" : @" DESC");
                    }
                    sb.Append(@");");
                }
                else if (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    if (index.Mode == dg.Sql.TableSchema.IndexMode.PrimaryKey)
                    {
                        sb.Append(@"ALTER TABLE ");

                        if (Schema.DatabaseOwner.Length > 0)
                        {
                            sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                            sb.Append('.');
                        }
                        sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                        sb.Append(@" ADD CONSTRAINT ");
                        sb.Append(connection.EncloseFieldName(index.Name));
                        sb.Append(@" PRIMARY KEY ");

                        if (index.Cluster == TableSchema.ClusterMode.Clustered) sb.Append(@"CLUSTERED ");
                        else if (index.Cluster == TableSchema.ClusterMode.NonClustered) sb.Append(@"NONCLUSTERED ");                      

                        sb.Append(@"(");
                        for (int i = 0; i < index.ColumnNames.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append(connection.EncloseFieldName(index.ColumnNames[i]));
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
                        sb.Append(connection.EncloseFieldName(index.Name));

                        sb.Append(@"ON ");
                        if (Schema.DatabaseOwner.Length > 0)
                        {
                            sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                            sb.Append('.');
                        }
                        sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                        
                        sb.Append(@"(");
                        for (int i = 0; i < index.ColumnNames.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append(connection.EncloseFieldName(index.ColumnNames[i]));
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

                if (Schema.DatabaseOwner.Length > 0)
                {
                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                    sb.Append('.');
                }
                sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                sb.Append(@" ADD CONSTRAINT ");
                sb.Append(connection.EncloseFieldName(foreignKey.Name));
                sb.Append(@" FOREIGN KEY (");

                for (int i = 0; i < foreignKey.Columns.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(connection.EncloseFieldName(foreignKey.Columns[i]));
                }
                sb.AppendFormat(@") REFERENCES {0} (", foreignKey.ForeignTable);
                for (int i = 0; i < foreignKey.ForeignColumns.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(connection.EncloseFieldName(foreignKey.ForeignColumns[i]));
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
            sb.Append(connection.EncloseFieldName(column.Name));
            sb.Append(' ');

            bool isTextField = false;

            DataType dataType = column.ActualDataType;
            if (!column.AutoIncrement || connection.TYPE != ConnectorBase.SqlServiceType.MSACCESS)
            {
                if (dataType == DataType.VarChar)
                {
                    if (column.MaxLength <= 0)
                    {
                        sb.Append(connection.type_TEXT);
                        isTextField = true;
                    }
                    else if (column.MaxLength < 256)
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
                    if (column.MaxLength <= 0 || column.MaxLength >= 255)
                    {
                        sb.Append(connection.type_CHAR);
                        sb.AppendFormat(@"({0})", 255);
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
                sb.Append(' ');
            }
            if (column.AutoIncrement)
            {
                sb.Append(connection.type_AUTOINCREMENT);
                sb.Append(' ');
            }
            if (!column.Nullable)
            {
                sb.Append(@"NOT NULL ");
            }
            if (!NoDefault && column.Default != null && (!(isTextField && connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)))
            {
                sb.Append(@"DEFAULT ");
                PrepareColumnValue(column, column.Default, sb, connection);
                sb.Append(' ');
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
                using (DbCommand cmd = FactoryBase.Factory().NewCommand(_StoredProcedureName))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (_StoredProcedureParameters != null)
                    {
                        foreach (DbParameter param in _StoredProcedureParameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    if (connection != null)
                    {
                        cmd.Connection = connection.Connection;
                        cmd.Transaction = connection.Transaction;
                    }
                    return cmd;
                }
            }
            else
            {
                DbCommand cmd = FactoryBase.Factory().NewCommand(BuildCommand(connection));
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
                bool bFirst;

                if (this.QueryMode != QueryMode.None)
                {
                    switch (this.QueryMode)
                    {
                        case QueryMode.Select:
                            {
                                if (_ListSelect.Count == 0) _ListSelect.Add(new SelectColumn(@"*", true));

                                bool mssqlLimitOffsetMode =
                                    (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                                    && Offset > 0;
                                bool msaccessOffsetMode =
                                    (connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
                                    && Offset > 0;
                                bool msaccessLimitOffsetMode = msaccessOffsetMode && Limit > 0;

                                // MSSQL: 
                                //   WITH [table] AS 
                                //   (
                                //     SELECT [selects],ROW_NUMBER() OVER([orders]) AS __ROWID__ 
                                //     FROM [tables, joins] WHERE [wheres]
                                //   ) 
                                //   SELECT * FROM [table] 
                                //   WHERE __ROWID__ BETWEEN [offset+1] AND [offset+1+limit]
                                // --WHERE __ROWID__ > offset

                                // MSACCESS - OFFSET:
                                // SELECT * FROM
                                //  (
                                //    SELECT TOP 
                                //     (SELECT COUNT(*) FROM [tables, joins] WHERE [wheres]) - [offset]
                                //     [selects] FROM [tables, joins] 
                                //     WHERE [wheres] 
                                //     ORDER BY [inverted orders]
                                //  ) p ORDER BY [orders]

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

                                if (mssqlLimitOffsetMode) sb.Append(@"WITH [Ordered Table] AS ( SELECT ");
                                else if (msaccessLimitOffsetMode)
                                {
                                    sb.AppendFormat(@"SELECT * FROM ( SELECT TOP {0} * FROM ( SELECT TOP {1} ", Limit, Offset + Limit);
                                }
                                else if (msaccessOffsetMode)
                                {
                                    sb.Append(@"SELECT * FROM ( SELECT TOP ( SELECT COUNT(*) FROM ");
                                    if (Schema != null)
                                    {
                                        if (Schema.DatabaseOwner.Length > 0)
                                        {
                                            sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                            sb.Append('.');
                                        }
                                        sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                    }
                                    else sb.Append(connection.EncloseFieldName(_FromExpressionTableAlias));
                                    BuildJoin(sb, connection);
                                    if (_ListWhere != null && _ListWhere.Count > 0)
                                    {
                                        sb.Append(@" WHERE ");
                                        _ListWhere.BuildCommand(sb, connection, this, null, null);
                                    }
                                    sb.AppendFormat(@") - {0} ", Offset);
                                }
                                else
                                { // DEFAULT MODE
                                    sb.Append(@" SELECT ");
                                }

                                if (IsDistinct) sb.Append(@"DISTINCT ");

                                if (!mssqlLimitOffsetMode
                                    && (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL || connection.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
                                    && Limit > 0 && Offset <= 0)
                                {
                                    sb.Append(@"TOP " + Limit);
                                    sb.Append(' ');
                                }

                                bFirst = true;
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
                                            sb.Append(connection.PrepareValue(sel.Value));
                                        }

                                        if (!string.IsNullOrEmpty(sel.Alias))
                                        {
                                            sb.Append(@" AS ");
                                            sb.Append(connection.EncloseFieldName(sel.Alias));
                                        }
                                    }
                                    else if (sel.ObjectType == ValueObjectType.Literal)
                                    {
                                        if (string.IsNullOrEmpty(sel.Alias))
                                        {
                                            sb.Append(sel.ColumnName);
                                        }
                                        else
                                        {
                                            sb.Append(sel.ColumnName);
                                            sb.Append(@" AS ");
                                            sb.Append(connection.EncloseFieldName(sel.Alias));
                                        }
                                    }
                                    else
                                    {
                                        if (_ListJoin != null && _ListJoin.Count > 0 && string.IsNullOrEmpty(sel.TableName))
                                        {
                                            if (Schema != null)
                                            {
                                                if (Schema.DatabaseOwner.Length > 0)
                                                {
                                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                                    sb.Append('.');
                                                }
                                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                            }
                                            else sb.Append(connection.EncloseFieldName(_FromExpressionTableAlias));
                                            sb.Append('.');
                                            sb.Append(connection.EncloseFieldName(sel.ColumnName));
                                            if (!string.IsNullOrEmpty(sel.Alias))
                                            {
                                                sb.Append(@" AS ");
                                                sb.Append(connection.EncloseFieldName(sel.Alias));
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(sel.TableName))
                                            {
                                                sb.Append(connection.EncloseFieldName(sel.TableName));
                                                sb.Append('.');
                                            }
                                            sb.Append(connection.EncloseFieldName(sel.ColumnName));
                                            if (!string.IsNullOrEmpty(sel.Alias))
                                            {
                                                sb.Append(@" AS ");
                                                sb.Append(connection.EncloseFieldName(sel.Alias));
                                            }
                                        }
                                    }
                                }

                                if (mssqlLimitOffsetMode)
                                {
                                    sb.Append(@",ROW_NUMBER() OVER(");
                                    BuildOrderBy(sb, connection, false);
                                    sb.Append(@") AS __ROWID__");
                                }

                                sb.Append(@" FROM ");
                                if (Schema != null)
                                {
                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                }
                                else
                                {
                                    sb.Append(@"(");
                                    if (_FromExpression is dg.Sql.BasePhrase)
                                    {
                                        sb.Append(((dg.Sql.BasePhrase)_FromExpression).BuildPhrase(connection));
                                    }
                                    else sb.Append(_FromExpression);
                                    sb.Append(@") ");
                                    sb.Append(connection.EncloseFieldName(_FromExpressionTableAlias));
                                }

                                BuildJoin(sb, connection);

                                if (_ListWhere != null && _ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    _ListWhere.BuildCommand(sb, connection, this, null, null);
                                }

                                if (mssqlLimitOffsetMode)
                                {
                                    sb.Append(@") SELECT * FROM [Ordered Table]");
                                    if (Limit > 0) sb.AppendFormat(@" WHERE __ROWID__ BETWEEN {0} AND {1}", Offset + 1, Offset + 1 + Limit);
                                    else sb.AppendFormat(@" WHERE __ROWID__ > {0}", Offset);
                                }
                                else if (msaccessLimitOffsetMode)
                                {
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
                                else if (msaccessOffsetMode)
                                {
                                    BuildGroupBy(sb, connection, true);
                                    BuildOrderBy(sb, connection, true);
                                    sb.Append(@") p ");
                                    BuildGroupBy(sb, connection, false);
                                    BuildOrderBy(sb, connection, false);
                                }
                                else
                                {
                                    BuildGroupBy(sb, connection, false);
                                    BuildOrderBy(sb, connection, false);

                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                    {
                                        if (Limit > 0)
                                        {
                                            sb.Append(@" LIMIT " + Limit);
                                            if (Offset > 0) sb.Append(@" OFFSET " + Offset);
                                        }
                                    }
                                }

                                switch (_QueryHint)
                                {
                                    case QueryHint.ForUpdate:
                                        if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL || connection.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                                        {
                                            sb.Append(@" FOR UPDATE");
                                        }
                                        break;
                                    case QueryHint.LockInSharedMode:
                                        if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                        {
                                            sb.Append(@" LOCK IN SHARED MODE");
                                        }
                                        break;
                                }
                            }

                            break;
                        case QueryMode.Insert:
                            {
                                sb.Append(@"INSERT INTO ");

                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                                sb.Append(@" (");
                                bFirst = true;
                                foreach (AssignmentColumn ins in _ListInsertUpdate)
                                {
                                    if (bFirst) bFirst = false;
                                    else sb.Append(',');
                                    sb.Append(connection.EncloseFieldName(ins.ColumnName));
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
                                            else PrepareColumnValue(Schema.Columns.Find(ins.ColumnName), ins.Second, sb, connection);
                                        }
                                        else if (ins.SecondType == ValueObjectType.ColumnName)
                                        {
                                            if (ins.SecondTableName != null)
                                            {
                                                sb.Append(connection.EncloseFieldName(ins.SecondTableName));
                                                sb.Append(@".");
                                            }
                                            sb.Append(connection.EncloseFieldName(ins.Second.ToString()));
                                        }
                                    }
                                    sb.Append(@")");

                                    if (_ListWhere != null && _ListWhere.Count > 0)
                                    {
                                        sb.Append(@" WHERE ");
                                        _ListWhere.BuildCommand(sb, connection, this, null, null);
                                    }
                                }
                            }

                            break;
                        case QueryMode.Update:
                            {
                                sb.Append(@"UPDATE ");

                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                                bFirst = true;
                                foreach (AssignmentColumn upd in _ListInsertUpdate)
                                {
                                    if (bFirst)
                                    {
                                        sb.Append(@" SET ");
                                        bFirst = false;
                                    }
                                    else sb.Append(',');
                                    sb.Append(connection.EncloseFieldName(upd.ColumnName));
                                    sb.Append('=');

                                    if (upd.SecondType == ValueObjectType.Literal)
                                    {
                                        sb.Append(upd.Second);
                                    }
                                    else if (upd.SecondType == ValueObjectType.Value)
                                    {
                                        PrepareColumnValue(Schema.Columns.Find(upd.ColumnName), upd.Second, sb, connection);
                                    }
                                    else if (upd.SecondType == ValueObjectType.ColumnName)
                                    {
                                        if (upd.SecondTableName != null)
                                        {
                                            sb.Append(connection.EncloseFieldName(upd.SecondTableName));
                                            sb.Append(@".");
                                        }
                                        sb.Append(connection.EncloseFieldName(upd.Second.ToString()));
                                    }
                                }

                                if (_ListWhere != null && _ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    _ListWhere.BuildCommand(sb, connection, this, null, null);
                                }

                                BuildOrderBy(sb, connection, false);
                            }

                            break;
                        case QueryMode.InsertOrUpdate:
                            {
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    sb.Append(@"REPLACE INTO ");

                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                                    sb.Append(@" (");
                                    bFirst = true;
                                    foreach (AssignmentColumn ins in _ListInsertUpdate)
                                    {
                                        if (bFirst) bFirst = false;
                                        else sb.Append(',');
                                        sb.Append(connection.EncloseFieldName(ins.ColumnName));
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
                                                else PrepareColumnValue(Schema.Columns.Find(ins.ColumnName), ins.Second, sb, connection);
                                            }
                                            else if (ins.SecondType == ValueObjectType.ColumnName)
                                            {
                                                if (ins.SecondTableName != null)
                                                {
                                                    sb.Append(connection.EncloseFieldName(ins.SecondTableName));
                                                    sb.Append(@".");
                                                }
                                                sb.Append(connection.EncloseFieldName(ins.Second.ToString()));
                                            }
                                        }
                                        sb.Append(@")");

                                        if (_ListWhere != null && _ListWhere.Count > 0)
                                        {
                                            sb.Append(@" WHERE ");
                                            _ListWhere.BuildCommand(sb, connection, this, null, null);
                                        }
                                    }
                                }
                                else
                                {
                                    _NeedTransaction = true;

                                    sb.Append(@"UPDATE ");

                                    #region Update clause

                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                                    bFirst = true;
                                    foreach (AssignmentColumn upd in _ListInsertUpdate)
                                    {
                                        if (bFirst)
                                        {
                                            sb.Append(@" SET ");
                                            bFirst = false;
                                        }
                                        else sb.Append(',');
                                        sb.Append(connection.EncloseFieldName(upd.ColumnName));
                                        sb.Append('=');

                                        if (upd.SecondType == ValueObjectType.Literal)
                                        {
                                            sb.Append(upd.Second);
                                        }
                                        else if (upd.SecondType == ValueObjectType.Value)
                                        {
                                            PrepareColumnValue(Schema.Columns.Find(upd.ColumnName), upd.Second, sb, connection);
                                        }
                                        else if (upd.SecondType == ValueObjectType.ColumnName)
                                        {
                                            if (upd.SecondTableName != null)
                                            {
                                                sb.Append(connection.EncloseFieldName(upd.SecondTableName));
                                                sb.Append(@".");
                                            }
                                            sb.Append(connection.EncloseFieldName(upd.Second.ToString()));
                                        }
                                    }

                                    if (_ListWhere != null && _ListWhere.Count > 0)
                                    {
                                        sb.Append(@" WHERE ");
                                        _ListWhere.BuildCommand(sb, connection, this, null, null);
                                    }

                                    BuildOrderBy(sb, connection, false);

                                    #endregion

                                    sb.Append(@"; IF @@rowcount = 0 BEGIN");

                                    #region Insert clause

                                    sb.Append(@"INSERT INTO ");

                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                                    sb.Append(@" (");
                                    bFirst = true;
                                    foreach (AssignmentColumn ins in _ListInsertUpdate)
                                    {
                                        if (bFirst) bFirst = false;
                                        else sb.Append(',');
                                        sb.Append(connection.EncloseFieldName(ins.ColumnName));
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
                                                else PrepareColumnValue(Schema.Columns.Find(ins.ColumnName), ins.Second, sb, connection);
                                            }
                                            else if (ins.SecondType == ValueObjectType.ColumnName)
                                            {
                                                if (ins.SecondTableName != null)
                                                {
                                                    sb.Append(connection.EncloseFieldName(ins.SecondTableName));
                                                    sb.Append(@".");
                                                }
                                                sb.Append(connection.EncloseFieldName(ins.Second.ToString()));
                                            }
                                        }
                                        sb.Append(@")");

                                        if (_ListWhere != null && _ListWhere.Count > 0)
                                        {
                                            sb.Append(@" WHERE ");
                                            _ListWhere.BuildCommand(sb, connection, this, null, null);
                                        }
                                    }

                                    #endregion

                                    sb.Append(@"END");
                                }
                            }
                            break;
                        case QueryMode.Delete:
                            {
                                sb.Append(@"DELETE");
                                if (_ListJoin != null && _ListJoin.Count > 0)
                                {
                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                }
                                sb.Append(@" FROM ");

                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                                BuildJoin(sb, connection);
                                if (_ListWhere != null && _ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    _ListWhere.BuildCommand(sb, connection, this, null, null);
                                }
                                BuildOrderBy(sb, connection, false);
                            }

                            break;
                        case QueryMode.CreateTable:
                            {
                                sb.Append(@"CREATE TABLE ");

                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));

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

                                    sb.AppendFormat(@"CONSTRAINT {0} PRIMARY KEY(", connection.EncloseFieldName(@"PK_" + Schema.SchemaName));
                                    bSep = false;
                                    foreach (TableSchema.Column col in Schema.Columns)
                                    {
                                        if (!col.IsPrimaryKey) continue;
                                        if (bSep) sb.Append(@", "); else bSep = true;
                                        if (col.IsPrimaryKey) sb.Append(connection.EncloseFieldName(col.Name));
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

                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                                sb.Append(@" ADD ");

                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    sb.Append(@"COLUMN ");
                                }
                                BuildColumnProperties(sb, connection, _AlterColumn, false);
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    int idx = Schema.Columns.IndexOf(_AlterColumn);
                                    if (idx == 0) sb.Append(@"FIRST ");
                                    else sb.AppendFormat(@"AFTER {0} ", connection.EncloseFieldName(Schema.Columns[idx - 1].Name));
                                }
                            }
                            break;
                        case QueryMode.ChangeColumn:
                            {
                                if (_AlterColumnOldName != null && _AlterColumnOldName.Length == 0) _AlterColumnOldName = null;
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MSSQL && _AlterColumnOldName != null)
                                {
                                    sb.Append(@"EXEC sp_rename ");
                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                    sb.Append('.');
                                    sb.Append(connection.EncloseFieldName(_AlterColumnOldName));
                                    sb.Append(',');
                                    sb.Append(connection.EncloseFieldName(_AlterColumn.Name));
                                    sb.Append(@",'COLUMN';");
                                }
                                sb.Append(@"ALTER TABLE ");
                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                sb.Append(' ');
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    sb.AppendFormat(@"CHANGE {0} ", connection.EncloseFieldName(_AlterColumnOldName != null ? _AlterColumnOldName : _AlterColumn.Name));
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
                                    else sb.AppendFormat(@"AFTER {0} ", connection.EncloseFieldName(Schema.Columns[idx - 1].Name));
                                }
                            }
                            break;
                        case QueryMode.DropColumn:
                            {
                                sb.Append(@"ALTER TABLE ");
                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                sb.Append(@" DROP COLUMN ");
                                sb.Append(connection.EncloseFieldName(_DropColumnName));
                            }
                            break;
                        case QueryMode.DropForeignKey:
                            {
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    sb.Append(@"ALTER TABLE ");
                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                    sb.Append(@" DROP FOREIGN KEY ");
                                    sb.Append(connection.EncloseFieldName(_DropColumnName));
                                }
                                else
                                {
                                    sb.Append(@"ALTER TABLE ");
                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                    sb.Append(@" DROP CONSTRAINT ");
                                    sb.Append(connection.EncloseFieldName(_DropColumnName));
                                }
                            }
                            break;
                        case QueryMode.DropIndex:
                            {
                                if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                {
                                    sb.Append(@"ALTER TABLE ");
                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                    sb.Append(@" DROP INDEX ");
                                    sb.Append(connection.EncloseFieldName(_DropColumnName));
                                }
                                else
                                {
                                    sb.Append(@"ALTER TABLE ");
                                    if (Schema.DatabaseOwner.Length > 0)
                                    {
                                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                        sb.Append('.');
                                    }
                                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));
                                    sb.Append(@" DROP CONSTRAINT ");
                                    sb.Append(connection.EncloseFieldName(_DropColumnName));
                                }
                            }
                            break;
                        case QueryMode.DropTable:
                            {
                                sb.Append(@"DROP TABLE ");
                                if (Schema.DatabaseOwner.Length > 0)
                                {
                                    sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                                    sb.Append('.');
                                }
                                sb.Append(connection.EncloseFieldName(Schema.SchemaName));
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
