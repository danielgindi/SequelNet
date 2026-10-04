using System;
using System.Text;
using SequelNet.Connector;

#nullable enable

namespace SequelNet;

public abstract class BaseAggregatePhrase : IPhrase
{
    public ValueWrapper Value;
    #region Constructors

    public BaseAggregatePhrase()
    {
        this.Value = ValueWrapper.Literal("*");
    }

    public BaseAggregatePhrase(string? tableName, string columnName)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public BaseAggregatePhrase(string columnName)
        : this(null, columnName)
    {
    }

    public BaseAggregatePhrase(ValueWrapper value)
    {
        this.Value = value;
    }

    public BaseAggregatePhrase(object? value, ValueObjectType valueType)
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

    protected void ValidateValue(bool allowWildcard = false)
    {
        if (!allowWildcard &&
            Value.Type == ValueObjectType.Literal &&
            Value.Value is string literal &&
            string.Equals(literal, "*", StringComparison.Ordinal))
            throw new InvalidOperationException("Only non-distinct COUNT supports a wildcard value");
    }

    public virtual void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        throw new NotImplementedException();
    }
}
