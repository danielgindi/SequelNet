using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Ceil : IPhrase
    {
        public ValueWrapper Value;
        public int DecimalPlaces;

        #region Constructors

        public Ceil(object value, ValueObjectType valueType, int decimalPlaces = 0)
        {
            this.Value = ValueWrapper.Make(value, valueType);
            this.DecimalPlaces = decimalPlaces;
        }

        public Ceil(string tableName, string columnName, int decimalPlaces = 0)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.DecimalPlaces = decimalPlaces;
        }

        public Ceil(string columnName, int decimalPlaces = 0)
            : this(null, columnName, decimalPlaces)
        {
        }

        public Ceil(IPhrase phrase, int decimalPlaces = 0)
            : this(phrase, ValueObjectType.Value, decimalPlaces)
        {
        }

        public Ceil(Where where)
            : this(where, ValueObjectType.Value)
        {
        }
        
        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append("CEIL(");

            sb.Append(Value.Build(conn, relatedQuery));

            if (DecimalPlaces != 0)
            {
                sb.Append(',');
                sb.Append(DecimalPlaces);
            }
            sb.Append(')');
        }
    }
}
