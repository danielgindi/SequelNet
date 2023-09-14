using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

/// <summary>
/// Aggregates the selected key and value into a json object
/// </summary>
public class JsonObjectAggregate : IPhrase
{
    public ValueWrapper Key;
    public ValueWrapper Value;
    public bool IsBinary = false;

    #region Constructors
    public JsonObjectAggregate(
        ValueWrapper key, ValueWrapper value, 
        bool binary = false)
    {
        this.Key = key;
        this.Value = value;
        this.IsBinary = binary;
    }

    public JsonObjectAggregate(
        object key, ValueObjectType keyType,
        object value, ValueObjectType valueType,
        bool binary = false)
        : this(ValueWrapper.Make(key, keyType), ValueWrapper.Make(value, valueType), binary)
    {
    }

    public JsonObjectAggregate(ValueWrapper key, IPhrase value, bool binary = false)
        : this(key, ValueWrapper.From(value), binary)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        conn.Language.BuildJsonObjectAggregate(Key, Value, IsBinary, sb, conn, relatedQuery);
    }
}
