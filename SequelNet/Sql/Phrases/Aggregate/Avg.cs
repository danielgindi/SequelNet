using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class Avg : BaseAggregatePhrase
{
    #region Constructors

    public Avg() : base()
    {
    }

    public Avg(string tableName, string columnName) : base(tableName, columnName)
    {
    }

    public Avg(string columnName) : base(columnName)
    {
    }

    public Avg(object value, ValueObjectType valueType) : base(value, valueType)
    {
    }

    public Avg(IPhrase phrase) : base(phrase)
    {
    }

    public Avg(Where where) : base(where)
    {
    }

    public Avg(WhereList where) : base(where)
    {
    }

    #endregion

    public override void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("AVG(");
        sb.Append(Value.Build(conn, relatedQuery));
        sb.Append(")");
    }
}
