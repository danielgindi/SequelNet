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
    public abstract class AbstractRecordList<ItemType, ListType> : BindableList<ItemType>
        where ItemType : AbstractRecord<ItemType>, new()
        where ListType : AbstractRecordList<ItemType, ListType>, new()
        
    {
        public virtual void SaveAll(ConnectorBase conn)
        {
            foreach (ItemType item in this) item.Save(conn);
        }
        public virtual void SaveAll()
        {
            foreach (ItemType item in this) item.Save();
        }
        public virtual void SaveAll(ConnectorBase conn, bool withTransaction)
        {
            bool ownsConnection = conn == null;
            bool ownsTransaction = false;
            try
            {
                if (conn == null) conn = ConnectorBase.NewInstance();
                if (withTransaction && !conn.hasTransaction)
                {
                    ownsTransaction = true;
                    conn.beginTransaction();
                }
                foreach (ItemType item in this) item.Save(conn);
                if (ownsTransaction)
                {
                    conn.commitTransaction();
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
        public static ListType FromReader(DataReaderBase reader)
        {
            ListType coll = new ListType();
            while (reader.Read()) coll.Add(AbstractRecord<ItemType>.FromReader(reader));
            return coll;
        }
        public static ListType FetchAll()
        {
            using (DataReaderBase reader = new Query(AbstractRecord<ItemType>.TableSchema).ExecuteReader())
            {
                return FromReader(reader);
            }
        }
        public static ListType FetchAll(ConnectorBase conn)
        {
            using (DataReaderBase reader = new Query(AbstractRecord<ItemType>.TableSchema).ExecuteReader(conn))
            {
                return FromReader(reader);
            }
        }
        public static ListType Where(string columnName, object columnValue)
        {
            Query qry = new Query(AbstractRecord<ItemType>.TableSchema);
            qry.Where(columnName, columnValue);
            return FetchByQuery(qry);
        }
        public static ListType FetchByQuery(Query qry)
        {
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                return FromReader(reader);
            }
        }
        public static ListType FetchByQuery(Query qry, ConnectorBase conn)
        {
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                return FromReader(reader);
            }
        }
        public ListType Clone()
        {
            ListType coll = new ListType();
            coll.InsertRange(0, this);
            return coll;
        }
    }
}
