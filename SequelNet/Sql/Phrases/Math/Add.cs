using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases;

public class Add : IPhrase
{
    public List<ValueWrapper> Values = new List<ValueWrapper>();

    #region Constructors

    public Add(params ValueWrapper[] values)
    {
        this.Values.AddRange(values);
    }

    public Add(
        string tableName1, string columnName1,
        string tableName2, string columnName2)
        : this(
              ValueWrapper.Column(tableName1, columnName1),
              ValueWrapper.Column(tableName2, columnName2))
    {
    }

    public Add(
        string tableName1, string columnName1,
        object value2, ValueObjectType valueType2)
        : this(
              ValueWrapper.Column(tableName1, columnName1),
              ValueWrapper.Make(value2, valueType2))
    {
    }

    public Add(
        object value1, ValueObjectType valueType1,
        string tableName2, string columnName2)
        : this(
              ValueWrapper.Make(value1, valueType1),
              ValueWrapper.Column(tableName2, columnName2))
    {
    }

    public Add(
        object value1, ValueObjectType valueType1,
        object value2, ValueObjectType valueType2)
        : this(
              ValueWrapper.Make(value1, valueType1),
              ValueWrapper.Make(value2, valueType2))
    {
    }

    public Add(string tableName1, string columnName1, Int32 value2)
        : this(
              ValueWrapper.Column(tableName1, columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string tableName1, string columnName1, Int64 value2)
        : this(
              ValueWrapper.Column(tableName1, columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string tableName1, string columnName1, decimal value2)
        : this(
              ValueWrapper.Column(tableName1, columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string tableName1, string columnName1, double value2)
        : this(
              ValueWrapper.Column(tableName1, columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string tableName1, string columnName1, float value2)
        : this(
              ValueWrapper.Column(tableName1, columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string columnName1, Int32 value2)
        : this(
              ValueWrapper.Column(columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string columnName1, Int64 value2)
        : this(
              ValueWrapper.Column(columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string columnName1, decimal value2)
        : this(
              ValueWrapper.Column(columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string columnName1, double value2)
        : this(
              ValueWrapper.Column(columnName1),
              ValueWrapper.From(value2))
    {
    }

    public Add(string columnName1, float value2)
        : this(
              ValueWrapper.Column(columnName1),
              ValueWrapper.From(value2))
    {
    }
    public Add(
        object value1,
        object value2
        )
    {
        Values.Add(value1 is ValueWrapper vw1 ? vw1 : ValueWrapper.Make(value1, ValueObjectType.Value));
        Values.Add(value2 is ValueWrapper vw2 ? vw2 : ValueWrapper.Make(value2, ValueObjectType.Value));
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        bool first = true;

        foreach (var value in Values)
        {
            if (first) first = false;
            else sb.Append(" + ");

            sb.Append(value.Build(conn, relatedQuery));
        }
    }

    #region Multiply operators

    public static Phrases.Multiply operator *(Add a, Add b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Add a, decimal b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Add a, double b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Add a, Int64 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Add a, Int32 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(Add a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(Add a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(decimal a, Add b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(double a, Add b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int64 a, Add b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int32 a, Add b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt64 a, Add b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt32 a, Add b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    #endregion

    #region Divide operators

    public static Phrases.Divide operator /(Add a, Add b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Add a, decimal b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Add a, double b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Add a, Int64 b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Add a, Int32 b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(Add a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(Add a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(decimal a, Add b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(double a, Add b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int64 a, Add b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int32 a, Add b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt64 a, Add b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt32 a, Add b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    #endregion

    #region Add operators

    public static Phrases.Add operator +(Add a, Add b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Add a, decimal b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Add a, double b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Add a, Int64 b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Add a, Int32 b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(Add a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(Add a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(decimal a, Add b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(double a, Add b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int64 a, Add b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int32 a, Add b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt64 a, Add b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt32 a, Add b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    #endregion

    #region Subtract operators

    public static Phrases.Subtract operator -(Add a, Add b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Add a, decimal b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Add a, double b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Add a, Int64 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Add a, Int32 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(Add a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(Add a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(decimal a, Add b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(double a, Add b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int64 a, Add b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int32 a, Add b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt64 a, Add b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt32 a, Add b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    #endregion
}
