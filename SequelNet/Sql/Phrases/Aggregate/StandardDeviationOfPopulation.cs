using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class StandardDeviationOfPopulation : BaseAggregatePhrase
{
    #region Constructors

    public StandardDeviationOfPopulation() : base()
    {
    }

    public StandardDeviationOfPopulation(string tableName, string columnName) : base(tableName, columnName)
    {
    }

    public StandardDeviationOfPopulation(string columnName) : base(columnName)
    {
    }

    public StandardDeviationOfPopulation(object value, ValueObjectType valueType) : base(value, valueType)
    {
    }

    public StandardDeviationOfPopulation(IPhrase phrase) : base(phrase)
    {
    }

    public StandardDeviationOfPopulation(Where where) : base(where)
    {
    }

    public StandardDeviationOfPopulation(WhereList where) : base(where)
    {
    }

    #endregion

    public override void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("STDDEV_POP(");
        sb.Append(Value.Build(conn, relatedQuery));
        sb.Append(")");
    }
}
