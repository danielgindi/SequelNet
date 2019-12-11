using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class StandardDeviationOfPopulation : BaseAggregatePhrase
    {
        #region Constructors

        public StandardDeviationOfPopulation() : base()
        {
        }

        public StandardDeviationOfPopulation(string tableName, string columnName) : base(tableName, columnName)
        {
        }

        public StandardDeviationOfPopulation(string columnName) : base(columnName)
        {
        }

        public StandardDeviationOfPopulation(object value, ValueObjectType valueType) : base(value, valueType)
        {
        }

        public StandardDeviationOfPopulation(IPhrase phrase) : base(phrase)
        {
        }

        public StandardDeviationOfPopulation(Where where) : base(where)
        {
        }

        public StandardDeviationOfPopulation(WhereList where) : base(where)
        {
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = "STDDEV_POP(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
