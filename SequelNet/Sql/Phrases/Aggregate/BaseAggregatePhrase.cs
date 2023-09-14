using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet;

public abstract class BaseAggregatePhrase : IPhrase
{
    public ValueWrapper Value;
    #region Constructors

    public BaseAggregatePhrase()
    {
        this.Value = ValueWrapper.Literal("*");
    }

    public BaseAggregatePhrase(string tableName, string columnName)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public BaseAggregatePhrase(string columnName)
        : this(null, columnName)
    {
    }

    public BaseAggregatePhrase(object value, ValueObjectType valueType)
    {
        this.Value = ValueWrapper.Make(value, valueType);
    }

    public BaseAggregatePhrase(IPhrase phrase)
        : this(phrase, ValueObjectType.Value)
    {
    }

    public BaseAggregatePhrase(Where where)
        : this(where, ValueObjectType.Value)
    {
    }

    public BaseAggregatePhrase(WhereList where)
        : this(where, ValueObjectType.Value)
    {
    }

    #endregion

    public virtual void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        throw new NotImplementedException();
    }
}
