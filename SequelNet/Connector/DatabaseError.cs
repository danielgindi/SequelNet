namespace SequelNet.Connector;

/// <summary>
/// Provider-independent categories of database errors.
/// </summary>
public enum DatabaseError
{
    /// <summary>An unrecognized error or an error from an unsupported provider.</summary>
    Unknown = 0,

    /// <summary>A duplicate value for a primary key or unique key.</summary>
    UniqueViolation,

    /// <summary>A foreign-key constraint was violated.</summary>
    ForeignKeyViolation,

    /// <summary>A constraint was violated, but the provider does not identify its kind.</summary>
    ConstraintViolation,

    /// <summary>A column with the requested name already exists.</summary>
    DuplicateColumn,

    /// <summary>A table with the requested name already exists. PostgreSQL also uses this for other relations.</summary>
    DuplicateTable,

    /// <summary>An index or key with the requested name already exists.</summary>
    DuplicateIndex,

    /// <summary>A named database object already exists, but its kind is not identified.</summary>
    DuplicateObject,

    /// <summary>A referenced column does not exist.</summary>
    UndefinedColumn,

    /// <summary>A referenced table does not exist.</summary>
    UndefinedTable,

    /// <summary>A referenced database object does not exist, or a drop target is not defined.</summary>
    UndefinedObject,
}
