using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Abs : IPhrase
    {
        public ValueWrapper Value;
        public int DecimalPlaces;

        #region Constructors
        
        public Abs(object value, ValueObjectType valueType, int decimalPlaces = 0)
        {
            this.Value = new ValueWrapper(value, valueType);
            this.DecimalPlaces = decimalPlaces;
        }

        public Abs(string tableName, string columnName, int decimalPlaces = 0)
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.DecimalPlaces = decimalPlaces;
        }

        public Abs(string columnName, int decimalPlaces = 0)
            : this(null, columnName, decimalPlaces)
        {
        }

        public Abs(IPhrase phrase, int decimalPlaces = 0)
            : this(phrase, ValueObjectType.Value, decimalPlaces)
        {
        }

        public Abs(Where where)
            : this(where, ValueObjectType.Value)
        {
        }
        
        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"ABS(";

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
