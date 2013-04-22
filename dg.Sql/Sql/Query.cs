using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using dg.Sql.Connector;

namespace dg.Sql
{
    public class Query
    {
        private TableSchema _Schema;
        private object FromExpression = null;
        private string FromExpressionTableAlias = null;
        private OrderByList ListOrderBy;
        private GroupByList ListGroupBy;
        private SelectColumnList ListSelect;
        private AssignmentColumnList ListInsertUpdate;
        private WhereList ListWhere;
        private JoinList ListJoin;
        private TableSchema.Column _AlterColumn;
        private object _InsertExpression = null;
        private string _AlterColumnOldName;
        private string _DropColumnName;
        private string _StoredProcedureName;
        private List<DbParameter> _StoredProcedureParameters = null;
        internal Dictionary<string, TableSchema> TableAliasMap = new Dictionary<string, TableSchema>();
        private QueryMode _QueryMode = QueryMode.Select;
        private bool _IsDistinct = false;
        private Int64 _Limit = 0;
        private Int64 _Offset = 0;
        public bool NeedTransaction = false;
        private object _CreateIndexObject = null;
        private QueryHint _QueryHint = QueryHint.None;

        public Query(TableSchema Schema)
        {
            this.Schema = Schema;
            TableAliasMap[this.Schema.DatabaseOwner + @"/" + this.Schema.SchemaName] = this.Schema;
            if (Schema == null)
            {
                throw new Exception("The Schema you passed in is null.");
            }
            if (Schema.Columns == null || Schema.Columns.Count == 0)
            {
                throw new Exception("The Schema Table you passed in has no columns");
            }
            ListSelect = new SelectColumnList();
            ListSelect.Add(new SelectColumn(@"*", true));
        }
        public Query(string TableName)
        {
            this.Schema = new TableSchema(TableName, null);
            TableAliasMap[this.Schema.DatabaseOwner + @"/" + this.Schema.SchemaName] = this.Schema;
            ListSelect = new SelectColumnList();
            ListSelect.Add(new SelectColumn(@"*", true));
        }
        public Query(object FromExpression, string FromExpressionTableAlias)
        {
            this.Schema = null;
            this.FromExpression = FromExpression;
            this.FromExpressionTableAlias = FromExpressionTableAlias;
            if (FromExpression == null)
            {
                throw new Exception("The expression you passed in is null.");
            } 
            if (FromExpressionTableAlias == null)
            {
                throw new Exception("The alias you passed in is null.");
            }
            ListSelect = new SelectColumnList();
            ListSelect.Add(new SelectColumn(@"*", true));
        }

        public static Query New<T>() where T : AbstractRecord<T>, new()
        {
            return new Query(AbstractRecord<T>.TableSchema);
        }
        public static Query New(TableSchema Schema)
        {
            return new Query(Schema);
        }
        public static Query New(string TableName)
        {
            return new Query(TableName);
        }
        public static Query New(object FromExpression, string FromExpressionTableAlias)
        {
            return new Query(FromExpression, FromExpressionTableAlias);
        }

        public Query Select()
        {
            if (this.QueryMode != QueryMode.Select)
            {
                return Select(@"*", true, true);
            }
            return this;
        }
        public Query SelectAll()
        {
            return Select(@"*", true, true);
        }
        public Query Select(string columnName, bool literalColumnName, bool clearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (ListSelect == null) ListSelect = new SelectColumnList();
            if (clearSelectList) ListSelect.Clear();
            ListSelect.Add(new SelectColumn(columnName, literalColumnName));
            return this;
        }
        public Query Select(string columnName, string alias, bool literalColumnName, bool clearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (ListSelect == null) ListSelect = new SelectColumnList();
            if (clearSelectList) ListSelect.Clear();
            ListSelect.Add(new SelectColumn(columnName, alias, literalColumnName));
            return this;
        }
        public Query Select(string tableName, string columnName, string alias, bool clearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (ListSelect == null) ListSelect = new SelectColumnList();
            if (clearSelectList) ListSelect.Clear();
            ListSelect.Add(new SelectColumn(tableName, columnName, alias));
            return this;
        }
        public Query Select(string columnName)
        {
            return Select(columnName, false, true);
        }
        public Query SelectLiteral(string columnName)
        {
            return Select(columnName, true, true);
        }
        public Query SelectValue(object Value, string Alias, bool clearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (ListSelect == null) ListSelect = new SelectColumnList();
            if (clearSelectList) ListSelect.Clear();
            ListSelect.Add(new SelectColumn(Value, Alias));
            return this;
        }
        public Query SelectValue(object Value, string Alias)
        {
            return SelectValue(Value, Alias, true);
        }
        public Query AddSelect(string columnName)
        {
            return Select(columnName, false, false);
        }
        public Query AddSelect(string tableName, string columnName, string alias)
        {
            return Select(tableName, columnName, alias, false);
        }
        public Query AddSelectLiteral(string columnName)
        {
            return Select(columnName, true, false);
        }
        public Query AddSelectLiteral(string columnName, string alias)
        {
            return Select(columnName, alias, true, false);
        }
        public Query AddSelectValue(object Value, string Alias)
        {
            return SelectValue(Value, Alias, false);
        }

        public Query Insert(string columnName, object columnValue)
        {
            return Insert(columnName, columnValue, false);
        }
        public Query Insert(string columnName, object columnValue, bool literalValue)
        {
            if (this.QueryMode != QueryMode.Insert && ListInsertUpdate != null) ListInsertUpdate.Clear();
            this.QueryMode = QueryMode.Insert;
            if (ListInsertUpdate == null) ListInsertUpdate = new AssignmentColumnList();
            ListInsertUpdate.Add(new AssignmentColumn(null, columnName, null, columnValue, literalValue ? ValueObjectType.Literal : ValueObjectType.Value));
            return this;
        }
        public Query SetInsertExpression(object insertExpression)
        {
            _InsertExpression = insertExpression;
            return this;
        }

