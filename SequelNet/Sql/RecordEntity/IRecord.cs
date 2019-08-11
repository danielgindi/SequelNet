using SequelNet.Connector;

namespace SequelNet
{
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

        void Insert(ConnectorBase connection = null, string userName = null);
        void Update(ConnectorBase connection = null, string userName = null);
        void Read(DataReader reader);
        void Save(string userName = null);
        void Save(ConnectorBase connection, string userName = null);

        /// <summary>
        /// Loads a record from the db, by matching the Primary Key to <paramref name="primaryKeyValue"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="primaryKeyValue">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the Primary Key.</param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        void LoadByKey(object primaryKeyValue, ConnectorBase connection = null);

        /// <summary>
        /// Loads a record from the db, by matching <paramref name="columnName"/> to <paramref name="value"/>.
        /// If the record was loaded, it will be marked as an "old" record.
        /// </summary>
        /// <param name="columnName">The column's name to Where. Could be a String or an IEnumerable of strings.</param>
        /// <param name="value">The columns' values to match. Could be a String or an IEnumerable of strings. Must match the <paramref name="columnName"/></param>
        /// <param name="connection">An optional db connection to use when executing the query.</param>
        void LoadByParam(object columnName, object value, ConnectorBase connection = null);
    }
}
