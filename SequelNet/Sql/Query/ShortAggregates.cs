using System;
using System.Threading;
using SequelNet.Connector;

namespace SequelNet
{
    public partial class Query
    {
        #region GetCount Helpers

        public Int64 GetCount()
        {
            return GetCount(null, "*", null);
        }

        public Int64 GetCount(ConnectorBase conn)
        {
            return GetCount(null, "*", conn);
        }

        public Int64 GetCount(string columnName, ConnectorBase conn = null)
        {
            return GetCount(null, columnName, conn);
        }

        public Int64 GetCount(string schemaName, string columnName, ConnectorBase conn = null)
        {
            object res = this.ExecuteAggregate(new Phrases.Count(schemaName, columnName, IsDistinct), conn);
            if (res == null) return 0;
            else return Convert.ToInt64(res);
        }

        public System.Threading.Tasks.Task<Int64> GetCountAsync(CancellationToken? cancellationToken)
        {
            return GetCountAsync(null, "*", null, cancellationToken);
        }

        public System.Threading.Tasks.Task<Int64> GetCountAsync(ConnectorBase conn, CancellationToken? cancellationToken = null)
        {
            return GetCountAsync(null, "*", conn, cancellationToken);
        }

        public System.Threading.Tasks.Task<Int64> GetCountAsync(
            string columnName = null,
            ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            return GetCountAsync(null, columnName, conn, cancellationToken);
        }

        public async System.Threading.Tasks.Task<Int64> GetCountAsync(
            string schemaName, string columnName,
            ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            object res = await this.ExecuteAggregateAsync(new Phrases.Count(schemaName, columnName, IsDistinct), conn, cancellationToken);
            if (res == null) return 0;
            else return Convert.ToInt64(res);
        }

        #endregion

        #region GetMax Helpers

        public object GetMax(string columnName)
        {
            return GetMax(null, columnName, null);
        }

        public object GetMax(string columnName, ConnectorBase conn)
        {
            return GetMax(null, columnName, conn);
        }

        public object GetMax(string schemaName, string columnName)
        {
            return GetMax(schemaName, columnName, null);
        }

        public object GetMax(string schemaName, string columnName, ConnectorBase conn)
        {
            return this.ExecuteAggregate(new Phrases.Max(schemaName, columnName, IsDistinct), conn);
        }

        public System.Threading.Tasks.Task<object> GetMaxAsync(CancellationToken? cancellationToken)
        {
            return GetMaxAsync(null, null, null, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetMaxAsync(ConnectorBase conn, CancellationToken? cancellationToken = null)
        {
            return GetMaxAsync(null, null, conn, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetMaxAsync(
            string columnName = null,
            ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            return GetMaxAsync(null, columnName, conn, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetMaxAsync(
            string schemaName, string columnName,
            ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            return this.ExecuteAggregateAsync(new Phrases.Max(schemaName, columnName, IsDistinct), conn, cancellationToken);
        }

        #endregion

        #region GetMin Helpers

        public object GetMin(string columnName)
        {
            return GetMin(null, columnName, null);
        }

        public object GetMin(string columnName, ConnectorBase conn)
        {
            return GetMin(null, columnName, conn);
        }

        public object GetMin(string schemaName, string columnName)
        {
            return GetMin(schemaName, columnName, null);
        }

        public object GetMin(string schemaName, string columnName, ConnectorBase conn)
        {
            return this.ExecuteAggregate(new Phrases.Min(schemaName, columnName, IsDistinct), conn);
        }

        public System.Threading.Tasks.Task<object> GetMinAsync(CancellationToken? cancellationToken)
        {
            return GetMinAsync(null, null, null, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetMinAsync(ConnectorBase conn, CancellationToken? cancellationToken = null)
        {
            return GetMinAsync(null, null, conn, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetMinAsync(
            string columnName = null,
            ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            return GetMinAsync(null, columnName, conn, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetMinAsync(
            string schemaName, string columnName,
            ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            return this.ExecuteAggregateAsync(new Phrases.Min(schemaName, columnName, IsDistinct), conn, cancellationToken);
        }

        #endregion

        #region GetSum Helpers

        public object GetSum(string columnName)
        {
            return GetSum(null, columnName, null);
        }

        public object GetSum(string columnName, ConnectorBase conn)
        {
            return GetSum(null, columnName, conn);
        }

        public object GetSum(string schemaName, string columnName)
        {
            return GetSum(schemaName, columnName, null);
        }

        public object GetSum(string schemaName, string columnName, ConnectorBase conn)
        {
            return this.ExecuteAggregate(new Phrases.Sum(schemaName, columnName, IsDistinct), conn);
        }

        public System.Threading.Tasks.Task<object> GetSumAsync(CancellationToken? cancellationToken)
        {
            return GetSumAsync(null, null, null, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetSumAsync(ConnectorBase conn, CancellationToken? cancellationToken = null)
        {
            return GetSumAsync(null, null, conn, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetSumAsync(
            string columnName = null,
            ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            return GetSumAsync(null, columnName, conn, cancellationToken);
        }

        public System.Threading.Tasks.Task<object> GetSumAsync(
            string schemaName, string columnName,
            ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            return this.ExecuteAggregateAsync(new Phrases.Sum(schemaName, columnName, IsDistinct), conn, cancellationToken);
        }

        #endregion
    }
}
