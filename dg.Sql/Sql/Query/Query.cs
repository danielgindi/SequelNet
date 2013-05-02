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
        #region Private variables

        private TableSchema _Schema;
        private object _FromExpression = null;
        private string _FromExpressionTableAlias = null;
        private OrderByList _ListOrderBy;
        private GroupByList _ListGroupBy;
        private SelectColumnList _ListSelect;
        private AssignmentColumnList _ListInsertUpdate;
        private WhereList _ListWhere;
        private JoinList _ListJoin;
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
        private object _CreateIndexObject = null;
        private QueryHint _QueryHint = QueryHint.None;
        private bool _NeedTransaction = false;

        #endregion

        #region Instantitaion

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
            _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(@"*", true));
        }
        public Query(string TableName)
        {
            this.Schema = new TableSchema(TableName, null);
            TableAliasMap[this.Schema.DatabaseOwner + @"/" + this.Schema.SchemaName] = this.Schema;
            _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(@"*", true));
        }
        public Query(object FromExpression, string FromExpressionTableAlias)
        {
            this.Schema = null;
            _FromExpression = FromExpression;
            _FromExpressionTableAlias = FromExpressionTableAlias;
            if (FromExpression == null)
            {
                throw new Exception("The expression you passed in is null.");
            } 
            if (FromExpressionTableAlias == null)
            {
                throw new Exception("The Alias you passed in is null.");
            }
            _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(@"*", true));
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

        #endregion

        #region Clearing a query

        public Query ClearSelect()
        {
            if (_ListSelect != null) _ListSelect.Clear();
            return this;
        }
        public Query ClearOrderBy()
        {
            if (_ListOrderBy != null) _ListOrderBy.Clear();
            return this;
        }
        public Query ClearGroupBy()
        {
            if (_ListGroupBy != null) _ListGroupBy.Clear();
            return this;
        }
        public Query ClearInsertAndUpdate()
        {
            if (_ListInsertUpdate != null) _ListInsertUpdate.Clear();
            InsertExpression = null;
            return this;
        }
        public Query ClearStoredProcedureParameters()
        {
            _StoredProcedureParameters = null;
            return this;
        }

        #endregion

        #region General Query Modifiers

        /// <summary>
        /// Sets DISTINCT mode for this query.
        /// </summary>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query Distinct()
        {
            IsDistinct = true;
            return this;
        }

        /// <summary>
        /// Sets DISTINCT mode for this query.
        /// </summary>
        /// <param name="IsDistinct">Is distinct?</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query Distinct(bool IsDistinct)
        {
            this.IsDistinct = IsDistinct;
            return this;
        }

        /// <summary>
        /// Sets a random select mode, on specified column
        /// </summary>
        /// <param name="TableName">Random column's table</param>
        /// <param name="ColumnName">Random column</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query Randomize(string TableName, string ColumnName)
        {
            OrderBy orderBy = new OrderBy(TableName, ColumnName, SortDirection.None);
            orderBy.Randomize = true;
            return OrderBy(orderBy);
        }

        /// <summary>
        /// Sets a random select mode, on specified column
        /// </summary>
        /// <param name="ColumnName">Random column</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query Randomize(string ColumnName)
        {
            OrderBy orderBy = new OrderBy(ColumnName, SortDirection.None);
            orderBy.Randomize = true;
            return OrderBy(orderBy);
        }

        /// <summary>
        /// Sets LIMIT for query results
        /// </summary>
        /// <param name="Limit">Limit. 0 for not limit.</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query LimitRows(Int64 Limit)
        {
            _Limit = Limit;
            return this;
        }

        /// <summary>
        /// Sets OFFSET for query results
        /// </summary>
        /// <param name="Offset">Offset</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query OffsetRows(Int64 Offset)
        {
            _Offset = Offset;
            return this;
        }

        /// <summary>
        /// Sets a hint for this query.
        /// </summary>
        /// <param name="QueryHint">Hint</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query Hint(QueryHint QueryHint)
        {
            _QueryHint = QueryHint;
            return this;
        }

        #endregion

        #region Object

        /// <summary>
        /// Returns a string representation of current Query.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return BuildCommand();
        }

        /// <summary>
        /// Returns a string representation of current Query.
        /// </summary>
        /// <param name="Connection">Connection to use for building the query.</param>
        /// <returns>String representation</returns>
        public string ToString(ConnectorBase Connection)
        {
            return BuildCommand(Connection);
        }

        #endregion

        #region General Helpers

        /// <summary>
        /// Prepares a value for SQL, considering the type of the column to which is going to be assigned.
        /// </summary>
        /// <param name="ColumnDefinition">A column definition, so we can adjust the value to it</param>
        /// <param name="Value">Value to be prepared</param>
        /// <param name="Connection">A connector to use. Mandatory.</param>
        /// <returns>The SQL expression</returns>
        public static string PrepareColumnValue(TableSchema.Column ColumnDefinition, object Value, ConnectorBase Connection)
        {
            StringBuilder sb = new StringBuilder();
            PrepareColumnValue(ColumnDefinition, Value, sb, Connection);
            return sb.ToString();
        }

        /// <summary>
        /// Prepares a value for SQL, considering the type of the column to which is going to be assigned.
        /// </summary>
        /// <param name="ColumnDefinition">A column definition, so we can adjust the value to it</param>
        /// <param name="Value">Value to be prepared</param>
        /// <param name="OutputBuilder">The <typeparamref name="StringBuilder"/> to output the SQL expression</param>
        /// <param name="Connection">A connector to use. Mandatory.</param>
        public static void PrepareColumnValue(TableSchema.Column ColumnDefinition, object Value, StringBuilder OutputBuilder, ConnectorBase Connection)
        {
            if (ColumnDefinition == null)
            {
                OutputBuilder.Append(Connection.PrepareValue(Value));
                return;
            }
            if (Value == null)
            {
                OutputBuilder.Append(@"NULL");
                return;
            }
            else if (Value.GetType() != ColumnDefinition.Type)
            {
                if (Value is string)
                {
                    // Try to convert from string to number if necessary
                    if (ColumnDefinition.Type == typeof(Int32))
                    {
                        Int32 iValue;
                        if (Int32.TryParse((string)Value, out iValue))
                        {
                            Value = iValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(UInt32))
                    {
                        UInt32 uiValue;
                        if (UInt32.TryParse((string)Value, out uiValue))
                        {
                            Value = uiValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(Int64))
                    {
                        Int64 iValue;
                        if (Int64.TryParse((string)Value, out iValue))
                        {
                            Value = iValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(UInt64))
                    {
                        UInt64 uiValue;
                        if (UInt64.TryParse((string)Value, out uiValue))
                        {
                            Value = uiValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(Decimal))
                    {
                        Decimal dValue;
                        if (Decimal.TryParse((string)Value, out dValue))
                        {
                            Value = dValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(float))
                    {
                        float fValue;
                        if (float.TryParse((string)Value, out fValue))
                        {
                            Value = fValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(Double))
                    {
                        Double dValue;
                        if (Double.TryParse((string)Value, out dValue))
                        {
                            Value = dValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(Single))
                    {
                        Single sValue;
                        if (Single.TryParse((string)Value, out sValue))
                        {
                            Value = sValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(Byte))
                    {
                        Byte bValue;
                        if (Byte.TryParse((string)Value, out bValue))
                        {
                            Value = bValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(SByte))
                    {
                        SByte sbValue;
                        if (SByte.TryParse((string)Value, out sbValue))
                        {
                            Value = sbValue;
                        }
                    }
                    else if (ColumnDefinition.Type == typeof(Guid))
                    {
                        Guid gValue = new Guid((string)Value);
                        Value = gValue;
                    }
                }
                else
                {
                    if (Value.GetType().BaseType.Name == @"Enum")
                    {
                        try
                        {
                            Value = (int)Value;
                        }
                        catch { }
                    }
                    else if (Value is dg.Sql.BasePhrase)
                    {
                        OutputBuilder.Append(((dg.Sql.BasePhrase)Value).BuildPhrase(Connection));
                        return;
                    }
                }
            }
            else if (Value is string)
            {
                if (ColumnDefinition.MaxLength > 0)
                {
                    if (((string)Value).Length > ColumnDefinition.MaxLength)
                    {
                        OutputBuilder.Append(Connection.PrepareValue(((string)Value).Remove(ColumnDefinition.MaxLength)));
                        return;
                    }
                }
            }
            else if (Value is Geometry)
            {
                ((Geometry)Value).BuildValue(OutputBuilder, Connection);
                return;
            }
            else if (Value.GetType().BaseType.Name == @"Enum")
            {
                try
                {
                    Value = (int)Value;
                }
                catch { }
            }
            OutputBuilder.Append(Connection.PrepareValue(Value));
        }

        /// <summary>
        /// Checks a value for <value>null</value>, <typeparamref name="DBNull"/>, and <typeparamref name="System.Data.SqlTypes.INullable"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns><value>true</value> if null</returns>
        public static bool IsNull(object value)
        {
            if (value == null) return true;
            if (value == DBNull.Value) return true;
            if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) return true;
            else return false;
        }

        #endregion

        #region Property accessors

        /// <summary>
        /// Current query type.
        /// </summary>
        public QueryMode QueryMode
        {
            get { return _QueryMode; }
            set { _QueryMode = value; }
        }

        /// <summary>
        /// Is this Query returning DISTINCT values.
        /// </summary>
        public bool IsDistinct
        {
            get { return _IsDistinct; }
            set { _IsDistinct = value; }
        }

        /// <summary>
        /// LIMIT for query results.
        /// </summary>
        public Int64 Limit
        {
            get { return _Limit; }
            set { _Limit = value; }
        }

        /// <summary>
        /// OFFSET for query results.
        /// Note: MSSQL do not natively support paging.
        /// </summary>
        public Int64 Offset
        {
            get { return _Offset; }
            set { _Offset = value; }
        }

        /// <summary>
        /// The expression that is used for INSERT. e.g. INSERT INTO {schema} FROM {expression}.
        /// This can be a <typeparamref name="Query"/> or a <typeparamref name="String"/>.
        /// </summary>
        public object InsertExpression
        {
            get { return _InsertExpression; }
            set { _InsertExpression = value; }
        }

        /// <summary>
        /// Main <typeparamref name="TableSchema"/> for this query
        /// </summary>
        public TableSchema Schema
        {
            get { return _Schema; }
            set
            {
                if (_Schema != null) TableAliasMap.Remove(_Schema.DatabaseOwner + @"/" + _Schema.SchemaName);
                _Schema = value;
                if (Schema != null) TableAliasMap[_Schema.DatabaseOwner + @"/" + _Schema.SchemaName] = _Schema;
            }
        }

        /// <summary>
        /// Does this query needs a transaction?
        /// The query will be automatically wrapped in a transaction, if the connection is not already in a transaction.
        /// </summary>
        public bool NeedTransaction
        {
            get { return _NeedTransaction; }
            set { _NeedTransaction = value; }
        }

        #endregion
    }
}
