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

                    if (connection.Language.GroupBySupportsOrdering)
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
            var language = connection.Language;

            sb.Append(language.WrapFieldName(column.Name));
            sb.Append(' ');

            language.BuildColumnPropertiesDataType(
                sb: sb,
                connection: connection, 
                column: column, 
                relatedQuery: this, 
                isDefaultAllowed: out var isDefaultAllowed);

            if (!string.IsNullOrEmpty(column.Comment) && language.SupportsColumnComment)
            {
                sb.AppendFormat(@" COMMENT {0}",
                    language.PrepareValue(column.Comment));
            }

            if (!column.Nullable)
            {
                sb.Append(@" NOT NULL");
            }

            if (column.SRID != null && language.ColumnSRIDLocation == LanguageFactory.ColumnSRIDLocationMode.AfterNullability)
            {
                sb.Append(" SRID " + column.SRID.Value);
            }

            if (column.ComputedColumn == null)
            {
                if (!noDefault && column.Default != null && isDefaultAllowed)
                {
                    sb.Append(@" DEFAULT ");
                    Query.PrepareColumnValue(column, column.Default, sb, connection, this);
                }
            }

            sb.Append(' ');
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
                                sb.Append("INSERT ");

                                if (IgnoreErrors && language.InsertSupportsIgnore)
                                {
                                    sb.Append("IGNORE ");
                                }

                                sb.Append("INTO ");

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

                                if (hasJoins && language.UpdateJoinRequiresFromLeftTable && !string.IsNullOrEmpty(_SchemaAlias))
                                {
                                    sb.Append(language.WrapFieldName(_SchemaAlias));
                                }
                                else
                                {
                                    language.BuildTableName(this, connection, sb, true);
                                }

                                if (hasJoins && !language.UpdateJoinRequiresFromLeftTable && !language.UpdateFromInsteadOfJoin)
                                {
                                    BuildJoin(sb, connection);
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
                                    if (language.UpdateJoinRequiresFromLeftTable)
                                    {
                                        sb.Append(" FROM ");

                                        language.BuildTableName(this, connection, sb, true);

                                        BuildJoin(sb, connection);
                                    }
                                    else if (language.UpdateFromInsteadOfJoin)
                                    {
                                        sb.Append(" FROM ");
                                        language.BuildTableName(this, connection, sb, true);

                                        foreach (var join in _ListJoin)
                                        {
                                            sb.Append(", ");

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
                                        }
                                    }
                                }

                                if ((_ListWhere != null && _ListWhere.Count > 0) || (hasJoins && language.UpdateFromInsteadOfJoin))
                                {
                                    sb.Append(@" WHERE ");

                                    if (_ListWhere != null)
                                    {
                                        _ListWhere.BuildCommand(sb, new Where.BuildContext
                                        {
                                            Conn = connection,
                                            RelatedQuery = this
                                        });
                                    }

                                    if (hasJoins && language.UpdateFromInsteadOfJoin)
                                    {
                                        foreach (var join in _ListJoin)
                                        {
                                            if (join.Pairs.Count > 1)
                                            {
                                                sb.Append(@" AND ");

                                                var wl = new WhereList();

                                                foreach (var joins in join.Pairs)
                                                    wl.AddRange(joins);

                                                wl.BuildCommand(
                                                    sb,
                                                    new Where.BuildContext
                                                    {
                                                        Conn = connection,
                                                        RelatedQuery = this,
                                                        RightTableSchema = join.RightTableSchema,
                                                        RightTableName = join.RightTableAlias == null ? join.RightTableSchema.Name : join.RightTableAlias
                                                    });
                                            }
                                            else if (join.Pairs.Count == 1)
                                            {
                                                sb.Append(@" AND ");

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
                                sb.Append("DELETE ");

                                if (IgnoreErrors && language.DeleteSupportsIgnore)
                                {
                                    sb.Append("IGNORE ");
                                }

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
                                    connection.Language.BuildColumnPropertiesDataType(
                                        sb: sb,
                                        connection: connection,
                                        column: _AlterColumn,
                                        relatedQuery: this,
                                        isDefaultAllowed: out _);
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
