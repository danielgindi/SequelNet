using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class ST_Y : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors
        
        public ST_Y(IPhrase phrase)
        {
            this.Value = new ValueWrapper(phrase);
        }

        public ST_Y(string tableName, string column)
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
                ret += "ST_Y(";
            }
            else
            {
                ret += "Y(";
            }
                        
            ret += Value.Build(conn, relatedQuery);
            
            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
            {
                ret += ".STY";
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
