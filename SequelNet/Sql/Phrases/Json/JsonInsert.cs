using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

#nullable enable

namespace SequelNet.Phrases;

/// <summary>
/// Inserts a value to a json object or array at the specified position, not replacing existing values.
/// </summary>
public class JsonInsert : IPhrase
{
    public ValueWrapper Document;
    public List<JsonPathValue> Values;

    #region Constructors

    public JsonInsert()
    {
        Values = new List<JsonPathValue>();
    }

    public JsonInsert(
        object doc, ValueObjectType docType,
        string path,
        object? value, ValueObjectType valueType)
        : this()
    {
        this.Document = ValueWrapper.Make(doc, docType);
        this.Values.Add(JsonPathValue.From(path, ValueWrapper.Make(value, valueType)));
    }

    public JsonInsert(
        string? docTableName, string docColumnName,
        string path,
        object? value, ValueObjectType valueType)
        : this()
    {
        this.Document = ValueWrapper.Column(docTableName, docColumnName);
        this.Values.Add(JsonPathValue.From(path, ValueWrapper.Make(value, valueType)));
    }

    public JsonInsert(
        string docColumnName,
        string path,
        object? value, ValueObjectType valueType)
        : this(null, docColumnName, path, value, valueType)
    {
    }

    public JsonInsert(
        IPhrase doc,
        string path,
        object? value, ValueObjectType valueType)
        : this()
    {
        this.Document = ValueWrapper.From(doc);
        this.Values.Add(JsonPathValue.From(path, ValueWrapper.Make(value, valueType)));
    }

    public JsonInsert(
        IPhrase doc,
        string path,
        IPhrase value)
        : this()
    {
        this.Document = ValueWrapper.From(doc);
        this.Values.Add(JsonPathValue.From(path, ValueWrapper.From(value)));
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        if (Values.Count == 0)
            throw new InvalidOperationException("JsonInsert requires at least one path/value pair");

        conn.Language.BuildJsonInsert(this, sb, conn, relatedQuery);
    }
}
