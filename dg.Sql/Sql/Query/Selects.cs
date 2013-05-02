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
        public Query Select()
        {
            if (this.QueryMode != QueryMode.Select)
            {
                return Select(@"*", true, true);
            }
            return this;
        }
        public Query SelectAll()
        {
            return Select(@"*", true, true);
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
