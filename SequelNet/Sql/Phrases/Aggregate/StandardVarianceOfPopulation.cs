using SequelNet.Connector;
using System.Text;

#nullable enable

namespace SequelNet.Phrases;

public class StandardVarianceOfPopulation : BaseAggregatePhrase
{
    #region Constructors

    public StandardVarianceOfPopulation() : base()
    {
    }

    public StandardVarianceOfPopulation(string? tableName, string columnName) : base(tableName, columnName)
    {
    }

    public StandardVarianceOfPopulation(string columnName) : base(columnName)
    {
    }

    public StandardVarianceOfPopulation(object? value, ValueObjectType valueType) : base(value, valueType)
    {
    }

    public StandardVarianceOfPopulation(IPhrase phrase) : base(phrase)
    {
    }

    public StandardVarianceOfPopulation(Where where) : base(where)
    {
    }

    public StandardVarianceOfPopulation(WhereList where) : base(where)
    {
    }

    #endregion

    public override void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        sb.Append("VAR_POP(");
        sb.Append(Value.Build(conn, relatedQuery));
        sb.Append(")");
    }
}
