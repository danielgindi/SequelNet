using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using SequelNet.Connector;

namespace SequelNet;

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

            bool first = true;
            foreach (SelectColumn sel in _ListSelect)
            {
                if (first) first = false;
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
                    if (join.RightTableSql is Query)
                    {
                        sb.Append(((Query)join.RightTableSql).BuildCommand(connection));
                    }
                    else if (join.RightTableSql is Phrases.Union)
                    {
                        sb.Append('(');
                        ((IPhrase)join.RightTableSql).Build(sb, connection);
                        sb.Append(')');
                    }
                    else if (join.RightTableSql is IPhrase)
                    {
                        ((IPhrase)join.RightTableSql).Build(sb, connection);
                    }
                    else
                    {
                        sb.Append(join.RightTableSql.ToString());
                    }
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
            bool first = true;
            foreach (GroupBy groupBy in _ListGroupBy)
            {
                if (first) first = false;
                else sb.Append(',');

                if (groupBy.ColumnName is IPhrase)
                {
                    ((IPhrase)groupBy.ColumnName).Build(sb, connection, this);
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
        StringBuilder sb = new StringBuilder();

        BuildCommand(sb, connection);
        
        return sb.ToString();
    }

    public void BuildCommand(StringBuilder sb, ConnectorBase connection)
    {
        if (this.QueryMode == QueryMode.ExecuteStoredProcedure ||
            this.QueryMode == QueryMode.None)
            return;

        bool ownsConnection = false;
        if (connection == null)
        {
            ownsConnection = true;
            connection = ConnectorBase.Create();
        }
        try
        {
            var language = connection.Language;

            bool first;

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

                            if (_ListIndexHint != null && _ListIndexHint.Count > 0)
                            {
                                language.BuildIndexHints(_ListIndexHint, sb, connection, this);
                            }

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

                            // UNION/INTERSECT/EXCEPT are before ORDER BY/LIMIT/OFFSET and hint
                            if (_QueryCombineData != null)
                            {
                                foreach (var combine in _QueryCombineData)
                                {
                                    switch (combine.Mode)
                                    {
                                        default:
                                        case QueryCombineMode.Union:
                                            sb.Append(" UNION ");
                                            break;
                                        case QueryCombineMode.Intersect:
                                            sb.Append(" INTERSECT ");
                                            break;
                                        case QueryCombineMode.Except:
                                            sb.Append(" EXCEPT ");
                                            break;
                                    }

                                    if (combine.All)
                                    {
                                        sb.Append("ALL ");
                                    }

                                    combine.Query.BuildCommand(sb, connection);
                                }
                            }

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
                                case QueryHint.LockInShareMode:
                                    if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                                    {
                                        sb.Append(@" LOCK IN SHARE MODE");
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
                            if (OnConflictDoUpdate != null && language.InsertSupportsMerge && !language.InsertSupportsOnConflictDoUpdate)
                            {
                                // MERGE INTO ... USING ... ON ... WHEN NOT MATCHED UPDATE ... WHEN MATCHED INSERT ...
                                language.BuildOnConflictSetMerge(sb, connection, OnConflictDoUpdate, this);
                            }
                            else
                            {
                                // INSERT [IGNORE] INTO (...) VALUES (...) [ON CONFLICT (...) DO NOTHING] [ON CONFLICT (...) DO UPDATE SET ...]

                                sb.Append("INSERT ");

                                if (OnConflictDoNothing != null && language.InsertSupportsIgnore && !language.InsertSupportsOnConflictDoNothing)
                                {
                                    sb.Append("IGNORE ");
                                }

                                sb.Append("INTO ");

                                language.BuildTableName(this, connection, sb, false);

                                sb.Append(@" (");
                                first = true;
                                foreach (AssignmentColumn ins in _ListInsertUpdate)
                                {
                                    if (first) first = false;
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
                                    sb.Append(") VALUES (");
                                    first = true;
                                    foreach (AssignmentColumn ins in _ListInsertUpdate)
                                    {
                                        if (first) first = false;
                                        else sb.Append(',');

                                        ins.BuildSecond(sb, connection, this);
                                    }
                                    sb.Append(")");
                                }

                                if (OnConflictDoNothing != null && !language.InsertSupportsIgnore && language.InsertSupportsOnConflictDoNothing)
                                {
                                    sb.Append(" ");
                                    language.BuildOnConflictDoNothing(sb, connection, OnConflictDoNothing, this);
                                }

                                if (OnConflictDoUpdate != null && language.InsertSupportsOnConflictDoUpdate)
                                {
                                    sb.Append(" ");
                                    language.BuildOnConflictDoUpdate(sb, connection, OnConflictDoUpdate, this);
                                }
                            }
                        }

                        break;

                    case QueryMode.Update:
                        {
                            bool hasJoins = _ListJoin != null && _ListJoin.Count > 0;

                            sb.Append(@"UPDATE ");

                            if (OnConflictDoNothing != null && language.UpdateSupportsIgnore)
                            {
                                sb.Append("IGNORE ");
                            }

                            if (hasJoins && language.UpdateJoinRequiresFromLeftTable && !string.IsNullOrEmpty(_SchemaAlias))
                            {
                                sb.Append(language.WrapFieldName(_SchemaAlias));
                            }
                            else
                            {
                                language.BuildTableName(this, connection, sb, true);

                                if (_ListIndexHint != null && _ListIndexHint.Count > 0)
                                {
                                    language.BuildIndexHints(_ListIndexHint, sb, connection, this);
                                }
                            }

                            if (hasJoins && !language.UpdateJoinRequiresFromLeftTable && !language.UpdateFromInsteadOfJoin)
                            {
                                BuildJoin(sb, connection);
                            }

                            first = true;
                            foreach (AssignmentColumn upd in _ListInsertUpdate)
                            {
                                if (first)
                                {
                                    sb.Append(@" SET ");
                                    first = false;
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

                                    if (_ListIndexHint != null && _ListIndexHint.Count > 0)
                                    {
                                        language.BuildIndexHints(_ListIndexHint, sb, connection, this);
                                    }

                                    BuildJoin(sb, connection);
                                }
                                else if (language.UpdateFromInsteadOfJoin)
                                {
                                    sb.Append(" FROM ");
                                    language.BuildTableName(this, connection, sb, true);

                                    if (_ListIndexHint != null && _ListIndexHint.Count > 0)
                                    {
                                        language.BuildIndexHints(_ListIndexHint, sb, connection, this);
                                    }

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
                                                ((IPhrase)join.RightTableSql).Build(sb, connection);
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
                                first = true;
                                foreach (AssignmentColumn ins in _ListInsertUpdate)
                                {
                                    if (first) first = false;
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
                                    first = true;
                                    foreach (AssignmentColumn ins in _ListInsertUpdate)
                                    {
                                        if (first) first = false;
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
                                NeedTransaction = true;

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

                            if (_ListIndexHint != null && _ListIndexHint.Count > 0)
                            {
                                language.BuildIndexHints(_ListIndexHint, sb, connection, this);
                            }

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
                                language.BuildColumnProperties(col, false, sb, connection, this);
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

                    case QueryMode.CreateIndexes:
                        {
                            if ((Schema.Indexes.Count + Schema.ForeignKeys.Count) > 1)
                            {
                                NeedTransaction = true;
                            }

                            var qry2 = Query.New(Schema);
                            qry2._SchemaAlias = _SchemaAlias;
                            qry2._SchemaName = _SchemaName;

                            foreach (TableSchema.Index index in Schema.Indexes)
                                qry2.CreateIndex(index);

                            foreach (TableSchema.ForeignKey foreignKey in Schema.ForeignKeys)
                                qry2.CreateForeignKey(foreignKey);

                            sb.Append(qry2.BuildCommand(connection));
                        }
                        break;

                    case QueryMode.AlterTable:
                        if (AlterTableSteps != null)
                        {
                            bool hasAlter = false;

                            Action addAlter = () => {
                                if (!hasAlter)
                                {
                                    sb.Append("ALTER TABLE ");
                                    language.BuildTableName(this, connection, sb, false);
                                    sb.Append(" ");
                                    hasAlter = true;
                                }
                                else
                                {
                                    sb.Append(", ");
                                }
                            };

                            Action closeAlter = () => {
                                if (hasAlter)
                                {
                                    sb.Append(";");
                                    hasAlter = false;
                                }
                            };

                            var lastStepType = AlterTableType.AddColumn;
                            
                            foreach (var step in AlterTableSteps)
                            {
                                if (!language.SupportsMultipleAlterTable)
                                    closeAlter();

                                var isStepAddType = 
                                    step.Type == AlterTableType.AddColumn || 
                                    step.Type == AlterTableType.CreateIndex;

                                if (!language.IsAlterTableTypeMixCompatible(step.Type, lastStepType))
                                {
                                    closeAlter();
                                }

                                bool shouldAddCommand = !language.SameAlterTableCommandsAreCommaSeparated || !hasAlter;

                                lastStepType = step.Type;

                                switch (step.Type)
                                {
                                    case AlterTableType.AddColumn:
                                        {
                                            addAlter();

                                            if (shouldAddCommand)
                                                sb.Append(language.AlterTableAddCommandName + " ");

                                            sb.Append(language.AlterTableAddColumnCommandName + " ");

                                            language.BuildAddColumn(step, sb, connection, this);
                                        }
                                        break;

                                    case AlterTableType.ChangeColumn:
                                        {
                                            if (!string.IsNullOrEmpty(step.OldItemName))
                                            {
                                                if (!language.SupportsRenameColumn)
                                                {
                                                    throw new NotImplementedException("Column rename is not implemented in this db connector.");
                                                }

                                                if (language.HasSeparateRenameColumn)
                                                {
                                                    closeAlter();

                                                    language.BuildSeparateRenameColumn(this, connection, step, sb);
                                                }
                                            }

                                            if (!language.SupportsMultipleAlterColumn)
                                                closeAlter();

                                            addAlter();

                                            if (shouldAddCommand)
                                                sb.Append(language.ChangeColumnCommandName + " ");

                                            language.BuildChangeColumn(step, sb, connection, this);
                                        }
                                        break;

                                    case AlterTableType.DropColumn:
                                        {
                                            addAlter();

                                            if (shouldAddCommand)
                                                sb.Append(language.DropColumnCommandName);

                                            sb.Append(language.WrapFieldName(step.OldItemName));
                                        }
                                        break;

                                    case AlterTableType.CreateIndex:
                                        {
                                            if (language.HasSeparateCreateIndex(step.Index))
                                            {
                                                closeAlter();
                                            }
                                            else
                                            {
                                                addAlter();

                                                if (shouldAddCommand)
                                                    sb.Append(language.AlterTableAddCommandName + " ");

                                                sb.Append(language.AlterTableAddIndexCommandName + " ");
                                            }

                                            language.BuildCreateIndex(step.Index, sb, this, connection);
                                        }
                                        break;

                                    case AlterTableType.DropIndex:
                                        {
                                            addAlter();

                                            if (shouldAddCommand)
                                                sb.Append(language.DropIndexCommandName + " ");

                                            sb.Append(language.WrapFieldName(step.OldItemName));
                                        }
                                        break;

                                    case AlterTableType.DropPrimaryKey:
                                        {
                                            addAlter();

                                            language.BuildDropPrimaryKey(step, sb, connection, this);
                                        }
                                        break;

                                    case AlterTableType.CreateForeignKey:
                                        {
                                            addAlter();

                                            if (shouldAddCommand)
                                                sb.Append(language.AlterTableAddCommandName + " ");

                                            sb.Append(language.AlterTableAddForeignKeyCommandName + " ");

                                            language.BuildCreateForeignKey(step.ForeignKey, sb, connection);
                                        }
                                        break;

                                    case AlterTableType.DropForeignKey:
                                        {
                                            addAlter();

                                            if (shouldAddCommand)
                                                sb.Append(language.DropForeignKeyCommandName + " ");

                                            sb.Append(language.WrapFieldName(step.OldItemName));
                                        }
                                        break;
                                }
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
    }
}
