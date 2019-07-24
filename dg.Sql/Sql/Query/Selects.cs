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
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query Select()
        {
            if (this.QueryMode != QueryMode.Select)
            {
                this.QueryMode = QueryMode.Select;
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
                return Select(tableName, null, null);
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
            if (_SchemaAlias != null)
            {
                return SelectAll(this._SchemaAlias);
            }
            else
            {
                return SelectAll(_SchemaName);
            }
        }

        public Query Select(string columnName)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(columnName, false));

            return this;
        }

        public Query Select(string columnName, string alias)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(columnName, alias, false));

            return this;
        }

        public Query Select(string tableName, string columnName, string alias)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(tableName, columnName, alias));

            return this;
        }

        public Query Select(IPhrase phrase, string alias = null)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(phrase, alias));

            return this;
        }

        public Query Select(Query query, string alias = null)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(query, alias));

            return this;
        }

        [Obsolete]
        public Query Select(string tableName, string columnName, string alias, bool clearSelectList)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null)
            {
                _ListSelect = new SelectColumnList();
            }
            else if (clearSelectList)
            {
                _ListSelect.Clear();
            }
            _ListSelect.Add(new SelectColumn(tableName, columnName, alias));

            return this;
        }

        public Query SelectLiteral(string literalExpression)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(literalExpression, true));

            return this;
        }

        public Query SelectLiteral(string literalExpression, string alias)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(literalExpression, alias, true));

            return this;
        }

        public Query SelectValue(object value)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(value, null));

            return this;
        }

        public Query SelectValue(object value, string alias)
        {
            this.QueryMode = QueryMode.Select;
            if (_ListSelect == null) _ListSelect = new SelectColumnList();
            _ListSelect.Add(new SelectColumn(value, alias));

            return this;
        }

        public Query AddSelect(string columnName)
        {
            return Select(columnName);
        }

        public Query AddSelect(string columnName, string alias)
        {
            return Select(columnName, alias);
        }

        public Query AddSelect(string tableName, string columnName, string alias)
        {
            return Select(tableName, columnName, alias);
        }

        public Query AddSelect(IPhrase phrase, string alias = null)
        {
            return Select(phrase, alias);
        }

        public Query AddSelect(Query query, string alias = null)
        {
            return Select(query, alias);
        }

        public Query AddSelectLiteral(string literalExpression)
        {
            return SelectLiteral(literalExpression);
        }

        public Query AddSelectLiteral(string literalExpression, string alias)
        {
            return SelectLiteral(literalExpression, alias);
        }

        public Query AddSelectValue(object value)
        {
            return SelectValue(value);
        }

        public Query AddSelectValue(object value, string alias)
        {
            return SelectValue(value, alias);
        }
    }
}
