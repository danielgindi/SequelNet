using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Count : IPhrase
    {
        public bool Distinct = false;
        public ValueWrapper Value;

        #region Constructors

        public Count(bool distinct = false)
        {
            this.Distinct = distinct;
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public Count(string tableName, string columnName, bool distinct = false)
        {
            this.Distinct = distinct;
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Count(string columnName, bool distinct = false)
            : this(null, columnName, distinct)
        {
        }

        public Count(object value, ValueObjectType valueType, bool distinct = false)
        {
            this.Distinct = distinct;
            this.Value = new ValueWrapper(value, valueType);
        }

        public Count(IPhrase phrase, bool distinct = false)
            : this(phrase, ValueObjectType.Value, distinct)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            if (Distinct)
            {
                ret = @"COUNT(DISTINCT ";
            }
            else
            {
                ret = @"COUNT(";
            }

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
