namespace SequelNet
{
    public enum WhereCondition
    {
        AND = 0,
        OR = 1
    }

    public enum ValueObjectType
    {
        Value = 0,
        ColumnName = 1,
        Literal = 2,
    }

    public enum SortDirection
    {
        None = 0,
        ASC = 1,
        DESC = 2
    }

    public enum QueryMode
    {
        None = 0,
        Select,
        Insert,
        Update,
        Delete,
        InsertOrUpdate,
        CreateTable,
        CreateIndexes,
        AlterTable,
        DropTable,
        ExecuteStoredProcedure
    }

    public enum AlterTableType
    {
        AddColumn,
        ChangeColumn,
        DropColumn,
        CreateIndex,
        DropIndex,
        CreateForeignKey,
        DropForeignKey,
        DropPrimaryKey,
    }

    public enum WhereComparison
    {
        None = 0,
        EqualsTo = 1,
        NotEqualsTo = 2,
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        LessThan = 5,
        LessThanOrEqual = 6,
        Is = 7,
        IsNot = 8,
        Like = 9,
        Between = 10,
        In = 11,
        NotIn = 12,
        NullSafeEqualsTo = 13,
        NullSafeNotEqualsTo = 14,
        NotLike = 15,
    }

    public enum JoinType
    {
        /// <summary>
        /// Returns all the rows which match on both tables
        /// </summary>
        InnerJoin = 1,
        /// <summary>
        /// This is actually a LEFT OUTER JOIN
        /// </summary>
        LeftJoin = 2,
        /// <summary>
        /// This is actually a RIGHT OUTER JOIN
        /// </summary>
        RightJoin = 3,
        /// <summary>
        /// Retain all rows from the LEFT table, and fill in with NULLs on the RIGHT side where needed.
        /// </summary>
        LeftOuterJoin = 4,
        /// <summary>
        /// Retain all rows from the RIGHT table, and fill in with NULLs on the LEF side where needed.
        /// </summary>
        RightOuterJoin = 5,
        /// <summary>
        /// This is the combination of LEFT OUTER JOIN and RIGHT OUTER JOIN, 
        /// keeping all records and filling in with NULLs the missing matches.
        /// </summary>
        FullOuterJoin = 6
    }
}
