namespace SequelNet
{
    public partial class Query
    {
        public Query Update(string columnName, object value)
        {
            return Update(null, columnName, value, false);
        }
        public Query Update(string tableName, string columnName, object value)
        {
            return Update(tableName, columnName, value, false);
        }

        public Query Update(string columnName, object value, bool columnValueIsLiteral)
        {
            return Update(null, columnName, value, columnValueIsLiteral);
        }

        public Query Update(string tableName, string columnName, object value, bool columnValueIsLiteral)
        {
            var prevMode = this.QueryMode;

            if (prevMode != QueryMode.Update &&
                prevMode != QueryMode.InsertOrUpdate)
            {
                this.QueryMode = QueryMode.Update;

                if (prevMode != QueryMode.Insert &&
                    _ListInsertUpdate != null)
                {
                    if (_ListInsertUpdate != null) _ListInsertUpdate.Clear();
                }
            }

            if (_ListInsertUpdate == null) _ListInsertUpdate = new AssignmentColumnList();
            _ListInsertUpdate.Add(new AssignmentColumn(tableName, columnName, null, value, columnValueIsLiteral ? ValueObjectType.Literal : ValueObjectType.Value));
            return this;
        }

        public Query UpdateFromColumn(string tableName, string columnName, string fromTableName, string fromTableColumn)
        {
            var prevMode = this.QueryMode;

            if (prevMode != QueryMode.Update &&
                prevMode != QueryMode.InsertOrUpdate)
            {
                this.QueryMode = QueryMode.Update;

                if (prevMode != QueryMode.Insert &&
                    _ListInsertUpdate != null)
                {
                    if (_ListInsertUpdate != null) _ListInsertUpdate.Clear();
                }
            }

            if (_ListInsertUpdate == null) _ListInsertUpdate = new AssignmentColumnList();
            _ListInsertUpdate.Add(new AssignmentColumn(tableName, columnName, fromTableName, fromTableColumn, ValueObjectType.ColumnName));
            return this;
        }

        public Query UpdateFromOtherColumn(string columnName, string fromColumn)
        {
            var prevMode = this.QueryMode;

            if (prevMode != QueryMode.Update &&
                prevMode != QueryMode.InsertOrUpdate)
            {
                this.QueryMode = QueryMode.Update;

                if (prevMode != QueryMode.Insert &&
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
