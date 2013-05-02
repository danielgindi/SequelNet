using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using dg.Sql.Bindable;
using dg.Sql.Connector;

namespace dg.Sql
{
    [Serializable]
    public abstract class AbstractRecord<T> where T : AbstractRecord<T>, new()
    {
        protected bool IsThisANewRecord = true;

        private static string __PRIMARY_KEY_NAME = null;
        private static TableSchema __TABLE_SCHEMA = null;

        public AbstractRecord(){}
        public AbstractRecord(object keyValue)
        {
            LoadByKey(keyValue);
        }
        public AbstractRecord(string columnName, object columnValue)
        {
            LoadByParam(columnName, columnValue);
        }

        [HiddenForDataBinding(true), XmlIgnore]
        public static string SchemaPrimaryKeyName
        {
            get
            {
                if (__PRIMARY_KEY_NAME == null)
                {
                    string name = null;
                    int keys = 0;
                    foreach (TableSchema.Column col in TableSchema.Columns)
                    {
                        if (col.IsPrimaryKey) 
                        {
                            keys++;
                            name = col.Name;
                        }
                    }

                    if (keys == 0)
                    {
                        foreach (TableSchema.Index idx in TableSchema.Indexes)
                        {
                            if (idx.Mode == TableSchema.IndexMode.PrimaryKey)
                            {
                                foreach(string nm in idx.ColumnNames)
                                {
                                    keys++;
                                    name = nm;
                                }
                                break;
                            }
                        }
                    }

                    if (keys == 1) __PRIMARY_KEY_NAME = name;
                }
                return __PRIMARY_KEY_NAME;
            }
            set
            {
                __PRIMARY_KEY_NAME = value;
            }
        }
        [HiddenForDataBinding(true), XmlIgnore]
        public static TableSchema TableSchema
        {
            get
            {
                if (__TABLE_SCHEMA == null) __TABLE_SCHEMA = (new T()).GetTableSchema();
                return __TABLE_SCHEMA;
            }
            set
            {
                __TABLE_SCHEMA = value;
            }
        }
        public abstract object GetPrimaryKeyValue();
        public abstract TableSchema GetTableSchema();

        [HiddenForDataBinding(true), XmlIgnore]
        public bool IsNewRecord
        {
            get { return IsThisANewRecord; }
            set { IsThisANewRecord = value; }
        }

