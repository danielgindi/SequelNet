using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

/// <summary>
/// Creates a json array.
/// </summary>
public class JsonArray : IPhrase
{
    public List<ValueWrapper> Values;

    #region Constructors

    public JsonArray()
    {
        this.Values = new List<ValueWrapper>();
    }

    public JsonArray(params ValueWrapper[] values)
    {
        this.Values = new List<ValueWrapper>(values);
    }

    public JsonArray(IEnumerable<ValueWrapper> values)
    {
        this.Values = values == null ? new List<ValueWrapper>() : new List<ValueWrapper>(values);
    }

    public JsonArray(params object[] values)
    {
        this.Values = new List<ValueWrapper>(values.Length);
        foreach (var val in values)
            this.Values.Add(ValueWrapper.Make(val, ValueObjectType.Value));
    }

    public JsonArray(params string[] values)
    {
        this.Values = new List<ValueWrapper>(values.Length);
        foreach (var val in values)
            this.Values.Add(ValueWrapper.From(val));
    }

    public JsonArray(params Int64[] values)
    {
        this.Values = new List<ValueWrapper>(values.Length);
        foreach (var val in values)
            this.Values.Add(ValueWrapper.From(val));
    }

    public JsonArray(params Int32[] values)
    {
        this.Values = new List<ValueWrapper>(values.Length);
        foreach (var val in values)
            this.Values.Add(ValueWrapper.From(val));
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        conn.Language.BuildJsonArray(this, sb, conn, relatedQuery);
    }
}
