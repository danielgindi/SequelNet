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
            this.QueryMode = QueryMode.CreateIndexes;
            return this;
        }

        /// <summary>
        /// Adds an index.
        /// </summary>
        /// <param name="Index">Index to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query CreateIndex(TableSchema.Index Index)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.CreateIndex;
            this._CreateIndexObject = Index;
            return this;
        }

        /// <summary>
        /// Adds a foreign key.
        /// </summary>
        /// <param name="ForeignKey">Key to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query CreateForeignKey(TableSchema.ForeignKey ForeignKey)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.CreateIndex;
            this._CreateIndexObject = ForeignKey;
            return this;
        }

        /// <summary>
        /// Adds a column.
        /// </summary>
        /// <param name="Column">Column to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddColumn(TableSchema.Column Column)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            _AlterColumn = Column;
            this.QueryMode = QueryMode.AddColumn;
            return this;
        }

        /// <summary>
        /// Adds a column.
        /// Will search in this table's schema for the column named <paramref name="ColumnName"/>
        /// </summary>
        /// <param name="ColumnName">Column to add</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddColumn(string ColumnName)
        {
            return AddColumn(Schema.Columns.Find(ColumnName));
        }
        
        /// <summary>
        /// Alters a column.
        /// Will use the same column name for old and updated column.
        /// </summary>
        /// <param name="Column">The column to alter</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query ChangeColumn(TableSchema.Column Column)
        {
            return ChangeColumn(null, Column);
        }

        /// <summary>
        /// Alters a column.
        /// Will use the same column name for old and updated column.
        /// Will search in this table's schema for the column named <paramref name="ColumnName"/>
        /// </summary>
        /// <param name="ColumnName">The column to alter</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query ChangeColumn(string ColumnName)
        {
            return ChangeColumn(Schema.Columns.Find(ColumnName));
        }

        /// <summary>
        /// Alters a column.
        /// </summary>
        /// <param name="ColumnOldName">The column's old name</param>
        /// <param name="Column">The updated column's definition</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query ChangeColumn(string ColumnOldName, TableSchema.Column Column)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            _AlterColumn = Column;
            _AlterColumnOldName = ColumnOldName;
            this.QueryMode = QueryMode.ChangeColumn;
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
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            _AlterColumn = Schema.Columns.Find(newColumnName);
            _AlterColumnOldName = oldColumnName;
            this.QueryMode = QueryMode.ChangeColumn;
            return this;
        }
        
        /// <summary>
        /// Drops a column
        /// </summary>
        /// <param name="ColumnName">Column to drop</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
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

        /// <summary>
        /// Drops a foreign key
        /// </summary>
        /// <param name="ForeignKeyName">Key to drop</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
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
        
        /// <summary>
        /// Drops an index
        /// </summary>
        /// <param name="IndexName">Index to drop</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
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
            this.QueryMode = QueryMode.DropTable;
            return this;
        }

        /// <summary>
        /// Drops a table (immediately executes!)
        /// </summary>
        /// <param name="IndexName">Index to drop</param>
        public static void DropTable(string TableName)
        {
            using (ConnectorBase connection = ConnectorBase.NewInstance())
            {
                string sql = string.Format(@"DROP TABLE {0}", connection.EncloseFieldName(TableName));
                connection.ExecuteNonQuery(sql);
            }
        }
    }
}
