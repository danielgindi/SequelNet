using SequelNet.Connector;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SequelNet;

/// <summary>
/// Defines an interface for a record representing class - which will supply a schema and a few utility functions
/// </summary>
/// <typeparam name="T">The name of the record class</typeparam>
public interface IRecord
{
    object GetPrimaryKeyValue();
    TableSchema GenerateTableSchema();

    bool IsNewRecord { get; set; }
    void MarkOld();
    void MarkNew();

    void MarkColumnMutated(string column);
    void MarkColumnNotMutated(string column);
    void MarkAllColumnsNotMutated();
    bool IsColumnMutated(string column);
    bool HasMutatedColumns();
    HashSet<string> GetMutatedColumnNamesSet();

    void Insert(ConnectorBase connection = null);
    Task InsertAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null);
    void Update(ConnectorBase connection = null);
    Task UpdateAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null);
    void Read(DataReader reader);
    void Save(ConnectorBase connection = null);
    Task SaveAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Loads a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
    /// If the record was loaded, it will be marked as an "old" record.
    /// </summary>
    /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
    /// <param name="connection">An optional db connection to use when executing the query.</param>
    void LoadByKey(object primaryKeyValue, ConnectorBase connection = null);

    /// <summary>
    /// Loads a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
    /// If the record was loaded, it will be marked as an "old" record.
    /// </summary>
    /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
    /// <param name="connection">An optional db connection to use when executing the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task LoadByKeyAsync(object primaryKeyValue, ConnectorBase connection = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Loads a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
    /// If the record was loaded, it will be marked as an "old" record.
    /// </summary>
    /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
    /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
    /// <param name="connection">An optional db connection to use when executing the query.</param>
    void LoadByParam(object columnName, object value, ConnectorBase connection = null);

    /// <summary>
    /// Loads a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
    /// If the record was loaded, it will be marked as an "old" record.
    /// </summary>
    /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
    /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
    /// <param name="connection">An optional db connection to use when executing the query.</param>
    Task LoadByParamAsync(object columnName, object value, ConnectorBase connection = null, CancellationToken? cancellationToken = null);
}
