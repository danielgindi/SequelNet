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
        /// <returns><typeparamref name="DataSet"/> object</returns>
        public DataSet ExecuteDataSet()
        {
            return ExecuteDataSet(null);
        }

        /// <summary>
        /// Will execute the query reading the results into a <typeparamref name="DataSet"/>.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns><typeparamref name="DataSet"/> object</returns>
        public DataSet ExecuteDataSet(ConnectorBase Connection)
        {
            bool needsDispose = Connection == null;
            try
            {
                if (needsDispose) Connection = ConnectorBase.NewInstance();
                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    return Connection.ExecuteDataSet(BuildDbCommand(Connection));
                }
                else
                {
                    return Connection.ExecuteDataSet(BuildCommand(Connection));
                }
            }
            finally
            {
                if (needsDispose && Connection != null)
                {
                    Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReaderBase"/> object.
        /// </summary>
        /// <returns><typeparamref name="DataReaderBase"/> object</returns>
        public DataReaderBase ExecuteReader()
        {
            return ExecuteReader(null);
        }

        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReaderBase"/> object.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns><typeparamref name="DataReaderBase"/> object</returns>
        public DataReaderBase ExecuteReader(ConnectorBase Connection)
        {
            bool needsDispose = Connection == null;
            try
            {
                if (needsDispose) Connection = ConnectorBase.NewInstance();
                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    return Connection.ExecuteReader(BuildDbCommand(Connection), needsDispose);
                }
                else
                {
                    return Connection.ExecuteReader(BuildCommand(Connection), needsDispose);
                }
            }
            catch
            {
                if (needsDispose && Connection != null)
                {
                    Connection.Dispose();
                }
                throw;
            }
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <returns>an object</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public object ExecuteScalar()
        {
            return ExecuteScalar(null);
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>an object</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public object ExecuteScalar(ConnectorBase Connection)
        {
            bool needsDispose = Connection == null;
            try
            {
                if (needsDispose) Connection = ConnectorBase.NewInstance();

                bool transaction = false;
                if (_NeedTransaction && !Connection.HasTransaction)
                {
                    transaction = true;
                    Connection.BeginTransaction();
                }
                object retValue = null;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = Connection.ExecuteScalar(BuildDbCommand(Connection));
                }
                else
                {
                    retValue = Connection.ExecuteScalar(BuildCommand(Connection));
                }

                if (retValue is DBNull) retValue = null;
                else if (retValue is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)retValue).IsNull) retValue = null;

                if (transaction)
                {
                    Connection.CommitTransaction();
                }

                return retValue;
            }
            finally
            {
                if (needsDispose && Connection != null)
                {
                    Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// </summary>
        /// <returns>Number of affected rows</returns>
        public int ExecuteNonQuery()
        {
            using (ConnectorBase connection = ConnectorBase.NewInstance())
            {
                bool transaction = false;
                if (_NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
                int retValue = 0;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = connection.ExecuteNonQuery(BuildDbCommand(connection));
                }
                else
                {
                    retValue = connection.ExecuteNonQuery(BuildCommand(connection));
                }

                if (transaction) connection.CommitTransaction();
                return retValue;
            }
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Number of affected rows</returns>
        public int ExecuteNonQuery(ConnectorBase connection)
        {
            if (connection == null) return ExecuteNonQuery();
            else
            {
                bool transaction = false;
                if (_NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
                int retValue = 0;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = connection.ExecuteNonQuery(BuildDbCommand(connection));
                }
                else
                {
                    retValue = connection.ExecuteNonQuery(BuildCommand(connection));
                }

                if (transaction) connection.CommitTransaction();
                return retValue;
            }
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <returns>Number of affected rows</returns>
        public int Execute()
        {
            return ExecuteNonQuery();
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(ConnectorBase Connection)
        {
            return ExecuteNonQuery(Connection);
        }

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="LastInsertId">Where to put the last inserted ROWID</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(out object LastInsertId)
        {
            using (ConnectorBase connection = ConnectorBase.NewInstance())
            {
                bool transaction = false;
                if (_NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
                int retValue = 0;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = connection.ExecuteNonQuery(BuildDbCommand(connection));
                }
                else
                {
                    retValue = connection.ExecuteNonQuery(BuildCommand(connection));
                }

                if (retValue > 0)
                {
                    LastInsertId = connection.GetLastInsertID();
                    if (LastInsertId is DBNull) LastInsertId = null;
                    else if (LastInsertId is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)LastInsertId).IsNull) LastInsertId = null;
                }
                else
                {
                    LastInsertId = null;
                }
                if (transaction) connection.CommitTransaction();
                return retValue;
            }
        }

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <param name="LastInsertId">Where to put the last inserted ROWID</param>
        /// <returns>Number of affected rows</returns>
        public int Execute(ConnectorBase Connection, out object LastInsertId)
        {
            if (Connection == null) return Execute(out LastInsertId);
            else
            {
                bool transaction = false;
                if (_NeedTransaction && !Connection.HasTransaction) Connection.BeginTransaction();

                int retValue = 0;

                if (_QueryMode == QueryMode.ExecuteStoredProcedure)
                {
                    retValue = Connection.ExecuteNonQuery(BuildDbCommand(Connection));
                }
                else
                {
                    retValue = Connection.ExecuteNonQuery(BuildCommand(Connection));
                }

                if (retValue > 0)
                {
                    LastInsertId = Connection.GetLastInsertID();
                    if (LastInsertId is DBNull) LastInsertId = null;
                    else if (LastInsertId is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)LastInsertId).IsNull) LastInsertId = null;
                }
                else
                {
                    LastInsertId = null;
                }
                if (transaction) Connection.CommitTransaction();
                return retValue;
            }
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Array of values. Will never return null.</returns>
        public T[] ExecuteScalarArray<T>()
        {
            return ExecuteScalarList<T>(null).ToArray();
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <returns>Array of values. Will never return null.</returns>
        public T[] ExecuteScalarArray<T>(ConnectorBase connection)
        {
            return ExecuteScalarList<T>(connection).ToArray();
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>List of values. Will never return null.</returns>
        public List<T> ExecuteScalarList<T>()
        {
            return ExecuteScalarList<T>(null);
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <returns>List of values. Will never return null.</returns>
        public List<T> ExecuteScalarList<T>(ConnectorBase connection)
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
        /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public List<object> ExecuteOneRowToList()
        {
            return ExecuteOneRowToList(null);
        }

        /// <summary>
        /// Executes the query and reads the first row only into a list.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public List<object> ExecuteOneRowToList(ConnectorBase Connection)
        {
            using (DataReaderBase reader = ExecuteReader(Connection))
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
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Dictionary<string, object> ExecuteOneRowToDictionary()
        {
            return ExecuteOneRowToDictionary(null);
        }

        /// <summary>
        /// Executes the query and reads the first row only into a dictionary.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Dictionary<string, object> ExecuteOneRowToDictionary(ConnectorBase Connection)
        {
            using (DataReaderBase reader = ExecuteReader(Connection))
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
        /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
        public List<List<object>> ExecuteListOfLists()
        {
            return ExecuteListOfLists(null);
        }

        /// <summary>
        /// Executes the query and reads all rows into a list of lists.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
        public List<List<object>> ExecuteListOfLists(ConnectorBase Connection)
        {
            List<List<object>> results = new List<List<object>>();
            using (DataReaderBase reader = ExecuteReader(Connection))
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
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public List<Dictionary<string, object>> ExecuteListOfDictionaries()
        {
            return ExecuteListOfDictionaries(null);
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="Connection">An existing connection to use.</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public List<Dictionary<string, object>> ExecuteListOfDictionaries(ConnectorBase Connection)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            using (DataReaderBase reader = ExecuteReader(Connection))
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

                if (_ListSelect.Count == 1 && _ListSelect[0].ObjectType == ValueObjectType.Literal && _ListSelect[0].ColumnName == "*")
                {
                    _ListSelect = new SelectColumnList();
                }
                if (schemaName != null)
                {
                    if (databaseOwner != null && databaseOwner.Length > 0)
                    {
                        schemaName = connection.EncloseFieldName(databaseOwner) + @"." + connection.EncloseFieldName(schemaName);
                    }
                    else
                    {
                        schemaName = connection.EncloseFieldName(schemaName);
                    }
                }
                else
                {
                    if (Schema == null)
                    {
                        schemaName = connection.EncloseFieldName(_FromExpressionTableAlias);
                    }
                    else
                    {
                        schemaName = @"";
                        if (Schema.DatabaseOwner.Length > 0)
                        {
                            schemaName = connection.EncloseFieldName(Schema.DatabaseOwner) + @".";
                        }
                        schemaName += connection.EncloseFieldName(_SchemaName);
                    }
                }
                SelectColumn select = new SelectColumn(aggregateFunction + (isDistinctQuery ? @"(DISTINCT " : @"(") + (columnName == "*" ? columnName : (schemaName + "." + connection.EncloseFieldName(columnName))) + @")", true);
                _ListSelect.Insert(0, select);

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
