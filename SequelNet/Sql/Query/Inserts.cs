namespace SequelNet
{
    public partial class Query
    {
        public Query Insert(string columnName, object Value)
        {
            return Insert(columnName, Value, false);
        }

        public Query Insert(string columnName, object Value, bool ColumnNameIsLiteral)
        {
            var prevMode = this.QueryMode;

            if (prevMode != QueryMode.Insert &&
                prevMode != QueryMode.InsertOrUpdate)
            {
                this.QueryMode = QueryMode.Insert;

                if (prevMode != QueryMode.Update &&
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
            InsertExpression = insertExpression;
            return this;
        }
    }
}
