using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class ST_X : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors
        
        public ST_X(IPhrase phrase)
        {
            this.Value = new ValueWrapper(phrase);
        }

        public ST_X(string tableName, string column)
        {
            this.Value = new ValueWrapper(tableName, column);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "(";

            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
            {
            }
            else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
            {
                ret += "ST_X(";
            }
            else
            {
                ret += "X(";
            }
                        
            ret += Value.Build(conn, relatedQuery);
            
            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
            {
                ret += ".STX";
            }
            else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
            {
                ret += ")";
            }
            else
            {
                ret += ")";
            }

            ret += ')';

            return ret;
        }
    }
}
