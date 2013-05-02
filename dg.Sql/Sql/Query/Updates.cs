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
        public Query Update(string columnName, object Value)
        {
            return Update(columnName, Value, false);
        }
        public Query Update(string columnName, object Value, bool ColumnNameIsLiteral)
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
            _ListInsertUpdate.Add(new AssignmentColumn(null, columnName, null, Value, ColumnNameIsLiteral ? ValueObjectType.Literal : ValueObjectType.Value));
            return this;
        }
        public Query UpdateFromOtherColumn(string tableName, string columnName, string fromTableName, string fromTableColumn)
        {
            QueryMode currentMode = this.QueryMode;
            if (currentMode != QueryMode.Update &&
                currentMode != QueryMode.Insert &&
                currentMode != QueryMode.InsertOrUpdate &&
                _ListInsertUpdate != null)
            {
                _ListInsertUpdate.Clear();
                this.QueryMode = QueryMode.Update;
            }
            if (_ListInsertUpdate == null) _ListInsertUpdate = new AssignmentColumnList();
            _ListInsertUpdate.Add(new AssignmentColumn(tableName, columnName, fromTableName, fromTableColumn, ValueObjectType.ColumnName));
            return this;
        }
    }
}
