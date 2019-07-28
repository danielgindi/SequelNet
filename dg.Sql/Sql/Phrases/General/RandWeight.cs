using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class RandWeight : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public RandWeight(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public RandWeight(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public RandWeight(string columnName)
            : this(null, columnName)
        {
        }

        public RandWeight(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public RandWeight(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;
            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                ret = @"RAND(CAST(NEWID() AS VARBINARY)) * ";
            else if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                ret = @"RAND() * ";
            else // if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                ret = @"RANDOM() * ";

            ret += Value.Build(conn, relatedQuery);

            return ret;
        }
    }
}
