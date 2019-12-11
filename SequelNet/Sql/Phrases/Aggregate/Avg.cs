using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Avg : BaseAggregatePhrase
    {
        #region Constructors

        public Avg() : base()
        {
        }

        public Avg(string tableName, string columnName) : base(tableName, columnName)
        {
        }

        public Avg(string columnName) : base(columnName)
        {
        }

        public Avg(object value, ValueObjectType valueType) : base(value, valueType)
        {
        }

        public Avg(IPhrase phrase) : base(phrase)
        {
        }

        public Avg(Where where) : base(where)
        {
        }

        public Avg(WhereList where) : base(where)
        {
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = "AVG(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
