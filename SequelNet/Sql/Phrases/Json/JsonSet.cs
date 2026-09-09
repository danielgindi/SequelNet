using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

#nullable enable

namespace SequelNet.Phrases;

/// <summary>
/// Sets a value to a json object or array at the specified position
/// </summary>
public class JsonSet : IPhrase
{
    public ValueWrapper Document;
    public List<JsonPathValue> Values;

    #region Constructors

    public JsonSet()
    {
        Values = new List<JsonPathValue>();
    }

    public JsonSet(
        ValueWrapper doc,
        string path,
        object? value, ValueObjectType valueType)
        : this()
    {
        this.Document = doc;
        this.Values.Add(JsonPathValue.From(path, ValueWrapper.Make(value, valueType)));
    }

    public JsonSet(
        object doc, ValueObjectType docType,
        string path,
        object? value, ValueObjectType valueType)
        : this(ValueWrapper.Make(doc, docType), path, value, valueType)
    {
    }

    public JsonSet(
        string? docTableName, string docColumnName,
        string path,
        object? value, ValueObjectType valueType)
        : this(ValueWrapper.Column(docTableName, docColumnName), path, value, valueType)
    {
    }

    public JsonSet(
        string docColumnName,
        string path,
        object? value, ValueObjectType valueType)
        : this(ValueWrapper.Column(docColumnName), path, value, valueType)
    {
    }

    public JsonSet(
        IPhrase doc,
        string path,
        object? value, ValueObjectType valueType)
        : this(ValueWrapper.From(doc), path, value, valueType)
    {
    }

    public JsonSet(
        IPhrase doc,
        string path,
        IPhrase value)
        : this()
    {
        this.Document = ValueWrapper.From(doc);
        this.Values.Add(JsonPathValue.From(path, ValueWrapper.From(value)));
    }

    public JsonSet(
        ValueWrapper doc,
        params JsonPathValue[] pathValues)
        : this()
    {
        this.Document = doc;

        foreach (var pair in pathValues)
            this.Values.Add(pair);
    }

    public JsonSet(
        IPhrase doc,
        params JsonPathValue[] pathValues)
        : this(ValueWrapper.From(doc), pathValues)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        if (Values.Count == 0)
            throw new InvalidOperationException("JsonSet requires at least one path/value pair");

        conn.Language.BuildJsonSet(this, sb, conn, relatedQuery);
    }
}
