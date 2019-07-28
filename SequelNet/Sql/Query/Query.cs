using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet
{
    public partial class Query
    {
        #region Private variables

        private TableSchema _Schema;
        private string _SchemaName = null;
        private object _FromExpression = null;
        private string _SchemaAlias = null;
        private OrderByList _ListOrderBy;
        private GroupByList _ListGroupBy;
        private WhereList _ListHaving;
        private SelectColumnList _ListSelect;
        private AssignmentColumnList _ListInsertUpdate;
        private WhereList _ListWhere;
        private JoinList _ListJoin;
        private TableSchema.Column _AlterColumn;
        private object _InsertExpression = null;
        private string _AlterColumnOldName;
        private string _DropColumnName;
        private string _StoredProcedureName;
        private List<DbParameterWrapper> _StoredProcedureParameters = null;
        internal Dictionary<string, TableSchema> TableAliasMap = new Dictionary<string, TableSchema>();
        private QueryMode _QueryMode = QueryMode.Select;
        private bool _IsDistinct = false;
        private Int64 _Limit = 0;
        private Int64 _Offset = 0;
        private object _CreateIndexObject = null;
        private QueryHint _QueryHint = QueryHint.None;
        private GroupByHint _GroupByHint = GroupByHint.None;
        private bool _NeedTransaction = false;
        private int? _CommandTimeout = null;

        #endregion

        #region Instantitaion

        public Query(TableSchema schema, string alias = null)
        {
            this.Schema = schema;
            this._SchemaAlias = alias;

            TableAliasMap[this.Schema.DatabaseOwner + @"/" + this.Schema.Name] = this.Schema;

            if (schema == null)
            {
                throw new Exception("The Schema you passed in is null.");
            }
        }

        public Query(string schemaName, string alias = null)
            : this(new TableSchema(schemaName, null), alias)
        {
        }

        public Query(object fromExpression, string alias)
        {
            this.Schema = null;

            _FromExpression = fromExpression;
            _SchemaAlias = alias;
            _SchemaName = _SchemaAlias;

            if (fromExpression == null)
            {
                throw new Exception("The expression you passed in is null.");
            } 

            if (alias == null)
            {
                throw new Exception("The Alias you passed in is null.");
            }
        }

        public static Query New<T>() where T : AbstractRecord<T>, new()
        {
            return new Query(AbstractRecord<T>.Schema);
        }

        public static Query New<T>(string schemaName) where T : AbstractRecord<T>, new()
        {
            return new Query(AbstractRecord<T>.Schema, schemaName);
        }

        public static Query New(TableSchema schema)
        {
            return new Query(schema);
        }

        public static Query New(string schemaName)
        {
            return new Query(schemaName);
        }

        public static Query New(object fromExpression, string fromExpressionTableAlias)
        {
            return new Query(fromExpression, fromExpressionTableAlias);
        }

        #endregion

        #region Clearing a query

        public Query ClearSelect()
        {
            if (_ListSelect != null)
            {
                _ListSelect.Clear();
            }
            return this;
        }

        public Query ClearInsertAndUpdate()
        {
            if (_ListInsertUpdate != null)
            {
                _ListInsertUpdate.Clear();
            }
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
        public Query Distinct(bool isDistinct)
        {
            this.IsDistinct = isDistinct;
            return this;
        }

        /// <summary>
        /// Sets a random select mode, on specified column.
        /// A randomization column is currently required for MsAccess only, so you can pass null.
        /// </summary>
        /// <param name="TableName">Random column's table</param>
        /// <param name="ColumnName">Column to randomize by.</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query Randomize(string tableName = null, string columnName = null)
        {
            OrderBy orderBy = new OrderBy(tableName, columnName, SortDirection.None);
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

        /// <summary>
        /// Sets the command timeout.
        /// </summary>
        /// <param name="timeout">Timeout in seconds</param>
        public Query SetCommandTimeout(int timeout)
        {
            CommandTimeout = timeout;
            return this;
        }

        /// <summary>
        /// Setting a schema name.
        /// This does not set an alias, but the actual schema name.
        /// When using an actual TableSchema class, this will allow to reuse it as different table names.
        /// </summary>
        /// <param name="schemaName">A name, or null to default to current schema or alias.</param>
        public void SetSchemaName(string schemaName)
        {
            this.SchemaName = schemaName;
        }

        /// <summary>
        /// Setting a schema alias.
        /// </summary>
        /// <param name="alias"></param>
        public void SetSchemaAlias(string alias)
        {
            this.SchemaAlias = alias;
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
        public string ToString(ConnectorBase connection)
        {
            return BuildCommand(connection);
        }

        #endregion

        #region General Helpers

        /// <summary>
        /// Prepares a value for SQL, considering the type of the column to which is going to be assigned.
        /// </summary>
        /// <param name="columnDefinition">A column definition, so we can adjust the value to it</param>
        /// <param name="value">Value to be prepared</param>
        /// <param name="connection">A connector to use. Mandatory.</param>
        /// <returns>The SQL expression</returns>
        public static string PrepareColumnValue(TableSchema.Column columnDefinition, object value, ConnectorBase connection, Query relatedQuery = null)
        {
            StringBuilder sb = new StringBuilder();
            PrepareColumnValue(columnDefinition, value, sb, connection, relatedQuery);
            return sb.ToString();
        }

        /// <summary>
        /// Prepares a value for SQL, considering the type of the column to which is going to be assigned.
        /// </summary>
        /// <param name="columnDefinition">A column definition, so we can adjust the value to it</param>
        /// <param name="value">Value to be prepared</param>
        /// <param name="outputBuilder">The <typeparamref name="StringBuilder"/> to output the SQL expression</param>
        /// <param name="connection">A connector to use. Mandatory.</param>
        public static void PrepareColumnValue(TableSchema.Column columnDefinition, object value, StringBuilder outputBuilder, ConnectorBase connection, Query relatedQuery = null)
        {
            if (value == null)
            {
                outputBuilder.Append(@"NULL");
                return;
            }

            if (value is IPhrase)
            {
                // Output the complete phrase

                outputBuilder.Append(((IPhrase)value).BuildPhrase(connection, relatedQuery));

                return;
            }

            if (value is Query)
            {
                // Output a properly wrapped query

                outputBuilder.Append("(" + ((Query)value).BuildCommand(connection) + ")");

                return;
            }

            if (columnDefinition == null)
            {
                // No definition to match against,
                // so just prepare the value as it is, output it and return

                outputBuilder.Append(connection.Language.PrepareValue(connection, value, relatedQuery));

                return;
            }

            if (value.GetType() != columnDefinition.Type)
            {
                if (value is string)
                {
                    // Try to convert from string to number if necessary
                    if (columnDefinition.Type == typeof(Int32))
                    {
                        Int32 iValue;
                        if (Int32.TryParse((string)value, out iValue))
                        {
                            value = iValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(UInt32))
                    {
                        UInt32 uiValue;
                        if (UInt32.TryParse((string)value, out uiValue))
                        {
                            value = uiValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(Int64))
                    {
                        Int64 iValue;
                        if (Int64.TryParse((string)value, out iValue))
                        {
                            value = iValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(UInt64))
                    {
                        UInt64 uiValue;
                        if (UInt64.TryParse((string)value, out uiValue))
                        {
                            value = uiValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(Decimal))
                    {
                        Decimal dValue;
                        if (Decimal.TryParse((string)value, out dValue))
                        {
                            value = dValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(float))
                    {
                        float fValue;
                        if (float.TryParse((string)value, out fValue))
                        {
                            value = fValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(Double))
                    {
                        Double dValue;
                        if (Double.TryParse((string)value, out dValue))
                        {
                            value = dValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(Single))
                    {
                        Single sValue;
                        if (Single.TryParse((string)value, out sValue))
                        {
                            value = sValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(Byte))
                    {
                        Byte bValue;
                        if (Byte.TryParse((string)value, out bValue))
                        {
                            value = bValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(SByte))
                    {
                        SByte sbValue;
                        if (SByte.TryParse((string)value, out sbValue))
                        {
                            value = sbValue;
                        }
                    }
                    else if (columnDefinition.Type == typeof(Guid))
                    {
                        Guid gValue;
                        if (Guid.TryParse((string)value, out gValue))
                        {
                            value = gValue;
                        }
                    }
                }
                else
                {
                    if (value.GetType().BaseType.Name == @"Enum")
                    {
                        try
                        {
                            value = (int)value;
                        }
                        catch { }
                    }
                }
            }
            else if (value is string)
            {
                if (columnDefinition.MaxLength > 0)
                {
                    if (((string)value).Length > columnDefinition.MaxLength)
                    {
                        outputBuilder.Append(connection.Language.PrepareValue(((string)value).Remove(columnDefinition.MaxLength)));
                        return;
                    }
                }
            }
            else if (value is Geometry)
            {
                ((Geometry)value).BuildValue(outputBuilder, connection);
                return;
            }

            outputBuilder.Append(connection.Language.PrepareValue(connection, value, relatedQuery));
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
        /// Current query type.
        /// </summary>
        public string SchemaName
        {
            get { return _SchemaName; }
            set 
            {
                _SchemaName = value != null ?
                    value :
                    (_Schema != null ? _Schema.Name : _SchemaAlias);
            }
        }

        /// <summary>
        /// Setting a schema name.
        /// This does not set an alias, but the actual schema name.
        /// When using an actual TableSchema class, this will allow to reuse it as different table names.
        /// </summary>
        /// <param name="schemaName">A name, or null to default to current schema or alias.</param>
        public string SchemaAlias
        {
            get { return _SchemaAlias; }
            set
            {
                bool modifySchemaName = _SchemaName == _SchemaAlias;

                _SchemaAlias = value;

                if (modifySchemaName)
                {
                    if (_SchemaAlias == null)
                    {
                        SchemaName = null;
                    }
                    else
                    {
                        SchemaName = _SchemaAlias;
                    }
                }
            }
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
        /// Do we have already some INSERTs or UPDATEs set for this query?
        /// </summary>
        public bool HasInsertsOrUpdates
        {
            get
            {
                return _ListInsertUpdate != null &&
                    _ListInsertUpdate.Count > 0 && (
                  _QueryMode == QueryMode.Insert ||
                  _QueryMode == QueryMode.Update ||
                  _QueryMode == QueryMode.InsertOrUpdate);
            }
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
                if (_Schema != null)
                {
                    TableAliasMap.Remove(_Schema.DatabaseOwner + @"/" + (_SchemaName ?? _Schema.Name));
                }
                _Schema = value;
                if (Schema != null)
                {
                    TableAliasMap[_Schema.DatabaseOwner + @"/" + _Schema.Name] = _Schema;
                    _SchemaName = Schema.Name;
                }
                else
                {
                    _SchemaName = null;
                }
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

        /// <summary>
        /// The command timeout. If null, default command timeout will be used.
        /// </summary>
        public int? CommandTimeout
        {
            get { return _CommandTimeout; }
            set { _CommandTimeout = value; }
        }

        #endregion
    }
}
