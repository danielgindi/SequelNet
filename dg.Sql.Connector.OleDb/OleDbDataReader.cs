using System;
using System.Collections.Generic;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
{
    public class OleDbDataReader : DataReaderBase
    {
        System.Data.OleDb.OleDbDataReader _reader = null;
        protected ConnectorBase _connectionToClose = null;
        public OleDbDataReader(System.Data.OleDb.OleDbDataReader reader)
        {
            if (_reader != null) _reader.Dispose();
            _reader = reader;
        }
        public OleDbDataReader(System.Data.OleDb.OleDbDataReader reader, ConnectorBase connectionToClose)
        {
            if (_reader != null) _reader.Dispose();
            if (_connectionToClose != null) _connectionToClose.Dispose();
            _reader = reader;
            _connectionToClose = connectionToClose;
        }
        ~OleDbDataReader()
        {
            Dispose(false);
        }
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
            // Now clean up Native Resources (Pointers)
        }
        public override void Close()
        {
            if (_reader != null) _reader.Dispose();
            if (_connectionToClose != null) _connectionToClose.Dispose();
        }
        public override bool Read()
        {
            return _reader.Read();
        }
        public override bool IsDBNull(int i)
        {
            return _reader.IsDBNull(i);
        }
        public override int GetInt32(int i)
        {
            return _reader.GetInt32(i);
        }
        public override int GetInt32OrZero(int i)
        {
            return _reader.IsDBNull(i) ? 0 : _reader.GetInt32(i);
        }
        public override Int64 GetInt64(int i)
        {
            return _reader.GetInt64(i);
        }
        public override Int64 GetInt64OrZero(int i)
        {
            return _reader.IsDBNull(i) ? 0 : _reader.GetInt64(i);
        }
        public override bool GetBoolean(int i)
        {
            return _reader.GetBoolean(i);
        }
        public override string GetString(int i)
        {
            return _reader.GetString(i);
        }
        public override string GetStringOrNull(int i)
        {
            return _reader.IsDBNull(i) ? null : _reader.GetString(i);
        }
        public override string GetStringOrEmpty(int i)
        {
            return _reader.IsDBNull(i) ? String.Empty : _reader.GetString(i);
        }
        public override DateTime GetDateTime(int i)
        {
            return _reader.IsDBNull(i) ? DateTime.FromBinary(0) : _reader.GetDateTime(i);
        }
        public override DateTime? GetDateTimeOrNull(int i)
        {
            return _reader.IsDBNull(i) ? null : (DateTime?)_reader.GetDateTime(i);
        }
        public override DateTime GetDateTimeOrMinValue(int i)
        {
            return _reader.IsDBNull(i) ? DateTime.MinValue : _reader.GetDateTime(i);
        }
        public override decimal GetDecimal(int i)
        {
            return _reader.GetDecimal(i);
        }
        public override decimal GetDecimalOrZero(int i)
        {
            return _reader.IsDBNull(i) ? 0 : _reader.GetDecimal(i);
        }
        public override bool HasColumn(string name)
        {
            for (int j = 0, len = _reader.VisibleFieldCount; j < len; j++)
            {
                if (_reader.GetName(j).Equals(name, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
        public override Int32 GetColumnCount()
        {
            return _reader.VisibleFieldCount;
        }
        public override string GetColumnName(Int32 ColumnIndex)
        {
            return _reader.GetName(ColumnIndex);
        }
        public override object this[int i]
        {
            get { return _reader[i]; }
        }
        public override object this[string name]
        {
            get { return _reader[name]; }
        }

        public override Geometry GetGeometry(int i)
        {
            byte[] geometryData = _reader[i] as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, false);
            }
            return null;
        }
        public override Geometry GetGeometry(string name)
        {
            byte[] geometryData = _reader[name] as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, false);
            }
            return null;
        }
    }
}
