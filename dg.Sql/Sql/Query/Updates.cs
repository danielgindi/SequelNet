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
        public Query Update(string columnName, object value)
        {
            return Update(columnName, value, false);
        }

        public Query Update(string columnName, object value, bool columnValueIsLiteral)
        {
            QueryMode currentMode = this.QueryMode;
            if (currentMode != QueryMode.Update)
            {
                this.QueryMode = QueryMode.Update;
                if (currentMode != QueryMode.Insert &&
                    currentMode != QueryMode.InsertOrUpdate &&
                    _ListInsertUpdate != null)
                {
                    if (_ListInsertUpdate != null) _ListInsertUpdate.Clear();
                }
            }
            if (_ListInsertUpdate == null) _ListInsertUpdate = new AssignmentColumnList();
            _ListInsertUpdate.Add(new AssignmentColumn(null, columnName, null, value, columnValueIsLiteral ? ValueObjectType.Literal : ValueObjectType.Value));
            return this;
        }

        public Query UpdateFromColumn(string tableName, string columnName, string fromTableName, string fromTableColumn)
        {
            QueryMode currentMode = this.QueryMode;
            if (currentMode != QueryMode.Update)
            {
                this.QueryMode = QueryMode.Update;
                if (currentMode != QueryMode.Insert &&
                    currentMode != QueryMode.InsertOrUpdate &&
                    _ListInsertUpdate != null)
                {
                    if (_ListInsertUpdate != null) _ListInsertUpdate.Clear();
                }
            }
            if (_ListInsertUpdate == null) _ListInsertUpdate = new AssignmentColumnList();
            _ListInsertUpdate.Add(new AssignmentColumn(tableName, columnName, fromTableName, fromTableColumn, ValueObjectType.ColumnName));
            return this;
        }

        [Obsolete("This method is deprecated. Use UpdateFromColumn() instead.")]
        public Query UpdateFromOtherColumn(string tableName, string columnName, string fromTableName, string fromTableColumn)
        {
            return UpdateFromColumn(tableName, columnName, fromTableName, fromTableColumn);
        }

        public Query UpdateFromOtherColumn(string columnName, string fromColumn)
        {
            QueryMode currentMode = this.QueryMode;
            if (currentMode != QueryMode.Update)
            {
                this.QueryMode = QueryMode.Update;
                if (currentMode != QueryMode.Insert &&
                    currentMode != QueryMode.InsertOrUpdate &&
                    _ListInsertUpdate != null)
                {
                    if (_ListInsertUpdate != null) _ListInsertUpdate.Clear();
                }
            }
            if (_ListInsertUpdate == null) _ListInsertUpdate = new AssignmentColumnList();
            _ListInsertUpdate.Add(new AssignmentColumn(null, columnName, null, fromColumn, ValueObjectType.ColumnName));
            return this;
        }
    }
}
