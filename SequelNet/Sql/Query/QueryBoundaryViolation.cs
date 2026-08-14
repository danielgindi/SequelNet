using System;
using System.Collections.Generic;

namespace SequelNet;

/// <summary>
/// Describes a query which omitted one or more query-boundary columns.
/// </summary>
public sealed class QueryBoundaryViolation
{
    internal QueryBoundaryViolation(Query query, TableSchema schema, IReadOnlyList<string> missingColumns)
    {
        Query = query;
        Schema = schema;
        MissingColumns = missingColumns;
    }

    public Query Query { get; }

    public TableSchema Schema { get; }

    public IReadOnlyList<string> MissingColumns { get; }
}

/// <summary>
/// Thrown by the default query-boundary violation handler.
/// </summary>
public sealed class QueryBoundaryViolationException : InvalidOperationException
{
    public QueryBoundaryViolationException(QueryBoundaryViolation violation)
        : base($"Query for '{violation.Schema.Name}' is missing query-boundary column(s): {string.Join(", ", violation.MissingColumns)}.")
    {
        Violation = violation;
    }

    public QueryBoundaryViolation Violation { get; }
}