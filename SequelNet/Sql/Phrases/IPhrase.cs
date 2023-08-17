using SequelNet.Connector;
using System;
using System.Text;

namespace SequelNet
{
    public interface IPhrase
    {
        void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null);

#if NETSTANDARD21 || NETCORE

        #region Multiply operators

        public static Phrases.Multiply operator *(IPhrase a, IPhrase b)
        {
            return PhraseHelper.Multiply(a, b);
        }

        public static Phrases.Multiply operator *(IPhrase a, decimal b)
        {
            return PhraseHelper.Multiply(a, b);
        }

        public static Phrases.Multiply operator *(IPhrase a, double b)
        {
            return PhraseHelper.Multiply(a, b);
        }

        public static Phrases.Multiply operator *(IPhrase a, Int64 b)
        {
            return PhraseHelper.Multiply(a, b);
        }

        public static Phrases.Multiply operator *(IPhrase a, Int32 b)
        {
            return PhraseHelper.Multiply(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Multiply operator *(IPhrase a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Multiply(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Multiply operator *(IPhrase a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Multiply(a, b);
        }

        public static Phrases.Multiply operator *(decimal a, IPhrase b)
        {
            return PhraseHelper.Multiply(a, b);
        }

        public static Phrases.Multiply operator *(double a, IPhrase b)
        {
            return PhraseHelper.Multiply(a, b);
        }

        public static Phrases.Multiply operator *(Int64 a, IPhrase b)
        {
            return PhraseHelper.Multiply(a, b);
        }

        public static Phrases.Multiply operator *(Int32 a, IPhrase b)
        {
            return PhraseHelper.Multiply(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Multiply operator *(UInt64 a, IPhrase b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Multiply(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Multiply operator *(UInt32 a, IPhrase b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Multiply(a, b);
        }

        #endregion

        #region Divide operators

        public static Phrases.Divide operator /(IPhrase a, IPhrase b)
        {
            return PhraseHelper.Divide(a, b);
        }

        public static Phrases.Divide operator /(IPhrase a, decimal b)
        {
            return PhraseHelper.Divide(a, b);
        }

        public static Phrases.Divide operator /(IPhrase a, double b)
        {
            return PhraseHelper.Divide(a, b);
        }

        public static Phrases.Divide operator /(IPhrase a, Int64 b)
        {
            return PhraseHelper.Divide(a, b);
        }

        public static Phrases.Divide operator /(IPhrase a, Int32 b)
        {
            return PhraseHelper.Divide(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Divide operator /(IPhrase a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Divide(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Divide operator /(IPhrase a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Divide(a, b);
        }

        public static Phrases.Divide operator /(decimal a, IPhrase b)
        {
            return PhraseHelper.Divide(a, b);
        }

        public static Phrases.Divide operator /(double a, IPhrase b)
        {
            return PhraseHelper.Divide(a, b);
        }

        public static Phrases.Divide operator /(Int64 a, IPhrase b)
        {
            return PhraseHelper.Divide(a, b);
        }

        public static Phrases.Divide operator /(Int32 a, IPhrase b)
        {
            return PhraseHelper.Divide(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Divide operator /(UInt64 a, IPhrase b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Divide(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Divide operator /(UInt32 a, IPhrase b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Divide(a, b);
        }

        #endregion

        #region Add operators

        public static Phrases.Add operator +(IPhrase a, IPhrase b)
        {
            return PhraseHelper.Add(a, b);
        }

        public static Phrases.Add operator +(IPhrase a, decimal b)
        {
            return PhraseHelper.Add(a, b);
        }

        public static Phrases.Add operator +(IPhrase a, double b)
        {
            return PhraseHelper.Add(a, b);
        }

        public static Phrases.Add operator +(IPhrase a, Int64 b)
        {
            return PhraseHelper.Add(a, b);
        }

        public static Phrases.Add operator +(IPhrase a, Int32 b)
        {
            return PhraseHelper.Add(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
        public static Phrases.Add operator +(IPhrase a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
        {
            return PhraseHelper.Add(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
        public static Phrases.Add operator +(IPhrase a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
        {
            return PhraseHelper.Add(a, b);
        }

        public static Phrases.Add operator +(decimal a, IPhrase b)
        {
            return PhraseHelper.Add(a, b);
        }

        public static Phrases.Add operator +(double a, IPhrase b)
        {
            return PhraseHelper.Add(a, b);
        }

        public static Phrases.Add operator +(Int64 a, IPhrase b)
        {
            return PhraseHelper.Add(a, b);
        }

        public static Phrases.Add operator +(Int32 a, IPhrase b)
        {
            return PhraseHelper.Add(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
        public static Phrases.Add operator +(UInt64 a, IPhrase b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
        {
            return PhraseHelper.Add(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS+compliant
        public static Phrases.Add operator +(UInt32 a, IPhrase b)
#pragma warning restore CS3001 // Argument type is not CLS+compliant
        {
            return PhraseHelper.Add(a, b);
        }

        #endregion

        #region Subtract operators

        public static Phrases.Subtract operator -(IPhrase a, IPhrase b)
        {
            return PhraseHelper.Subtract(a, b);
        }

        public static Phrases.Subtract operator -(IPhrase a, decimal b)
        {
            return PhraseHelper.Subtract(a, b);
        }

        public static Phrases.Subtract operator -(IPhrase a, double b)
        {
            return PhraseHelper.Subtract(a, b);
        }

        public static Phrases.Subtract operator -(IPhrase a, Int64 b)
        {
            return PhraseHelper.Subtract(a, b);
        }

        public static Phrases.Subtract operator -(IPhrase a, Int32 b)
        {
            return PhraseHelper.Subtract(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Subtract operator -(IPhrase a, UInt64 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Subtract(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Subtract operator -(IPhrase a, UInt32 b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Subtract(a, b);
        }

        public static Phrases.Subtract operator -(decimal a, IPhrase b)
        {
            return PhraseHelper.Subtract(a, b);
        }

        public static Phrases.Subtract operator -(double a, IPhrase b)
        {
            return PhraseHelper.Subtract(a, b);
        }

        public static Phrases.Subtract operator -(Int64 a, IPhrase b)
        {
            return PhraseHelper.Subtract(a, b);
        }

        public static Phrases.Subtract operator -(Int32 a, IPhrase b)
        {
            return PhraseHelper.Subtract(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Subtract operator -(UInt64 a, IPhrase b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Subtract(a, b);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static Phrases.Subtract operator -(UInt32 a, IPhrase b)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            return PhraseHelper.Subtract(a, b);
        }

        #endregion

#endif
    }
}
