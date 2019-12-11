using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class StandardVarianceOfSample : BaseAggregatePhrase
    {
        #region Constructors

        public StandardVarianceOfSample() : base()
        {
        }

        public StandardVarianceOfSample(string tableName, string columnName) : base(tableName, columnName)
        {
        }

        public StandardVarianceOfSample(string columnName) : base(columnName)
        {
        }

        public StandardVarianceOfSample(object value, ValueObjectType valueType) : base(value, valueType)
        {
        }

        public StandardVarianceOfSample(IPhrase phrase) : base(phrase)
        {
        }

        public StandardVarianceOfSample(Where where) : base(where)
        {
        }

        public StandardVarianceOfSample(WhereList where) : base(where)
        {
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = "VAR_SAMP(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
