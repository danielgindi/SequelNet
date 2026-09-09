using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

/// <summary>
/// Returns the length of a json object/array
/// </summary>
public class JsonLength : IPhrase
{
    public ValueWrapper Value;

    #region Constructors
    
    public JsonLength(object value, ValueObjectType valueType)
    {
        this.Value = ValueWrapper.Make(value, valueType);
    }

    public JsonLength(string tableName, string columnName)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public JsonLength(string columnName)
        : this(null, columnName)
    {
    }

    public JsonLength(IPhrase phrase)
        : this(phrase, ValueObjectType.Value)
    {
    }

    public JsonLength(Where where)
        : this(where, ValueObjectType.Value)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        conn.Language.BuildJsonLength(this, sb, conn, relatedQuery);
    }
}
