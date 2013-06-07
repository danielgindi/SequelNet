using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using dg.Sql.Bindable;
using dg.Sql.Connector;

namespace dg.Sql
{
    [Serializable]
    public abstract class AbstractRecordList<TItemType, TListType> : BindableList<TItemType>
        where TItemType : AbstractRecord<TItemType>, new()
        where TListType : AbstractRecordList<TItemType, TListType>, new()
        
    {
        public virtual void SaveAll(ConnectorBase conn)
        {
            foreach (TItemType item in this) item.Save(conn);
        }
        public virtual void SaveAll()
        {
            foreach (TItemType item in this) item.Save();
        }
        public virtual void SaveAll(ConnectorBase conn, bool withTransaction)
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
                foreach (TItemType item in this) item.Save(conn);
                if (ownsTransaction)
                {
                    conn.CommitTransaction();
                    ownsTransaction = false;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
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
        public virtual void SaveAll(bool withTransaction)
        {
            SaveAll(null, withTransaction);
        }
        public static TListType FromReader(DataReaderBase reader)
        {
            TListType coll = new TListType();
            while (reader.Read()) coll.Add(AbstractRecord<TItemType>.FromReader(reader));
            return coll;
        }
        public static TListType FetchAll()
        {
            using (DataReaderBase reader = new Query(AbstractRecord<TItemType>.TableSchema).ExecuteReader())
            {
                return FromReader(reader);
            }
        }
        public static TListType FetchAll(ConnectorBase conn)
        {
            using (DataReaderBase reader = new Query(AbstractRecord<TItemType>.TableSchema).ExecuteReader(conn))
            {
                return FromReader(reader);
            }
        }
        public static TListType Where(string columnName, object columnValue)
        {
            Query qry = new Query(AbstractRecord<TItemType>.TableSchema);
            qry.Where(columnName, columnValue);
            return FetchByQuery(qry);
        }
        public static TListType FetchByQuery(Query qry)
        {
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                return FromReader(reader);
            }
        }
        public static TListType FetchByQuery(Query qry, ConnectorBase conn)
        {
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                return FromReader(reader);
            }
        }
        public TListType Clone()
        {
            TListType coll = new TListType();
            coll.InsertRange(0, this);
            return coll;
        }
    }
}
