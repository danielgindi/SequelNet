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
        switch (conn.TYPE)
        {
            case ConnectorBase.SqlServiceType.MYSQL:
                {
                    sb.Append("JSON_SET(");
                    sb.Append(Document.Build(conn, relatedQuery));
                    foreach (var pair in Values)
                    {
                        sb.Append(", ");
                        sb.Append(conn.Language.PrepareValue(pair.Path));
                        sb.Append(", ");
                        sb.Append(pair.Value.Build(conn, relatedQuery));
                    }
                    sb.Append(")");
                }
                break;

            default:
                throw new NotSupportedException("JsonSet is not supported by current DB type");
        }
    }
}
