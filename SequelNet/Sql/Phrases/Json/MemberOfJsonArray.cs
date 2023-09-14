using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

/// <summary>
/// Tests whether a primitive value is a member of a JSON array.
/// </summary>
public class MemberOfJsonArray : IPhrase
{
    public ValueWrapper Value;
    public ValueWrapper JsonArray;

    #region Constructors

    public MemberOfJsonArray(ValueWrapper value, ValueWrapper jsonArray)
    {
        this.Value = value;
        this.JsonArray = jsonArray;
    }

    public MemberOfJsonArray(object value, ValueObjectType valueType, object jsonArray, ValueObjectType jsonArrayType)
        : this(ValueWrapper.Make(value, valueType), ValueWrapper.Make(jsonArray, jsonArrayType))
    {
    }

    public MemberOfJsonArray(IPhrase value, ValueWrapper jsonArray)
        : this(ValueWrapper.From(value), jsonArray)
    {
    }

    public MemberOfJsonArray(ValueWrapper value, IPhrase jsonArray)
        : this(value, ValueWrapper.From(jsonArray))
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        conn.Language.BuildMemberOfJsonArray(Value, JsonArray, sb, conn, relatedQuery);
    }
}
