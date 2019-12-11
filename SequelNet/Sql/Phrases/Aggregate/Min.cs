using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Min : BaseAggregatePhrase
    {
        public bool Distinct = false;

        #region Constructors

        public Min(bool distinct = false) : base()
        {
            this.Distinct = distinct;
        }

        public Min(string tableName, string columnName, bool distinct = false) : base(tableName, columnName)
        {
            this.Distinct = distinct;
        }

        public Min(string columnName, bool distinct = false) : base(columnName)
        {
            this.Distinct = distinct;
        }

        public Min(object value, ValueObjectType valueType, bool distinct = false) : base(value, valueType)
        {
            this.Distinct = distinct;
        }

        public Min(IPhrase phrase, bool distinct = false) : base(phrase)
        {
            this.Distinct = distinct;
        }

        public Min(Where where, bool distinct = false) : base(where)
        {
            this.Distinct = distinct;
        }

        public Min(WhereList where, bool distinct = false) : base(where)
        {
            this.Distinct = distinct;
        }

        #endregion

        public override string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = Distinct ? "MIN(DISTINCT " : "MIN(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
