using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class StandardDeviationOfSample : BaseAggregatePhrase
{
    #region Constructors

    public StandardDeviationOfSample() : base()
    {
    }

    public StandardDeviationOfSample(string tableName, string columnName) : base(tableName, columnName)
    {
    }

    public StandardDeviationOfSample(string columnName) : base(columnName)
    {
    }

    public StandardDeviationOfSample(object value, ValueObjectType valueType) : base(value, valueType)
    {
    }

    public StandardDeviationOfSample(IPhrase phrase) : base(phrase)
    {
    }

    public StandardDeviationOfSample(Where where) : base(where)
    {
    }

    public StandardDeviationOfSample(WhereList where) : base(where)
    {
    }

    #endregion

    public override void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("STDDEV_SAMP(");
        sb.Append(Value.Build(conn, relatedQuery));
        sb.Append(")");
    }
}
