using System;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class StandardVarianceOfSample : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public StandardVarianceOfSample()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public StandardVarianceOfSample(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public StandardVarianceOfSample(string columnName)
            : this(null, columnName)
        {
        }

        public StandardVarianceOfSample(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public StandardVarianceOfSample(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"VAR_SAMP(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
