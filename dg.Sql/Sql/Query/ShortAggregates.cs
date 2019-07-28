using System;
using dg.Sql.Connector;

namespace dg.Sql
{
    public partial class Query
    {
        #region GetCount Helpers

        public Int64 GetCount()
        {
            return GetCount(null, null, @"*", null);
        }

        public Int64 GetCount(ConnectorBase conn)
        {
            return GetCount(null, null, @"*", conn);
        }

        public Int64 GetCount(string columnName)
        {
            return GetCount(null, null, columnName, null);
        }

        public Int64 GetCount(string columnName, ConnectorBase conn)
        {
            return GetCount(null, null, columnName, conn);
        }

        public Int64 GetCount(string schemaName, string columnName)
        {
            return GetCount(null, schemaName, columnName, null);
        }

        public Int64 GetCount(string schemaName, string columnName, ConnectorBase conn)
        {
            return GetCount(null, schemaName, columnName, conn);
        }

        public Int64 GetCount(string databaseOwner, string schemaName, string columnName)
        {
            return GetCount(databaseOwner, schemaName, columnName, null);
        }

        public Int64 GetCount(string databaseOwner, string schemaName, string columnName, ConnectorBase conn)
        {
            object res = this.ExecuteAggregate(databaseOwner, schemaName, columnName, @"COUNT", IsDistinct, conn);
            if (IsNull(res)) return 0;
            else return Convert.ToInt64(res);
        }

        #endregion

        #region GetMax Helpers

        public object GetMax(string columnName)
        {
            return GetMax(null, null, columnName, null);
        }

        public object GetMax(string columnName, ConnectorBase conn)
        {
            return GetMax(null, null, columnName, conn);
        }

        public object GetMax(string schemaName, string columnName)
        {
            return GetMax(null, schemaName, columnName, null);
        }

        public object GetMax(string schemaName, string columnName, ConnectorBase conn)
        {
            return GetMax(null, schemaName, columnName, conn);
        }

        public object GetMax(string databaseOwner, string schemaName, string columnName)
        {
            return GetMax(databaseOwner, schemaName, columnName, null);
        }

        public object GetMax(string databaseOwner, string schemaName, string columnName, ConnectorBase conn)
        {
            return this.ExecuteAggregate(databaseOwner, schemaName, columnName, @"MAX", false, conn);
        }

        #endregion

        #region GetMin Helpers

        public object GetMin(string columnName)
        {
            return GetMin(null, null, columnName, null);
        }

        public object GetMin(string columnName, ConnectorBase conn)
        {
            return GetMin(null, null, columnName, conn);
        }

        public object GetMin(string schemaName, string columnName)
        {
            return GetMin(null, schemaName, columnName, null);
        }

        public object GetMin(string schemaName, string columnName, ConnectorBase conn)
        {
            return GetMin(null, schemaName, columnName, conn);
        }

        public object GetMin(string databaseOwner, string schemaName, string columnName)
        {
            return GetMin(databaseOwner, schemaName, columnName, null);
        }

        public object GetMin(string databaseOwner, string schemaName, string columnName, ConnectorBase conn)
        {
            return this.ExecuteAggregate(databaseOwner, schemaName, columnName, @"MIN", false, conn);
        }

        #endregion

        #region GetSum Helpers

        public object GetSum(string columnName)
        {
            return GetSum(null, null, columnName, null);
        }

        public object GetSum(string columnName, ConnectorBase conn)
        {
            return GetSum(null, null, columnName, conn);
        }

        public object GetSum(string schemaName, string columnName)
        {
            return GetSum(null, schemaName, columnName, null);
        }

        public object GetSum(string schemaName, string columnName, ConnectorBase conn)
        {
            return GetSum(null, schemaName, columnName, conn);
        }

        public object GetSum(string databaseOwner, string schemaName, string columnName)
        {
            return GetSum(databaseOwner, schemaName, columnName, null);
        }

        public object GetSum(string databaseOwner, string schemaName, string columnName, ConnectorBase conn)
        {
            return this.ExecuteAggregate(databaseOwner, schemaName, columnName, @"SUM", IsDistinct, conn);
        }

        #endregion
    }
}
