using System;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class StandardDeviationOfSample : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        [Obsolete]
        public StandardDeviationOfSample(string tableName, object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(tableName, value, valueType);
        }

        public StandardDeviationOfSample()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public StandardDeviationOfSample(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public StandardDeviationOfSample(string columnName)
            : this(null, columnName)
        {
        }

        public StandardDeviationOfSample(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public StandardDeviationOfSample(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"STDDEV_SAMP(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
