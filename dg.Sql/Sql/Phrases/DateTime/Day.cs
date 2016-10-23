using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Day : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        [Obsolete]
        public Day(string tableName, object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(tableName, value, valueType);
        }

        public Day(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Day(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Day(string columnName)
            : this(null, columnName)
        {
        }

        public Day(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Day(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return conn.func_DAY(ret);
        }
    }
}
