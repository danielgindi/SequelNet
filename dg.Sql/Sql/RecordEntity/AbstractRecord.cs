using System;
using System.Collections.Generic;
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

        public AbstractRecord(object keyValue)
        {
            LoadByKey(keyValue);
        }

        public AbstractRecord(string columnName, object columnValue)
        {
            LoadByParam(columnName, columnValue);
        }

        #endregion

        #region Table schema stuff

        /// <summary>
        /// Re-sets a cached version of the primary key names of this record. 
        /// This is automatically called the first time that a primary-key-related action is called.
        /// </summary>
        public static void CachePrimaryKeyName()
        {
            var keyNames = new List<string>();

            foreach (var col in Schema.Columns)
            {
                if (col.IsPrimaryKey)
                {
                    keyNames.Add(col.Name);
                }
            }

            if (keyNames.Count == 0)
            {
                foreach (var idx in Schema.Indexes)
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

        [XmlIgnore]
        public static string SchemaName => Schema.Name;

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
            __HAS_DELETED = Schema.Columns.Find(@"Deleted") != null;
            __HAS_IS_DELETED = Schema.Columns.Find(@"IsDeleted") != null;
            __HAS_CREATED_BY = Schema.Columns.Find(@"CreatedBy") != null;
            __HAS_CREATED_ON = Schema.Columns.Find(@"CreatedOn") != null;
            __HAS_MODIFIED_BY = Schema.Columns.Find(@"ModifiedBy") != null;
            __HAS_MODIFIED_ON = Schema.Columns.Find(@"ModifiedOn") != null;
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

        public virtual void Insert(ConnectorBase connection = null, string userName = null)
        {
            if (!__FLAGS_RETRIEVED)
            {
                RetrieveFlags();
            }
            if (__CLASS_TYPE == null)
            {
                __CLASS_TYPE = this.GetType();
            }

            var qry = new Query(Schema);

            foreach (var column in Schema.Columns)
            {
                var propInfo = __CLASS_TYPE.GetProperty(column.Name);

                if (propInfo == null) 
                    propInfo = __CLASS_TYPE.GetProperty(column.Name + @"X");

                if (propInfo != null)
                    qry.Insert(column.Name, propInfo.GetValue(this, null));
            }

            if (__HAS_CREATED_BY && userName != null)
            {
                qry.Insert(@"CreatedBy", userName);
            }

            if (__HAS_CREATED_ON)
            {
                qry.Insert(@"CreatedOn", DateTime.UtcNow);
            }

            qry.Execute(connection);

            _NewRecord = false;
            MarkAllColumnsNotMutated();
        }

        public virtual void Update(ConnectorBase connection = null, string userName = null)
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

            var qry = new Query(Schema);

            foreach (var Column in Schema.Columns)
            {
                if ((isPrimaryKeyNullOrString && Column.Name == (string)primaryKey) ||
                    (!isPrimaryKeyNullOrString && StringArrayContains((string[])primaryKey, Column.Name))) continue;

                var propInfo = __CLASS_TYPE.GetProperty(Column.Name);
                if (propInfo == null) propInfo = __CLASS_TYPE.GetProperty(Column.Name + @"X");
                if (propInfo != null)
                {
                    if (_AtomicUpdates && !IsColumnMutated(Column.Name)) continue;

                    qry.Update(Column.Name, propInfo.GetValue(this, null));
                }
            }

            if (!_AtomicUpdates || qry.HasInsertsOrUpdates)
            {
                if (__HAS_MODIFIED_BY && userName != null)
                {
                    qry.Update(@"ModifiedBy", userName);
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
            foreach (var Column in Schema.Columns)
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

        public virtual void Save(string userName = null)
        {
            if (_NewRecord)
            {
                Insert(null, userName);
            }
            else
            {
                Update(null, userName);
            }
        }

        public virtual void Save(ConnectorBase connection, string userName = null)
        {
            if (_NewRecord)
            {
                Insert(connection, userName);
            }
            else
            {
                Update(connection, userName);
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

            if (__HAS_DELETED || __HAS_IS_DELETED)
            {
                Query qry = new Query(Schema);

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
            Query qry = new Query(Schema).Delete();

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
            foreach (var col in Schema.Columns)
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
            foreach (var col in Schema.Columns)
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
            Query qry = new Query(Schema);

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
            Query qry = new Query(Schema);

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

        #region Helpers

        private static bool StringArrayContains(string[] array, string containsItem)
        {
            foreach (string item in array)
            {
                if (item == containsItem) return true;
            }
            return false;
        }

        #endregion
    }
}
