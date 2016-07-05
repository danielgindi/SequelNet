﻿using System;
using System.Data.Common;
using System.ComponentModel;
using System.Collections;
using System.Data;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
{
    public class DataReaderBase : IDisposable, IDataRecord, IEnumerable
    {
        #region Instancing

        protected DbDataReader UnderlyingReader = null;
        protected ConnectorBase AttachedConnection = null;

        public DataReaderBase(DbDataReader reader)
        {
            UnderlyingReader = reader;
        }

        public DataReaderBase(DbDataReader reader, ConnectorBase connectionToClose)
        {
            UnderlyingReader = reader;
            AttachedConnection = connectionToClose;
        }

        #endregion

        #region IDisposable

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UnderlyingReader != null)
                {
                    UnderlyingReader.Dispose();
                }
                if (AttachedConnection != null)
                {
                    AttachedConnection.Dispose();
                    AttachedConnection = null;
                }
            }
            // Now clean up Native Resources (Pointers)
        }

        ~DataReaderBase()
        {
            Dispose(false);
        }

        #endregion
        
        #region IEnumerable

        /// <summary>
        /// Returns an System.Collections.IEnumerator that can be used to iterate through
        ///    the rows in the data reader.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator that can be used to iterate through the rows</returns>
        ///    in the data reader.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerator GetEnumerator()
        {
            return UnderlyingReader.GetEnumerator();
        }

        #endregion

        #region IDataRecord

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
        public object this[int ordinal]
        {
            get
            {
                return UnderlyingReader[ordinal];
            }
        }
        
        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
        public object this[string name]
        {
            get
            {
                return UnderlyingReader[name];
            }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <returns>The number of columns in the current row.</returns>
        /// <exception cref="System.NotSupportedException">There is no current connection to an instance of SQL Server.</exception>
        public int FieldCount
        {
            get
            {
                return UnderlyingReader.FieldCount;
            }
        }
        
        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public bool GetBoolean(int ordinal)
        {
            return UnderlyingReader.GetBoolean(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public byte GetByte(int ordinal)
        {
            return UnderlyingReader.GetByte(ordinal);
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column, starting at location indicated
        ///    by dataOffset, into the buffer, starting at the location indicated by bufferOffset.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return UnderlyingReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Gets the value of the specified column as a single character.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public char GetChar(int ordinal)
        {
            return UnderlyingReader.GetChar(ordinal);
        }

        /// <summary>
        /// Reads a stream of characters from the specified column, starting at location
        ///    indicated by dataOffset, into the buffer, starting at the location indicated
        ///    by bufferOffset.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of characters read.</returns>
        public long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return UnderlyingReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Returns a <see cref="DataReaderBase"/> object for the requested column ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>A System.Data.Common.DbDataReader object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DataReaderBase GetData(int ordinal)
        {
            return new DataReaderBase(UnderlyingReader.GetData(ordinal));
        }

        /// <summary>
        /// Gets name of the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>A string representing the name of the data type.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public string GetDataTypeName(int ordinal)
        {
            return UnderlyingReader.GetDataTypeName(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a System.DateTime object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public DateTime GetDateTime(int ordinal)
        {
            return UnderlyingReader.GetDateTime(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a System.Decimal object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public decimal GetDecimal(int ordinal)
        {
            return UnderlyingReader.GetDecimal(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a double-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public double GetDouble(int ordinal)
        {
            return UnderlyingReader.GetDouble(ordinal);
        }

        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The data type of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public Type GetFieldType(int ordinal)
        {
            return UnderlyingReader.GetFieldType(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a single-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public float GetFloat(int ordinal)
        {
            return UnderlyingReader.GetFloat(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a globally-unique identifier (GUID).
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public Guid GetGuid(int ordinal)
        {
            return UnderlyingReader.GetGuid(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public short GetInt16(int ordinal)
        {
            return UnderlyingReader.GetInt16(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public int GetInt32(int ordinal)
        {
            return UnderlyingReader.GetInt32(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public long GetInt64(int ordinal)
        {
            return UnderlyingReader.GetInt64(ordinal);
        }

        /// <summary>
        /// Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the specified column.</returns>
        public string GetName(int ordinal)
        {
            return UnderlyingReader.GetName(ordinal);
        }

        /// <summary>
        /// Gets the column ordinal given the name of the column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The name specified is not a valid column name.</exception>
        public int GetOrdinal(string name)
        {
            return UnderlyingReader.GetOrdinal(name);
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of System.String.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="System.InvalidCastException">The specified cast is not valid.</exception>
        public string GetString(int ordinal)
        {
            return UnderlyingReader.GetString(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public object GetValue(int ordinal)
        {
            return UnderlyingReader.GetValue(ordinal);
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">An array of System.Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of System.Object in the array.</returns>
        public int GetValues(object[] values)
        {
            return UnderlyingReader.GetValues(values);
        }

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing
        ///    values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>true if the specified column is equivalent to System.DBNull; otherwise false.</returns>
        public bool IsDBNull(int ordinal)
        {
            return UnderlyingReader.IsDBNull(ordinal);
        }

        #endregion

        /// <summary>
        /// Closes the System.Data.Common.DbDataReader object.
        /// </summary>
        public void Close()
        {
            UnderlyingReader.Close();

            if (AttachedConnection != null)
            {
                AttachedConnection.Close();
            }
        }

        /// <summary>
        /// Returns the provider-specific field type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The System.Type object that describes the data type of the specified column.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Type GetProviderSpecificFieldType(int ordinal)
        {
            return UnderlyingReader.GetProviderSpecificFieldType(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual object GetProviderSpecificValue(int ordinal)
        {
            return UnderlyingReader.GetProviderSpecificValue(ordinal);
        }

        /// <summary>
        /// Gets all provider-specific attribute columns in the collection for the current
        ///    row.
        /// </summary>
        /// <param name="values">An array of System.Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of System.Object in the array.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual int GetProviderSpecificValues(object[] values)
        {
            return UnderlyingReader.GetProviderSpecificValues(values);
        }

        /// <summary>
        /// Returns a System.Data.DataTable that describes the column metadata of the System.Data.Common.DbDataReader.
        /// </summary>
        /// <returns>A System.Data.DataTable that describes the column metadata.</returns>
        /// <exception cref="System.InvalidOperationException">The System.Data.SqlClient.SqlDataReader is closed.</exception>
        public DataTable GetSchemaTable()
        {
            return UnderlyingReader.GetSchemaTable();
        }

        /// <summary>
        /// Advances the reader to the next result when reading the results of a batch of
        ///    statements.
        /// </summary>
        /// <returns>true if there are more result sets; otherwise false.</returns>
        public bool NextResult()
        {
            return UnderlyingReader.NextResult();
        }

        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns>true if there are more rows; otherwise false.</returns>
        public bool Read()
        {
            return UnderlyingReader.Read();
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <returns>The depth of nesting for the current row.</returns>
        public int Depth
        {
            get
            {
                return UnderlyingReader.Depth;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this System.Data.Common.DbDataReader contains one or more rows.
        /// </summary>
        /// <returns>true if the System.Data.Common.DbDataReader contains one or more rows; otherwise false.</returns>
        public bool HasRows
        {
            get
            {
                return UnderlyingReader.HasRows;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the System.Data.Common.DbDataReader is closed.
        /// </summary>
        /// <returns>true if the System.Data.Common.DbDataReader is closed; otherwise false.</returns>
        /// <exception cref="System.InvalidOperationException">The System.Data.SqlClient.SqlDataReader is closed.</exception>
        public bool IsClosed
        {
            get
            {
                return UnderlyingReader.IsClosed;
            }
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL
        ///    statement.
        /// </summary>
        /// <returns>
        /// The number of rows changed, inserted, or deleted. -1 for SELECT statements;
        /// 0 if no rows were affected or the statement failed.
        /// </returns>
        public int RecordsAffected
        {
            get
            {
                return UnderlyingReader.RecordsAffected;
            }
        }

        /// <summary>
        /// Gets the number of fields in the System.Data.Common.DbDataReader that are not
        ///    hidden.
        /// </summary>
        /// <returns>The number of fields that are not hidden.</returns>
        public virtual int VisibleFieldCount
        {
            get
            {
                return UnderlyingReader.VisibleFieldCount;
            }
        }

        #region Convenience Functions

        public int GetInt32OrZero(int columnIndex)
        {
            return UnderlyingReader.IsDBNull(columnIndex) ? 0 : UnderlyingReader.GetInt32(columnIndex);
        }

        public Int64 GetInt64OrZero(int columnIndex)
        {
            return UnderlyingReader.IsDBNull(columnIndex) ? 0 : UnderlyingReader.GetInt64(columnIndex);
        }

        public string GetStringOrNull(int columnIndex)
        {
            return UnderlyingReader.IsDBNull(columnIndex) ? null : UnderlyingReader.GetString(columnIndex);
        }

        public string GetStringOrEmpty(int columnIndex)
        {
            return UnderlyingReader.IsDBNull(columnIndex) ? String.Empty : UnderlyingReader.GetString(columnIndex);
        }

        public DateTime? GetDateTimeOrNull(int columnIndex)
        {
            return UnderlyingReader.IsDBNull(columnIndex) ? null : (DateTime?)UnderlyingReader.GetDateTime(columnIndex);
        }

        public DateTime GetDateTimeOrMinValue(int columnIndex)
        {
            return UnderlyingReader.IsDBNull(columnIndex) ? DateTime.MinValue : UnderlyingReader.GetDateTime(columnIndex);
        }

        public decimal GetDecimalOrZero(int columnIndex)
        {
            return UnderlyingReader.IsDBNull(columnIndex) ? 0 : UnderlyingReader.GetDecimal(columnIndex);
        }

        public bool HasColumn(string columnName)
        {
            for (int j = 0, len = UnderlyingReader.VisibleFieldCount; j < len; j++)
            {
                if (UnderlyingReader.GetName(j).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public int GetColumnCount()
        {
            return UnderlyingReader.VisibleFieldCount;
        }

        public string GetColumnName(int columnIndex)
        {
            return UnderlyingReader.GetName(columnIndex);
        }

        public int GetColumnIndex(string columnName)
        {
            return UnderlyingReader.GetOrdinal(columnName);
        }

        #endregion

        #region Geometry

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column index.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount</exception>
        public Geometry GetGeometry(int columnIndex)
        {
            byte[] geometryData = UnderlyingReader[columnIndex] as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, true);
            }
            return null;
        }

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column name.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found</exception>
        public Geometry GetGeometry(string columnName)
        {
            byte[] geometryData = UnderlyingReader[columnName] as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, true);
            }
            return null;
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
