using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SequelNet.Connector;

namespace SequelNet
{
    /// <summary>
    /// Provides a base class for a record collection class - which will few utility functions for operating on a collection
    /// </summary>
    /// <typeparam name="TItemType">The name of the record class</typeparam>
    /// <typeparam name="TListType">The name of the collection class</typeparam>
    [Serializable]
    public abstract class AbstractRecordList<TItemType, TListType> 
        : List<TItemType>, IRecordList<TListType>
        where TItemType : AbstractRecord<TItemType>, new()
        where TListType : AbstractRecordList<TItemType, TListType>, new()
    {
        public virtual void SaveAll(ConnectorBase conn = null, bool withTransaction = false)
        {
            bool ownsConnection = conn == null;
            bool ownsTransaction = false;
            try
            {
                if (conn == null) conn = ConnectorBase.Create();
                if (withTransaction && !conn.HasTransaction)
                {
                    ownsTransaction = true;
                    conn.BeginTransaction();
                }

                foreach (TItemType item in this) 
                    item.Save(conn);

                if (ownsTransaction)
                {
                    conn.CommitTransaction();
                    ownsTransaction = false;
                }
            }
            finally
            {
                if (conn != null && ownsConnection)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }

        public virtual void SaveAll(IConnectorFactory factory, bool withTransaction = false)
        {
            SaveAll(factory.Connector(), withTransaction);
        }

        public virtual async Task SaveAllAsync(ConnectorBase conn = null, bool withTransaction = false, CancellationToken? cancellationToken = null)
        {
            bool ownsConnection = conn == null;
            bool ownsTransaction = false;
            try
            {
                if (conn == null) conn = ConnectorBase.Create();
                if (withTransaction && !conn.HasTransaction)
                {
                    ownsTransaction = true;
                    conn.BeginTransaction();
                }

                foreach (TItemType item in this)
                    await item.SaveAsync(conn, cancellationToken).ConfigureAwait(false);

                if (ownsTransaction)
                {
                    conn.CommitTransaction();
                    ownsTransaction = false;
                }
            }
            finally
            {
                if (conn != null && ownsConnection)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }

        public virtual Task SaveAllAsync(IConnectorFactory factory, bool withTransaction = false, CancellationToken? cancellationToken = null)
        {
            return SaveAllAsync(factory.Connector(), withTransaction, cancellationToken);
        }

        public Task SaveAllAsync(ConnectorBase conn, CancellationToken? cancellationToken)
        {
            return SaveAllAsync(conn, false, cancellationToken);
        }

        public Task SaveAllAsync(IConnectorFactory factory, CancellationToken? cancellationToken)
        {
            return SaveAllAsync(factory.Connector(), false, cancellationToken);
        }

        public void SaveAll(bool withTransaction)
        {
            SaveAll((ConnectorBase)null, withTransaction);
        }

        public Task SaveAllAsync(bool withTransaction, CancellationToken? cancellationToken = null)
        {
            return SaveAllAsync((ConnectorBase)null, withTransaction, cancellationToken);
        }

        public Task SaveAllAsync(CancellationToken? cancellationToken)
        {
            return SaveAllAsync((ConnectorBase)null, false, cancellationToken);
        }

        public static TListType FromReader(DataReader reader)
        {
            TListType coll = new TListType();

            while (reader.Read())
                coll.Add(AbstractRecord<TItemType>.FromReader(reader));

            return coll;
        }

        public static async Task<TListType> FromReaderAsync(DataReader reader, CancellationToken? cancellationToken = null)
        {
            TListType coll = new TListType();
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                coll.Add(AbstractRecord<TItemType>.FromReader(reader));
            return coll;
        }

        public static TListType FetchAll(ConnectorBase conn = null)
        {
            using (var reader = new Query(AbstractRecord<TItemType>.Schema).ExecuteReader(conn))
            {
                return FromReader(reader);
            }
        }

        public static TListType FetchAll(IConnectorFactory factory)
        {
            using (var reader = new Query(AbstractRecord<TItemType>.Schema).ExecuteReader(factory.Connector()))
            {
                return FromReader(reader);
            }
        }

        public static async Task<TListType> FetchAllAsync(ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            using (var reader = await new Query(AbstractRecord<TItemType>.Schema).ExecuteReaderAsync(conn, cancellationToken).ConfigureAwait(false))
            {
                return await FromReaderAsync(reader, cancellationToken).ConfigureAwait(false);
            }
        }

        public static Task<TListType> FetchAllAsync(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return FetchAllAsync(factory.Connector(), cancellationToken);
        }

        public static Task<TListType> FetchAllAsync(CancellationToken? cancellationToken)
        {
            return FetchAllAsync((ConnectorBase)null, cancellationToken);
        }

        public static TListType Where(string columnName, object columnValue)
        {
            Query qry = new Query(AbstractRecord<TItemType>.Schema);
            qry.Where(columnName, columnValue);
            return FetchByQuery(qry);
        }

        public static TListType FetchByQuery(Query qry, ConnectorBase conn = null)
        {
            using (var reader = qry.ExecuteReader(conn))
                return FromReader(reader);
        }

        public static TListType FetchByQuery(Query qry, IConnectorFactory factory)
        {
            using (var reader = qry.ExecuteReader(factory.Connector()))
                return FromReader(reader);
        }

        public static async Task<TListType> FetchByQueryAsync(Query qry, ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            using (var reader = await qry.ExecuteReaderAsync(conn, cancellationToken).ConfigureAwait(false))
                return await FromReaderAsync(reader, cancellationToken).ConfigureAwait(false);
        }

        public static Task<TListType> FetchByQueryAsync(Query qry, IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return FetchByQueryAsync(qry, factory.Connector(), cancellationToken);
        }

        public static async Task<TListType> FetchByQueryAsync(Query qry, CancellationToken? cancellationToken)
        {
            using (var reader = await qry.ExecuteReaderAsync((ConnectorBase)null, cancellationToken).ConfigureAwait(false))
                return await FromReaderAsync(reader, cancellationToken).ConfigureAwait(false);
        }

        public TListType Clone()
        {
            TListType coll = new TListType();
            coll.InsertRange(0, this);
            return coll;
        }
    }
}
