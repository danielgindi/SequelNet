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

        public ST_Y(ValueWrapper value)
        {
            this.Value = value;
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.func_ST_Y(Value.Build(conn, relatedQuery));
        }
    }
}
