using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Sum : BaseAggregatePhrase
    {
        public bool Distinct = false;

        #region Constructors

        public Sum(bool distinct = false) : base()
        {
            this.Distinct = distinct;
        }

        public Sum(string tableName, string columnName, bool distinct = false) : base(tableName, columnName)
        {
            this.Distinct = distinct;
        }

        public Sum(string columnName, bool distinct = false) : base(columnName)
        {
            this.Distinct = distinct;
        }

        public Sum(object value, ValueObjectType valueType, bool distinct = false) : base(value, valueType)
        {
            this.Distinct = distinct;
        }

        public Sum(IPhrase phrase, bool distinct = false) : base(phrase)
        {
            this.Distinct = distinct;
        }

        public Sum(Where where, bool distinct = false) : base(where)
        {
            this.Distinct = distinct;
        }

        public Sum(WhereList where, bool distinct = false) : base(where)
        {
            this.Distinct = distinct;
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = Distinct ? "SUM(DISTINCT " : "SUM(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
