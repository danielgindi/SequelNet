namespace SequelNet.Phrases;

#nullable enable

public class UnionAll : Union
{
    public UnionAll(params Query[] queries)
        : base(queries)
    {
        this.All = true;
    }
}
