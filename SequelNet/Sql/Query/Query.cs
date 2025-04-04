﻿using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

#nullable enable

namespace SequelNet;

public partial class Query
{
    #region Private variables

    private TableSchema? _Schema;
    private string? _SchemaName = null;
    private string? _SchemaAlias = null;
    private OrderByList? _ListOrderBy;
    private GroupByList? _ListGroupBy;
    private WhereList? _ListHaving;
    private SelectColumnList? _ListSelect;
    private AssignmentColumnList? _ListInsertUpdate;
    private WhereList? _ListWhere;
    private JoinList? _ListJoin;
    private IndexHintList? _ListIndexHint;
    private string? _StoredProcedureName;
    private List<DbParameterWrapper>? _StoredProcedureParameters = null;
    internal Dictionary<string, TableSchema> TableAliasMap = new Dictionary<string, TableSchema>();
    private QueryHint _QueryHint = QueryHint.None;
    private GroupByHint _GroupByHint = GroupByHint.None;
    private List<QueryCombineData>? _QueryCombineData = null;

    #endregion

    #region Instantitaion

    /// <summary>
    /// Initialize a query without any FROM specified yet
    /// </summary>
    public Query()
    {
    }

    public Query(TableSchema schema, string? alias = null)
    {
        this.Schema = schema;
        this._SchemaAlias = alias;

        TableAliasMap[this.Schema.DatabaseOwner + @"/" + this.Schema.Name] = this.Schema;

        if (schema == null)
        {
            throw new Exception("The Schema you passed in is null.");
        }
    }

    public Query(string schemaName, string? alias = null)
        : this(new TableSchema(schemaName, null), alias)
    {
    }

