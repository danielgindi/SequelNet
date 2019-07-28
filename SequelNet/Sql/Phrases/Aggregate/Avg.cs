using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Avg : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Avg()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public Avg(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Avg(string columnName)
            : this(null, columnName)
        {
        }

        public Avg(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Avg(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Avg(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"AVG(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
