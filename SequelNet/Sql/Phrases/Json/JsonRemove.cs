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
        switch (conn.TYPE)
        {
            case ConnectorBase.SqlServiceType.MYSQL:
                {
                    sb.Append("JSON_REMOVE(");
                    sb.Append(Document.Build(conn, relatedQuery));
                    foreach (var path in Paths)
                    {
                        sb.Append(", ");
                        sb.Append(conn.Language.PrepareValue(path));
                    }
                    sb.Append(")");
                }
                break;

            default:
                throw new NotSupportedException("JsonSet is not supported by current DB type");
        }
    }
}
