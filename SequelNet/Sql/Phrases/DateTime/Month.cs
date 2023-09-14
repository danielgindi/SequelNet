using SequelNet.Connector;
using System;
using System.Text;

namespace SequelNet.Phrases;

public class Month : IPhrase
{
    public ValueWrapper Value;

    #region Constructors

    public Month(object value, ValueObjectType valueType)
    {
        this.Value = ValueWrapper.Make(value, valueType);
    }

    public Month(string tableName, string columnName)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public Month(string columnName)
        : this(null, columnName)
    {
    }

    public Month(IPhrase phrase)
        : this(phrase, ValueObjectType.Value)
    {
    }

    public Month(ValueWrapper value)
    {
        this.Value = value;
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.MonthPartOfDateTime(Value.Build(conn, relatedQuery)));
    }

    #region Multiply operators

    public static Phrases.Multiply operator *(Month a, Month b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Month a, decimal b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Month a, double b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Month a, Int64 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Month a, Int32 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(Month a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(Month a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(decimal a, Month b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(double a, Month b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int64 a, Month b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int32 a, Month b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt64 a, Month b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt32 a, Month b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    #endregion

    #region Divide operators

    public static Phrases.Divide operator /(Month a, Month b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Month a, decimal b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Month a, double b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Month a, Int64 b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Month a, Int32 b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(Month a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(Month a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(decimal a, Month b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(double a, Month b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int64 a, Month b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int32 a, Month b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt64 a, Month b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt32 a, Month b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    #endregion

    #region Add operators

    public static Phrases.Add operator +(Month a, Month b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Month a, decimal b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Month a, double b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Month a, Int64 b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Month a, Int32 b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(Month a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(Month a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(decimal a, Month b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(double a, Month b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int64 a, Month b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int32 a, Month b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt64 a, Month b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt32 a, Month b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    #endregion

    #region Subtract operators

    public static Phrases.Subtract operator -(Month a, Month b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Month a, decimal b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Month a, double b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Month a, Int64 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Month a, Int32 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(Month a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(Month a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(decimal a, Month b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(double a, Month b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int64 a, Month b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int32 a, Month b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt64 a, Month b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt32 a, Month b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    #endregion
}
