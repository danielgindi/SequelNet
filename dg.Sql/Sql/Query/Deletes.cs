namespace dg.Sql
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
