using SequelNet.Connector;
using System.Text;

#nullable enable

namespace SequelNet.Phrases;

/// <summary>
/// Tests equality of two values. Returns NULL if the values are equal, otherwise returns the first value.
/// </summary>
public class NullIf : IPhrase
{
    public ValueWrapper Value1;
    public ValueWrapper Value2;

    #region Constructors

    public NullIf(
        ValueWrapper first,
        ValueWrapper second)
    {
        this.Value1 = first;
        this.Value2 = second;
    }

    public NullIf(
        string? firstTableName, string firstColumnName,
        string? secondTableName, string secondColumnName)
    {
        this.Value1 = ValueWrapper.Column(firstTableName, firstColumnName);
        this.Value2 = ValueWrapper.Column(secondTableName, secondColumnName);
    }

    public NullIf(
         object? firstValue, ValueObjectType firstValueType,
         object? secondValue, ValueObjectType secondValueType)
    {
        this.Value1 = ValueWrapper.Make(firstValue, firstValueType);
        this.Value2 = ValueWrapper.Make(secondValue, secondValueType);
    }

    public NullIf(
         string? firstTableName, string firstColumnName,
         object? secondValue, ValueObjectType secondValueType)
    {
        this.Value1 = ValueWrapper.Column(firstTableName, firstColumnName);
        this.Value2 = ValueWrapper.Make(secondValue, secondValueType);
    }

    public NullIf(
         string? firstTableName, string firstColumnName,
         ValueWrapper second)
    {
        this.Value1 = ValueWrapper.Column(firstTableName, firstColumnName);
        this.Value2 = second;
    }

    public NullIf(
         object? firstValue, ValueObjectType firstValueType,
         string? secondTableName, string secondColumnName)
    {
        this.Value1 = ValueWrapper.Make(firstValue, firstValueType);
        this.Value2 = ValueWrapper.Column(secondTableName, secondColumnName);
    }

    public NullIf(
         ValueWrapper first,
         string? secondTableName, string secondColumnName)
    {
        this.Value1 = first;
        this.Value2 = ValueWrapper.Column(secondTableName, secondColumnName);
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        sb.Append(conn.Language.IfEqualThenNull(
            Value1.Build(conn, relatedQuery),
            Value2.Build(conn, relatedQuery)
        ));
    }
}
