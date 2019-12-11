using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class StandardVarianceOfPopulation : BaseAggregatePhrase
    {
        #region Constructors

        public StandardVarianceOfPopulation() : base()
        {
        }

        public StandardVarianceOfPopulation(string tableName, string columnName) : base(tableName, columnName)
        {
        }

        public StandardVarianceOfPopulation(string columnName) : base(columnName)
        {
        }

        public StandardVarianceOfPopulation(object value, ValueObjectType valueType) : base(value, valueType)
        {
        }

        public StandardVarianceOfPopulation(IPhrase phrase) : base(phrase)
        {
        }

        public StandardVarianceOfPopulation(Where where) : base(where)
        {
        }

        public StandardVarianceOfPopulation(WhereList where) : base(where)
        {
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = "VAR_POP(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
