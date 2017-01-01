using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
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
        private HashSet<string> _MutatedColumns = null;

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

        [XmlIgnore]
        public static TableSchema Schema
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
        /// Synonym for <see cref="Schema"/>
        /// </summary>
        [XmlIgnore]
        public static TableSchema TableSchema
        {
            get
            {
                return Schema;
            }
            set
            {
                Schema = value;
            }
        }

        [XmlIgnore]
        public static string SchemaName
        {
            get
            {
                return TableSchema.Name;
            }
        }

        /// <summary>
        /// The primary key name for this record's schema.
        /// It is found automatically and cached.
        /// Could be either a String, an <typeparamref name="Array&lt;String&gt;"/> or <value>null</value>.
        /// </summary>
        [XmlIgnore]
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

        #region Mutated

        public bool IsNewRecord
        {
            get { return _NewRecord; }
            set { _NewRecord = value; }
        }

        // Deprecate this
        public bool IsThisANewRecord
        {
            get { return _NewRecord; }
            set { _NewRecord = value; }
        }
        
        public static bool AtomicUpdates
        {
            get { return _AtomicUpdates; }
            set { _AtomicUpdates = value; }
        }

        public virtual void MarkColumnMutated(string column)
        {
            if (_MutatedColumns == null)
            {
                _MutatedColumns = new HashSet<string>();
            }

            _MutatedColumns.Add(column);
        }

        public virtual void MarkColumnNotMutated(string column)
        {
            if (_MutatedColumns == null) return;

            _MutatedColumns.Remove(column);
        }

        public virtual void MarkAllColumnsNotMutated()
        {
            if (_MutatedColumns == null) return;

            _MutatedColumns.Clear();
        }

        public virtual bool IsColumnMutated(string column)
        {
            return _MutatedColumns != null && _MutatedColumns.Contains(column);
        }

        public virtual bool HasMutatedColumns()
        {
            return _MutatedColumns != null && _MutatedColumns.Count > 0;
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

        public virtual void Insert(ConnectorBase connection)
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

            qry.Execute(connection);

            _NewRecord = false;
            MarkAllColumnsNotMutated();
        }

        public virtual void Update(ConnectorBase connection)
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
                    if (_AtomicUpdates && !IsColumnMutated(Column.Name)) continue;

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
                qry.Execute(connection);
            }

            MarkAllColumnsNotMutated();
        }

        public virtual void Read(DataReaderBase reader)
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
                    propInfo.SetValue(this, Convert.ChangeType(reader[Column.Name], Column.Type), null);
                }
            }

            _NewRecord = false;
            MarkAllColumnsNotMutated();
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

        public virtual void Save(ConnectorBase connection)
        {
            if (_NewRecord)
            {
                Insert(connection);
            }
            else
            {
                Update(connection);
            }
        }

        #endregion

        #region Deleting a record

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedBy column, it will be updated with the current identified Username.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Delete(object primaryKeyValue, ConnectorBase connection = null)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return 0;
            return DeleteByParameter(columnName, primaryKeyValue, null, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedBy column, it will be updated with the current identified Username.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Delete(string columnName, object value, ConnectorBase connection = null)
        {
            return DeleteByParameter(columnName, value, null, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedBy column, it will be updated with <paramref name="userName"/> or the current identified Username.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="userName">An optional username to use if updating a ModifiedBy column.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Delete(string columnName, object value, string userName, ConnectorBase connection = null)
        {
            return DeleteByParameter(columnName, value, userName, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the table has a Deleted or IsDeleted column, it will be marked instead of actually deleted.
        /// If the table has a ModifiedBy column, it will be updated with the current identified Username.
        /// If the table has a ModifiedOn column, it will be updated with the current UTC date/time.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        private static int DeleteByParameter(object columnName, object value, string userName, ConnectorBase connection)
        {
            if (!__FLAGS_RETRIEVED)
            {
                RetrieveFlags();
            }

            string strUpdate = string.Empty;
            if (__HAS_DELETED || __HAS_IS_DELETED)
            {
                if (userName == null || userName.Length == 0)
                {
                    if (System.Web.HttpContext.Current != null)
                        userName = System.Web.HttpContext.Current.User.Identity.Name;
                    else userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                }

                Query qry = new Query(TableSchema);

                if (__HAS_DELETED) qry.Update(@"Deleted", true);
                if (__HAS_IS_DELETED) qry.Update(@"IsDeleted", true);
                if (__HAS_MODIFIED_BY && !string.IsNullOrEmpty(userName)) qry.Update(@"ModifiedBy", userName);
                if (__HAS_MODIFIED_ON) qry.Update(@"ModifiedOn", DateTime.UtcNow);

                if (columnName is ICollection)
                {
                    if (!(value is ICollection)) return 0;

                    IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                    IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                    while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                    {
                        qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                    }
                }
                else
                {
                    qry.Where((string)columnName, value);
                }
                return qry.ExecuteNonQuery(connection);
            }
            return DestroyByParameter(columnName, value, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Destroy(object primaryKeyValue, ConnectorBase connection = null)
        {
            object columnName = SchemaPrimaryKeyName;
            if (columnName == null) return 0;
            return DestroyByParameter(columnName, primaryKeyValue, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Destroy(string columnName, object value, ConnectorBase connection = null)
        {
            return DestroyByParameter(columnName, value, connection);
        }

        /// <summary>
        /// Deletes a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>Number of affected rows.</returns>
        private static int DestroyByParameter(object columnName, object value, ConnectorBase connection)
        {
            Query qry = new Query(TableSchema).Delete();

            if (columnName is ICollection)
            {
                if (!(value is ICollection)) return 0;

                IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)columnName, value);
            }

            return qry.ExecuteNonQuery(connection);
        }

        #endregion

        #region Column utilities

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
        
        #endregion

        #region Loading utilities

        /// <summary>
        /// Fetches a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        /// <returns>A record (marked as "old") or null.</returns>
        public static T FetchByID(object primaryKeyValue, ConnectorBase connection = null)
        {
            Query qry = new Query(TableSchema);

            object primaryKey = SchemaPrimaryKeyName;
            if (__PRIMARY_KEY_MULTI)
            {
                if (!(primaryKeyValue is ICollection)) return null;

                IEnumerator keyEnumerator = ((IEnumerable)primaryKey).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)primaryKeyValue).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)primaryKey, primaryKeyValue);
            }

            using (DataReaderBase reader = qry.ExecuteReader(connection))
            {
                if (reader.Read()) return FromReader(reader);
            }
            return null;
        }

        /// <summary>
        /// Loads a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        public void LoadByKey(object primaryKeyValue, ConnectorBase connection = null)
        {
            LoadByParam(SchemaPrimaryKeyName, primaryKeyValue, connection);
        }
        
        /// <summary>
        /// Loads a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        public void LoadByParam(object columnName, object value, ConnectorBase connection = null)
        {
            Query qry = new Query(TableSchema);

            if (columnName is ICollection)
            {
                if (!(value is ICollection)) return;

                IEnumerator keyEnumerator = ((IEnumerable)columnName).GetEnumerator();
                IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();

                while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
                {
                    qry.AND((string)keyEnumerator.Current, valueEnumerator.Current);
                }
            }
            else
            {
                qry.Where((string)columnName, value);
            }

            using (DataReaderBase reader = qry.ExecuteReader(connection))
            {
                if (reader.Read())
                {
                    Read(reader);
                    MarkOld();
                }
            }
        }

        /// <summary>
        /// Creates a new instance of this record, and loads it from the <paramref name="reader"/>.
        /// Will me marked as "old".
        /// </summary>
        /// <param name="reader">The reader to use for loading the new record</param>
        /// <returns>The new <typeparamref name="T"/>.</returns>
        public static T FromReader(DataReaderBase reader)
        {
            T item = new T();
            item.Read(reader);
            return item;
        }

        #endregion

        #region Utilities for loading from db

        protected static string StringOrNullFromDb(object value)
        {
            if (value is System.DBNull || value == null) return null;
            else return (string)value;
        }

        protected static string StringOrEmptyFromDb(object value)
        {
            if (value is System.DBNull || value == null) return string.Empty;
            else return (string)value;
        }

        protected static Int32? Int32OrNullFromDb(object value)
        {
            if (value is System.DBNull || value == null) return null;
            else return Convert.ToInt32(value);
        }

        protected static Int32 Int32OrZero(object value)
        {
            if (value is System.DBNull || value == null) return 0;
            else return Convert.ToInt32(value);
        }

        protected static Int64? Int64OrNullFromDb(object value)
        {
            if (value is System.DBNull || value == null) return null;
            else return Convert.ToInt64(value);
        }

        protected static Int64 Int64OrZero(object value)
        {
            if (value is System.DBNull || value == null) return 0;
            else return Convert.ToInt64(value);
        }

        protected static decimal? DecimalOrNullFromDb(object value)
        {
            if (value is System.DBNull || value == null) return null;
            else return Convert.ToDecimal(value);
        }

        protected static decimal DecimalOrZeroFromDb(object value)
        {
            if (value is System.DBNull || value == null) return 0m;
            else return Convert.ToDecimal(value);
        }

        protected static float? FloatOrNullFromDb(object value)
        {
            if (value is System.DBNull || value == null) return null;
            else return Convert.ToSingle(value);
        }

        protected static float FloatOrZeroFromDb(object value)
        {
            if (value is System.DBNull || value == null) return 0f;
            else return Convert.ToSingle(value);
        }

        protected static double? DoubleOrNullFromDb(object value)
        {
            if (value is System.DBNull || value == null) return null;
            else return Convert.ToDouble(value);
        }

        protected static double DoubleOrZeroFromDb(object value)
        {
            if (value is System.DBNull || value == null) return 0.0;
            else return Convert.ToDouble(value);
        }

        protected static DateTime? DateTimeOrNullFromDb(object value)
        {
            if (value is System.DBNull || value == null) return null;
            else return (DateTime)value;
        }

        protected static DateTime DateTimeOrNow(object value)
        {
            if (value is System.DBNull || value == null) return DateTime.UtcNow;
            else return (DateTime)value;
        }

        protected static DateTime DateTimeOrMinValue(object value)
        {
            if (value is System.DBNull || value == null) return DateTime.MinValue;
            else return (DateTime)value;
        }

        protected static Guid GuidFromDb(object value)
        {
            Guid? ret = value as Guid?;
            if (ret == null) ret = new Guid((string)value);
            return ret.Value;
        }

        protected static Guid? GuidOrNullFromDb(object value)
        {
            Guid? ret = value as Guid?;
            if (ret == null && !IsNull(value)) ret = new Guid((string)value);
            return ret.Value;
        }

        protected static bool IsNull(object value)
        {
            if (value == null) return true;
            if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) return true;
            if (value == DBNull.Value) return true;
            else return false;
        }

        #endregion

        #region Helpers

        private static bool StringArrayContains(string[] array, string containsItem)
        {
            foreach (string item in array)
            {
                if (item == containsItem) return true;
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
