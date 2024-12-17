using SequelNet.Connector;
using System.Text;

#nullable enable

namespace SequelNet.Phrases;

public class Sum : BaseAggregatePhrase
{
    public bool Distinct = false;

    #region Constructors

    public Sum(bool distinct = false) : base()
    {
        this.Distinct = distinct;
    }

    public Sum(string? tableName, string columnName, bool distinct = false) : base(tableName, columnName)
    {
        this.Distinct = distinct;
    }

    public Sum(string columnName, bool distinct = false) : base(columnName)
    {
        this.Distinct = distinct;
    }

    public Sum(object? value, ValueObjectType valueType, bool distinct = false) : base(value, valueType)
    {
        this.Distinct = distinct;
    }

    public Sum(IPhrase phrase, bool distinct = false) : base(phrase)
    {
        this.Distinct = distinct;
    }

    public Sum(Where where, bool distinct = false) : base(where)
    {
        this.Distinct = distinct;
    }

    public Sum(WhereList where, bool distinct = false) : base(where)
    {
        this.Distinct = distinct;
    }

    #endregion

    public override void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        sb.Append(Distinct ? "SUM(DISTINCT " : "SUM(");
        sb.Append(Value.Build(conn, relatedQuery));
        sb.Append(")");
    }
}
