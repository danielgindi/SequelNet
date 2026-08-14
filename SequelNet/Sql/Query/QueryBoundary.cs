using System;
using System.Collections.Generic;

#nullable enable

namespace SequelNet;

public partial class Query
{
    private static readonly Func<Query, bool> DefaultQueryBoundaryValidationPredicate =
        query => query.QueryMode == QueryMode.Select ||
            query.QueryMode == QueryMode.Insert ||
            query.QueryMode == QueryMode.Update ||
            query.QueryMode == QueryMode.Delete ||
            query.QueryMode == QueryMode.InsertOrUpdate;

    /// <summary>
    /// Globally handles queries which omit one or more columns declared as query boundary by their <see cref="TableSchema"/>.
    /// The default handler throws <see cref="QueryBoundaryViolationException"/>. Replace it with a logging handler
    /// to report violations without interrupting execution, or set it to null to disable reporting.
    /// </summary>
    public static Action<QueryBoundaryViolation>? QueryBoundaryViolationHandler { get; set; } =
        violation => throw new QueryBoundaryViolationException(violation);

    /// <summary>
    /// Globally selects which queries are checked for query boundary. By default SELECT, INSERT, UPDATE, DELETE,
    /// and INSERT OR UPDATE queries are checked. Set this to null to disable validation altogether.
    /// </summary>
    public static Func<Query, bool>? QueryBoundaryValidationPredicate { get; set; } =
        DefaultQueryBoundaryValidationPredicate;

    /// <summary>
    /// Marks this query as intentionally exempt from query-boundary validation.
    /// Use this only for exceptional queries, such as explicitly authorized cross-tenant operations.
    /// </summary>
    public Query IgnoreQueryBoundary()
    {
        IgnoreQueryBoundaryValidation = true;
        return this;
    }

    /// <summary>
    /// Whether this query is intentionally exempt from query-boundary validation.
    /// </summary>
    public bool IgnoreQueryBoundaryValidation { get; set; }

    /// <summary>
    /// Validates query boundary. SELECT, UPDATE, and DELETE queries must constrain scope columns in WHERE; INSERT and
    /// INSERT OR UPDATE queries must include scope columns in their insert assignments.
    /// </summary>
    public void ValidateQueryBoundary()
    {
        if (IgnoreQueryBoundaryValidation)
            return;

        var queryScopeColumns = _Schema?.QueryBoundaryColumns;
        if (queryScopeColumns == null || queryScopeColumns.Length == 0)
            return;

        var predicate = QueryBoundaryValidationPredicate;
        var handler = QueryBoundaryViolationHandler;
        if (predicate == null || handler == null || !predicate(this))
            return;

        List<string>? missingColumns = null;
        foreach (var queryScopeColumn in queryScopeColumns)
        {
            if (!IsQueryBoundaryColumnInvolved(queryScopeColumn))
            {
                if (missingColumns == null)
                    missingColumns = new List<string>();
                missingColumns.Add(queryScopeColumn);
            }
        }

        if (missingColumns != null)
            handler(new QueryBoundaryViolation(this, _Schema!, missingColumns));
    }

    private bool IsQueryBoundaryColumnInvolved(string queryScopeColumn)
    {
        return QueryMode == QueryMode.Insert || QueryMode == QueryMode.InsertOrUpdate
            ? IsQueryBoundaryColumnInInsert(queryScopeColumn)
            : IsQueryBoundaryColumnInWhere(queryScopeColumn);
    }

    private bool IsQueryBoundaryColumnInInsert(string queryScopeColumn)
    {
        if (_ListInsertUpdate == null)
            return false;

        foreach (var assignment in _ListInsertUpdate)
        {
            if (string.Equals(assignment.ColumnName, queryScopeColumn, StringComparison.Ordinal) &&
                IsMainSchemaTable(assignment.TableName))
                return true;
        }

        return false;
    }

    private bool IsQueryBoundaryColumnInWhere(string queryScopeColumn)
    {
        return _ListWhere != null && _ListWhere.IsColumnInWhere(queryScopeColumn, IsMainSchemaTable);
    }

    private bool IsMainSchemaTable(string? tableName)
    {
        return tableName == null ||
            string.Equals(tableName, _SchemaAlias, StringComparison.Ordinal) ||
            string.Equals(tableName, _SchemaName, StringComparison.Ordinal) ||
            string.Equals(tableName, _Schema?.Name, StringComparison.Ordinal);
    }
}