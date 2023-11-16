using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

/// <summary>
/// Creates a json object.
/// </summary>
public class JsonObject : IPhrase
{
    public Dictionary<ValueWrapper, ValueWrapper> Values;

    #region Constructors

    public JsonObject()
    {
        this.Values = new Dictionary<ValueWrapper, ValueWrapper>();
    }

    public JsonObject(Dictionary<ValueWrapper, ValueWrapper> values)
    {
        this.Values = values == null
            ? new Dictionary<ValueWrapper, ValueWrapper>()
            : new Dictionary<ValueWrapper, ValueWrapper>(values);
    }

    public JsonObject(Dictionary<string, ValueWrapper> values)
    {
        this.Values = new Dictionary<ValueWrapper, ValueWrapper>();

        if (values != null)
        {
            foreach (var entry in values)
                Values.Add(ValueWrapper.From(entry.Key), entry.Value);
        }
    }

    public JsonObject(Dictionary<string, object> values)
    {
        this.Values = new Dictionary<ValueWrapper, ValueWrapper>();

        if (values != null)
        {
            foreach (var entry in values)
                Values.Add(ValueWrapper.From(entry.Key), ValueWrapper.Make(entry.Value, ValueObjectType.Value));
        }
    }

    public JsonObject(Dictionary<string, string> values)
    {
        this.Values = new Dictionary<ValueWrapper, ValueWrapper>();

        if (values != null)
        {
            foreach (var entry in values)
                Values.Add(ValueWrapper.From(entry.Key), ValueWrapper.From(entry.Value));
        }
    }

    public JsonObject(Dictionary<string, Int64> values)
    {
        this.Values = new Dictionary<ValueWrapper, ValueWrapper>();

        if (values != null)
        {
            foreach (var entry in values)
                Values.Add(ValueWrapper.From(entry.Key), ValueWrapper.From(entry.Value));
        }
    }

    public JsonObject(Dictionary<string, Int32> values)
    {
        this.Values = new Dictionary<ValueWrapper, ValueWrapper>();

        if (values != null)
        {
            foreach (var entry in values)
                Values.Add(ValueWrapper.From(entry.Key), ValueWrapper.From(entry.Value));
        }
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        switch (conn.TYPE)
        {
            case ConnectorBase.SqlServiceType.MYSQL:
                {
                    sb.Append("JSON_OBJECT(");

                    bool first = true;
                    foreach (var val in Values)
                    {
                        if (first) first = false;
                        else sb.Append(",");

                        sb.Append(val.Key.Build(conn, relatedQuery));
                        sb.Append(",");
                        sb.Append(val.Value.Build(conn, relatedQuery));
                    }

                    sb.Append(")");
                }
                break;

            case ConnectorBase.SqlServiceType.POSTGRESQL:
                {
                    sb.Append("json_build_object(");

                    bool first = true;
                    foreach (var val in Values)
                    {
                        if (first) first = false;
                        else sb.Append(",");

                        sb.Append(val.Key.Build(conn, relatedQuery));
                        sb.Append(",");
                        sb.Append(val.Value.Build(conn, relatedQuery));
                    }

                    sb.Append(")");
                }
                break;

            default:
                throw new NotSupportedException("JsonExtract is not supported by current DB type");
        }
    }
}
