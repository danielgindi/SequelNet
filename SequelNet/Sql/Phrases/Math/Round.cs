using SequelNet.Connector;

namespace SequelNet.Phrases
{
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

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"ROUND(";

            ret += Value.Build(conn, relatedQuery);

            if (DecimalPlaces != 0)
            {
                ret += ',';
                ret += DecimalPlaces;
            }
            ret += ')';

            return ret;
        }
    }
}