        protected string CurrentSessionUserName
        {
            get
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.User != null)
                    return System.Web.HttpContext.Current.User.Identity.Name;
                else return System.Threading.Thread.CurrentPrincipal.Identity.Name;
            }
        }
        
        public void MarkOld()
        {
            IsNewRecord = false;
        }
        public void MarkNew()
        {
            IsNewRecord = true;
        }

        public virtual void Insert() { Insert(null); }
        public virtual void Update() { Update(null); }
        public abstract void Insert(ConnectorBase conn);
        public abstract void Update(ConnectorBase conn);
        public abstract void Read(DataReaderBase reader);
        public virtual void Save()
        {
            if (IsThisANewRecord) Insert(null);
            else Update(null);
        }
        public virtual void Save(ConnectorBase conn)
        {
            if (IsThisANewRecord) Insert(conn);
            else Update(conn);
        }
        public static int Delete(object primaryKey)
        {
            return Delete(primaryKey, null);
        }
        public static int Delete(object primaryKey, ConnectorBase conn)
        {
            string columnName = SchemaPrimaryKeyName;
            if (columnName == null) return 0;
            return DeleteByParameter(columnName, primaryKey, null, conn);
        }
        public static int Delete(string columnName, object value)
        {
            return DeleteByParameter(columnName, value, null, null);
        }
        public static int Delete(string columnName, object value, ConnectorBase conn)
        {
            return DeleteByParameter(columnName, value, null, conn);
        }
        public static int Delete(string columnName, object value, string userName)
        {
            return DeleteByParameter(columnName, value, userName, null);
        }
        public static int Delete(string columnName, object value, string userName, ConnectorBase conn)
        {
            return DeleteByParameter(columnName, value, userName, conn);
        }
        private static int DeleteByParameter(string columnName, object value, string userName, ConnectorBase conn)
        {
            bool flag = TableSchema.Columns.Find(@"Deleted") != null;
            bool flag2 = TableSchema.Columns.Find(@"IsDeleted") != null;
            bool flag3 = TableSchema.Columns.Find(@"ModifiedBy") != null;
            bool flag4 = TableSchema.Columns.Find(@"ModifiedOn") != null;

            string strUpdate = string.Empty;
            if (flag || flag2)
            {
                if (userName == null || userName.Length == 0)
                {
                    if (System.Web.HttpContext.Current != null)
                        userName = System.Web.HttpContext.Current.User.Identity.Name;
                    else userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                }

                Query qry = new Query(TableSchema);

                if (flag) qry.Update(@"Deleted", true);
                if (flag2) qry.Update(@"IsDeleted", true); 
                if (flag3 && !string.IsNullOrEmpty(userName)) qry.Update(@"ModifiedBy", userName);
                if (flag4) qry.Update(@"ModifiedOn", DateTime.UtcNow);

                qry.Where(columnName, value);
                return qry.ExecuteNonQuery();
            }
            return DestroyByParameter(columnName, value, conn);
        }
        public static int Destroy(object primaryKey)
        {
            return Destroy(primaryKey, null);
        }
        public static int Destroy(object primaryKey, ConnectorBase conn)
        {
            string columnName = SchemaPrimaryKeyName;
            if (columnName == null) return 0;
            return DestroyByParameter(columnName, primaryKey, conn);
        }
        public static int Destroy(string columnName, object value)
        {
            return DestroyByParameter(columnName, value, null);
        }
        public static int Destroy(string columnName, object value, ConnectorBase conn)
        {
            return DestroyByParameter(columnName, value, conn);
        }
        private static int DestroyByParameter(string columnName, object value, ConnectorBase conn)
        {
            return new Query(TableSchema).Delete().Where(columnName, value).ExecuteNonQuery(conn);
        }

        public static T FromReader(DataReaderBase reader)
        {
            T item = new T();
            item.Read(reader);
            return item;
        }

        private Type FindColumnType(string columnName)
        {
            foreach (TableSchema.Column col in TableSchema.Columns)
            {
                if (col.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return col.Type;
                }
            }
            return null;
        }
        private DataType FindColumnDataType(string columnName)
        {
            foreach (TableSchema.Column col in TableSchema.Columns)
            {
                if (col.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return col.ActualDataType;
                }
            }
            return DataType.None;
        }

        public static T FetchByID(object keyValue)
        {
            Query qry = new Query(TableSchema);
            qry.Where(SchemaPrimaryKeyName, keyValue);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read()) return FromReader(reader);
            }
            return null;
        }
        public void LoadByKey(object keyValue)
        {
            LoadByParam(SchemaPrimaryKeyName, keyValue);
        }
        public void LoadByParam(string columnName, object columnValue)
        {
            Query qry = new Query(TableSchema);
            qry.Where(columnName, columnValue);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Read(reader);
                    MarkOld();
                }
            }
        }


        // Utility
        protected static string StringOrNullFromDb(object obj)
        {
            if (obj is System.DBNull || obj == null) return null;
            else return (string)obj;
        }
        protected static string StringOrEmptyFromDb(object obj)
        {
            if (obj is System.DBNull || obj == null) return string.Empty;
            else return (string)obj;
        }
        protected static Int32? Int32OrNullFromDb(object obj)
        {
            if (obj is System.DBNull || obj == null) return null;
            else return Convert.ToInt32(obj);
        }
        protected static Int32 Int32OrZero(object obj)
        {
            if (obj is System.DBNull || obj == null) return 0;
            else return Convert.ToInt32(obj);
        }
        protected static Int64? Int64OrNullFromDb(object obj)
        {
            if (obj is System.DBNull || obj == null) return null;
            else return Convert.ToInt64(obj);
        }
        protected static Int64 Int64OrZero(object obj)
        {
            if (obj is System.DBNull || obj == null) return 0;
            else return Convert.ToInt64(obj);
        }
        protected static DateTime DateTimeOrNow(object obj)
        {
            if (obj is System.DBNull || obj == null) return DateTime.UtcNow;
            else return (DateTime)obj;
        }
        protected static DateTime? DateTimeOrNullFromDb(object obj)
        {
            if (obj is System.DBNull || obj == null) return null;
            else return (DateTime)obj;
        }
        protected static decimal? DecimalOrNullFromDb(object obj)
        {
            if (obj is System.DBNull || obj == null) return null;
            else return Convert.ToDecimal(obj);
        }
        protected static decimal DecimalOrZeroFromDb(object obj)
        {
            if (obj is System.DBNull || obj == null) return 0m;
            else return Convert.ToDecimal(obj);
        }
        protected static Guid GuidFromDb(object obj)
        {
            Guid? ret = obj as Guid?;
            if (ret == null) ret = new Guid((string)obj);
            return ret.Value;
        }
        protected static Guid? GuidOrNullFromDb(object obj)
        {
            Guid? ret = obj as Guid?;
            if (ret == null && !IsNull(obj)) ret = new Guid((string)obj);
            return ret.Value;
        }
        protected static bool IsNull(object value)
        {
            if (value == null) return true;
            if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) return true;
            if (value == DBNull.Value) return true;
            else return false;
        }
    }
}
