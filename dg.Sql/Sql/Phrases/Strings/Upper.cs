using System;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Upper : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        [Obsolete]
        public Upper(string tableName, object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(tableName, value, valueType);
        }

        public Upper(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Upper(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Upper(string columnName)
            : this(null, columnName)
        {
        }

        public Upper(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Upper(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.func_UPPER(ret);
        }
    }
}
