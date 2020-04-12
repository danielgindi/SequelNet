using SequelNet.Connector;
using System.Collections.Generic;

namespace SequelNet
{
    public partial class Query
    {
        /// <summary>
        /// Creates current table in the database.
        /// </summary>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query CreateTable()
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            ClearAlterTable();

            this.QueryMode = QueryMode.CreateTable;

            return this;
        }

        /// <summary>
        /// Adds all indexes in this table's schema.
        /// </summary>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query CreateIndexes()
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            ClearAlterTable();

            this.QueryMode = QueryMode.CreateIndexes;

            return this;
        }

        /// <summary>
        /// Adds an index.
        /// </summary>
        /// <param name="index">Index to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query CreateIndex(TableSchema.Index index)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();

            this.QueryMode = QueryMode.AlterTable;

            if (this.AlterTableSteps == null)
                this.AlterTableSteps = new List<AlterTableQueryData>();

            this.AlterTableSteps.Add(new AlterTableQueryData
            {
                Type = AlterTableType.CreateIndex,
                Index = index,
            });

            return this;
        }

        /// <summary>
        /// Adds an index.
        /// Will search in this table's schema for the index named <paramref name="indexName"/>
        /// </summary>
        /// <param name="indexName">Name of the index to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query CreateIndex(string indexName)
        {
            return CreateIndex(Schema.Indexes.Find(indexName));
        }

        /// <summary>
        /// Adds a foreign key.
        /// </summary>
        /// <param name="foreignKey">Key to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query CreateForeignKey(TableSchema.ForeignKey foreignKey)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();

            this.QueryMode = QueryMode.AlterTable;

            if (this.AlterTableSteps == null)
                this.AlterTableSteps = new List<AlterTableQueryData>();

            this.AlterTableSteps.Add(new AlterTableQueryData
            {
                Type = AlterTableType.CreateForeignKey,
                ForeignKey = foreignKey,
            });

            return this;
        }

        /// <summary>
        /// Adds an foreign key.
        /// Will search in this table's schema for the foreign key named <paramref name="fkName"/>
        /// </summary>
        /// <param name="fkName">Name of the foreign key to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query CreateForeignKey(string fkName)
        {
            return CreateForeignKey(Schema.ForeignKeys.Find(fkName));
        }

        /// <summary>
        /// Adds a column.
        /// </summary>
        /// <param name="column">Column to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddColumn(TableSchema.Column column)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();

            this.QueryMode = QueryMode.AlterTable;

            if (this.AlterTableSteps == null)
                this.AlterTableSteps = new List<AlterTableQueryData>();

            this.AlterTableSteps.Add(new AlterTableQueryData
            {
                Type = AlterTableType.AddColumn,
                Column = column,
            });

            return this;
        }

        /// <summary>
        /// Adds a column.
        /// Will search in this table's schema for the column named <paramref name="columnName"/>
        /// </summary>
        /// <param name="columnName">Column to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddColumn(string columnName)
        {
            return AddColumn(Schema.Columns.Find(columnName));
        }
        
        /// <summary>
        /// Alters a column.
        /// Will use the same column name for old and updated column.
        /// </summary>
        /// <param name="column">The column to alter</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query ChangeColumn(TableSchema.Column column)
        {
            return ChangeColumn(null, column);
        }

        /// <summary>
        /// Alters a column.
        /// Will use the same column name for old and updated column.
        /// Will search in this table's schema for the column named <paramref name="ColumnName"/>
        /// </summary>
        /// <param name="columnName">The column to alter</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query ChangeColumn(string columnName)
        {
            return ChangeColumn(Schema.Columns.Find(columnName));
        }

        /// <summary>
        /// Alters a column.
        /// </summary>
        /// <param name="oldColumnName">The column's old name</param>
        /// <param name="column">The updated column's definition</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query ChangeColumn(string oldColumnName, TableSchema.Column column)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();

            this.QueryMode = QueryMode.AlterTable;

            if (this.AlterTableSteps == null)
                this.AlterTableSteps = new List<AlterTableQueryData>();

            this.AlterTableSteps.Add(new AlterTableQueryData
            {
                Type = AlterTableType.ChangeColumn,
                Column = column,
                OldItemName = oldColumnName,
            });

            return this;
        }

        /// <summary>
        /// Alters a column.
        /// </summary>
        /// <param name="oldColumnName">The column's old name</param>
        /// <param name="newColumnName">The column's new name. This column must have a definition under this name in the TableSchema.</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query ChangeColumn(string oldColumnName, string newColumnName)
        {
            return ChangeColumn(oldColumnName, Schema.Columns.Find(newColumnName));
        }
        
        /// <summary>
        /// Drops a column
        /// </summary>
        /// <param name="columnName">Column to drop</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query DropColumn(string columnName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();

            this.QueryMode = QueryMode.AlterTable;

            if (this.AlterTableSteps == null)
                this.AlterTableSteps = new List<AlterTableQueryData>();

            this.AlterTableSteps.Add(new AlterTableQueryData
            {
                Type = AlterTableType.DropColumn,
                OldItemName = columnName,
            });

            return this;
        }

        /// <summary>
        /// Drops a foreign key
        /// </summary>
        /// <param name="foreignKeyName">Key to drop</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query DropForeignKey(string foreignKeyName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();

            this.QueryMode = QueryMode.AlterTable;

            if (this.AlterTableSteps == null)
                this.AlterTableSteps = new List<AlterTableQueryData>();

            this.AlterTableSteps.Add(new AlterTableQueryData
            {
                Type = AlterTableType.DropForeignKey,
                OldItemName = foreignKeyName,
            });

            return this;
        }
        
        /// <summary>
        /// Drops an index
        /// </summary>
        /// <param name="indexName">Index to drop</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query DropIndex(string indexName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();

            this.QueryMode = QueryMode.AlterTable;

            if (this.AlterTableSteps == null)
                this.AlterTableSteps = new List<AlterTableQueryData>();

            this.AlterTableSteps.Add(new AlterTableQueryData
            {
                Type = AlterTableType.DropIndex,
                OldItemName = indexName,
            });

            return this;
        }

        /// <summary>
        /// Drops current table
        /// </summary>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query DropTable()
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            ClearAlterTable();

            this.QueryMode = QueryMode.DropTable;

            return this;
        }

        /// <summary>
        /// Drops a table (immediately executes!)
        /// </summary>
        /// <param name="tableName">Table to drop</param>
        public static void DropTable(string tableName)
        {
            using (ConnectorBase connection = ConnectorBase.NewInstance())
            {
                string sql = string.Format(@"DROP TABLE {0}", connection.Language.WrapFieldName(tableName));
                connection.ExecuteNonQuery(sql);
            }
        }
    }
}