        public Query Update(string columnName, object columnValue)
        {
            return Update(columnName, columnValue, false);
        }
        public Query Update(string columnName, object columnValue, bool literalValue)
        {
            if (this.QueryMode != QueryMode.Update && ListInsertUpdate != null) ListInsertUpdate.Clear();
            this.QueryMode = QueryMode.Update;
            if (ListInsertUpdate == null) ListInsertUpdate = new AssignmentColumnList();
            ListInsertUpdate.Add(new AssignmentColumn(null, columnName, null, columnValue, literalValue ? ValueObjectType.Literal : ValueObjectType.Value));
            return this;
        }
        public Query UpdateFromOtherColumn(string tableName, string columnName, string fromTableName, string fromTableColumn)
        {
            if (this.QueryMode != QueryMode.Update && ListInsertUpdate != null) ListInsertUpdate.Clear();
            this.QueryMode = QueryMode.Update;
            if (ListInsertUpdate == null) ListInsertUpdate = new AssignmentColumnList();
            ListInsertUpdate.Add(new AssignmentColumn(tableName, columnName, fromTableName, fromTableColumn, ValueObjectType.ColumnName));
            return this;
        }

        public Query Delete()
        {
            this.QueryMode = QueryMode.Delete;
            return this;
        }

        public Query OrderBy(string columnName, SortDirection sortDirection)
        {
            if (ListOrderBy == null) ListOrderBy = new OrderByList();
            ListOrderBy.Add(new OrderBy(columnName, sortDirection));
            return this;
        }
        public Query OrderBy(string columnName, SortDirection sortDirection, bool IsLiteral)
        {
            if (ListOrderBy == null) ListOrderBy = new OrderByList();
            ListOrderBy.Add(new OrderBy(columnName, sortDirection, IsLiteral));
            return this;
        }
        public Query OrderBy(string tableName, string columnName, SortDirection sortDirection)
        {
            if (ListOrderBy == null) ListOrderBy = new OrderByList();
            ListOrderBy.Add(new OrderBy(tableName, columnName, sortDirection));
            return this;
        }
        private Query OrderBy(OrderBy orderBy)
        {
            if (ListOrderBy == null) ListOrderBy = new OrderByList();
            ListOrderBy.Add(orderBy);
            return this;
        }

        public Query GroupBy(string columnName)
        {
            if (ListGroupBy == null) ListGroupBy = new GroupByList();
            ListGroupBy.Add(new GroupBy(columnName));
            return this;
        }
        public Query GroupBy(object columnName, bool IsLiteral)
        {
            if (ListGroupBy == null) ListGroupBy = new GroupByList();
            ListGroupBy.Add(new GroupBy(columnName, IsLiteral));
            return this;
        }
        public Query GroupBy(string tableName, string columnName)
        {
            if (ListGroupBy == null) ListGroupBy = new GroupByList();
            ListGroupBy.Add(new GroupBy(tableName, columnName));
            return this;
        }
        private Query GroupBy(GroupBy GroupBy)
        {
            if (ListGroupBy == null) ListGroupBy = new GroupByList();
            ListGroupBy.Add(GroupBy);
            return this;
        }

