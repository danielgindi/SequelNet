using SequelNet.Connector;
using System;
using System.Text;

namespace SequelNet.Phrases;

public class Round : IPhrase
{
    public ValueWrapper Value;
    public int DecimalPlaces;

    #region Constructors

    public Round(object value, ValueObjectType valueType, int decimalPlaces = 0)
    {
        this.Value = ValueWrapper.Make(value, valueType);
        this.DecimalPlaces = decimalPlaces;
    }

    public Round(string tableName, string columnName, int decimalPlaces = 0)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
        this.DecimalPlaces = decimalPlaces;
    }

    public Round(string columnName, int decimalPlaces = 0)
        : this(null, columnName, decimalPlaces)
    {
    }

    public Round(IPhrase phrase, int decimalPlaces = 0)
        : this(phrase, ValueObjectType.Value, decimalPlaces)
    {
    }

    public Round(Where where)
        : this(where, ValueObjectType.Value)
    {
    }
    
    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("ROUND(");

        sb.Append(Value.Build(conn, relatedQuery));

        if (DecimalPlaces != 0)
        {
            sb.Append(',');
            sb.Append(DecimalPlaces);
        }
        sb.Append(')');
    }

    #region Multiply operators

    public static Phrases.Multiply operator *(Round a, Round b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Round a, decimal b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Round a, double b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Round a, Int64 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Round a, Int32 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(Round a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(Round a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(decimal a, Round b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(double a, Round b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int64 a, Round b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int32 a, Round b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt64 a, Round b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt32 a, Round b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    #endregion

    #region Divide operators

    public static Phrases.Divide operator /(Round a, Round b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Round a, decimal b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Round a, double b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Round a, Int64 b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Round a, Int32 b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(Round a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(Round a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(decimal a, Round b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(double a, Round b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int64 a, Round b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int32 a, Round b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt64 a, Round b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt32 a, Round b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    #endregion

    #region Add operators

    public static Phrases.Add operator +(Round a, Round b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Round a, decimal b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Round a, double b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Round a, Int64 b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Round a, Int32 b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(Round a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(Round a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(decimal a, Round b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(double a, Round b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int64 a, Round b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int32 a, Round b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt64 a, Round b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt32 a, Round b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    #endregion

    #region Subtract operators

    public static Phrases.Subtract operator -(Round a, Round b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Round a, decimal b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Round a, double b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Round a, Int64 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Round a, Int32 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(Round a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(Round a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(decimal a, Round b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(double a, Round b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int64 a, Round b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int32 a, Round b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt64 a, Round b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt32 a, Round b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    #endregion
}
