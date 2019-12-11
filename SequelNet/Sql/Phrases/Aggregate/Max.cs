using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Max : BaseAggregatePhrase
    {
        public bool Distinct = false;

        #region Constructors

        public Max(bool distinct = false) : base()
        {
            this.Distinct = distinct;
        }

        public Max(string tableName, string columnName, bool distinct = false) : base(tableName, columnName)
        {
            this.Distinct = distinct;
        }

        public Max(string columnName, bool distinct = false) : base(columnName)
        {
            this.Distinct = distinct;
        }

        public Max(object value, ValueObjectType valueType, bool distinct = false) : base(value, valueType)
        {
            this.Distinct = distinct;
        }

        public Max(IPhrase phrase, bool distinct = false) : base(phrase)
        {
            this.Distinct = distinct;
        }

        public Max(Where where, bool distinct = false) : base(where)
        {
            this.Distinct = distinct;
        }

        public Max(WhereList where, bool distinct = false) : base(where)
        {
            this.Distinct = distinct;
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = Distinct ? "MAX(DISTINCT " : "MAX(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
