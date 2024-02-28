using System;
using System.Text;
using SequelNet.Connector;

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
public class JsonValue : IPhrase
{
    public ValueWrapper Value;
    public JsonPathExpression Path = new JsonPathExpression("$");
    public DataTypeDef ReturnType = null;
    public DefaultAction OnEmptyAction = DefaultAction.Value;
    public object OnEmptyValue = null;
    public DefaultAction OnErrorAction = DefaultAction.Value;
    public object OnErrorValue = null;

    #region Constructors

    public JsonValue(object value, ValueObjectType valueType, JsonPathExpression path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
    {
        this.Value = ValueWrapper.Make(value, valueType);
        this.Path = path;
        this.ReturnType = returnType;
        this.OnEmptyAction = onEmpty;
        this.OnEmptyValue = onEmptyValue;
        this.OnErrorAction = onError;
        this.OnErrorValue = onErrorValue;
    }

    public JsonValue(string tableName, string columnName, JsonPathExpression path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
        this.Path = path;
        this.ReturnType = returnType;
        this.OnEmptyAction = onEmpty;
        this.OnEmptyValue = onEmptyValue;
        this.OnErrorAction = onError;
        this.OnErrorValue = onErrorValue;
    }

    public JsonValue(string columnName, JsonPathExpression path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
        : this(null, columnName, path, returnType, onEmpty, onEmptyValue, onError, onErrorValue)
    {
    }

    public JsonValue(IPhrase phrase, JsonPathExpression path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
        : this(phrase, ValueObjectType.Value, path, returnType, onEmpty, onEmptyValue, onError, onErrorValue)
    {
    }

    public JsonValue(Where where, JsonPathExpression path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
        : this(where, ValueObjectType.Value, path, returnType, onEmpty, onEmptyValue, onError, onErrorValue)
    {
    }

    public JsonValue(object value, ValueObjectType valueType, string path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
        : this(value, valueType, new JsonPathExpression(path),
               returnType,
               onEmpty, onEmptyValue,
               onError, onErrorValue)
    {
    }

    public JsonValue(string tableName, string columnName, string path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
        : this(tableName, columnName, new JsonPathExpression(path),
               returnType,
               onEmpty, onEmptyValue,
               onError, onErrorValue)
    {
    }

    public JsonValue(string columnName, string path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
        : this(null, columnName, path, returnType, onEmpty, onEmptyValue, onError, onErrorValue)
    {
    }

    public JsonValue(IPhrase phrase, string path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
        : this(phrase, ValueObjectType.Value, path, returnType, onEmpty, onEmptyValue, onError, onErrorValue)
    {
    }

    public JsonValue(Where where, string path,
        DataTypeDef returnType = null,
        DefaultAction onEmpty = DefaultAction.Value, object onEmptyValue = null,
        DefaultAction onError = DefaultAction.Value, object onErrorValue = null)
        : this(where, ValueObjectType.Value, path, returnType, onEmpty, onEmptyValue, onError, onErrorValue)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        conn.Language.BuildJsonExtractValue(Value, Path, 
            ReturnType,
            OnEmptyAction, OnEmptyValue,
            OnErrorAction, OnErrorValue, 
            sb, conn, relatedQuery);
    }

    public enum DefaultAction
    {
        Value,
        Error
    }
}
