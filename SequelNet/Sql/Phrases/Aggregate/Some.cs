using SequelNet.Connector;
using System.Text;

#nullable enable

namespace SequelNet.Phrases;

public class Some : BaseAggregatePhrase
{
    #region Constructors

    public Some() : base()
    {
    }

    public Some(string? tableName, string columnName) : base(tableName, columnName)
    {
    }

    public Some(string columnName) : base(columnName)
    {
    }

    public Some(object? value, ValueObjectType valueType) : base(value, valueType)
    {
    }

    public Some(IPhrase phrase) : base(phrase)
    {
    }

    public Some(Where where) : base(where)
    {
    }

    public Some(WhereList where) : base(where)
    {
    }

    #endregion

    public override void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        sb.Append(conn.Language.Aggregate_Some(Value.Build(conn, relatedQuery)));
    }
}
