using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet
{
    public struct ConflictColumn : IPhrase, IEquatable<ConflictColumn>
    {
        public string Column;

        #region Constructors

        public ConflictColumn(string column)
        {
            this.Column = column;
        }

        #endregion

        #region Builders

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {

        }

        #endregion

        #region Casts

        public static explicit operator ConflictColumn(string column)
        {
            return new ConflictColumn(column);
        }

        #endregion

        #region IEquatable

        public override bool Equals(object obj)
        {
            if (!(obj is ConflictColumn other))
                return false;

            if (Column != other.Column)
                return false;

            return true;
        }

        public bool Equals(ConflictColumn other)
        {
            if (Column != other.Column)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 1551432323;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Column);
            return hashCode;
        }

        public static bool operator ==(ConflictColumn lhs, ConflictColumn rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ConflictColumn lhs, ConflictColumn rhs)
        {
            return !lhs.Equals(rhs);
        }

        #endregion
    }
}
