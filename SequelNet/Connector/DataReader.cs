﻿using System;
using System.Data.Common;
using System.ComponentModel;
using System.Collections;
using System.Data;
using SequelNet.Sql.Spatial;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

namespace SequelNet.Connector;

public class DataReader : IDisposable, IDataRecord, IEnumerable
{
    #region Instancing

    protected DbDataReader UnderlyingReader = null;
    protected ConnectorBase AttachedConnection = null;
    protected DbCommand AttachedDbCommand = null;

    public DataReader(DbDataReader reader, DbCommand attachedDbCommand = null, ConnectorBase attachedConnection = null)
    {
        UnderlyingReader = reader;
        AttachedDbCommand = attachedDbCommand;
        AttachedConnection = attachedConnection;
    }

    public DataReader(DbDataReader reader, ConnectorBase connectionToClose)
    {
        UnderlyingReader = reader;
        AttachedConnection = connectionToClose;
    }

    #endregion

    #region Swap

    /// <summary>
    /// Swaps the underlying reader with the given reader.
    /// CAn be used to control the flow of the reader, during Read()/ReadAsync() calls.
    /// </summary>
    /// <param name="reader"></param>
    public void Swap(DataReader reader)
    {
        var underlyingReader = UnderlyingReader;
        var attachedConnection = AttachedConnection;
        var attachedDbCommand = AttachedDbCommand;
        this.UnderlyingReader = reader.UnderlyingReader;
        this.AttachedConnection = reader.AttachedConnection;
        this.AttachedDbCommand = reader.AttachedDbCommand;
        reader.UnderlyingReader = underlyingReader;
        reader.AttachedConnection = attachedConnection;
        reader.AttachedDbCommand = attachedDbCommand;
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
            if (AttachedDbCommand != null)
            {
                AttachedDbCommand.Dispose();
                AttachedDbCommand = null;
            }
        }
        // Now clean up Native Resources (Pointers)
    }

    ~DataReader()
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
    /// <exception cref="IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
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
    /// <exception cref="IndexOutOfRangeException">No column with the specified name was found.</exception>
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
    /// <exception cref="NotSupportedException">There is no current connection to an instance of SQL Server.</exception>
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
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public bool GetBoolean(int ordinal)
    {
        return UnderlyingReader.GetBoolean(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a byte.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public byte GetByte(int ordinal)
    {
        return UnderlyingReader.GetByte(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public sbyte GetSByte(int ordinal)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var value = UnderlyingReader.GetValue(ordinal);
        if (value is sbyte svalue)
            return svalue;
        return Convert.ToSByte(value);
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
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
    {
        return UnderlyingReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
    }

    /// <summary>
    /// Gets the value of the specified column as a single character.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
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
    /// Returns a <see cref="DataReader"/> object for the requested column ordinal.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>A System.Data.Common.DbDataReader object.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataReader GetData(int ordinal)
    {
        return new DataReader(UnderlyingReader.GetData(ordinal));
    }

    /// <summary>
    /// Gets name of the data type of the specified column.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>A string representing the name of the data type.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public string GetDataTypeName(int ordinal)
    {
        return UnderlyingReader.GetDataTypeName(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a System.DateTime object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public DateTime GetDateTime(int ordinal)
    {
        return UnderlyingReader.GetDateTime(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a System.TimeSpan object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public TimeSpan GetTimeSpan(int ordinal)
    {
        var value = UnderlyingReader.GetValue(ordinal);
        if (value is TimeSpan)
            return (TimeSpan)value;
        else if (value is string)
            return TimeSpan.Parse((string)value);
        else if (value != null)
            throw new InvalidCastException("Failed to convert " + value.GetType().ToString() + " to TimeSpan"); 
        
        throw new NoNullAllowedException("Value is null while the return value is not nullable");
    }

    /// <summary>
    /// Gets the value of the specified column as a System.Decimal object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public decimal GetDecimal(int ordinal)
    {
        return UnderlyingReader.GetDecimal(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a double-precision floating point number.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public double GetDouble(int ordinal)
    {
        return UnderlyingReader.GetDouble(ordinal);
    }

    /// <summary>
    /// Gets the data type of the specified column.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The data type of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public Type GetFieldType(int ordinal)
    {
        return UnderlyingReader.GetFieldType(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a single-precision floating point number.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public float GetFloat(int ordinal)
    {
        return UnderlyingReader.GetFloat(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a globally-unique identifier (GUID).
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public Guid GetGuid(int ordinal)
    {
        return UnderlyingReader.GetGuid(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a 16-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public short GetInt16(int ordinal)
    {
        return UnderlyingReader.GetInt16(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt16 GetUInt16(int ordinal)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var value = UnderlyingReader.GetValue(ordinal);
        if (value is UInt16 uvalue)
            return uvalue;
        return Convert.ToUInt16(value);
    }

    /// <summary>
    /// Gets the value of the specified column as a 32-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public int GetInt32(int ordinal)
    {
        return UnderlyingReader.GetInt32(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt32 GetUInt32(int ordinal)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var value = UnderlyingReader.GetValue(ordinal);
        if (value is UInt32 uvalue)
            return uvalue;
        return Convert.ToUInt32(value);
    }

    /// <summary>
    /// Gets the value of the specified column as a 64-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public long GetInt64(int ordinal)
    {
        return UnderlyingReader.GetInt64(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt64 GetUInt64(int ordinal)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var value = UnderlyingReader.GetValue(ordinal);
        if (value is UInt64 uvalue)
            return uvalue;
        return Convert.ToUInt64(value);
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
    /// <exception cref="IndexOutOfRangeException">The name specified is not a valid column name.</exception>
    public int GetOrdinal(string name)
    {
        return UnderlyingReader.GetOrdinal(name);
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of System.String.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
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
        return UnderlyingReader.IsDBNull(ordinal) ? null : UnderlyingReader.GetValue(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of the specified type.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal</param>
    /// <returns>The value of the specified column.</returns>
    public T GetFieldValue<T>(int ordinal)
    {
        return UnderlyingReader.GetFieldValue<T>(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of the specified type.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The value of the specified column.</returns>
    public Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken? cancellationToken = null)
    {
        return UnderlyingReader.GetFieldValueAsync<T>(ordinal, cancellationToken ?? CancellationToken.None);
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

        if (AttachedDbCommand != null)
        {
            AttachedDbCommand.Dispose();
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
    /// <exception cref="InvalidOperationException">The System.Data.SqlClient.SqlDataReader is closed.</exception>
    public DataTable GetSchemaTable()
    {
        return UnderlyingReader.GetSchemaTable();
    }

    /// <summary>
    /// Advances the reader to the next result when reading the results of a batch of
    ///    statements.
    /// </summary>
    /// <returns>true if there are more result sets; otherwise false.</returns>
    public virtual bool NextResult()
    {
        return UnderlyingReader.NextResult();
    }

    /// <summary>
    /// Advances the reader to the next result when reading the results of a batch of
    ///    statements.
    /// </summary>
    /// <returns>true if there are more result sets; otherwise false.</returns>
    public virtual Task<bool> NextResultAsync()
    {
        return UnderlyingReader.NextResultAsync();
    }

    /// <summary>
    /// Advances the reader to the next record in a result set.
    /// </summary>
    /// <returns>true if there are more rows; otherwise false.</returns>
    public virtual bool Read()
    {
        return UnderlyingReader.Read();
    }

    /// <summary>
    /// Advances the reader to the next record in a result set.
    /// </summary>
    /// <returns>true if there are more rows; otherwise false.</returns>
    public virtual Task<bool> ReadAsync(CancellationToken? cancellationToken = null)
    {
        return UnderlyingReader.ReadAsync(cancellationToken ?? CancellationToken.None);
    }

    /// <summary>
    /// Gets a value indicating the depth of nesting for the current row.
    /// </summary>
    /// <returns>The depth of nesting for the current row.</returns>
    public virtual int Depth
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
    public virtual bool HasRows
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
    /// <exception cref="InvalidOperationException">The System.Data.SqlClient.SqlDataReader is closed.</exception>
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

    #region Column-name base getters

    public bool GetBoolean(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetBoolean(ordinal);
    }

    public byte GetByte(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetByte(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public sbyte GetSByte(string columnName)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return GetSByte(ordinal);
    }

    public long GetBytes(string columnName, long dataOffset, byte[] buffer, int bufferOffset, int length)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
    }
    
    public char GetChar(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetChar(ordinal);
    }
    
    public long GetChars(string columnName, long dataOffset, char[] buffer, int bufferOffset, int length)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataReader GetData(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return new DataReader(UnderlyingReader.GetData(ordinal));
    }
    
    public string GetDataTypeName(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetDataTypeName(ordinal);
    }

    public DateTime GetDateTime(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetDateTime(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a System.TimeSpan object.
    /// </summary>
    /// <param name="columnName">The column name.</param>
    /// <returns>The value of the specified column.</returns>
    /// <exception cref="InvalidCastException">The specified cast is not valid.</exception>
    public TimeSpan GetTimeSpan(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return GetTimeSpan(ordinal);
    }

#if NET6_0_OR_GREATER
	    public DateOnly GetDateOnly(string name) => GetDateOnly(UnderlyingReader.GetOrdinal(name));
    
	    public DateOnly? GetDateOnlyOrNull(string name) => GetDateOnlyOrNull(UnderlyingReader.GetOrdinal(name));

	    public TimeOnly GetTimeOnly(string name) => GetTimeOnly(UnderlyingReader.GetOrdinal(name));

	    public TimeOnly? GetTimeOnlyOrNull(string name) => GetTimeOnlyOrNull(UnderlyingReader.GetOrdinal(name));
#endif

    public decimal GetDecimal(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetDecimal(ordinal);
    }
    
    public double GetDouble(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetDouble(ordinal);
    }
    
    public Type GetFieldType(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetFieldType(ordinal);
    }
    
    public float GetFloat(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetFloat(ordinal);
    }
    
    public Guid GetGuid(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetGuid(ordinal);
    }
    
    public short GetInt16(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetInt16(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt16 GetUInt16(string columnName)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return GetUInt16(ordinal);
    }

    public int GetInt32(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetInt32(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt32 GetUInt32(string columnName)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return GetUInt32(ordinal);
    }

    public long GetInt64(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return GetInt64(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt64 GetUInt64(string columnName)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return GetUInt64(ordinal);
    }

    public string GetString(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.GetString(ordinal);
    }
    
    public object GetValue(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? null : UnderlyingReader.GetValue(ordinal);
    }

    #endregion

    #region Convenience Functions

    /// <summary>
    /// Gets the column ordinal given the name of the column.
    /// Returns -1 if the column does not exist.
    /// This is a "safe" version that will not throw an IndexOutOfRangeException.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The zero-based column ordinal.</returns>
    public int GetSafeOrdinal(string name)
    {
        try
        {
            return UnderlyingReader.GetOrdinal(name);
        }
        catch (IndexOutOfRangeException)
        {
            return -1;
        }
    }

    public bool? GetBooleanOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (bool?)null : UnderlyingReader.GetBoolean(ordinal);
    }

    public bool? GetBooleanOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (bool?)null : UnderlyingReader.GetBoolean(ordinal);
    }

    public bool GetBooleanOrDefault(int ordinal, bool defaultValue)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? defaultValue : UnderlyingReader.GetBoolean(ordinal);
    }

    public bool GetBooleanOrDefault(string columnName, bool defaultValue)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? defaultValue : UnderlyingReader.GetBoolean(ordinal);
    }

    public byte? GetByteOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (byte?)null : UnderlyingReader.GetByte(ordinal);
    }

    public byte? GetByteOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (byte?)null : UnderlyingReader.GetByte(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public sbyte? GetSByteOrNull(int ordinal)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (sbyte?)null : GetSByte(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public sbyte? GetSByteOrNull(string columnName)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (sbyte?)null : GetSByte(ordinal);
    }

    public Int16? GetInt16OrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (Int16?)null : UnderlyingReader.GetInt16(ordinal);
    }

    public Int16? GetInt16OrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (Int16?)null : UnderlyingReader.GetInt16(ordinal);
    }

    public int? GetInt32OrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (int?)null : UnderlyingReader.GetInt32(ordinal);
    }

    public int? GetInt32OrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (int?)null : UnderlyingReader.GetInt32(ordinal);
    }

    [Obsolete("Use GetInt32OrNull(ordinal) ?? 0")]
    public int GetInt32OrZero(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetInt32(ordinal);
    }

    [Obsolete("Use GetInt32OrNull(columnName) ?? 0")]
    public int GetInt32OrZero(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetInt32(ordinal);
    }

    public Int64? GetInt64OrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (Int64?)null : UnderlyingReader.GetInt64(ordinal);
    }

    public Int64? GetInt64OrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (Int64?)null : UnderlyingReader.GetInt64(ordinal);
    }

    [Obsolete("Use GetInt64OrZero(ordinal) ?? 0")]
    public Int64 GetInt64OrZero(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetInt64(ordinal);
    }

    [Obsolete("Use GetInt64OrZero(columnName) ?? 0")]
    public Int64 GetInt64OrZero(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetInt64(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt16? GetUInt16OrNull(int ordinal)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (UInt16?)null : GetUInt16(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt16? GetUInt16OrNull(string columnName)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (UInt16?)null : GetUInt16(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt32? GetUInt32OrNull(int ordinal)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (UInt32?)null : GetUInt32(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt32? GetUInt32OrNull(string columnName)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (UInt32?)null : GetUInt32(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt64? GetUInt64OrNull(int ordinal)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (UInt64?)null : GetUInt64(ordinal);
    }

#pragma warning disable CS3002 // Return type is not CLS-compliant
    public UInt64? GetUInt64OrNull(string columnName)
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (UInt64?)null : GetUInt64(ordinal);
    }

    public string GetStringOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? null : UnderlyingReader.GetString(ordinal);
    }

    public string GetStringOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? null : UnderlyingReader.GetString(ordinal);
    }

    public string GetStringOrEmpty(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? String.Empty : UnderlyingReader.GetString(ordinal);
    }

    public string GetStringOrEmpty(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? String.Empty : UnderlyingReader.GetString(ordinal);
    }

    public DateTime GetDateTimeLocal(string columnName)
    {
        var date = GetDateTime(columnName);
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Local);
        }
        return date;
    }

    public DateTime GetDateTimeUtc(string columnName)
    {
        var date = GetDateTime(columnName);
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
        return date;
    }

    public DateTime GetDateTimeLocal(int ordinal)
    {
        var date = GetDateTime(ordinal);
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Local);
        }
        return date;
    }

    public DateTime GetDateTimeUtc(int ordinal)
    {
        var date = GetDateTime(ordinal);
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
        return date;
    }

    public DateTime? GetDateTimeOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? null : (DateTime?)UnderlyingReader.GetDateTime(ordinal);
    }

    public DateTime? GetDateTimeOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? null : (DateTime?)UnderlyingReader.GetDateTime(ordinal);
    }

    public DateTime GetDateTimeOrMinValue(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? DateTime.MinValue : UnderlyingReader.GetDateTime(ordinal);
    }

    public DateTime GetDateTimeOrMinValue(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? DateTime.MinValue : UnderlyingReader.GetDateTime(ordinal);
    }

    public DateTime? GetDateTimeLocalOrNull(int ordinal)
    {
        var date = GetDateTimeOrNull(ordinal);
        if (date != null && date.Value.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date.Value, DateTimeKind.Local);
        }
        return date;
    }

    public DateTime? GetDateTimeUtcOrNull(int ordinal)
    {
        var date = GetDateTimeOrNull(ordinal);
        if (date != null && date.Value.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
        }
        return date;
    }

    public DateTime? GetDateTimeLocalOrNull(string columnName)
    {
        var date = GetDateTimeOrNull(columnName);
        if (date != null && date.Value.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date.Value, DateTimeKind.Local);
        }
        return date;
    }

    public DateTime? GetDateTimeUtcOrNull(string columnName)
    {
        var date = GetDateTimeOrNull(columnName);
        if (date != null && date.Value.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
        }
        return date;
    }

    public DateTime GetDateTimeLocalOrMinValue(int ordinal)
    {
        var date = GetDateTimeOrMinValue(ordinal);
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Local);
        }
        return date;
    }

    public DateTime GetDateTimeUtcOrMinValue(int ordinal)
    {
        var date = GetDateTimeOrMinValue(ordinal);
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
        return date;
    }

    public DateTime GetDateTimeLocalOrMinValue(string columnName)
    {
        var date = GetDateTimeOrMinValue(columnName);
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Local);
        }
        return date;
    }

    public DateTime GetDateTimeUtcOrMinValue(string columnName)
    {
        var date = GetDateTimeOrMinValue(columnName);
        if (date.Kind == DateTimeKind.Unspecified)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
        return date;
    }

    public TimeSpan? GetTimeSpanOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? null : (TimeSpan?)GetTimeSpan(ordinal);
    }

    public TimeSpan? GetTimeSpanOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? null : (TimeSpan?)GetTimeSpan(ordinal);
    }

#if NET6_0_OR_GREATER
    private static Regex DATE_ONLY_RGX = new Regex("^(\\d{1,4})-(\\d{1,2})-(\\d{1,2})$", RegexOptions.Compiled);

    public DateOnly GetDateOnly(int ordinal)
    {
        var value = UnderlyingReader.GetValue(ordinal);
        if (value is DateTime dt)
        {
            return DateOnly.FromDateTime(dt);
        }
        else if (value is string str)
        {
            var match = DATE_ONLY_RGX.Match(str);
            if (match?.Success == true)
            {
                return new DateOnly(
                    Int32.Parse(match.Groups[1].Value),
                    Int32.Parse(match.Groups[2].Value),
                    Int32.Parse(match.Groups[3].Value)
                );
            }
        }
        
        return (DateOnly)value;
    }
    
	    public DateOnly? GetDateOnlyOrNull(int ordinal)
    {
        if (UnderlyingReader.IsDBNull(ordinal))
            return null;

        var value = UnderlyingReader.GetValue(ordinal);
        if (value is DateTime dt)
        {
            return DateOnly.FromDateTime(dt);
        }
        else if (value is string str)
        {
            var match = DATE_ONLY_RGX.Match(str);
            if (match?.Success == true)
            {
                return new DateOnly(
                    Int32.Parse(match.Groups[1].Value),
                    Int32.Parse(match.Groups[2].Value),
                    Int32.Parse(match.Groups[3].Value)
                );
            }
        }

        return (DateOnly)value;
    }

	    public TimeOnly GetTimeOnly(int ordinal)
    {
        var value = UnderlyingReader.GetValue(ordinal);
        if (value is TimeSpan ts)
            return TimeOnly.FromTimeSpan(ts);
        if (value is DateTime dt)
            return TimeOnly.FromDateTime(dt);

        return (TimeOnly)value;
    }

	    public TimeOnly? GetTimeOnlyOrNull(int ordinal)
    {
        if (UnderlyingReader.IsDBNull(ordinal))
            return null;

        var value = UnderlyingReader.GetValue(ordinal);
        if (value is TimeSpan ts)
            return TimeOnly.FromTimeSpan(ts);
        if (value is DateTime dt)
            return TimeOnly.FromDateTime(dt);

        return (TimeOnly)value;
    }
#endif

    public float? GetFloatOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (float?)null : UnderlyingReader.GetFloat(ordinal);
    }

    public float? GetFloatOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (float?)null : UnderlyingReader.GetFloat(ordinal);
    }

    [Obsolete("Use GetFloatOrZero(ordinal) ?? 0")]
    public float GetFloatOrZero(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetFloat(ordinal);
    }

    [Obsolete("Use GetFloatOrZero(columnName) ?? 0")]
    public float GetFloatOrZero(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetFloat(ordinal);
    }

    public Guid? GetGuidOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (Guid?)null : UnderlyingReader.GetGuid(ordinal);
    }

    public Guid? GetGuidOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (Guid?)null : UnderlyingReader.GetGuid(ordinal);
    }

    public double? GetDoubleOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (double?)null : UnderlyingReader.GetDouble(ordinal);
    }

    public double? GetDoubleOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (double?)null : UnderlyingReader.GetDouble(ordinal);
    }

    [Obsolete("Use GetDoubleOrZero(ordinal) ?? 0")]
    public double GetDoubleOrZero(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetDouble(ordinal);
    }

    [Obsolete("Use GetDoubleOrZero(columnName) ?? 0")]
    public double GetDoubleOrZero(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetDouble(ordinal);
    }

    public decimal? GetDecimalOrNull(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? (decimal?)null : UnderlyingReader.GetDecimal(ordinal);
    }

    public decimal? GetDecimalOrNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? (decimal?)null : UnderlyingReader.GetDecimal(ordinal);
    }

    [Obsolete("Use GetDecimalOrZero(ordinal) ?? 0")]
    public decimal GetDecimalOrZero(int ordinal)
    {
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetDecimal(ordinal);
    }

    [Obsolete("Use GetDecimalOrZero(columnName) ?? 0")]
    public decimal GetDecimalOrZero(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal) ? 0 : UnderlyingReader.GetDecimal(ordinal);
    }

    public bool IsDBNull(string columnName)
    {
        var ordinal = UnderlyingReader.GetOrdinal(columnName);
        return UnderlyingReader.IsDBNull(ordinal);
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

    /// <summary>
    /// This is a synonym for <see cref="GetName(int)"/>,
    /// as sometimes the developer looks for it and can't find it. As he looks for a GetColumnName(...)
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public string GetColumnName(int ordinal)
    {
        return UnderlyingReader.GetName(ordinal);
    }

    /// <summary>
    /// This is a synonym for <see cref="GetOrdinal(string)"/>,
    /// as sometimes the developer looks for it and can't find it. As he looks for a GetColumnIndex(...)
    /// </summary>
    /// <param name="columnName"></param>
    /// <returns></returns>
    public int GetColumnIndex(string columnName)
    {
        return UnderlyingReader.GetOrdinal(columnName);
    }

    #endregion

    #region Geometry

    /// <summary>
    /// Gets the value of the specified column in Geometry type given the column index.
    /// </summary>
    /// <param name="columnIndex">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column in Geometry type.</returns>
    /// <exception cref="IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount</exception>
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
    /// <param name="columnName">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column in Geometry type.</returns>
    /// <exception cref="IndexOutOfRangeException">No column with the specified name was found</exception>
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
