using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SequelNet.Connector;

namespace SequelNet
{
    public partial class Query
    {
        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReader"/> object.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="commandBehavior">Command behavior</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><typeparamref name="DataReader"/> object</returns>
        public Task<DataReader> ExecuteReaderAsync(
            ConnectorBase connection = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            CancellationToken? cancellationToken = null)
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
                return connection.ExecuteReaderAsync(cmd, true, commandBehavior, cancellationToken);
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
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><typeparamref name="DataReader"/> object</returns>
        public Task<DataReader> ExecuteReaderAsync(ConnectorBase connection, CancellationToken? cancellationToken)
        {
            return ExecuteReaderAsync(connection, CommandBehavior.Default, cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReader"/> object.
        /// </summary>
        /// <param name="factory">A connector factory.</param>
        /// <param name="commandBehavior">Command behavior</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><typeparamref name="DataReader"/> object</returns>
        public Task<DataReader> ExecuteReaderAsync(
            IConnectorFactory factory,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            CancellationToken? cancellationToken = null)
        {
            return ExecuteReaderAsync(factory.Connector(), commandBehavior, cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReader"/> object.
        /// </summary>
        /// <param name="factory">A connector factory.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><typeparamref name="DataReader"/> object</returns>
        public Task<DataReader> ExecuteReaderAsync(
            IConnectorFactory factory,
            CancellationToken? cancellationToken)
        {
            return ExecuteReaderAsync(factory.Connector(), CommandBehavior.Default, cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReader"/> object.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><typeparamref name="DataReader"/> object</returns>
        public Task<DataReader> ExecuteReaderAsync(CancellationToken? cancellationToken)
        {
            return ExecuteReaderAsync((ConnectorBase)null, CommandBehavior.Default, cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning a <typeparamref name="DataReader"/> object.
        /// </summary>
        /// <param name="commandBehavior">Command behavior</param>
        /// <returns><typeparamref name="DataReader"/> object</returns>
        public Task<DataReader> ExecuteReaderAsync(CommandBehavior commandBehavior)
        {
            return ExecuteReaderAsync((ConnectorBase)null, commandBehavior, null);
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>an object</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public async Task<object> ExecuteScalarAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
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
                    retValue = await connection.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false);

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>an object</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Task<object> ExecuteScalarAsync(
            IConnectorFactory factory,
            CancellationToken? cancellationToken = null)
        {
            return ExecuteScalarAsync(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>an object</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Task<object> ExecuteScalarAsync(CancellationToken? cancellationToken)
        {
            return ExecuteScalarAsync((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
        public async Task<Nullable<T>> ExecuteScalarOrNullAsync<T>(ConnectorBase connection = null, CancellationToken? cancellationToken = null) where T : struct, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
        {
            var scalar = await ExecuteScalarAsync(connection, cancellationToken).ConfigureAwait(false);

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
        public Task<Nullable<T>> ExecuteScalarOrNullAsync<T>(
            IConnectorFactory factory,
            CancellationToken? cancellationToken = null) where T : struct, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
        {
            return ExecuteScalarOrNullAsync<T>(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
        public Task<Nullable<T>> ExecuteScalarOrNullAsync<T>(CancellationToken? cancellationToken) where T : struct, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
        {
            return ExecuteScalarOrNullAsync<T>((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
        public async Task<T> ExecuteScalarAsync<T>(ConnectorBase connection = null, CancellationToken? cancellationToken = null) where T : class, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
        {
            var scalar = await ExecuteScalarAsync(connection, cancellationToken).ConfigureAwait(false);

            if (scalar == null || !(scalar is IConvertible)) return null;

            return Convert.ChangeType(scalar, typeof(T)) as T;
        }
        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="factory">A connector factory.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
        public Task<T> ExecuteScalarAsync<T>(
            IConnectorFactory factory,
            CancellationToken? cancellationToken = null) where T : class, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
        {
            return ExecuteScalarAsync<T>(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Will execute the query returning the first value of the first row.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>a value of the required type, or null if the returned value was null or could not be converted to specified type</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
        public Task<T> ExecuteScalarAsync<T>(CancellationToken? cancellationToken) where T : class, IConvertible
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
        {
            return ExecuteScalarAsync<T>((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public async Task<int> ExecuteNonQueryAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            bool needsDispose = connection == null;
            try
            {
                if (needsDispose) connection = ConnectorBase.Create();

                bool transaction = false;
                if (NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();
                int retValue = 0;

                using (var cmd = BuildDbCommand(connection))
                    retValue = await connection.ExecuteNonQueryAsync(cmd, cancellationToken)
                        .ConfigureAwait(false);

                if (transaction) connection.CommitTransaction();
                return retValue;
            }
            catch (Exception ex) when (!(ex is OperationCanceledException) && connection.Language.OnExecuteNonQueryException != null)
            {
                return await connection.Language.OnExecuteNonQueryExceptionAsync(this, connection, ex)
                    .ConfigureAwait(false);
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public Task<int> ExecuteNonQueryAsync(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteNonQueryAsync(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public Task<int> ExecuteNonQueryAsync(CancellationToken? cancellationToken)
        {
            return ExecuteNonQueryAsync((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public Task<int> ExecuteAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            return ExecuteNonQueryAsync(connection, cancellationToken);
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="factory">A connector factory.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public Task<int> ExecuteAsync(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteAsync(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Will execute the query without reading any results.
        /// This is a synonym for <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public Task<int> ExecuteAsync(CancellationToken? cancellationToken)
        {
            return ExecuteNonQueryAsync((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows - and the lastInserId</returns>
        public async Task<(int updates, object lastInsertId)> ExecuteWithLastInsertIdAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            bool needsDispose = connection == null;
            try
            {
                if (needsDispose) connection = ConnectorBase.Create();

                bool transaction = false;
                if (NeedTransaction && !connection.HasTransaction) connection.BeginTransaction();

                int retValue = 0;

                using (var cmd = BuildDbCommand(connection))
                    retValue = await connection.ExecuteNonQueryAsync(cmd, cancellationToken)
                        .ConfigureAwait(false);

                object lastInsertId;

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

                return (retValue, lastInsertId);
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
        /// <param name="factory">A connector factory.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows - and the lastInserId</returns>
        public Task<(int updates, object lastInsertId)> ExecuteWithLastInsertIdAsync(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteWithLastInsertIdAsync(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows - and the lastInserId</returns>
        public Task<(int updates, object lastInsertId)> ExecuteWithLastInsertIdAsync(CancellationToken? cancellationToken)
        {
            return ExecuteWithLastInsertIdAsync((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Array of values. Will never return null.</returns>
        public async Task<T[]> ExecuteScalarArrayAsync<T>(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            return (await ExecuteScalarListAsync<T>(connection, cancellationToken).ConfigureAwait(false)).ToArray();
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="factory">A connector factory.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Array of values. Will never return null.</returns>
        public Task<T[]> ExecuteScalarArrayAsync<T>(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteScalarArrayAsync<T>(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Array of values. Will never return null.</returns>
        public Task<T[]> ExecuteScalarArrayAsync<T>(CancellationToken? cancellationToken)
        {
            return ExecuteScalarArrayAsync<T>((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of values. Will never return null.</returns>
        public async Task<List<T>> ExecuteScalarListAsync<T>(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            var list = new List<T>();
            using (var reader = await ExecuteReaderAsync(connection, cancellationToken).ConfigureAwait(false))
            {
                object value;
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of values. Will never return null.</returns>
        public Task<List<T>> ExecuteScalarListAsync<T>(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteScalarListAsync<T>(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first value of each row.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of values. Will never return null.</returns>
        public Task<List<T>> ExecuteScalarListAsync<T>(CancellationToken? cancellationToken)
        {
            return ExecuteScalarListAsync<T>((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only into a list.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public async Task<List<object>> ExecuteOneRowToListAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            using (var reader = await ExecuteReaderAsync(connection, cancellationToken).ConfigureAwait(false))
            {
                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var row = new List<object>();
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Task<List<object>> ExecuteOneRowToListAsync(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteOneRowToListAsync(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only into a list.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of values by the SELECT order. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Task<List<object>> ExecuteOneRowToListAsync(CancellationToken? cancellationToken)
        {
            return ExecuteOneRowToListAsync((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only into a dictionary.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public async Task<Dictionary<string, object>> ExecuteOneRowToDictionaryAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            using (var reader = await ExecuteReaderAsync(connection, cancellationToken).ConfigureAwait(false))
            {
                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var row = new Dictionary<string, object>();
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Task<Dictionary<string, object>> ExecuteOneRowToDictionaryAsync(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteOneRowToDictionaryAsync(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only into a dictionary.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        /// <remarks>You might want to limit the query return rows, to optimize the query.</remarks>
        public Task<Dictionary<string, object>> ExecuteOneRowToDictionaryAsync(CancellationToken? cancellationToken)
        {
            return ExecuteOneRowToDictionaryAsync((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads all rows into a list of lists.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
        public async Task<List<List<object>>> ExecuteListOfListsAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            var results = new List<List<object>>();
            using (var reader = await ExecuteReaderAsync(connection, cancellationToken).ConfigureAwait(false))
            {
                List<object> row;
                object value;
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
        public Task<List<List<object>>> ExecuteListOfListsAsync(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteListOfListsAsync(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads all rows into a list of lists.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Each item in the list is a list of values by the SELECT order. Will never return null.</returns>
        public Task<List<List<object>>> ExecuteListOfListsAsync(CancellationToken? cancellationToken)
        {
            return ExecuteListOfListsAsync((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteListOfDictionariesAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
        {
            var results = new List<Dictionary<string, object>>();
            using (var reader = await ExecuteReaderAsync(connection, cancellationToken).ConfigureAwait(false))
            {
                Dictionary<string, object> row;
                int i, c = reader.GetColumnCount();
                object value;
                string[] columnNames = new string[c];
                for (i = 0; i < c; i++)
                {
                    columnNames[i] = reader.GetColumnName(i);
                }

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public Task<List<Dictionary<string, object>>> ExecuteListOfDictionariesAsync(IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteListOfDictionariesAsync(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public Task<List<Dictionary<string, object>>> ExecuteListOfDictionariesAsync(CancellationToken? cancellationToken)
        {
            return ExecuteListOfDictionariesAsync((ConnectorBase)null, cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="connection">An existing connection to use.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public Task<L> ExecuteCollectionAsync<R, L>(ConnectorBase connection = null, CancellationToken? cancellationToken = null)
            where R : AbstractRecord<R>, new()
            where L : AbstractRecordList<R, L>, new()
        {
            return AbstractRecordList<R, L>.FetchByQueryAsync(this, connection, cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="factory">A connector factory.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public Task<L> ExecuteCollectionAsync<R, L>(IConnectorFactory factory, CancellationToken? cancellationToken = null)
            where R : AbstractRecord<R>, new()
            where L : AbstractRecordList<R, L>, new()
        {
            return ExecuteCollectionAsync<R, L>(factory.Connector(), cancellationToken);
        }

        /// <summary>
        /// Executes the query and reads the first row only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary of values by the SELECT order, where the key is the column name. null if no results were returned by the query.</returns>
        public Task<L> ExecuteCollectionAsync<R, L>(CancellationToken? cancellationToken)
            where R : AbstractRecord<R>, new()
            where L : AbstractRecordList<R, L>, new()
        {
            return ExecuteCollectionAsync<R, L>((ConnectorBase)null, cancellationToken);
        }

        public Task<object> ExecuteAggregateAsync(
            BaseAggregatePhrase aggregate,
            CancellationToken? cancellationToken = null)
        {
            return ExecuteAggregateAsync(aggregate, (ConnectorBase)null, cancellationToken);
        }

        public async Task<object> ExecuteAggregateAsync(
            BaseAggregatePhrase aggregate,
            ConnectorBase connection,
            CancellationToken? cancellationToken = null)
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

                object ret = await ExecuteScalarAsync(connection, cancellationToken).ConfigureAwait(false);

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

        /// <summary>
        /// Will execute the query, and fetch the last inserted ROWID.
        /// </summary>
        /// <param name="factory">A connector factory.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows - and the lastInserId</returns>
        public Task<object> ExecuteAggregateAsync(BaseAggregatePhrase aggregate, IConnectorFactory factory, CancellationToken? cancellationToken = null)
        {
            return ExecuteAggregateAsync(aggregate, factory.Connector(), cancellationToken);
        }
    }
}
