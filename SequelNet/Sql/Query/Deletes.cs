namespace SequelNet
{
    public partial class Query
    {
        public Query Delete()
        {
            this.QueryMode = QueryMode.Delete;
            return this;
        }
    }
}
