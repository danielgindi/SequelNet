using System.Text;
using SequelNet.Connector;

#nullable enable

namespace SequelNet.Phrases;

/// <summary>
/// Tests whether a JSON object/array/scalar contains another JSON object/array/scalar.
/// The path is optional, and consists of $ sign, . period, [] for array indexes, and member names.
/// Both the target and the candidate are jsons, not primitive values.
/// </summary>
public class JsonContains : IPhrase
{
    public ValueWrapper Target;
    public ValueWrapper Candidate;
    public JsonPathExpression? Path = null;

    #region Constructors

    public JsonContains(ValueWrapper target, ValueWrapper candidate, JsonPathExpression? path = null)
    {
        this.Target = target;
        this.Candidate = candidate;
        this.Path = path;
    }

    public JsonContains(object target, ValueObjectType targetType, object candidate, ValueObjectType candidateType, JsonPathExpression? path = null)
        : this(ValueWrapper.Make(target, targetType), ValueWrapper.Make(candidate, candidateType), path)
    {
    }

    public JsonContains(string targetTableName, string targetColumnName, object candidate, ValueObjectType candidateType, JsonPathExpression? path = null)
        : this(ValueWrapper.Column(targetTableName, targetColumnName), ValueWrapper.Make(candidate, candidateType), path)
    {
    }

    public JsonContains(IPhrase target, object candidate, ValueObjectType candidateType, JsonPathExpression? path = null)
        : this(ValueWrapper.From(target), ValueWrapper.Make(candidate, candidateType), path)
    {
    }

    public JsonContains(ValueWrapper target, ValueWrapper candidate, string? path)
        : this(target, candidate, path == null ? null : new JsonPathExpression(path))
    {
    }

    public JsonContains(object target, ValueObjectType targetType, object candidate, ValueObjectType candidateType, string? path)
        : this(target, targetType, candidate, candidateType, path == null ? null : new JsonPathExpression(path))
    {
    }

    public JsonContains(string targetTableName, string targetColumnName, object candidate, ValueObjectType candidateType, string? path)
        : this(targetTableName, targetColumnName, candidate, candidateType, path == null ? null : new JsonPathExpression(path))
    {
    }

    public JsonContains(IPhrase target, object candidate, ValueObjectType candidateType, string? path)
        : this(target, candidate, candidateType, path == null ? null : new JsonPathExpression(path))
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        conn.Language.BuildJsonContains(Target, Candidate, Path, sb, conn, relatedQuery);
    }
}
