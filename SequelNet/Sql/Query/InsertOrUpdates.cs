namespace SequelNet
{
    public partial class Query
    {
        public Query InsertOrUpdate()
        {
            QueryMode currentMode = this.QueryMode;
            if (currentMode != QueryMode.InsertOrUpdate)
            {
                this.QueryMode = QueryMode.InsertOrUpdate;
                if (currentMode != QueryMode.Insert &&
                    currentMode != QueryMode.Update &&
                    _ListInsertUpdate != null)
                {
                    if (_ListInsertUpdate != null) _ListInsertUpdate.Clear();
                }
            }
            return this;
        }
    }
}
