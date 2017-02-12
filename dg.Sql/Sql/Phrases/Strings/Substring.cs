using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Substring : IPhrase
    {
        public ValueWrapper Value;
        public int From;
        public int Length;

        #region Constructors
        
        public Substring(object value, ValueObjectType valueType, int from, int length)
        {
            this.Value = new ValueWrapper(value, valueType);
            this.From = from;
            this.Length = length;
        }

        public Substring(string tableName, string columnName, int from, int length)
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.From = from;
            this.Length = length;
        }

        public Substring(string columnName, int from, int length)
            : this(null, columnName, from, length)
        {
        }

        public Substring(IPhrase phrase, int from, int length)
            : this(phrase, ValueObjectType.Value, from, length)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "SUBSTRING(";

            ret += Value.Build(conn, relatedQuery);

            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL ||
                conn.TYPE == ConnectorBase.SqlServiceType.MSACCESS)
            {
                ret += ", " + From + ", " + Length;
            }
            else
            {
                ret += " FROM " + From + " FOR " + Length;
            }

            ret += ")";

            return ret;
        }
    }
}
