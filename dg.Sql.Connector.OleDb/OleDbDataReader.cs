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
        #region Instancing

        System.Data.OleDb.OleDbDataReader _Reader = null;
        protected ConnectorBase _ConnectionToClose = null;

        public OleDbDataReader(System.Data.OleDb.OleDbDataReader Reader)
        {
            _Reader = Reader;
        }

        public OleDbDataReader(System.Data.OleDb.OleDbDataReader Reader, ConnectorBase ConnectionToClose)
        {
            _Reader = Reader;
            _ConnectionToClose = ConnectionToClose;
        }

        #endregion

        #region IDisposable

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

        ~OleDbDataReader()
        {
            Dispose(false);
        }

        #endregion

        public override void Close()
        {
            if (_Reader != null) _Reader.Dispose();
            if (_ConnectionToClose != null) _ConnectionToClose.Dispose();
        }

        public override bool Read()
        {
            return _Reader.Read();
        }

        public override bool IsDBNull(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex);
        }

        public override int GetInt32(int columnIndex)
        {
            return _Reader.GetInt32(columnIndex);
        }

        public override int GetInt32OrZero(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex) ? 0 : _Reader.GetInt32(columnIndex);
        }

        public override Int64 GetInt64(int columnIndex)
        {
            return _Reader.GetInt64(columnIndex);
        }

        public override Int64 GetInt64OrZero(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex) ? 0 : _Reader.GetInt64(columnIndex);
        }

        public override bool GetBoolean(int columnIndex)
        {
            return _Reader.GetBoolean(columnIndex);
        }

        public override string GetString(int columnIndex)
        {
            return _Reader.GetString(columnIndex);
        }

        public override string GetStringOrNull(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex) ? null : _Reader.GetString(columnIndex);
        }

        public override string GetStringOrEmpty(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex) ? String.Empty : _Reader.GetString(columnIndex);
        }

        public override DateTime GetDateTime(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex) ? DateTime.FromBinary(0) : _Reader.GetDateTime(columnIndex);
        }

        public override DateTime? GetDateTimeOrNull(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex) ? null : (DateTime?)_Reader.GetDateTime(columnIndex);
        }

        public override DateTime GetDateTimeOrMinValue(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex) ? DateTime.MinValue : _Reader.GetDateTime(columnIndex);
        }

        public override decimal GetDecimal(int columnIndex)
        {
            return _Reader.GetDecimal(columnIndex);
        }

        public override decimal GetDecimalOrZero(int columnIndex)
        {
            return _Reader.IsDBNull(columnIndex) ? 0 : _Reader.GetDecimal(columnIndex);
        }
        public override bool HasColumn(string name)
        {
            for (int j = 0, len = _Reader.VisibleFieldCount; j < len; j++)
            {
                if (_Reader.GetName(j).Equals(name, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public override Int32 GetColumnCount()
        {
            return _Reader.VisibleFieldCount;
        }

        public override string GetColumnName(Int32 columnIndex)
        {
            return _Reader.GetName(columnIndex);
        }

        public override object this[int columnIndex]
        {
            get { return _Reader[columnIndex]; }
        }

        public override object this[string columnName]
        {
            get { return _Reader[columnName]; }
        }

        public override Geometry GetGeometry(int columnIndex)
        {
            byte[] geometryData = _Reader[columnIndex] as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, false);
            }
            return null;
        }

        public override Geometry GetGeometry(string columnName)
        {
            byte[] geometryData = _Reader[columnName] as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, false);
            }
            return null;
        }
    }
}
