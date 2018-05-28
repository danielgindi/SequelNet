using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using dg.Sql.Connector;

namespace dg.Sql
{
    public partial class Query
    {
        /// <summary>
        /// Will execute the query reading the results into a <typeparamref name="DataSet"/>.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns><typeparamref name="DataSet"/> object</returns>
        public DataSet ExecuteDataSet(ConnectorBase connection = null)
        {
            bool needsDispose = connection == null;
            try
            {
                if (needsDispose) connection = ConnectorBase.NewInstance();
                using (var cmd = BuildDbCommand(connection))
                    return connection.ExecuteDataSet(cmd);
            }
            finally
            {
                if (needsDispose && connection != null)
                {
                    connection.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReaderBase"/> object.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns><typeparamref name="DataReaderBase"/> object</returns>
        public DataReaderBase ExecuteReader(ConnectorBase connection = null)
        {
            bool needsDispose = connection == null;
            try
            {
                if (needsDispose) connection = ConnectorBase.NewInstance();

                using (var cmd = BuildDbCommand(connection))
                    return connection.ExecuteReader(cmd, needsDispose);
            }
            catch
            {
                if (needsDispose && connection != null)
                {
                    connection.Dispose();
                }
                throw;
            }
        }
        
        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>an object</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public object ExecuteScalar(ConnectorBase connection = null)
        {
            bool needsDispose = connection == null;
            try
            {
                if (needsDispose) connection = ConnectorBase.NewInstance();

                bool transaction = false;
                if (_NeedTransaction && !connection.HasTransaction)
                {
                    transaction = true;
                    connection.BeginTransaction();
                }
                object retValue = null;

                using (var cmd = BuildDbCommand(connection))
                    retValue = connection.ExecuteScalar(cmd);

                if (retValue is DBNull) retValue = null;
                else if (retValue is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)retValue).IsNull) retValue = null;

                if (transaction)
                {
                    connection.CommitTransaction();
                }

                return retValue;
            }
            finally
            {
                if (needsDispose && connection != null)
                {
                    connection.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Will execute the query without reading any results.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>Number of affected rows</returns>
        public int ExecuteNonQuery(ConnectorBase connection = null)
        {
            bool needsDispose = connection == null;
            try
            {
                if (needsDispose) connection = ConnectorBase.NewInstance();

                bool transaction = false;
                if (_NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
                int retValue = 0;

                using (var cmd = BuildDbCommand(connection))
                    retValue = connection.ExecuteNonQuery(cmd);

                if (transaction) connection.CommitTransaction();
                return retValue;
            }
            finally
            {
                if (needsDispose && connection != null)
                {
                    connection.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Will execute the query without reading any results.
        /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(ConnectorBase connection = null)
        {
            return ExecuteNonQuery(connection);
        }

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="lastInsertId">Where to put the last inserted ROWID</param>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(out object lastInsertId, ConnectorBase connection = null)
        {
            bool needsDispose = connection == null;
            try
            {
                if (needsDispose) connection = ConnectorBase.NewInstance();

                bool transaction = false;
                if (_NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();

                int retValue = 0;

                using (var cmd = BuildDbCommand(connection))
                    retValue = connection.ExecuteNonQuery(cmd);

                if (retValue > 0)
                {
                    lastInsertId = connection.GetLastInsertID();
                    if (lastInsertId is DBNull) lastInsertId = null;
                    else if (lastInsertId is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)lastInsertId).IsNull) lastInsertId = null;
                }
                else
                {
                    lastInsertId = null;
                }
                if (transaction) connection.CommitTransaction();
                return retValue;
            }
            finally
            {
                if (needsDispose && connection != null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="lastInsertId">Where to put the last inserted ROWID</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(ConnectorBase connection, out object lastInsertId)
        {
            return Execute(out lastInsertId, connection);
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>Array of values. Will never return null.</returns>
        public T[] ExecuteScalarArray<T>(ConnectorBase connection = null)
        {
            return ExecuteScalarList<T>(connection).ToArray();
        }
        
        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>List of values. Will never return null.</returns>
        public List<T> ExecuteScalarList<T>(ConnectorBase connection = null)
        {
            List<T> list = new List<T>();
            using (DataReaderBase reader = ExecuteReader(connection))
            {
                object value;
                while (reader.Read())
                {
                    value = reader[0];
                    if (value is DBNull) value = null;
                    else if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) value = null;
                    if (value is T) list.Add((T)value);
                    else list.Add((T)Convert.ChangeType(value, typeof(T)));
                }
            }
            return list;
        }
        
        /// <summary>
        /// Executes the query and reads the first row only into a list.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public List<object> ExecuteOneRowToList(ConnectorBase connection = null)
        {
            using (DataReaderBase reader = ExecuteReader(connection))
            {
                if (reader.Read())
                {
                    List<object> row = new List<object>();
                    int i, c = reader.GetColumnCount();
                    object value;
                    for (i = 0; i < c; i++)
                    {
                        value = reader[i];
                        if (value is DBNull) value = null;
                        else if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) value = null;
                        row.Add(value);
                    }
                    return row;
                }
                else
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Executes the query and reads the first row only into a dictionary.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Dictionary<string, object> ExecuteOneRowToDictionary(ConnectorBase connection = null)
        {
            using (DataReaderBase reader = ExecuteReader(connection))
            {
                if (reader.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    int i, c = reader.GetColumnCount();
                    object value;
                    string[] columnNames = new string[c];
                    for (i = 0; i < c; i++)
                    {
                        columnNames[i] = reader.GetColumnName(i);
                    }
                    for (i = 0; i < c; i++)
                    {
                        value = reader[i];
                        if (value is DBNull) value = null;
                        else if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) value = null;
                        row[columnNames[i]] = value;
                    }
                    return row;
                }
                else
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Executes the query and reads all rows into a list of lists.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
        public List<List<object>> ExecuteListOfLists(ConnectorBase connection = null)
        {
            List<List<object>> results = new List<List<object>>();
            using (DataReaderBase reader = ExecuteReader(connection))
            {
                List<object> row;
                object value;
                while (reader.Read())
                {
                    row = new List<object>();
                    int i, c = reader.GetColumnCount();
                    for (i = 0; i < c; i++)
                    {
                        value = reader[i];
                        if (value is DBNull) value = null;
                        else if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) value = null;
                        row.Add(value);
                    }
                    results.Add(row);
                }
            }
            return results;
        }
        
        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public List<Dictionary<string, object>> ExecuteListOfDictionaries(ConnectorBase connection = null)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            using (DataReaderBase reader = ExecuteReader(connection))
            {
                Dictionary<string, object> row;
                int i, c = reader.GetColumnCount();
                object value;
                string[] columnNames = new string[c];
                for (i = 0; i < c; i++)
                {
                    columnNames[i] = reader.GetColumnName(i);
                }
                while (reader.Read())
                {
                    row = new Dictionary<string, object>();
                    for (i = 0; i < c; i++)
                    {
                        value = reader[i];
                        if (value is DBNull) value = null;
                        else if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) value = null;
                        row[columnNames[i]] = value;
                    }
                    results.Add(row);
                }
            }
            return results;
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public L ExecuteCollection<R, L>(ConnectorBase connection = null)
            where R : AbstractRecord<R>, new()
            where L : AbstractRecordList<R, L>, new()
        {
            return AbstractRecordList<R, L>.FetchByQuery(this, connection);
        }

        public object ExecuteAggregate(string columnName, string aggregateFunction, bool isDistinctQuery)
        {
            return ExecuteAggregate(null, null, columnName, aggregateFunction, isDistinctQuery, null);
        }

        public object ExecuteAggregate(string schemaName, string columnName, string aggregateFunction, bool isDistinctQuery)
        {
            return ExecuteAggregate(null, schemaName, columnName, aggregateFunction, isDistinctQuery, null);
        }

        public object ExecuteAggregate(string schemaName, string columnName, string aggregateFunction, bool isDistinctQuery, ConnectorBase connection)
        {
            return ExecuteAggregate(null, schemaName, columnName, aggregateFunction, isDistinctQuery, connection);
        }

        public object ExecuteAggregate(string databaseOwner, string schemaName, string columnName, string aggregateFunction, bool isDistinctQuery)
        {
            return ExecuteAggregate(databaseOwner, schemaName, columnName, aggregateFunction, isDistinctQuery, null);
        }

        public object ExecuteAggregate(string databaseOwner, string schemaName, string columnName, string aggregateFunction, bool isDistinctQuery, ConnectorBase connection)
        {
            bool ownsConnection = false;
            if (connection == null)
            {
                ownsConnection = true;
                connection = ConnectorBase.NewInstance();
            }
            try
            {
                SelectColumnList oldSelectList = _ListSelect;
                bool oldIsDistinct = IsDistinct;
                IsDistinct = false;
                
                _ListSelect = new SelectColumnList();
                if (schemaName != null)
                {
                    if (databaseOwner != null && databaseOwner.Length > 0)
                    {
                        schemaName = connection.WrapFieldName(databaseOwner) + @"." + connection.WrapFieldName(schemaName);
                    }
                    else
                    {
                        schemaName = connection.WrapFieldName(schemaName);
                    }
                }
                else
                {
                    if (Schema == null)
                    {
                        schemaName = connection.WrapFieldName(_FromExpressionTableAlias);
                    }
                    else
                    {
                        schemaName = @"";
                        if (Schema.DatabaseOwner.Length > 0)
                        {
                            schemaName = connection.WrapFieldName(Schema.DatabaseOwner) + @".";
                        }
                        schemaName += connection.WrapFieldName(_SchemaName);
                    }
                }

                SelectColumn select = new SelectColumn(
                    aggregateFunction + (isDistinctQuery ? @"(DISTINCT " : @"(") + 
                    (columnName == "*" 
                    ? columnName 
                    : (schemaName + "." + connection.WrapFieldName(columnName)))
                    + @")", true);

                _ListSelect.Add(select);

                object ret = ExecuteScalar(connection);

                _ListSelect = oldSelectList;
                IsDistinct = oldIsDistinct;

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ownsConnection && connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }
        }
    }
}
