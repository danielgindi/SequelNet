using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using dg.Sql.Bindable;
using dg.Sql.Connector;
using System.Collections;
using System.Reflection;

namespace dg.Sql
{
    [Serializable]
    public abstract class AbstractRecord<T> where T : AbstractRecord<T>, new()
    {
        #region Static private variables, caches

        private static bool __LOOKED_FOR_PRIMARY_KEY_NAME = false;
        private static bool __PRIMARY_KEY_MULTI = false;
        private static object __PRIMARY_KEY_NAME = null;
        private static TableSchema __TABLE_SCHEMA = null;
        private static Type __CLASS_TYPE = null;

        private static bool __FLAGS_RETRIEVED = false;
        private static bool __HAS_DELETED = false;
        private static bool __HAS_IS_DELETED = false;
        private static bool __HAS_CREATED_BY = false;
        private static bool __HAS_CREATED_ON= false;
        private static bool __HAS_MODIFIED_BY = false;
        private static bool __HAS_MODIFIED_ON = false;

        #endregion

        #region Private variables

        private static bool _AtomicUpdates = false;
        private HashSet<string> _DirtyColumns = null;

        #endregion

        #region Protected variables

        private bool _NewRecord = true;

        #endregion

        #region Constructors

        public AbstractRecord() { }
        public AbstractRecord(object KeyValue)
        {
            LoadByKey(KeyValue);
        }
        public AbstractRecord(string ColumnName, object ColumnValue)
        {
            LoadByParam(ColumnName, ColumnValue);
        }

        #endregion

        #region Table schema stuff

        /// <summary>
        /// Re-sets a cached version of the primary key names of this record. 
        /// This is automatically called the first time that a primary-key-related action is called.
        /// </summary>
        public static void CachePrimaryKeyName()
        {
            List<string> keyNames = new List<string>();

            foreach (TableSchema.Column col in TableSchema.Columns)
            {
                if (col.IsPrimaryKey)
                {
                    keyNames.Add(col.Name);
                }
            }

            if (keyNames.Count == 0)
            {
                foreach (TableSchema.Index idx in TableSchema.Indexes)
                {
                    if (idx.Mode == TableSchema.IndexMode.PrimaryKey)
                    {
                        keyNames.AddRange(idx.ColumnNames);
                        break;
                    }
                }
            }

            if (keyNames.Count == 1)
            {
                __PRIMARY_KEY_NAME = keyNames[0];
            }
            else if (keyNames.Count > 1)
            {
                __PRIMARY_KEY_NAME = keyNames.ToArray();
            }

            __PRIMARY_KEY_MULTI = keyNames.Count > 1;

            __LOOKED_FOR_PRIMARY_KEY_NAME = true;
        }

        public abstract object GetPrimaryKeyValue();

        public abstract TableSchema GetTableSchema();

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

        /// <summary>
        /// The primary key name for this record's schema.
        /// It is found automatically and cached.
        /// Could be either a String, an <typeparamref name="Array&lt;String&gt;"/> or <value>null</value>.
        /// </summary>
        [HiddenForDataBinding(true), XmlIgnore]
        public static object SchemaPrimaryKeyName
        {
            get
            {
                if (!__LOOKED_FOR_PRIMARY_KEY_NAME)
                {
                    CachePrimaryKeyName();
                }
                return __PRIMARY_KEY_NAME;
            }
            set
            {
                if (value is ICollection)
                { // Make sure this is an Array!
                    List<string> keys = new List<string>();
                    foreach (string key in (IEnumerable)__PRIMARY_KEY_NAME)
                    {
                        keys.Add(key);
                    }
                    value = keys.ToArray();
                }
                __PRIMARY_KEY_NAME = value;
                __PRIMARY_KEY_MULTI = __PRIMARY_KEY_NAME is ICollection;
                
                __LOOKED_FOR_PRIMARY_KEY_NAME = true;
            }
        }