        public Query Where(Where where, bool clearWhereList)
        {
            if (ListWhere == null) ListWhere = new WhereList();
            if (clearWhereList) ListWhere.Clear();
            ListWhere.Add(where);
            return this;
        }
        public Query Where(string columnName, object columnValue)
        {
            if (ListWhere == null) ListWhere = new WhereList();
            ListWhere.Clear();
            ListWhere.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, columnValue, ValueObjectType.Value));
            return this;
        }
        public Query Where(string columnName, WhereComparision comparison, object columnValue)
        {
            if (ListWhere == null) ListWhere = new WhereList();
            ListWhere.Clear();
            ListWhere.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
            return this;
        }
        public Query Where(string tableName, string columnName, WhereComparision comparison, object columnValue)
        {
            if (ListWhere == null) ListWhere = new WhereList();
            ListWhere.Clear();
            ListWhere.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue));
            return this;
        }
        public Query AddWhere(Where where)
        {
            return Where(where, false);
        }
        public Query AddWhere(WhereCondition condition, object thisObject, ValueObjectType thisObjectType, WhereComparision comparison, object thatObject, ValueObjectType thatObjectType)
        {
            return Where(new Where(condition, thisObject, thisObjectType, comparison, thatObject, thatObjectType), false);
        }
        public Query AddWhere(string columnName, object columnValue)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, columnValue, ValueObjectType.Value), false);
        }
        public Query AddWhere(string columnName, WhereComparision comparison, object columnValue)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value), false);
        }
        public Query AddWhere(WhereCondition condition, string literalExpression)
        {
            return Where(new Where(condition, literalExpression, ValueObjectType.Literal, WhereComparision.None, null, ValueObjectType.Literal), false);
        }
        public Query AddWhere(WhereCondition condition, BasePhrase phrase)
        {
            return Where(new Where(condition, phrase, ValueObjectType.Value, WhereComparision.None, null, ValueObjectType.Literal), false);
        }
        public Query AddWhere(WhereCondition condition, WhereList whereList)
        {
            return Where(new Where(condition, whereList), false);
        }
        public Query AddWhere(WhereList whereList)
        {
            if (ListWhere == null) ListWhere = new WhereList();
            foreach (Where where in whereList)
            {
                ListWhere.Add(where);
            }
            return this;
        }
        public Query AddWhere(string tableName, string columnName, WhereComparision comparison, object columnValue)
        {
            if (ListWhere == null) ListWhere = new WhereList();
            ListWhere.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue));
            return this;
        }
        public Query AND(string columnName, object columnValue)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, columnValue, ValueObjectType.Value), false);
        }
        public Query AND(string columnName, WhereComparision comparison, object columnValue)
        {
            return Where(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value), false);
        }
        public Query AND(WhereList whereList)
        {
            return Where(new Where(WhereCondition.AND, whereList), false);
        }
        public Query AND(string tableName, string columnName, WhereComparision comparison, object columnValue)
        {
            return Where(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue), false);
        }
        public Query OR(string columnName, object columnValue)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, WhereComparision.EqualsTo, columnValue, ValueObjectType.Value), false);
        }
        public Query OR(string columnName, WhereComparision comparison, object columnValue)
        {
            return Where(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value), false);
        }
        public Query OR(WhereList whereList)
        {
            return Where(new Where(WhereCondition.OR, whereList), false);
        }
        public Query OR(string tableName, string columnName, WhereComparision comparison, object columnValue)
        {
            return Where(new Where(WhereCondition.OR, tableName, columnName, comparison, columnValue), false);
        }

        public Query CreateTable()
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.CreateTable;
            return this;
        }

        public Query CreateIndexes()
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.CreateIndexes;
            return this;
        }

        public Query CreateIndex(TableSchema.Index index)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.CreateIndex;
            this._CreateIndexObject = index;
            return this;
        }

        public Query CreateForeignKey(TableSchema.ForeignKey foreignKey)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.CreateIndex;
            this._CreateIndexObject = foreignKey;
            return this;
        }

        public Query AddColumn(TableSchema.Column column)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            _AlterColumn = column;
            this.QueryMode = QueryMode.AddColumn;
            return this;
        }
        public Query AddColumn(string columnName)
        {
            return AddColumn(Schema.Columns.Find(columnName));
        }

        public Query ChangeColumn(TableSchema.Column column)
        {
            return ChangeColumn(null, column);
        }
        public Query ChangeColumn(string columnName)
        {
            return ChangeColumn(Schema.Columns.Find(columnName));
        }
        public Query ChangeColumn(string ColumnOldName, TableSchema.Column column)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            _AlterColumn = column;
            _AlterColumnOldName = ColumnOldName;
            this.QueryMode = QueryMode.ChangeColumn;
            return this;
        }

        public Query DropColumn(string ColumnName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            _DropColumnName = ColumnName;
            this.QueryMode = QueryMode.DropColumn;
            return this;
        }

        public Query DropForeignKey(string ForeignKeyName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            _DropColumnName = ForeignKeyName;
            this.QueryMode = QueryMode.DropForeignKey;
            return this;
        }

        public Query DropIndex(string IndexName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            _DropColumnName = IndexName;
            this.QueryMode = QueryMode.DropIndex;
            return this;
        }

        public Query DropTable()
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.DropTable;
            return this;
        }
        public static int DropTable(string TableName)
        {
            using (ConnectorBase connection = ConnectorBase.NewInstance())
            {
                string sql = string.Format(@"DROP TABLE {0}", connection.EncloseFieldName(TableName));
                return connection.ExecuteNonQuery(sql);
            }
        }

        public Query StoredProcedure(string StoredProcedureName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.ExecuteStoredProcedure;
            _StoredProcedureName = StoredProcedureName;
            return this;
        }

        /// <summary>
        /// Add a parameter to the Stored Procedure execution.
        /// You can use SqlMgrFactoryBase.Factory() in order to create parameters.
        /// </summary>
        /// <param name="DbParameter">Parameter</param>
        /// <returns>self Query object</returns>
        public Query AddStoredProcedureParameter(DbParameter DbParameter)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(DbParameter);
            return this;
        }

        public Query AddStoredProcedureParameter(string Name, object Value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(Name, Value));
            return this;
        }
        public Query AddStoredProcedureParameter(string Name, DbType Type, object Value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(Name, Type, Value));
            return this;
        }
        public Query AddStoredProcedureParameter(string Name, object Value, ParameterDirection ParameterDirection)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(Name, Value, ParameterDirection));
            return this;
        }
        public Query AddStoredProcedureParameter(string Name, DbType Type, ParameterDirection ParameterDirection, int Size, bool IsNullable, byte Precision, byte Scale, string SourceColumn, DataRowVersion SourceVersion, object Value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(Name, Type, ParameterDirection, Size, IsNullable, Precision, Scale, SourceColumn, SourceVersion, Value));
            return this;
        }

        public Query Join(JoinType joinType,
            TableSchema leftTableSchema, string leftColumn, string leftTableAlias, 
            TableSchema rightTableSchema, string rightColumn, string rightTableAlias)
        {
            if (ListJoin == null) ListJoin = new JoinList();
            Join join = new Join(joinType, leftTableSchema, leftColumn, leftTableAlias, rightTableSchema, rightColumn, rightTableAlias);
            ListJoin.Add(join);
            TableAliasMap[join.RightTableAlias] = join.RightTableSchema;
            return this;
        }
        public Query Join(JoinType joinType, 
            TableSchema rightTableSchema, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            if (ListJoin == null) ListJoin = new JoinList();
            Join join = new Join(joinType, rightTableSchema, rightTableAlias, pairs);
            ListJoin.Add(join);
            TableAliasMap[join.RightTableAlias] = join.RightTableSchema;
            return this;
        }
        public Query Join(JoinType joinType,
            TableSchema leftTableSchema, string leftColumn, string leftTableAlias,
            string rightTableSql, string rightColumn, string rightTableAlias)
        {
            if (ListJoin == null) ListJoin = new JoinList();
            Join join = new Join(joinType, leftTableSchema, leftColumn, leftTableAlias, rightTableSql, rightColumn, rightTableAlias);
            ListJoin.Add(join);
            return this;
        }
        public Query Join(JoinType joinType,
            string rightTableSql, string rightTableAlias,
            params JoinColumnPair[] pairs)
        {
            if (ListJoin == null) ListJoin = new JoinList();
            Join join = new Join(joinType, rightTableSql, rightTableAlias, pairs);
            ListJoin.Add(join);
            return this;
        }

        public Query ClearSelect()
        {
            if (ListSelect != null) ListSelect.Clear();
            return this;
        }
        public Query ClearOrderBy()
        {
            if (ListOrderBy != null) ListOrderBy.Clear();
            return this;
        }
        public Query ClearGroupBy()
        {
            if (ListGroupBy != null) ListGroupBy.Clear();
            return this;
        }
        public Query ClearInsertAndUpdate()
        {
            if (ListInsertUpdate != null) ListInsertUpdate.Clear();
            InsertExpression = null;
            return this;
        }
        public Query ClearStoredProcedureParameters()
        {
            _StoredProcedureParameters = null;
            return this;
        }

        public Query ORDER_BY(string columnName, SortDirection sortDirection)
        {
            return OrderBy(columnName, sortDirection);
        }
        public Query ORDER_BY(string tableName, string columnName, SortDirection sortDirection)
        {
            return OrderBy(tableName, columnName, sortDirection);
        }
        public Query GROUP_BY(string columnName)
        {
            return GroupBy(columnName);
        }
        public Query GROUP_BY(string tableName, string columnName)
        {
            return GroupBy(tableName, columnName);
        }
        public Query Distinct()
        {
            IsDistinct = true;
            return this;
        }
        [CLSCompliant(false)]
        public Query DISTINCT()
        {
            IsDistinct = true;
            return this;
        }
        public Query Randomize(string tableName, string columnName)
        {
            OrderBy orderBy = new OrderBy(tableName, columnName, SortDirection.None);
            orderBy.Randomize = true;
            return OrderBy(orderBy);
        }
        public Query Randomize(string columnName)
        {
            OrderBy orderBy = new OrderBy(columnName, SortDirection.None);
            orderBy.Randomize = true;
            return OrderBy(orderBy);
        }
        public Query LimitRows(Int64 limit)
        {
            Limit = limit;
            return this;
        }
        public Query OffsetRows(Int64 offset)
        {
            Offset = offset;
            return this;
        }
        public Query Hint(QueryHint QueryHint)
        {
            _QueryHint = QueryHint;
            return this;
        }

        private void BuildJoin(StringBuilder sb, ConnectorBase connection)
        {
            if (ListJoin != null)
            {
                foreach (Join join in ListJoin)
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
                    JoinColumnPair pair = join.Pairs[0];
                    sb.Append(' ');
                    sb.Append((join.RightTableAlias != null && join.RightTableAlias.Length > 0) ? join.RightTableAlias : (join.RightTableSchema != null ? join.RightTableSchema.SchemaName : @""));
                    sb.Append(@" ON ");
                    if (pair.LeftColumnType == ValueObjectType.ColumnName)
                    {
                        sb.Append(connection.EncloseFieldName(pair.LeftTableNameOrAlias));
                        sb.Append('.');
                        sb.Append(connection.EncloseFieldName((string)pair.LeftColumn));
                    }
                    else if (pair.LeftColumnType == ValueObjectType.Literal)
                    {
                        sb.Append(pair.LeftColumn);
                    }
                    else if (pair.LeftColumnType == ValueObjectType.Value)
                    {
                        PrepareColumnValue(join.RightTableSchema != null ? (join.RightTableSchema.Columns.Find(pair.RightColumn)) : null, pair.LeftColumn, sb, connection);
                    }
                    sb.Append(@" = ");
                    sb.Append(connection.EncloseFieldName(join.RightTableAlias));
                    sb.Append('.');
                    sb.Append(connection.EncloseFieldName(pair.RightColumn));

                    for (int lj = 1; lj < join.Pairs.Count; lj++)
                    {
                        pair = join.Pairs[lj];
                        sb.Append(@" AND ");
                        if (pair.LeftColumnType == ValueObjectType.ColumnName)
                        {
                            sb.Append(connection.EncloseFieldName(pair.LeftTableNameOrAlias));
                            sb.Append('.');
                            sb.Append(connection.EncloseFieldName((string)pair.LeftColumn));
                        }
                        else if (pair.LeftColumnType == ValueObjectType.Literal)
                        {
                            sb.Append(pair.LeftColumn);
                        }
                        else if (pair.LeftColumnType == ValueObjectType.Value)
                        {
                            PrepareColumnValue(join.RightTableSchema != null ? (join.RightTableSchema.Columns.Find(pair.RightColumn)) : null, pair.LeftColumn, sb, connection);
                        }
                        sb.Append(@" = ");
                        sb.Append(connection.EncloseFieldName(join.RightTableAlias));
                        sb.Append('.');
                        sb.Append(connection.EncloseFieldName(pair.RightColumn));
                    }
                }
            }
        }
        private void BuildOrderBy(StringBuilder sb, ConnectorBase connection, bool invert)
        {
            if (ListOrderBy != null && ListOrderBy.Count > 0)
            {
                sb.Append(@" ORDER BY ");
                bool bFirst = true;
                foreach (OrderBy orderBy in ListOrderBy)
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
            if (ListGroupBy != null && ListGroupBy.Count > 0)
            {
                sb.Append(@" GROUP BY ");
                bool bFirst = true;
                foreach (GroupBy groupBy in ListGroupBy)
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
                    sb.Append(@"ALTER TABLE ");

                    if (Schema.DatabaseOwner.Length > 0)
                    {
                        sb.Append(connection.EncloseFieldName(Schema.DatabaseOwner));
                        sb.Append('.');
                    }
                    sb.Append(connection.EncloseFieldName(Schema.SchemaName));

                    sb.Append(@" ADD CONSTRAINT ");
                    sb.Append(connection.EncloseFieldName(index.Name));
                    sb.Append(' ');

                    if (index.Mode == dg.Sql.TableSchema.IndexMode.PrimaryKey)
                    {
                        sb.AppendFormat(@"PRIMARY KEY ");

                        if (index.Cluster == TableSchema.ClusterMode.Clustered) sb.Append(@"CLUSTERED ");
                        else if (index.Cluster == TableSchema.ClusterMode.NonClustered) sb.Append(@"NONCLUSTERED ");
                    }
                    else
                    {
                        if (index.Mode == TableSchema.IndexMode.Unique) sb.Append(@"UNIQUE ");
                        if (index.Cluster == TableSchema.ClusterMode.Clustered) sb.Append(@"CLUSTERED ");
                        else if (index.Cluster == TableSchema.ClusterMode.NonClustered) sb.Append(@"NONCLUSTERED ");
                        if (index.Mode != TableSchema.IndexMode.Unique) sb.Append(@"INDEX ");
                    }
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
                                if (ListSelect.Count == 0) ListSelect.Add(new SelectColumn(@"*", true));

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
                                    sb.AppendFormat(@"SELECT * FROM ( SELECT TOP {0} * FROM ( SELECT TOP {1} ", Limit, Offset+Limit);
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
                                    else sb.Append(connection.EncloseFieldName(FromExpressionTableAlias));
                                    BuildJoin(sb, connection);
                                    if (ListWhere != null && ListWhere.Count > 0)
                                    {
                                        sb.Append(@" WHERE ");
                                        ListWhere.BuildCommand(sb, connection, this);
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
                                foreach (SelectColumn sel in ListSelect)
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
                                        if (ListJoin != null && ListJoin.Count > 0 && string.IsNullOrEmpty(sel.TableName))
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
                                            else sb.Append(connection.EncloseFieldName(FromExpressionTableAlias));
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
                                    if (FromExpression is dg.Sql.BasePhrase)
                                    {
                                        sb.Append(((dg.Sql.BasePhrase)FromExpression).BuildPhrase(connection));
                                    }
                                    else sb.Append(FromExpression);
                                    sb.Append(@") ");
                                    sb.Append(connection.EncloseFieldName(FromExpressionTableAlias));
                                }

                                BuildJoin(sb, connection);

                                if (ListWhere != null && ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    ListWhere.BuildCommand(sb, connection, this);
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
                                foreach (AssignmentColumn ins in ListInsertUpdate)
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
                                    foreach (AssignmentColumn ins in ListInsertUpdate)
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

                                    if (ListWhere != null && ListWhere.Count > 0)
                                    {
                                        sb.Append(@" WHERE ");
                                        ListWhere.BuildCommand(sb, connection, this);
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
                                foreach (AssignmentColumn upd in ListInsertUpdate)
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

                                if (ListWhere != null && ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    ListWhere.BuildCommand(sb, connection, this);
                                }

                                BuildOrderBy(sb, connection, false);
                            }

                            break;
                        case QueryMode.Delete:
                            {
                                sb.Append(@"DELETE");
                                if (ListJoin != null && ListJoin.Count > 0)
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
                                if (ListWhere != null && ListWhere.Count > 0)
                                {
                                    sb.Append(@" WHERE ");
                                    ListWhere.BuildCommand(sb, connection, this);
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
                                    NeedTransaction = true;
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

        public override string ToString()
        {
            return BuildCommand();
        }
        public string ToString(ConnectorBase connection)
        {
            return BuildCommand(connection);
        }

        #region Execute methods

        /// <summary>
        /// Will execute the query reading the results into a <typeparamref name="DataSet"/>.
        /// </summary>
        /// <returns><typeparamref name="DataSet"/> object</returns>
        public DataSet ExecuteDataSet()
        {
            return ExecuteDataSet(null);
        }

        /// <summary>
        /// Will execute the query reading the results into a <typeparamref name="DataSet"/>.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns><typeparamref name="DataSet"/> object</returns>
        public DataSet ExecuteDataSet(ConnectorBase Connection)
        {
            bool needsDispose = Connection == null;
            try
            {
                if (needsDispose) Connection = ConnectorBase.NewInstance();
                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    return Connection.ExecuteDataSet(BuildDbCommand(Connection));
                }
                else
                {
                    return Connection.ExecuteDataSet(BuildCommand(Connection));
                }
            }
            finally
            {
                if (needsDispose && Connection != null)
                {
                    Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReaderBase"/> object.
        /// </summary>
        /// <returns><typeparamref name="DataReaderBase"/> object</returns>
        public DataReaderBase ExecuteReader()
        {
            return ExecuteReader(null);
        }

        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReaderBase"/> object.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns><typeparamref name="DataReaderBase"/> object</returns>
        public DataReaderBase ExecuteReader(ConnectorBase Connection)
        {
            bool needsDispose = Connection == null;
            try
            {
                if (needsDispose) Connection = ConnectorBase.NewInstance();
                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    return Connection.ExecuteReader(BuildDbCommand(Connection), needsDispose);
                }
                else
                {
                    return Connection.ExecuteReader(BuildCommand(Connection), needsDispose);
                }
            }
            catch
            {
                if (needsDispose && Connection != null)
                {
                    Connection.Dispose();
                }
                throw;
            }
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <returns>an object</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public object ExecuteScalar()
        {
            return ExecuteScalar(null);
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>an object</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public object ExecuteScalar(ConnectorBase Connection)
        {
            bool needsDispose = Connection == null;
            try
            {
                if (needsDispose) Connection = ConnectorBase.NewInstance();

                bool transaction = false;
                if (NeedTransaction && !Connection.HasTransaction)
                {
                    transaction = true;
                    Connection.BeginTransaction();
                }
                object retValue = null;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = Connection.ExecuteScalar(BuildDbCommand(Connection));
                }
                else
                {
                    retValue = Connection.ExecuteScalar(BuildCommand(Connection));
                }

                if (transaction)
                {
                    Connection.CommitTransaction();
                }

                return retValue;
            }
            finally
            {
                if (needsDispose && Connection != null)
                {
                    Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// </summary>
        /// <returns>Number of affected rows</returns>
        public int ExecuteNonQuery()
        {
            using (ConnectorBase connection = ConnectorBase.NewInstance())
            {
                bool transaction = false;
                if (NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
                int retValue = 0;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = connection.ExecuteNonQuery(BuildDbCommand(connection));
                }
                else
                {
                    retValue = connection.ExecuteNonQuery(BuildCommand(connection));
                }

                if (transaction) connection.CommitTransaction();
                return retValue;
            }
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Number of affected rows</returns>
        public int ExecuteNonQuery(ConnectorBase connection)
        {
            if (connection == null) return ExecuteNonQuery();
            else
            {
                bool transaction = false;
                if (NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
                int retValue = 0;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = connection.ExecuteNonQuery(BuildDbCommand(connection));
                }
                else
                {
                    retValue = connection.ExecuteNonQuery(BuildCommand(connection));
                }

                if (transaction) connection.CommitTransaction();
                return retValue;
            }
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <returns>Number of affected rows</returns>
        public int Execute()
        {
            return ExecuteNonQuery();
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(ConnectorBase Connection)
        {
            return ExecuteNonQuery(Connection);
        }

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="LastInsertId">Where to put the last inserted ROWID</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(out object LastInsertId)
        {
            using (ConnectorBase connection = ConnectorBase.NewInstance())
            {
                bool transaction = false;
                if (NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
                int retValue = 0;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = connection.ExecuteNonQuery(BuildDbCommand(connection));
                }
                else
                {
                    retValue = connection.ExecuteNonQuery(BuildCommand(connection));
                }

                if (retValue > 0) LastInsertId = connection.GetLastInsertID();
                else LastInsertId = null;
                if (transaction) connection.CommitTransaction();
                return retValue;
            }
        }

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <param name="LastInsertId">Where to put the last inserted ROWID</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(ConnectorBase Connection, out object LastInsertId)
        {
            if (Connection == null) return Execute(out LastInsertId);
            else
            {
                bool transaction = false;
                if (NeedTransaction && !Connection.HasTransaction) Connection.BeginTransaction();

                int retValue = 0;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = Connection.ExecuteNonQuery(BuildDbCommand(Connection));
                }
                else
                {
                    retValue = Connection.ExecuteNonQuery(BuildCommand(Connection));
                }

                if (retValue > 0) LastInsertId = Connection.GetLastInsertID();
                else LastInsertId = null;
                if (transaction) Connection.CommitTransaction();
                return retValue;
            }
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Array of values. Will never return null.</returns>
        public T[] ExecuteScalarArray<T>()
        {
            return ExecuteScalarList<T>(null).ToArray();
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <returns>Array of values. Will never return null.</returns>
        public T[] ExecuteScalarArray<T>(ConnectorBase connection)
        {
            return ExecuteScalarList<T>(connection).ToArray();
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>List of values. Will never return null.</returns>
        public List<T> ExecuteScalarList<T>()
        {
            return ExecuteScalarList<T>(null);
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <returns>List of values. Will never return null.</returns>
        public List<T> ExecuteScalarList<T>(ConnectorBase connection)
        {
            List<T> list = new List<T>();
            using (DataReaderBase reader = ExecuteReader(connection))
            {
                object value;
                while (reader.Read())
                {
                    value = reader[0];
                    if (value is T) list.Add((T)value);
                    else list.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            return list;
        }

        /// <summary>
        /// Executes the query and reads the first row only into a list.
        /// </summary>
        /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public List<object> ExecuteOneRowToList()
        {
            return ExecuteOneRowToList(null);
        }

        /// <summary>
        /// Executes the query and reads the first row only into a list.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public List<object> ExecuteOneRowToList(ConnectorBase Connection)
        {
            using (DataReaderBase reader = ExecuteReader(Connection))
            {
                if (reader.Read())
                {
                    List<object> row = new List<object>();
                    int i, c = reader.GetColumnCount();
                    for (i = 0; i < c; i++)
                    {
                        row.Add(reader[i]);
                    }
                    return row;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Executes the query and reads the first row only into a dictionary.
        /// </summary>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Dictionary<string, object> ExecuteOneRowToDictionary()
        {
            return ExecuteOneRowToDictionary(null);
        }

        /// <summary>
        /// Executes the query and reads the first row only into a dictionary.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Dictionary<string, object> ExecuteOneRowToDictionary(ConnectorBase Connection)
        {
            using (DataReaderBase reader = ExecuteReader(Connection))
            {
                if (reader.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    int i, c = reader.GetColumnCount();
                    string[] columnNames = new string[c];
                    for (i = 0; i < c; i++)
                    {
                        columnNames[i] = reader.GetColumnName(i);
                    }
                    for (i = 0; i < c; i++)
                    {
                        row[columnNames[i]] = reader[i];
                    }
                    return row;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Executes the query and reads all rows into a list of lists.
        /// </summary>
        /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
        public List<List<object>> ExecuteListOfLists()
        {
            return ExecuteListOfLists(null);
        }

        /// <summary>
        /// Executes the query and reads all rows into a list of lists.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
        public List<List<object>> ExecuteListOfLists(ConnectorBase Connection)
        {
            List<List<object>> results = new List<List<object>>();
            using (DataReaderBase reader = ExecuteReader(Connection))
            {
                List<object> row;
                while (reader.Read())
                {
                    row = new List<object>();
                    int i, c = reader.GetColumnCount();
                    for (i = 0; i < c; i++)
                    {
                        row.Add(reader[i]);
                    }
                    results.Add(row);
                }
            }
            return results;
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public List<Dictionary<string, object>> ExecuteListOfDictionaries()
        {
            return ExecuteListOfDictionaries(null);
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public List<Dictionary<string, object>> ExecuteListOfDictionaries(ConnectorBase Connection)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            using (DataReaderBase reader = ExecuteReader(Connection))
            {
                Dictionary<string, object> row;
                int i, c = reader.GetColumnCount();
                string[] columnNames = new string[c];
                for (i = 0; i < c; i++)
                {
                    columnNames[i] = reader.GetColumnName(i);
                }
                while (reader.Read())
                {
                    row = new Dictionary<string, object>();
                    for (i = 0; i < c; i++)
                    {
                        row[columnNames[i]] = reader[i];
                    }
                    results.Add(row);
                }
            }
            return results;
        }

        public object ExecuteAggregate(string columnName, string aggregateFunction, bool isDistinctQuery)
        {
            return ExecuteAggregate(null, null, columnName, aggregateFunction, isDistinctQuery, null);
        }
        public object ExecuteAggregate(string schemaName, string columnName, string aggregateFunction, bool isDistinctQuery)
        {
            return ExecuteAggregate(null, schemaName, columnName, aggregateFunction, isDistinctQuery, null);
        }
        public object ExecuteAggregate(string schemaName, string columnName, string aggregateFunction, bool isDistinctQuery, ConnectorBase connection)
        {
            return ExecuteAggregate(null, schemaName, columnName, aggregateFunction, isDistinctQuery, connection);
        }
        public object ExecuteAggregate(string databaseOwner, string schemaName, string columnName, string aggregateFunction, bool isDistinctQuery)
        {
            return ExecuteAggregate(databaseOwner, schemaName, columnName, aggregateFunction, isDistinctQuery, null);
        }
        public object ExecuteAggregate(string databaseOwner, string schemaName, string columnName, string aggregateFunction, bool isDistinctQuery, ConnectorBase connection)
        {
            bool ownsConnection = false;
            if (connection == null)
            {
                ownsConnection = true;
                connection = ConnectorBase.NewInstance();
            }
            try
            {
                SelectColumnList oldSelectList = ListSelect;
                bool oldIsDistinct = IsDistinct;
                IsDistinct = false;

                if (ListSelect.Count == 1 && ListSelect[0].ObjectType == ValueObjectType.Literal && ListSelect[0].ColumnName == "*")
                {
                    ListSelect = new SelectColumnList();
                }
                if (schemaName != null)
                {
                    if (databaseOwner != null && databaseOwner.Length > 0)
                    {
                        schemaName = connection.EncloseFieldName(databaseOwner) + @"." + connection.EncloseFieldName(schemaName);
                    }
                    else
                    {
                        schemaName = connection.EncloseFieldName(schemaName);
                    }
                }
                else
                {
                    if (Schema == null)
                    {
                        schemaName = connection.EncloseFieldName(FromExpressionTableAlias);
                    }
                    else
                    {
                        schemaName = @"";
                        if (Schema.DatabaseOwner.Length > 0)
                        {
                            schemaName = connection.EncloseFieldName(Schema.DatabaseOwner) + @".";
                        }
                        schemaName += connection.EncloseFieldName(Schema.SchemaName);
                    }
                }
                SelectColumn select = new SelectColumn(aggregateFunction + (isDistinctQuery ? @"(DISTINCT " : @"(") + (columnName == "*" ? columnName : (schemaName + "." + connection.EncloseFieldName(columnName))) + @")", true);
                ListSelect.Insert(0, select);

                object ret = ExecuteScalar(connection);

                ListSelect = oldSelectList;
                IsDistinct = oldIsDistinct;

                return ret;
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
        }

        #endregion

        #region GetCount Helpers
        public Int64 GetCount(string columnName)
        {
            return GetCount(null, null, columnName, null);
        }
        public Int64 GetCount(string columnName, ConnectorBase conn)
        {
            return GetCount(null, null, columnName, conn);
        }
        public Int64 GetCount(string schemaName, string columnName)
        {
            return GetCount(null, schemaName, columnName, null);
        }
        public Int64 GetCount(string schemaName, string columnName, ConnectorBase conn)
        {
            return GetCount(null, schemaName, columnName, conn);
        }
        public Int64 GetCount(string databaseOwner, string schemaName, string columnName)
        {
            return GetCount(databaseOwner, schemaName, columnName, null);
        }
        public Int64 GetCount(string databaseOwner, string schemaName, string columnName, ConnectorBase conn)
        {
            object res = this.ExecuteAggregate(databaseOwner, schemaName, columnName, @"COUNT", IsDistinct, conn);
            if (IsNull(res)) return 0;
            else return Convert.ToInt64(res);
        }
        #endregion

        #region GetMax Helpers
        public object GetMax(string columnName)
        {
            return GetMax(null, null, columnName, null);
        }
        public object GetMax(string columnName, ConnectorBase conn)
        {
            return GetMax(null, null, columnName, conn);
        }
        public object GetMax(string schemaName, string columnName)
        {
            return GetMax(null, schemaName, columnName, null);
        }
        public object GetMax(string schemaName, string columnName, ConnectorBase conn)
        {
            return GetMax(null, schemaName, columnName, conn);
        }
        public object GetMax(string databaseOwner, string schemaName, string columnName)
        {
            return GetMax(databaseOwner, schemaName, columnName, null);
        }
        public object GetMax(string databaseOwner, string schemaName, string columnName, ConnectorBase conn)
        {
            return this.ExecuteAggregate(databaseOwner, schemaName, columnName, @"MAX", false, conn);
        }
        #endregion

        #region GetMin Helpers
        public object GetMin(string columnName)
        {
            return GetMin(null, null, columnName, null);
        }
        public object GetMin(string columnName, ConnectorBase conn)
        {
            return GetMin(null, null, columnName, conn);
        }
        public object GetMin(string schemaName, string columnName)
        {
            return GetMin(null, schemaName, columnName, null);
        }
        public object GetMin(string schemaName, string columnName, ConnectorBase conn)
        {
            return GetMin(null, schemaName, columnName, conn);
        }
        public object GetMin(string databaseOwner, string schemaName, string columnName)
        {
            return GetMin(databaseOwner, schemaName, columnName, null);
        }
        public object GetMin(string databaseOwner, string schemaName, string columnName, ConnectorBase conn)
        {
            return this.ExecuteAggregate(databaseOwner, schemaName, columnName, @"MIN", false, conn);
        }
        #endregion

        #region GetSum Helpers
        public Int64 GetSum(string columnName)
        {
            return GetSum(null, null, columnName, null);
        }
        public Int64 GetSum(string columnName, ConnectorBase conn)
        {
            return GetSum(null, null, columnName, conn);
        }
        public Int64 GetSum(string schemaName, string columnName)
        {
            return GetSum(null, schemaName, columnName, null);
        }
        public Int64 GetSum(string schemaName, string columnName, ConnectorBase conn)
        {
            return GetSum(null, schemaName, columnName, conn);
        }
        public Int64 GetSum(string databaseOwner, string schemaName, string columnName)
        {
            return GetSum(databaseOwner, schemaName, columnName, null);
        }
        public Int64 GetSum(string databaseOwner, string schemaName, string columnName, ConnectorBase conn)
        {
            object res = this.ExecuteAggregate(databaseOwner, schemaName, columnName, @"SUM", IsDistinct, conn);
            if (IsNull(res)) return 0;
            else return Convert.ToInt64(res);
        }
        #endregion

        public static object PrepareColumnValue(TableSchema.Column columnDef, object columnValue, ConnectorBase connection)
        {
            StringBuilder sb = new StringBuilder();
            PrepareColumnValue(columnDef, columnValue, sb, connection);
            return sb.ToString();
        }
        public static void PrepareColumnValue(TableSchema.Column columnDef, object columnValue, StringBuilder sb, ConnectorBase connection)
        {
            if (columnDef == null)
            {
                sb.Append(connection.PrepareValue(columnValue));
                return;
            }
            if (columnValue == null)
            {
                sb.Append(@"NULL");
                return;
            }
            else if (columnValue.GetType() != columnDef.Type)
            {
                if (columnValue is string)
                {
                    // Try to convert from string to number if necessary
                    if (columnDef.Type == typeof(Int32))
                    {
                        Int32 iValue;
                        if (Int32.TryParse((string)columnValue, out iValue))
                        {
                            columnValue = iValue;
                        }
                    } 
                    else if (columnDef.Type == typeof(UInt32))
                    {
                        UInt32 uiValue;
                        if (UInt32.TryParse((string)columnValue, out uiValue))
                        {
                            columnValue = uiValue;
                        }
                    }
                    else if (columnDef.Type == typeof(Int64))
                    {
                        Int64 iValue;
                        if (Int64.TryParse((string)columnValue, out iValue))
                        {
                            columnValue = iValue;
                        }
                    }
                    else if (columnDef.Type == typeof(UInt64))
                    {
                        UInt64 uiValue;
                        if (UInt64.TryParse((string)columnValue, out uiValue))
                        {
                            columnValue = uiValue;
                        }
                    }
                    else if (columnDef.Type == typeof(Decimal))
                    {
                        Decimal dValue;
                        if (Decimal.TryParse((string)columnValue, out dValue))
                        {
                            columnValue = dValue;
                        }
                    }
                    else if (columnDef.Type == typeof(float))
                    {
                        float fValue;
                        if (float.TryParse((string)columnValue, out fValue))
                        {
                            columnValue = fValue;
                        }
                    }
                    else if (columnDef.Type == typeof(Double))
                    {
                        Double dValue;
                        if (Double.TryParse((string)columnValue, out dValue))
                        {
                            columnValue = dValue;
                        }
                    }
                    else if (columnDef.Type == typeof(Single))
                    {
                        Single sValue;
                        if (Single.TryParse((string)columnValue, out sValue))
                        {
                            columnValue = sValue;
                        }
                    }
                    else if (columnDef.Type == typeof(Byte))
                    {
                        Byte bValue;
                        if (Byte.TryParse((string)columnValue, out bValue))
                        {
                            columnValue = bValue;
                        }
                    }
                    else if (columnDef.Type == typeof(SByte))
                    {
                        SByte sbValue;
                        if (SByte.TryParse((string)columnValue, out sbValue))
                        {
                            columnValue = sbValue;
                        }
                    }
                    else if (columnDef.Type == typeof(Guid))
                    {
                        Guid gValue = new Guid((string)columnValue);
                        columnValue = gValue;
                    }
                }
                else
                {
                    if (columnValue.GetType().BaseType.Name == @"Enum")
                    {
                        try
                        {
                            columnValue = (int)columnValue;
                        }
                        catch { }
                    }
                    else if (columnValue is dg.Sql.BasePhrase)
                    {
                        columnValue = ((dg.Sql.BasePhrase)columnValue).BuildPhrase(connection);
                    }
                }
            }
            else if (columnValue is string)
            {
                if (columnDef.MaxLength > 0)
                {
                    if (((string)columnValue).Length > columnDef.MaxLength)
                    {
                        sb.Append(connection.PrepareValue(((string)columnValue).Remove(columnDef.MaxLength)));
                        return;
                    }
                }
            }
            else if (columnValue is Geometry)
            {
                ((Geometry)columnValue).BuildValue(sb, connection);
                return;
            }
            else if (columnValue.GetType().BaseType.Name == @"Enum")
            {
                try
                {
                    columnValue = (int)columnValue;
                }
                catch { }
            }
            sb.Append(connection.PrepareValue(columnValue));
        }

        public static bool IsNull(object value)
        {
            if (value == null) return true;
            if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) return true;
            if (value == DBNull.Value) return true;
            else return false;
        }

        public QueryMode QueryMode
        {
            get { return _QueryMode; }
            set { _QueryMode = value; }
        }
        public bool IsDistinct
        {
            get { return _IsDistinct; }
            set { _IsDistinct = value; }
        }
        public Int64 Limit
        {
            get { return _Limit; }
            set { _Limit = value; }
        }
        public Int64 Offset
        {
            get { return _Offset; }
            set { _Offset = value; }
        }
        public object InsertExpression
        {
            get { return _InsertExpression; }
            set { _InsertExpression = value; }
        }
        public TableSchema Schema
        {
            get { return _Schema; }
            set {
                if (_Schema != null) TableAliasMap.Remove(_Schema.DatabaseOwner + @"/" + _Schema.SchemaName);
                _Schema = value;
                if (Schema != null) TableAliasMap[_Schema.DatabaseOwner + @"/" + _Schema.SchemaName] = _Schema;
            }
        }
    }
}
