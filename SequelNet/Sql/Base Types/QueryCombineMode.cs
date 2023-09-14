namespace SequelNet;

public enum QueryCombineMode
{
    /// <summary>
    /// Union
    /// </summary>
    Union,

    /// <summary>
    /// Intersect
    /// </summary>
    Intersect,

    /// <summary>
    /// Except
    /// </summary>
    Except,
}

public class QueryCombineData
{
    public QueryCombineMode Mode = QueryCombineMode.Union;
    public bool All;
    public Query Query;
}