    public Query(object fromExpression, string alias)
    {
        this.Schema = null;

        FromExpression = fromExpression;
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

    public static Query New()
    {
        return new Query();
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

    public Query ClearAlterTable()
    {
        AlterTableSteps = null;
        return this;
    }

    #endregion

    #region General Query Modifiers

    /// <summary>
    /// Sets DISTINCT mode for this query.
    /// </summary>
    /// <returns>Current <see cref="Query"/> object</returns>
    public Query Distinct()
    {
        IsDistinct = true;
        return this;
    }

    /// <summary>
    /// Sets DISTINCT mode for this query.
    /// </summary>
    /// <param name="isDistinct">Is distinct?</param>
    /// <returns>Current <see cref="Query"/> object</returns>
    public Query Distinct(bool isDistinct)
    {
        this.IsDistinct = isDistinct;
        return this;
    }

    /// <summary>
    /// Sets a random select mode, on specified column.
    /// A randomization column is currently required for MsAccess only, so you can pass null.
    /// </summary>
    /// <param name="tableName">Random column's table</param>
    /// <param name="columnName">Column to randomize by.</param>
    /// <returns>Current <see cref="Query"/> object</returns>
    public Query Randomize(string? tableName = null, string? columnName = null)
    {
        OrderBy orderBy = new OrderBy(tableName, columnName, SortDirection.None);
        orderBy.Randomize = true;
        return OrderBy(orderBy);
    }

    /// <summary>
    /// Sets LIMIT for query results
    /// </summary>
    /// <param name="limit">Limit. 0 for not limit.</param>
    /// <returns>Current <see cref="Query"/> object</returns>
    public Query LimitRows(Int64 limit)
    {
        this.Limit = limit;
        return this;
    }

    /// <summary>
    /// Sets OFFSET for query results
    /// </summary>
    /// <param name="offset">Offset</param>
    /// <returns>Current <see cref="Query"/> object</returns>
    public Query OffsetRows(Int64 offset)
    {
        this.Offset = offset;
        return this;
    }

    /// <summary>
    /// Sets a hint for this query.
    /// </summary>
    /// <param name="queryHint">Hint</param>
    /// <returns>Current <see cref="Query"/> object</returns>
    public Query Hint(QueryHint queryHint)
    {
        _QueryHint = queryHint;
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
    /// Ignore constraint errors (INSERT IGNORE / ON CONFLICT DO NOTHING etc.)
    /// Caution: Supported only by some RDBMS.
    /// </summary>
    /// <param name="ignoreErrors">Should ignore?</param>
    public Query SetIgnoreErrors(bool ignoreErrors)
    {
        IgnoreErrors = ignoreErrors;
        return this;
    }

    /// <summary>
    /// Ignore constraint errors (INSERT IGNORE / ON CONFLICT DO NOTHING etc.)
    /// Caution: Supported only by some RDBMS.
    /// </summary>
    /// <param name="onConflictDoNothing">On conflict rule</param>
    public Query SetOnConflictDoNothing(OnConflict onConflictDoNothing)
    {
        OnConflictDoNothing = onConflictDoNothing;
        return this;
    }

    /// <summary>
    /// Perform an update constraint errors (ON DUPLICATE KEY UPDATE / ON CONFLICT DO UPDATE etc.)
    /// Caution: Supported only by some RDBMS.
    /// </summary>
    /// <param name="onConflictDoUpdate">On conflict rule</param>
    public Query SetOnConflictDoUpdate(OnConflict onConflictDoUpdate)
    {
        OnConflictDoUpdate = onConflictDoUpdate;
        return this;
    }

    /// <summary>
    /// Setting a schema name.
    /// This does not set an alias, but the actual schema name.
    /// When using an actual TableSchema class, this will allow to reuse it as different table names.
    /// </summary>
    /// <param name="schemaName">A name, or null to default to current schema or alias.</param>
    public Query SetSchemaName(string schemaName)
    {
        this.SchemaName = schemaName;
        return this;
    }

    /// <summary>
    /// Setting a schema alias.
    /// </summary>
    /// <param name="alias"></param>
    public Query SetSchemaAlias(string alias)
    {
        this.SchemaAlias = alias;
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
    /// <param name="connection">Connection to use for building the query.</param>
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
    /// <param name="relatedQuery">The query that this call is involved in. It may be used to fetch the related schema in order to correctly format a type.</param>
    /// <returns>The SQL expression</returns>
    public static string PrepareColumnValue(TableSchema.Column? columnDefinition, object value, ConnectorBase connection, Query? relatedQuery = null)
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
    /// <param name="outputBuilder">The <see cref="StringBuilder"/> to output the SQL expression</param>
    /// <param name="connection">A connector to use. Mandatory.</param>
    /// <param name="relatedQuery">The query that this call is involved in. It may be used to fetch the related schema in order to correctly format a type.</param>
    public static void PrepareColumnValue(TableSchema.Column? columnDefinition, object value, StringBuilder outputBuilder, ConnectorBase connection, Query? relatedQuery = null)
    {
        if (value == null)
        {
            outputBuilder.Append(@"NULL");
            return;
        }

        if (value is IPhrase)
        {
            // Output the complete phrase

            ((IPhrase)value).Build(outputBuilder, connection, relatedQuery);

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
                if (value.GetType().BaseType?.Name == @"Enum")
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
    /// Checks a value for <value>null</value>, <see cref="DBNull"/>, and <see cref="System.Data.SqlTypes.INullable"/>
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
    public QueryMode QueryMode { get; set; } = QueryMode.Select;

    /// <summary>
    /// Current schema name (defaults to alias if no schema is specified).
    /// </summary>
    public string? SchemaName
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
    /// Expression that replaces a table to select from.
    /// </summary>
    public object? FromExpression { get; set; } = null;

    /// <summary>
    /// Setting a schema alias name.
    /// </summary>
    public string? SchemaAlias
    {
        get { return _SchemaAlias; }
        set
        {
            bool modifySchemaName = _SchemaName == _SchemaAlias;

            _SchemaAlias = value;

            if (modifySchemaName)
            {
                SchemaName = _SchemaAlias ?? _Schema?.Name;
            }
        }
    }

    /// <summary>
    /// Is this Query returning DISTINCT values.
    /// </summary>
    public bool IsDistinct { get; set; } = false;

    /// <summary>
    /// Do we have already some INSERTs or UPDATEs set for this query?
    /// </summary>
    public bool HasInsertsOrUpdates
    {
        get
        {
            return _ListInsertUpdate != null &&
                _ListInsertUpdate.Count > 0 && (
              QueryMode == QueryMode.Insert ||
              QueryMode == QueryMode.Update ||
              QueryMode == QueryMode.InsertOrUpdate);
        }
    }

    public AssignmentColumnList? GetInsertUpdateList()
    {
        return _ListInsertUpdate;
    }

    /// <summary>
    /// Do we have already some GROUP BY set for this query?
    /// </summary>
    public bool HasGroupBy
    {
        get
        {
            return _ListGroupBy != null &&
                _ListGroupBy.Count > 0;
        }
    }

    /// <summary>
    /// LIMIT for query results.
    /// </summary>
    public Int64 Limit { get; set; } = 0;

    /// <summary>
    /// OFFSET for query results.
    /// Note: MSSQL do not natively support paging.
    /// </summary>
    public Int64 Offset { get; set; } = 0;

    /// <summary>
    /// The expression that is used for INSERT. e.g. INSERT INTO {schema} FROM {expression}.
    /// This can be a <see cref="Query"/> or a <see cref="String"/>.
    /// </summary>
    public object? InsertExpression { get; set; } = null;

    /// <summary>
    /// Main <see cref="TableSchema"/> for this query
    /// </summary>
    public TableSchema? Schema
    {
        get { return _Schema; }
        set
        {
            if (_Schema != null)
            {
                TableAliasMap.Remove(_Schema.DatabaseOwner + @"/" + (_SchemaName ?? _Schema.Name));
            }
            _Schema = value;

            if (_Schema != null)
            {
                TableAliasMap[_Schema.DatabaseOwner + @"/" + _Schema.Name] = _Schema;
                _SchemaName = _Schema.Name;
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
    public bool NeedTransaction { get; set; } = false;

    /// <summary>
    /// The command timeout. If null, default command timeout will be used.
    /// </summary>
    public int? CommandTimeout { get; set; } = null;

    private OnConflict? _OnConflictDoNothing = null;
    private OnConflict? _OnConflictDoUpdate = null;

    /// <summary>
    /// Ignore constraint errors (INSERT IGNORE / ON CONFLICT DO NOTHING etc.)
    /// Caution: Supported only by some RDBMS.
    /// </summary>
    public bool IgnoreErrors
    {
        get { return _OnConflictDoNothing != null; }
        set
        {
            if (value == false)
            {
                _OnConflictDoNothing = null;
            }
            else
            {
                if (_OnConflictDoNothing == null)
                    _OnConflictDoNothing = new OnConflict();
            }
        }
    }

    /// <summary>
    /// Ignore constraint errors (INSERT IGNORE / ON CONFLICT DO NOTHING etc.)
    /// Caution: Supported only by some RDBMS.
    /// </summary>
    public OnConflict? OnConflictDoNothing
    {
        get { return _OnConflictDoNothing; }
        set
        {
            _OnConflictDoNothing = value;
        }
    }

    /// <summary>
    /// Perform an update constraint errors (ON DUPLICATE KEY UPDATE / ON CONFLICT DO UPDATE etc.)
    /// Caution: Supported only by some RDBMS.
    /// </summary>
    public OnConflict? OnConflictDoUpdate
    {
        get { return _OnConflictDoUpdate; }
        set
        {
            _OnConflictDoUpdate = value;
        }
    }

    /// <summary>
    /// List of alter table steps in a big ALTER TABLE statement.
    /// May be null if not steps were added yet.
    /// </summary>
    public List<AlterTableQueryData>? AlterTableSteps { get; set; } = null;

    #endregion
}
