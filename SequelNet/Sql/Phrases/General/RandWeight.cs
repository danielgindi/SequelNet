using SequelNet.Connector;
using System.Text;

#nullable enable

namespace SequelNet.Phrases;

public class RandWeight : IPhrase
{
    public ValueWrapper Value;

    #region Constructors

    public RandWeight(object? value, ValueObjectType valueType)
    {
        this.Value = ValueWrapper.Make(value, valueType);
    }

    public RandWeight(string? tableName, string columnName)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public RandWeight(string columnName)
        : this(null, columnName)
    {
    }

    public RandWeight(IPhrase phrase)
        : this(phrase, ValueObjectType.Value)
    {
    }

    public RandWeight(Where where)
        : this(where, ValueObjectType.Value)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        conn.Language.BuildRandWeight(this, sb, conn, relatedQuery);
    }
}
