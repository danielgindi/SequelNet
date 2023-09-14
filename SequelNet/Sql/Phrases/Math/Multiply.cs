using SequelNet.Connector;
using System;
using System.Text;

namespace SequelNet.Phrases;

public class Multiply : IPhrase
{
    public ValueWrapper Value1;
    public ValueWrapper Value2;

    #region Constructors

    public Multiply(
        object value1, ValueObjectType valueType1,
        object value2, ValueObjectType valueType2
        )
    {
        this.Value1 = ValueWrapper.Make(value1, valueType1);
        this.Value2 = ValueWrapper.Make(value2, valueType2);
    }

    public Multiply(
        string tableName1, string column1,
        string tableName2, string column2
        )
    {
        this.Value1 = ValueWrapper.Column(tableName1, column1);
        this.Value2 = ValueWrapper.Column(tableName2, column2);
    }

    public Multiply(
        string tableName1, string column1,
        object value2, ValueObjectType valueType2
        )
    {
        this.Value1 = ValueWrapper.Column(tableName1, column1);
        this.Value2 = ValueWrapper.Make(value2, valueType2);
    }

    public Multiply(
        string tableName1, string column1,
        object value2
        )
    {
        this.Value1 = ValueWrapper.Column(tableName1, column1);
        this.Value2 = value2 is ValueWrapper vw2 ? vw2 : ValueWrapper.Make(value2, ValueObjectType.Value);
    }

    public Multiply(
        object value1, ValueObjectType valueType1,
        string tableName2, string column2
        )
    {
        this.Value1 = ValueWrapper.Make(value1, valueType1);
        this.Value2 = ValueWrapper.Column(tableName2, column2);
    }

    public Multiply(
        object value1,
        string tableName2, string column2
        )
    {
        this.Value1 = value1 is ValueWrapper vw1 ? vw1 : ValueWrapper.Make(value1, ValueObjectType.Value);
        this.Value2 = ValueWrapper.Column(tableName2, column2);
    }

    public Multiply(
        object value1,
        object value2
        )
    {
        this.Value1 = value1 is ValueWrapper vw1 ? vw1 : ValueWrapper.Make(value1, ValueObjectType.Value);
        this.Value2 = value2 is ValueWrapper vw2 ? vw2 : ValueWrapper.Make(value2, ValueObjectType.Value);
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("(");
        
        sb.Append(Value1.Build(conn, relatedQuery));

        sb.Append(" * ");
        
        sb.Append(Value2.Build(conn, relatedQuery));

        sb.Append(')');
    }

    #region Multiply operators

    public static Phrases.Multiply operator *(Multiply a, Multiply b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Multiply a, decimal b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Multiply a, double b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Multiply a, Int64 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Multiply a, Int32 b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(Multiply a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(Multiply a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(decimal a, Multiply b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(double a, Multiply b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int64 a, Multiply b)
    {
        return PhraseHelper.Multiply(a, b);
    }

    public static Phrases.Multiply operator *(Int32 a, Multiply b)
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt64 a, Multiply b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Multiply operator *(UInt32 a, Multiply b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Multiply(a, b);
    }

    #endregion

    #region Divide operators

    public static Phrases.Divide operator /(Multiply a, Multiply b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Multiply a, decimal b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Multiply a, double b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Multiply a, Int64 b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Multiply a, Int32 b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(Multiply a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(Multiply a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(decimal a, Multiply b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(double a, Multiply b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int64 a, Multiply b)
    {
        return PhraseHelper.Divide(a, b);
    }

    public static Phrases.Divide operator /(Int32 a, Multiply b)
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt64 a, Multiply b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Divide operator /(UInt32 a, Multiply b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Divide(a, b);
    }

    #endregion

    #region Add operators

    public static Phrases.Add operator +(Multiply a, Multiply b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Multiply a, decimal b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Multiply a, double b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Multiply a, Int64 b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Multiply a, Int32 b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(Multiply a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(Multiply a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(decimal a, Multiply b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(double a, Multiply b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int64 a, Multiply b)
    {
        return PhraseHelper.Add(a, b);
    }

    public static Phrases.Add operator +(Int32 a, Multiply b)
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt64 a, Multiply b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
    public static Phrases.Add operator +(UInt32 a, Multiply b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
    {
        return PhraseHelper.Add(a, b);
    }

    #endregion

    #region Subtract operators

    public static Phrases.Subtract operator -(Multiply a, Multiply b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Multiply a, decimal b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Multiply a, double b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Multiply a, Int64 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Multiply a, Int32 b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(Multiply a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(Multiply a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(decimal a, Multiply b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(double a, Multiply b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int64 a, Multiply b)
    {
        return PhraseHelper.Subtract(a, b);
    }

    public static Phrases.Subtract operator -(Int32 a, Multiply b)
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt64 a, Multiply b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public static Phrases.Subtract operator -(UInt32 a, Multiply b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
        return PhraseHelper.Subtract(a, b);
    }

    #endregion
}
