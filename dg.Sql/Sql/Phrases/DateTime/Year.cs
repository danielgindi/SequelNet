using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Year : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        [Obsolete]
        public Year(string tableName, object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(tableName, value, valueType);
        }

        public Year(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Year(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Year(string columnName)
            : this(null, columnName)
        {
        }

        public Year(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Year(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.func_YEAR(ret);
        }
    }
}