        private static void RetrieveFlags()
        {
            __HAS_DELETED = TableSchema.Columns.Find(@"Deleted") != null;
            __HAS_IS_DELETED = TableSchema.Columns.Find(@"IsDeleted") != null;
            __HAS_CREATED_BY = TableSchema.Columns.Find(@"CreatedBy") != null;
            __HAS_CREATED_ON = TableSchema.Columns.Find(@"CreatedOn") != null;
            __HAS_MODIFIED_BY = TableSchema.Columns.Find(@"ModifiedBy") != null;
            __HAS_MODIFIED_ON = TableSchema.Columns.Find(@"ModifiedOn") != null;
            __FLAGS_RETRIEVED = true;
        }

        #endregion

        #region Dirty

        [HiddenForDataBinding(true), XmlIgnore]
        public bool IsNewRecord
        {
            get { return _NewRecord; }
            set { _NewRecord = value; }
        }

        // Deprecate this
        [HiddenForDataBinding(true), XmlIgnore]
        public bool IsThisANewRecord
        {
            get { return _NewRecord; }
            set { _NewRecord = value; }
        }

        [HiddenForDataBinding(true), XmlIgnore]
        public static bool AtomicUpdates
        {
            get { return _AtomicUpdates; }
            set { _AtomicUpdates = value; }
        }

        public void MarkColumnDirty(string column)
        {
            if (_DirtyColumns == null)
            {
                _DirtyColumns = new HashSet<string>();
            }

            _DirtyColumns.Add(column);
        }

        public void MarkColumnNotDirty(string column)
        {
            if (_DirtyColumns == null) return;

            _DirtyColumns.Remove(column);
        }

        public void MarkAllColumnsNotDirty()
        {
            if (_DirtyColumns == null) return;

            _DirtyColumns.Clear();
        }

        public bool IsColumnDirty(string column)
        {
            return _DirtyColumns != null && _DirtyColumns.Contains(column);
        }

        public bool HasDirtyColumns()
        {
            return _DirtyColumns != null && _DirtyColumns.Count > 0;
        }
        
        public void MarkOld()
        {
            IsNewRecord = false;
        }
        public void MarkNew()
        {
            IsNewRecord = true;
        }

        #endregion

        #region Virtual Read/Write Actions

        public virtual void Insert() { Insert(null); }
        public virtual void Update() { Update(null); }

        public virtual void Insert(ConnectorBase Connection)
        {
            if (!__FLAGS_RETRIEVED)
            {
                RetrieveFlags();
            }
            if (__CLASS_TYPE == null)
            {
                __CLASS_TYPE = this.GetType();
            }

            Query qry = new Query(TableSchema);

            PropertyInfo propInfo;
            foreach (TableSchema.Column Column in TableSchema.Columns)
            {
                propInfo = __CLASS_TYPE.GetProperty(Column.Name);
                if (propInfo == null) propInfo = __CLASS_TYPE.GetProperty(Column.Name + @"X");
                if (propInfo != null)
                {
                    qry.Insert(Column.Name, propInfo.GetValue(this, null));
                }
            }

            if (__HAS_CREATED_BY)
            {
                string userName = null;
                if (System.Web.HttpContext.Current != null)
                {
                    userName = System.Web.HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                }

                if (userName == null || userName.Length == 0)
                {
                    propInfo = __CLASS_TYPE.GetProperty(@"CreatedBy");
                    if (propInfo != null) userName = propInfo.GetValue(this, null) as string;
                }
                if (userName != null)
                {
                    qry.Insert(@"CreatedBy", userName);
                }
            }

            if (__HAS_CREATED_ON)
            {
                qry.Insert(@"CreatedOn", DateTime.UtcNow);
            }

            qry.Execute(Connection);

            _NewRecord = false;
            MarkAllColumnsNotDirty();
        }

