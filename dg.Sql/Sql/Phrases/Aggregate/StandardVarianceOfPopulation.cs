using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class StandardVarianceOfPopulation : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        [Obsolete]
        public StandardVarianceOfPopulation(string tableName, object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(tableName, value, valueType);
        }

        public StandardVarianceOfPopulation()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public StandardVarianceOfPopulation(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public StandardVarianceOfPopulation(string columnName)
            : this(null, columnName)
        {
        }

        public StandardVarianceOfPopulation(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public StandardVarianceOfPopulation(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"VAR_POP(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
