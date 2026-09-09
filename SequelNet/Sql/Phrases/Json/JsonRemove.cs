using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

/// <summary>
/// Removes a value from a json object or array at the specified position
/// </summary>
public class JsonRemove : IPhrase
{
    public ValueWrapper Document;
    public List<string> Paths;

    #region Constructors

    public JsonRemove()
    {
        Paths = new List<string>();
    }

    public JsonRemove(
        ValueWrapper doc,
        params string[] paths)
        : this()
    {
        this.Document = doc;

        foreach (var path in paths)
            this.Paths.Add(path);
    }

    public JsonRemove(
        object doc, ValueObjectType docType,
        params string[] paths)
        : this(ValueWrapper.Make(doc, docType), paths)
    {
    }

    public JsonRemove(
        string docTableName, string docColumnName,
        params string[] paths)
        : this(ValueWrapper.Column(docTableName, docColumnName), paths)
    {
    }

    public JsonRemove(
        string docColumnName,
        params string[] paths)
        : this(ValueWrapper.Column(docColumnName), paths)
    {
    }

    public JsonRemove(
        IPhrase doc,
        params string[] paths)
        : this(ValueWrapper.From(doc), paths)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        if (Paths.Count == 0)
            throw new InvalidOperationException("JsonRemove requires at least one path");

        conn.Language.BuildJsonRemove(this, sb, conn, relatedQuery);
    }
}
