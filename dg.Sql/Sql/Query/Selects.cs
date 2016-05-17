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
        /// Changes query type to SELECT.
        /// If the query type was not already SELECT, then existing selects will be cleared.
        /// If the query type was already SELECT, then this is a No-op.
        /// 
        /// Note: All `Select...` methods clear the select list first.
        /// </summary>
        /// <returns></returns>
        public Query Select()
        {
            if (this.QueryMode != QueryMode.Select)
            {
                return ClearSelect();
            }
            return this;
        }

        /// <summary>
        /// Select * or table_name.*
        /// This will select all fields from all involved tables.
        /// 
        /// Note: All `Select...` methods clear the select list first.
        /// </summary>
        /// <param name="tableName">Optional. Table name to select all (table_name.*). If null, then it simply selects all (*).</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query SelectAll(string tableName = null)
        {
            if (tableName != null)
            {
                return Select(tableName, null, null, true);
            }
            else
            {
                return ClearSelect();
            }
        }

        /// <summary>
        /// Select [Table].*
        /// This will only return the fields which belong to the main table in the Query.
        /// 
        /// Note: All `Select...` methods clear the select list first.
        /// </summary>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query SelectAllTableColumns()
        {
            if (_Schema != null)
            {
                return SelectAll(_SchemaName);
            }
            else
            {
                return SelectAll(this._FromExpressionTableAlias);
            }
        }


        public Query Select(string ColumnName, bool ColumnNameIsLiteral, bool ClearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            if (ClearSelectList) _ListSelect.Clear();
            _ListSelect.Add(new SelectColumn(ColumnName, ColumnNameIsLiteral));
            return this;
        }
        public Query Select(string ColumnName, string Alias, bool ColumnNameIsLiteral, bool ClearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            if (ClearSelectList) _ListSelect.Clear();
            _ListSelect.Add(new SelectColumn(ColumnName, Alias, ColumnNameIsLiteral));
            return this;
        }
        public Query Select(string TableName, string ColumnName, string Alias, bool ClearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            if (ClearSelectList) _ListSelect.Clear();
            _ListSelect.Add(new SelectColumn(TableName, ColumnName, Alias));
            return this;
        }
        public Query Select(string ColumnName)
        {
            return Select(ColumnName, false, true);
        }
        public Query SelectLiteral(string ColumnName)
        {
            return Select(ColumnName, true, true);
        }
        public Query SelectValue(object Value, string Alias, bool ClearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            if (ClearSelectList) _ListSelect.Clear();
            _ListSelect.Add(new SelectColumn(Value, Alias));
            return this;
        }
        public Query SelectValue(object Value, string Alias)
        {
            return SelectValue(Value, Alias, true);
        }
        public Query AddSelect(string ColumnName)
        {
            return Select(ColumnName, false, false);
        }
        public Query AddSelect(string TableName, string ColumnName, string Alias)
        {
            return Select(TableName, ColumnName, Alias, false);
        }
        public Query AddSelectLiteral(string Expression)
        {
            return Select(Expression, true, false);
        }
        public Query AddSelectLiteral(string Expression, string Alias)
        {
            return Select(Expression, Alias, true, false);
        }
        public Query AddSelectValue(object Value, string Alias)
        {
            return SelectValue(Value, Alias, false);
        }
    }
}
