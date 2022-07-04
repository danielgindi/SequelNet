namespace SequelNet
{
    public partial class Query
    {
        public Query ClearIndexHints()
        {
            if (_ListIndexHint != null)
            {
                _ListIndexHint.Clear();
            }
            return this;
        }


        public Query IndexHint(string indexName, IndexHintMode hint)
        {
            if (_ListIndexHint == null) _ListIndexHint = new IndexHintList();
            _ListIndexHint.Add(new IndexHint(indexName, hint));
            return this;
        }


        public Query IndexHint(string[] indexNames, IndexHintMode hint)
        {
            if (_ListIndexHint == null) _ListIndexHint = new IndexHintList();
            _ListIndexHint.Add(new IndexHint(indexNames, hint));
            return this;
        }
    }
}
