using System;
using System.Collections.Generic;
using System.Data;
using SequelNet.Connector;

namespace SequelNet;

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
            if (needsDispose) connection = ConnectorBase.Create();
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
    /// Will execute the query returning a <typeparamref name="DataReader"/> object.
    /// </summary>
    /// <param name="connection">An existing connection to use.</param>
    /// <returns><typeparamref name="DataReader"/> object</returns>
    public DataReader ExecuteReader(ConnectorBase connection = null, CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        bool needsDispose = connection == null;
        try
        {
            if (needsDispose)
            {
                connection = ConnectorBase.Create();
                commandBehavior |= CommandBehavior.CloseConnection;
            }

            var cmd = BuildDbCommand(connection);
            return connection.ExecuteReader(cmd, true, commandBehavior);
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
    /// Will execute the query returning a <typeparamref name="DataReader"/> object.
    /// </summary>
    /// <param name="factory">A connector factory.</param>
    /// <returns><typeparamref name="DataReader"/> object</returns>
    public DataReader ExecuteReader(IConnectorFactory factory, CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        return ExecuteReader(factory.Connector(), commandBehavior);
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
            if (needsDispose) connection = ConnectorBase.Create();

            bool transaction = false;
            if (NeedTransaction && !connection.HasTransaction)
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
    /// Will execute the query returning the first value of the first row.
    /// </summary>
    /// <param name="factory">A connector factory.</param>
    /// <returns>an object</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
    public object ExecuteScalar(IConnectorFactory factory)
    {
        return ExecuteScalar(factory.Connector());
    }

    /// <summary>
    /// Will execute the query returning the first value of the first row.
    /// </summary>
    /// <typeparam name="T">Type to convert to</typeparam>
    /// <param name="connection">An existing connection to use.</param>
    /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
    public Nullable<T> ExecuteScalarOrNull<T>(ConnectorBase connection = null) where T : struct, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
    {
        var scalar = ExecuteScalar(connection);

        if (scalar == null || !(scalar is IConvertible)) return null;

        var converted = Convert.ChangeType(scalar, typeof(T));
        if (converted == null) return null;

        return (T)converted;
    }

    /// <summary>
    /// Will execute the query returning the first value of the first row.
    /// </summary>
    /// <typeparam name="T">Type to convert to</typeparam>
    /// <param name="factory">A connector factory.</param>
    /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
    public Nullable<T> ExecuteScalarOrNull<T>(IConnectorFactory factory) where T : struct, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
    {
        return ExecuteScalarOrNull<T>(factory.Connector());
    }

    /// <summary>
    /// Will execute the query returning the first value of the first row.
    /// </summary>
    /// <typeparam name="T">Type to convert to</typeparam>
    /// <param name="connection">An existing connection to use.</param>
    /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
    public T ExecuteScalar<T>(ConnectorBase connection = null) where T : class, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
    {
        var scalar = ExecuteScalar(connection);

        if (scalar == null || !(scalar is IConvertible)) return null;

        return Convert.ChangeType(scalar, typeof(T)) as T;
    }

    /// <summary>
    /// Will execute the query returning the first value of the first row.
    /// </summary>
    /// <typeparam name="T">Type to convert to</typeparam>
    /// <param name="factory">A connector factory.</param>
    /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
    public T ExecuteScalar<T>(IConnectorFactory factory) where T : class, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
    {
        return ExecuteScalar<T>(factory.Connector());
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
            if (needsDispose) connection = ConnectorBase.Create();

            bool transaction = false;
            if (NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
            int retValue = 0;

            using (var cmd = BuildDbCommand(connection))
                retValue = connection.ExecuteNonQuery(cmd);

            if (transaction) connection.CommitTransaction();
            return retValue;
        }
        catch (Exception ex) when (connection.Language.OnExecuteNonQueryException != null)
        {
            return connection.Language.OnExecuteNonQueryException(this, connection, ex);
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
    /// <param name="factory">A connector factory.</param>
    /// <returns>Number of affected rows</returns>
    public int ExecuteNonQuery(IConnectorFactory factory)
    {
        return ExecuteNonQuery(factory.Connector());
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
    /// Will execute the query without reading any results.
    /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
    /// </summary>
    /// <param name="factory">A connector factory.</param>
    /// <returns>Number of affected rows</returns>
    public int Execute(IConnectorFactory factory)
    {
        return Execute(factory.Connector());
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
            if (needsDispose) connection = ConnectorBase.Create();

            bool transaction = false;
            if (NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();

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
    /// <param name="lastInsertId">Where to put the last inserted ROWID</param>
    /// <param name="factory">A connector factory.</param>
    /// <returns>Number of affected rows</returns>
    public int Execute(out object lastInsertId, IConnectorFactory factory)
    {
        return Execute(out lastInsertId, factory.Connector());
    }

    /// <summary>
    /// Will execute the query, and fetch the last inserted ROWID.
    /// </summary>
    /// <param name="connection">An existing connection to use.</param>
    /// <param name="lastInsertId">Where to put the last inserted ROWID</param>
    /// <returns>Number of affected rows</returns>
    [Obsolete]
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
    /// <param name="factory">A connector factory.</param>
    /// <returns>Array of values. Will never return null.</returns>
    public T[] ExecuteScalarArray<T>(IConnectorFactory factory)
    {
        return ExecuteScalarArray<T>(factory.Connector());
    }

    /// <summary>
    /// Executes the query and reads the first value of each row.
    /// </summary>
    /// <param name="connection">An existing connection to use.</param>
    /// <returns>List of values. Will never return null.</returns>
    public List<T> ExecuteScalarList<T>(ConnectorBase connection = null)
    {
        var list = new List<T>();
        using (var reader = ExecuteReader(connection))
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
    /// Executes the query and reads the first value of each row.
    /// </summary>
    /// <param name="factory">A connector factory.</param>
    /// <returns>List of values. Will never return null.</returns>
    public List<T> ExecuteScalarList<T>(IConnectorFactory factory)
    {
        return ExecuteScalarList<T>(factory.Connector());
    }

    /// <summary>
    /// Executes the query and reads the first row only into a list.
    /// </summary>
    /// <param name="connection">An existing connection to use.</param>
    /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
    public List<object> ExecuteOneRowToList(ConnectorBase connection = null)
    {
        using (var reader = ExecuteReader(connection))
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
    /// Executes the query and reads the first row only into a list.
    /// </summary>
    /// <param name="factory">A connector factory.</param>
    /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
    public List<object> ExecuteOneRowToList(IConnectorFactory factory)
    {
        return ExecuteOneRowToList(factory.Connector());
    }

    /// <summary>
    /// Executes the query and reads the first row only into a dictionary.
    /// </summary>
    /// <param name="connection">An existing connection to use.</param>
    /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
    public Dictionary<string, object> ExecuteOneRowToDictionary(ConnectorBase connection = null)
    {
        using (var reader = ExecuteReader(connection))
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
    /// Executes the query and reads the first row only into a dictionary.
    /// </summary>
    /// <param name="factory">A connector factory.</param>
    /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
    /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
    public Dictionary<string, object> ExecuteOneRowToDictionary(IConnectorFactory factory)
    {
        return ExecuteOneRowToDictionary(factory.Connector());
    }

    /// <summary>
    /// Executes the query and reads all rows into a list of lists.
    /// </summary>
    /// <param name="connection">An existing connection to use.</param>
    /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
    public List<List<object>> ExecuteListOfLists(ConnectorBase connection = null)
    {
        List<List<object>> results = new List<List<object>>();
        using (var reader = ExecuteReader(connection))
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
    /// Executes the query and reads all rows into a list of lists.
    /// </summary>
    /// <param name="factory">A connector factory.</param>
    /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
    public List<List<object>> ExecuteListOfLists(IConnectorFactory factory)
    {
        return ExecuteListOfLists(factory.Connector());
    }

    /// <summary>
    /// Executes the query and reads the first row only.
    /// </summary>
    /// <param name="connection">An existing connection to use.</param>
    /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
    public List<Dictionary<string, object>> ExecuteListOfDictionaries(ConnectorBase connection = null)
    {
        var results = new List<Dictionary<string, object>>();
        using (var reader = ExecuteReader(connection))
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
    /// <param name="factory">A connector factory.</param>
    /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
    public List<Dictionary<string, object>> ExecuteListOfDictionaries(IConnectorFactory factory)
    {
        return ExecuteListOfDictionaries(factory.Connector());
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

    /// <summary>
    /// Executes the query and reads the first row only.
    /// </summary>
    /// <param name="factory">A connector factory.</param>
    /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
    public L ExecuteCollection<R, L>(IConnectorFactory factory)
        where R : AbstractRecord<R>, new()
        where L : AbstractRecordList<R, L>, new()
    {
        return ExecuteCollection<R, L>(factory.Connector());
    }

    private SelectColumnList GenerateAggregateSelectList(
        string schemaName, string columnName,
        string aggregateFunction,
        bool isDistinctQuery,
        ConnectorBase connection)
    {
        var list = new SelectColumnList();

        if (schemaName != null || _SchemaAlias == null)
        {
            schemaName = schemaName ?? _SchemaName;

            if (Schema.DatabaseOwner.Length > 0)
            {
                schemaName = connection.Language.WrapFieldName(Schema.DatabaseOwner) + "."
                    + connection.Language.WrapFieldName(schemaName);
            }
            else
            {
                schemaName = connection.Language.WrapFieldName(schemaName);
            }
        }
        else
        {
            schemaName = schemaName ?? _SchemaAlias;
            schemaName = connection.Language.WrapFieldName(schemaName);
        }

        var select = new SelectColumn(
            aggregateFunction + (isDistinctQuery ? @"(DISTINCT " : @"(") +
            (columnName == "*"
            ? columnName
            : (schemaName + "." + connection.Language.WrapFieldName(columnName)))
            + @")", true);

        list.Add(select);

        return list;
    }

    public object ExecuteAggregate(string columnName, string aggregateFunction, bool isDistinctQuery, ConnectorBase connection = null)
    {
        return ExecuteAggregate(null, columnName, aggregateFunction, isDistinctQuery, connection);
    }

    public object ExecuteAggregate(string columnName, string aggregateFunction, bool isDistinctQuery, IConnectorFactory factory)
    {
        return ExecuteAggregate(columnName, aggregateFunction, isDistinctQuery, factory.Connector());
    }

    public object ExecuteAggregate(
        string schemaName, string columnName, string aggregateFunction, 
        bool isDistinctQuery,
        ConnectorBase connection = null)
    {
        bool ownsConnection = false;
        if (connection == null)
        {
            ownsConnection = true;
            connection = ConnectorBase.Create();
        }
        try
        {
            var oldSelectList = _ListSelect;
            bool oldIsDistinct = IsDistinct;
            IsDistinct = false;

            _ListSelect = GenerateAggregateSelectList(schemaName, columnName, aggregateFunction, isDistinctQuery, connection);

            object ret = ExecuteScalar(connection);

            _ListSelect = oldSelectList;
            IsDistinct = oldIsDistinct;

            return ret;
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

    public object ExecuteAggregate(
        string schemaName, string columnName, string aggregateFunction,
        bool isDistinctQuery,
        IConnectorFactory factory)
    {
        return ExecuteAggregate(schemaName, columnName, aggregateFunction, isDistinctQuery, factory.Connector());
    }

    public object ExecuteAggregate(BaseAggregatePhrase aggregate, ConnectorBase connection = null)
    {
        bool ownsConnection = false;
        if (connection == null)
        {
            ownsConnection = true;
            connection = ConnectorBase.Create();
        }
        try
        {
            var oldSelectList = _ListSelect;
            bool oldIsDistinct = IsDistinct;
            IsDistinct = false;

            var list = new SelectColumnList();
            list.Add(new SelectColumn(aggregate));
            _ListSelect = list;

            object ret = ExecuteScalar(connection);

            _ListSelect = oldSelectList;
            IsDistinct = oldIsDistinct;

            return ret;
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

    public object ExecuteAggregate(BaseAggregatePhrase aggregate, IConnectorFactory factory)
    {
        return ExecuteAggregate(aggregate, factory.Connector());
    }
}
