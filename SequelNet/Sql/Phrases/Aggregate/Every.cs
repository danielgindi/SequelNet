using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class Every : BaseAggregatePhrase
{
    #region Constructors

    public Every() : base()
    {
    }

    public Every(string tableName, string columnName) : base(tableName, columnName)
    {
    }

    public Every(string columnName) : base(columnName)
    {
    }

    public Every(object value, ValueObjectType valueType) : base(value, valueType)
    {
    }

    public Every(IPhrase phrase) : base(phrase)
    {
    }

    public Every(Where where) : base(where)
    {
    }

    public Every(WhereList where) : base(where)
    {
    }

    #endregion

    public override void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.Aggregate_Every(Value.Build(conn, relatedQuery)));
    }
}
