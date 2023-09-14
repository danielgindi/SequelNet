using System.Collections.Generic;

namespace SequelNet;

public partial class Query
{
    public Query Union(Query otherQuery, bool all = false)
    {
        if (_QueryCombineData == null)
            _QueryCombineData = new List<QueryCombineData>();

        _QueryCombineData.Add(new QueryCombineData
        {
            Mode = QueryCombineMode.Union,
            All = all,
            Query = otherQuery
        });

        return this;
    }

    public Query Intersect(Query otherQuery, bool all = false)
    {
        if (_QueryCombineData == null)
            _QueryCombineData = new List<QueryCombineData>();

        _QueryCombineData.Add(new QueryCombineData
        {
            Mode = QueryCombineMode.Intersect,
            All = all,
            Query = otherQuery
        });

        return this;
    }

    public Query Except(Query otherQuery, bool all = false)
    {
        if (_QueryCombineData == null)
            _QueryCombineData = new List<QueryCombineData>();

        _QueryCombineData.Add(new QueryCombineData
        {
            Mode = QueryCombineMode.Except,
            All = all,
            Query = otherQuery
        });

        return this;
    }

    public Query ClearCombinations()
    {
        _QueryCombineData = null;
        return this;
    }
}