        public virtual void Update(ConnectorBase Connection)
        {
            if (!__FLAGS_RETRIEVED)
            {
                RetrieveFlags();
            }
            if (__CLASS_TYPE == null)
            {
                __CLASS_TYPE = this.GetType();
            }

            object primaryKey = SchemaPrimaryKeyName;
            bool isPrimaryKeyNullOrString = primaryKey == null || primaryKey is string;

            Query qry = new Query(TableSchema);

            PropertyInfo propInfo;
            foreach (TableSchema.Column Column in TableSchema.Columns)
            {
                if ((isPrimaryKeyNullOrString && Column.Name == (string)primaryKey) ||
                    (!isPrimaryKeyNullOrString && StringArrayContains((string[])primaryKey, Column.Name))) continue;

                propInfo = __CLASS_TYPE.GetProperty(Column.Name);
                if (propInfo == null) propInfo = __CLASS_TYPE.GetProperty(Column.Name + @"X");
                if (propInfo != null)
                {
                    if (_AtomicUpdates && !IsColumnDirty(Column.Name)) continue;

                    qry.Update(Column.Name, propInfo.GetValue(this, null));
                }
            }

            if (!_AtomicUpdates || qry.HasInsertsOrUpdates)
            {
                if (__HAS_MODIFIED_BY)
                {
                    string userName = null;
                    if (System.Web.HttpContext.Current != null)
                    {
                        userName = System.Web.HttpContext.Current.User.Identity.Name;
                    }
                    else
                    {
                        userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                    }

                    if (userName == null || userName.Length == 0)
                    {
                        propInfo = __CLASS_TYPE.GetProperty(@"ModifiedBy");
                        if (propInfo != null) userName = propInfo.GetValue(this, null) as string;
                    }
                    if (userName != null)
                    {
                        qry.Update(@"ModifiedBy", userName);
                    }
                }

                if (__HAS_MODIFIED_ON)
                {
                    qry.Update(@"ModifiedOn", DateTime.UtcNow);
                }
            }

            if (qry.HasInsertsOrUpdates)
            {
                qry.Execute(Connection);
            }

            MarkAllColumnsNotDirty();
        }

        public virtual void Read(DataReaderBase Reader)
        {
            if (__CLASS_TYPE == null)
            {
                __CLASS_TYPE = this.GetType();
            }

            PropertyInfo propInfo;
            foreach (TableSchema.Column Column in TableSchema.Columns)
            {
                propInfo = __CLASS_TYPE.GetProperty(Column.Name);
                if (propInfo == null) propInfo = __CLASS_TYPE.GetProperty(Column.Name + @"X");
                if (propInfo != null)
                {
                    propInfo.SetValue(this, Convert.ChangeType(Reader[Column.Name], Column.Type), null);
                }
            }

            _NewRecord = false;
            MarkAllColumnsNotDirty();
        }

        public virtual void Save()
        {
            if (_NewRecord)
            {
                Insert(null);
            }
            else
            {
                Update(null);
            }
        }

        public virtual void Save(ConnectorBase Connection)
        {
            if (_NewRecord)
            {
                Insert(Connection);
            }
            else
            {
                Update(Connection);
            }
        }

        #endregion

