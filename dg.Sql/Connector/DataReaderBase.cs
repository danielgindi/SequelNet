using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql.Connector
{
    public abstract class DataReaderBase : IDisposable
    {
        abstract public void Dispose();
        abstract public void Close();
        abstract public bool Read();
        abstract public bool IsDBNull(int columnIndex);
        abstract public int GetInt32(int columnIndex);
        abstract public int GetInt32OrZero(int columnIndex);
        abstract public Int64 GetInt64(int columnIndex);
        abstract public Int64 GetInt64OrZero(int columnIndex);
        abstract public bool GetBoolean(int columnIndex);
        abstract public string GetString(int columnIndex);
        abstract public string GetStringOrNull(int columnIndex);
        abstract public string GetStringOrEmpty(int columnIndex);
        abstract public DateTime GetDateTime(int columnIndex);
        abstract public DateTime? GetDateTimeOrNull(int columnIndex);
        abstract public DateTime GetDateTimeOrMinValue(int columnIndex);
        abstract public decimal GetDecimal(int columnIndex);
        abstract public decimal GetDecimalOrZero(int columnIndex);
        abstract public bool HasColumn(string columnIndex);
        abstract public Int32 GetColumnCount();
        abstract public string GetColumnName(Int32 columnIndex);

        /// <summary>
        /// Gets the value of the specified column in its native format given the column index.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in its native format.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount</exception>
        abstract public object this[int columnIndex] { get; }

        /// <summary>
        /// Gets the value of the specified column in its native format given the column name.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <returns>The value of the specified column in its native format.</returns>
        /// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found</exception>
        abstract public object this[string columnName] { get; }

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column index.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount</exception>
        public virtual Geometry GetGeometry(int columnIndex)
        {
            throw new NotImplementedException(@"GetGeometry not implemented for this connector");
        }

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column name.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found</exception>
        public virtual Geometry GetGeometry(string columnName)
        {
            throw new NotImplementedException(@"GetGeometry not implemented for this connector");
        }
    }
}
