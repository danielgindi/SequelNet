namespace SequelNet.Phrases;

public class UnionAll : Union
{
    public UnionAll(params Query[] queries)
        : base(queries)
    {
        this.All = true;
    }
}
