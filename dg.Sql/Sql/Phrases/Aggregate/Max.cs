using System;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Max : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Max()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public Max(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Max(string columnName)
            : this(null, columnName)
        {
        }

        public Max(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Max(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Max(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"MAX(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
