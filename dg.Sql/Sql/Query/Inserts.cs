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
        public Query Insert(string columnName, object Value)
        {
            return Insert(columnName, Value, false);
        }

        public Query Insert(string columnName, object Value, bool ColumnNameIsLiteral)
        {
            QueryMode currentMode = this.QueryMode;
            if (currentMode != QueryMode.Insert)
            {
                this.QueryMode = QueryMode.Insert;
                if (currentMode != QueryMode.Update &&
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

        public Query SetInsertExpression(object insertExpression)
        {
            _InsertExpression = insertExpression;
            return this;
        }
    }
}