        #region Deleting a record

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="PrimaryKeyValue"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedBy column, it will be updated with the current identified Username.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="PrimaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Delete(object PrimaryKeyValue, ConnectorBase Connection = null)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return 0;
            return DeleteByParameter(columnName, PrimaryKeyValue, null, Connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="ColumnName"/> to <paramref name="Value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedBy column, it will be updated with the current identified Username.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="ColumnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="Value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="ColumnName"/></param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Delete(string ColumnName, object Value, ConnectorBase Connection = null)
        {
            return DeleteByParameter(ColumnName, Value, null, Connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="ColumnName"/> to <paramref name="Value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedBy column, it will be updated with <paramref name="UserName"/> or the current identified Username.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="ColumnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="Value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="ColumnName"/></param>
        /// <param name="UserName">An optional username to use if updating a ModifiedBy column.</param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Delete(string ColumnName, object Value, string UserName, ConnectorBase Connection = null)
        {
            return DeleteByParameter(ColumnName, Value, UserName, Connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="ColumnName"/> to <paramref name="Value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedBy column, it will be updated with the current identified Username.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="ColumnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="Value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="ColumnName"/></param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        private static int DeleteByParameter(object ColumnName, object Value, string UserName, ConnectorBase Connection)
        {
            if (!__FLAGS_RETRIEVED)
            {
                RetrieveFlags();
            }

            string strUpdate = string.Empty;
            if (__HAS_DELETED || __HAS_IS_DELETED)
            {
                if (UserName == null || UserName.Length == 0)
                {
                    if (System.Web.HttpContext.Current != null)
                        UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    else UserName = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                }

                Query qry = new Query(TableSchema);

                if (__HAS_DELETED) qry.Update(@"Deleted", true);
                if (__HAS_IS_DELETED) qry.Update(@"IsDeleted", true);
                if (__HAS_MODIFIED_BY && !string.IsNullOrEmpty(UserName)) qry.Update(@"ModifiedBy", UserName);
                if (__HAS_MODIFIED_ON) qry.Update(@"ModifiedOn", DateTime.UtcNow);

                if (ColumnName is ICollection)
                {
                    if (!(Value is ICollection)) return 0;

                    IEnumerator keyEnumerator = ((IEnumerable)ColumnName).GetEnumerator();
                    IEnumerator valueEnumerator = ((IEnumerable)Value).GetEnumerator();

                    while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                    {
                        qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                    }
                }
                else
                {
                    qry.Where((string)ColumnName, Value);
                }
                return qry.ExecuteNonQuery(Connection);
            }
            return DestroyByParameter(ColumnName, Value, Connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="PrimaryKeyValue"/>.
        /// </summary>
        /// <param name="PrimaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Destroy(object PrimaryKeyValue, ConnectorBase Connection = null)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return 0;
            return DestroyByParameter(columnName, PrimaryKeyValue, Connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="ColumnName"/> to <paramref name="Value"/>.
        /// </summary>
        /// <param name="ColumnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="Value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="ColumnName"/></param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Destroy(string ColumnName, object Value, ConnectorBase Connection = null)
        {
            return DestroyByParameter(ColumnName, Value, Connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="ColumnName"/> to <paramref name="Value"/>.
        /// </summary>
        /// <param name="ColumnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="Value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="ColumnName"/></param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        private static int DestroyByParameter(object ColumnName, object Value, ConnectorBase Connection)
        {
            Query qry = new Query(TableSchema).Delete();

            if (ColumnName is ICollection)
            {
                if (!(Value is ICollection)) return 0;

                IEnumerator keyEnumerator = ((IEnumerable)ColumnName).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)Value).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)ColumnName, Value);
            }

            return qry.ExecuteNonQuery(Connection);
        }

        #endregion

        #region Column utilities

        private Type FindColumnType(string ColumnName)
        {
            foreach (TableSchema.Column col in TableSchema.Columns)
            {
                if (col.Name.Equals(ColumnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return col.Type;
                }
            }
            return null;
        }

        private DataType FindColumnDataType(string ColumnName)
        {
            foreach (TableSchema.Column col in TableSchema.Columns)
            {
                if (col.Name.Equals(ColumnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return col.ActualDataType;
                }
            }
            return DataType.None;
        }
        
        #endregion

        #region Loading utilities

        /// <summary>
        /// Fetches a record from the db, by matching the Primary Key to <paramref name="PrimaryKeyValue"/>.
        /// </summary>
        /// <param name="PrimaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        /// <returns>A record (marked as "old") or null.</returns>
        public static T FetchByID(object PrimaryKeyValue, ConnectorBase Connection = null)
        {
            Query qry = new Query(TableSchema);

            object primaryKey = SchemaPrimaryKeyName;
            if (__PRIMARY_KEY_MULTI)
            {
                if (!(PrimaryKeyValue is ICollection)) return null;

                IEnumerator keyEnumerator = ((IEnumerable)primaryKey).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)PrimaryKeyValue).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)primaryKey, PrimaryKeyValue);
            }

            using (DataReaderBase reader = qry.ExecuteReader(Connection))
            {
                if (reader.Read()) return FromReader(reader);
            }
            return null;
        }

        /// <summary>
        /// Loads a record from the db, by matching the Primary Key to <paramref name="PrimaryKeyValue"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="PrimaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        public void LoadByKey(object PrimaryKeyValue, ConnectorBase Connection = null)
        {
            LoadByParam(SchemaPrimaryKeyName, PrimaryKeyValue, Connection);
        }
        
        /// <summary>
        /// Loads a record from the db, by matching <paramref name="ColumnName"/> to <paramref name="Value"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="ColumnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="Value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="ColumnName"/></param>
        /// <param name="Connection">An optional db connection to use when executing the query.</param>
        public void LoadByParam(object ColumnName, object Value, ConnectorBase Connection = null)
        {
            Query qry = new Query(TableSchema);

            if (ColumnName is ICollection)
            {
                if (!(Value is ICollection)) return;

                IEnumerator keyEnumerator = ((IEnumerable)ColumnName).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)Value).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)ColumnName, Value);
            }

            using (DataReaderBase reader = qry.ExecuteReader(Connection))
            {
                if (reader.Read())
                {
                    Read(reader);
                    MarkOld();
                }
            }
        }

