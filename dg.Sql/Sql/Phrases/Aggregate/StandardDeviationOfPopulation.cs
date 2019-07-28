using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class StandardDeviationOfPopulation : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public StandardDeviationOfPopulation()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public StandardDeviationOfPopulation(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public StandardDeviationOfPopulation(string columnName)
            : this(null, columnName)
        {
        }

        public StandardDeviationOfPopulation(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public StandardDeviationOfPopulation(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"STDDEV_POP(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
