using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SequelNet.Connector;

namespace SequelNet
{
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
                if (conn == null) conn = ConnectorBase.NewInstance();
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

        public virtual async Task SaveAllAsync(ConnectorBase conn = null, bool withTransaction = false, CancellationToken? cancellationToken = null)
        {
            bool ownsConnection = conn == null;
            bool ownsTransaction = false;
            try
            {
                if (conn == null) conn = ConnectorBase.NewInstance();
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

        public Task SaveAllAsync(ConnectorBase conn, CancellationToken? cancellationToken)
        {
            return SaveAllAsync(conn, false, cancellationToken);
        }

        public void SaveAll(bool withTransaction)
        {
            SaveAll(null, withTransaction);
        }

        public Task SaveAllAsync(bool withTransaction, CancellationToken? cancellationToken = null)
        {
            return SaveAllAsync(null, withTransaction, cancellationToken);
        }

        public Task SaveAllAsync(CancellationToken? cancellationToken)
        {
            return SaveAllAsync(null, false, cancellationToken);
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

        public static async Task<TListType> FetchAllAsync(ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            using (var reader = await new Query(AbstractRecord<TItemType>.Schema).ExecuteReaderAsync(conn, cancellationToken).ConfigureAwait(false))
            {
                return await FromReaderAsync(reader, cancellationToken).ConfigureAwait(false);
            }
        }

        public static Task<TListType> FetchAllAsync(CancellationToken? cancellationToken)
        {
            return FetchAllAsync(null, cancellationToken);
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

        public static async Task<TListType> FetchByQueryAsync(Query qry, ConnectorBase conn = null, CancellationToken? cancellationToken = null)
        {
            using (var reader = await qry.ExecuteReaderAsync(conn, cancellationToken).ConfigureAwait(false))
                return await FromReaderAsync(reader, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<TListType> FetchByQueryAsync(Query qry, CancellationToken? cancellationToken)
        {
            using (var reader = await qry.ExecuteReaderAsync(null, cancellationToken).ConfigureAwait(false))
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