        /// <summary>
        /// Creates a new instance of this record, and loads it from the <paramref name="Reader"/>.
        /// Will me marked as "old".
        /// </summary>
        /// <param name="Reader">The reader to use for loading the new record</param>
        /// <returns>The new <typeparamref name="T"/>.</returns>
        public static T FromReader(DataReaderBase Reader)
        {
            T item = new T();
            item.Read(Reader);
            return item;
        }

        #endregion

        #region Utilities for loading from db

        protected static string StringOrNullFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return null;
            else return (string)Value;
        }

        protected static string StringOrEmptyFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return string.Empty;
            else return (string)Value;
        }

        protected static Int32? Int32OrNullFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return null;
            else return Convert.ToInt32(Value);
        }

        protected static Int32 Int32OrZero(object Value)
        {
            if (Value is System.DBNull || Value == null) return 0;
            else return Convert.ToInt32(Value);
        }

        protected static Int64? Int64OrNullFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return null;
            else return Convert.ToInt64(Value);
        }

        protected static Int64 Int64OrZero(object Value)
        {
            if (Value is System.DBNull || Value == null) return 0;
            else return Convert.ToInt64(Value);
        }

        protected static decimal? DecimalOrNullFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return null;
            else return Convert.ToDecimal(Value);
        }

        protected static decimal DecimalOrZeroFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return 0m;
            else return Convert.ToDecimal(Value);
        }

        protected static float? FloatOrNullFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return null;
            else return Convert.ToSingle(Value);
        }

        protected static float FloatOrZeroFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return 0f;
            else return Convert.ToSingle(Value);
        }

        protected static double? DoubleOrNullFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return null;
            else return Convert.ToDouble(Value);
        }

        protected static double DoubleOrZeroFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return 0.0;
            else return Convert.ToDouble(Value);
        }

        protected static DateTime? DateTimeOrNullFromDb(object Value)
        {
            if (Value is System.DBNull || Value == null) return null;
            else return (DateTime)Value;
        }

        protected static DateTime DateTimeOrNow(object Value)
        {
            if (Value is System.DBNull || Value == null) return DateTime.UtcNow;
            else return (DateTime)Value;
        }

        protected static DateTime DateTimeOrMinValue(object Value)
        {
            if (Value is System.DBNull || Value == null) return DateTime.MinValue;
            else return (DateTime)Value;
        }

        protected static Guid GuidFromDb(object Value)
        {
            Guid? ret = Value as Guid?;
            if (ret == null) ret = new Guid((string)Value);
            return ret.Value;
        }

        protected static Guid? GuidOrNullFromDb(object Value)
        {
            Guid? ret = Value as Guid?;
            if (ret == null && !IsNull(Value)) ret = new Guid((string)Value);
            return ret.Value;
        }

        protected static bool IsNull(object Value)
        {
            if (Value == null) return true;
            if (Value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)Value).IsNull) return true;
            if (Value == DBNull.Value) return true;
            else return false;
        }

        #endregion

        #region Helpers

        private static bool StringArrayContains(string[] Array, string Item)
        {
            foreach (string item in Array)
            {
                if (item == Item) return true;
            }
            return false;
        }

        protected string CurrentSessionUserName
        {
            get
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.User != null)
                {
                    return System.Web.HttpContext.Current.User.Identity.Name;
                }
                else
               {
                    return System.Threading.Thread.CurrentPrincipal.Identity.Name;
                }
            }
        }

        #endregion
    }
}
