using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Text;
using dg.Sql.Sql.Spatial;

namespace dg.Sql.Connector
{
    public class MySqlDataReader : DataReaderBase
    {
        #region Instancing

        MySql.Data.MySqlClient.MySqlDataReader _Reader = null;
        protected ConnectorBase _ConnectionToClose = null;

        public MySqlDataReader(MySql.Data.MySqlClient.MySqlDataReader Reader)
        {
            _Reader = Reader;
        }
        public MySqlDataReader(MySql.Data.MySqlClient.MySqlDataReader Reader, ConnectorBase ConnectionToClose)
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
        ~MySqlDataReader()
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
        public override bool IsDBNull(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex);
        }
        public override int GetInt32(int ColumnIndex)
        {
            return _Reader.GetInt32(ColumnIndex);
        }
        public override int GetInt32OrZero(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex) ? 0 : _Reader.GetInt32(ColumnIndex);
        }
        public override Int64 GetInt64(int ColumnIndex)
        {
            return _Reader.GetInt64(ColumnIndex);
        }
        public override Int64 GetInt64OrZero(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex) ? 0 : _Reader.GetInt64(ColumnIndex);
        }
        public override bool GetBoolean(int ColumnIndex)
        {
            return _Reader.GetBoolean(ColumnIndex);
        }
        public override string GetString(int ColumnIndex)
        {
            return _Reader.GetString(ColumnIndex);
        }
        public override string GetStringOrNull(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex) ? null : _Reader.GetString(ColumnIndex);
        }
        public override string GetStringOrEmpty(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex) ? String.Empty : _Reader.GetString(ColumnIndex);
        }
        public override DateTime GetDateTime(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex) ? DateTime.FromBinary(0) : _Reader.GetDateTime(ColumnIndex);
        }
        public override DateTime? GetDateTimeOrNull(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex) ? null : (DateTime?)_Reader.GetDateTime(ColumnIndex);
        }
        public override DateTime GetDateTimeOrMinValue(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex) ? DateTime.MinValue : _Reader.GetDateTime(ColumnIndex);
        }
        public override decimal GetDecimal(int ColumnIndex)
        {
            return _Reader.GetDecimal(ColumnIndex);
        }
        public override decimal GetDecimalOrZero(int ColumnIndex)
        {
            return _Reader.IsDBNull(ColumnIndex) ? 0 : _Reader.GetDecimal(ColumnIndex);
        }
        public override bool HasColumn(string ColumnName)
        {
            for (int j = 0, len = _Reader.VisibleFieldCount; j < len; j++)
            {
                if (_Reader.GetName(j).Equals(ColumnName, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
        public override Int32 GetColumnCount()
        {
            return _Reader.VisibleFieldCount;
        }
        public override string GetColumnName(Int32 ColumnIndex)
        {
            return _Reader.GetName(ColumnIndex);
        }
        public override object this[int ColumnIndex]
        {
            get { return _Reader[ColumnIndex]; }
        }
        public override object this[string ColumnName]
        {
            get { return _Reader[ColumnName]; }
        }

        public override Geometry GetGeometry(int ColumnIndex)
        {
            byte[] geometryData = _Reader[ColumnIndex] as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, true);
            }
            return null;
        }
        public override Geometry GetGeometry(string ColumnName)
        {
            byte[] geometryData = _Reader[ColumnName] as byte[];
            if (geometryData != null)
            {
                return WkbReader.GeometryFromWkb(geometryData, true);
            }
            return null;
        }
    }
}
