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
        abstract public bool IsDBNull(int i);
        abstract public int GetInt32(int i);
        abstract public int GetInt32OrZero(int i);
        abstract public Int64 GetInt64(int i);
        abstract public Int64 GetInt64OrZero(int i);
        abstract public bool GetBoolean(int i);
        abstract public string GetString(int i);
        abstract public string GetStringOrNull(int i);
        abstract public string GetStringOrEmpty(int i);
        abstract public DateTime GetDateTime(int i);
        abstract public DateTime? GetDateTimeOrNull(int i);
        abstract public DateTime GetDateTimeOrMinValue(int i);
        abstract public decimal GetDecimal(int i);
        abstract public decimal GetDecimalOrZero(int i);
        abstract public bool HasColumn(string name);
        abstract public Int32 GetColumnCount();
        abstract public string GetColumnName(Int32 ColumnIndex);

        /// <summary>
        /// Gets the value of the specified column in its native format given the column index.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in its native format.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount</exception>
        abstract public object this[int i] { get; }

        /// <summary>
        /// Gets the value of the specified column in its native format given the column name.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <returns>The value of the specified column in its native format.</returns>
        /// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found</exception>
        abstract public object this[string name] { get; }

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column index.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount</exception>
        public virtual Geometry GetGeometry(int i)
        {
            throw new NotImplementedException(@"GetGeometry not implemented for this connector");
        }

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column name.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found</exception>
        public virtual Geometry GetGeometry(string name)
        {
            throw new NotImplementedException(@"GetGeometry not implemented for this connector");
        }
    }
}
