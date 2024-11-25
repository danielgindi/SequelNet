using System.Text;
using SequelNet.Connector;

#nullable enable

namespace SequelNet.Phrases;

/// <summary>
/// Extract a member of a JSON object/array.
/// The path consists of $ sign, . period, [] for array indexes, and member names.
/// The dollar sign($) represents the context item.
/// The property path is a set of path steps.
/// Path steps can contain the following elements and operators.
/// 
///     Key names:
///         If the key name starts with a dollar sign or contains special characters such as spaces, surround it with double quotes.
///         For example, $.name and $."first name".
///         
///     Array elements:
///         For example, $.product[3]. Arrays are zero-based.
///         
///     The dot operator (.) indicates a member of an object.
/// </summary>
public class JsonExtract : IPhrase
{
    public ValueWrapper Value;
    public JsonPathExpression Path = new JsonPathExpression("$");
    public bool Unquote = false;

    #region Constructors

    public JsonExtract(object? value, ValueObjectType valueType, JsonPathExpression jsonPath, bool unquote = true)
    {
        this.Value = ValueWrapper.Make(value, valueType);
        this.Path = jsonPath;
        this.Unquote = unquote;
    }

    public JsonExtract(string? tableName, string columnName, JsonPathExpression jsonPath, bool unquote = true)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
        this.Path = jsonPath;
        this.Unquote = unquote;
    }

    public JsonExtract(string columnName, JsonPathExpression jsonPath, bool unquote = true)
        : this(null, columnName, jsonPath, unquote)
    {
    }

    public JsonExtract(IPhrase phrase, JsonPathExpression jsonPath, bool unquote = true)
        : this(phrase, ValueObjectType.Value, jsonPath, unquote)
    {
    }

    public JsonExtract(Where where, JsonPathExpression jsonPath, bool unquote = true)
        : this(where, ValueObjectType.Value, jsonPath, unquote)
    {
    }

    public JsonExtract(object? value, ValueObjectType valueType, string jsonPath, bool unquote = true)
        : this(value, valueType, new JsonPathExpression(jsonPath), unquote)
    {
    }

    public JsonExtract(string? tableName, string columnName, string jsonPath, bool unquote = true)
        : this(tableName, columnName, new JsonPathExpression(jsonPath), unquote)
    {
    }

    public JsonExtract(string columnName, string jsonPath, bool unquote = true)
        : this(columnName, new JsonPathExpression(jsonPath), unquote)
    {
    }

    public JsonExtract(IPhrase phrase, string jsonPath, bool unquote = true)
        : this(phrase, new JsonPathExpression(jsonPath), unquote)
    {
    }

    public JsonExtract(Where where, string jsonPath, bool unquote = true)
        : this(where, new JsonPathExpression(jsonPath), unquote)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        conn.Language.BuildJsonExtract(Value, Path, Unquote, sb, conn, relatedQuery);
    }
}
