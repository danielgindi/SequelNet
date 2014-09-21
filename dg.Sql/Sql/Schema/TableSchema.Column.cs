using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    public partial class TableSchema
    {
        public class ColumnList : List<Column> 
        {
            public Column Find(string columnName)
            {
                if (this == null) return null;
                foreach (Column col in this)
                {
                    if (col.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase)) return col;
                }
                return null;
            }
        }
        public class Column
        {
            public string Name;
            private System.Type _Type;
            private DataType _DataType;
            private DataType _ActualDataType;
            public bool AutoIncrement;
            public object Default;
            public bool Nullable;
            public bool IsPrimaryKey;
            private int _MaxLength;
            public int NumberPrecision;
            public int NumberScale;

            public Column() { }
            public Column(string Name, System.Type Type, int MaxLength, int NumberPrecision, int NumberScale, bool AutoIncrement, bool IsPrimaryKey, bool Nullable, object Default)
            {
                this.Name = Name;
                this.Type = Type;
                this._MaxLength = MaxLength;
                this.NumberPrecision = NumberPrecision;
                this.NumberScale = NumberScale;
                this.AutoIncrement = AutoIncrement;
                this.IsPrimaryKey = IsPrimaryKey;
                this.Nullable = Nullable;
                this.Default = Default;
                this._ActualDataType = GetDataType();
            }
            public Column(string Name, System.Type Type, DataType DataType, int MaxLength, int NumberPrecision, int NumberScale, bool AutoIncrement, bool IsPrimaryKey, bool Nullable, object Default)
            {
                this.Name = Name;
                this.Type = Type;
                this._DataType = DataType;
                this._MaxLength = MaxLength;
                this.NumberPrecision = NumberPrecision;
                this.NumberScale = NumberScale;
                this.AutoIncrement = AutoIncrement;
                this.IsPrimaryKey = IsPrimaryKey;
                this.Nullable = Nullable;
                this.Default = Default;
                this._ActualDataType = GetDataType();
            }

            public System.Type Type
            {
                get { return this._Type; }
                set
                {
                    this._Type = value;
                    this._ActualDataType = GetDataType();
                }
            }
            public DataType DataType
            {
                get { return this._DataType; }
                set
                {
                    this._DataType = value;
                    this._ActualDataType = GetDataType();
                }
            }
            public DataType ActualDataType
            {
                get { return this._ActualDataType; }
            }
            public int MaxLength
            {
                set { _MaxLength = value; _ActualDataType = GetDataType(); }
                get { return _MaxLength; }
            }

            private DataType GetDataType()
            {
                if (this._DataType != DataType.Automatic) return _DataType;
                if (this.Type.Equals(typeof(string)))
                {
                    if (this.MaxLength > 0) return DataType.VarChar;
                    return DataType.Text;
                }
                else if (this.Type.Equals(typeof(float)))
                {
                    return DataType.Float;
                }
                else if (this.Type.Equals(typeof(double)))
                {
                    return DataType.Double;
                }
                else if (this.Type.Equals(typeof(decimal)))
                {
                    return DataType.Decimal;
                }
                else if (this.Type.Equals(typeof(bool)))
                {
                    return DataType.Boolean;
                }
                else if (this.Type.Equals(typeof(DateTime)))
                {
                    return DataType.DateTime;
                }
                else if (this.Type.Equals(typeof(Guid)))
                {
                    return DataType.Guid;
                }
                else if (this.Type.Equals(typeof(object)) || this.Type.Equals(typeof(byte[])))
                {
                    return DataType.Blob;
                }
                else if (this.Type.Equals(typeof(Byte)) || this.Type.Equals(typeof(SByte)))
                {
                    return DataType.TinyInt;
                }
                else if (this.Type.Equals(typeof(Int16)))
                {
                    return DataType.SmallInt;
                }
                else if (this.Type.Equals(typeof(UInt16)))
                {
                    return DataType.UnsignedSmallInt;
                }
                else if (this.Type.Equals(typeof(Int32)))
                {
                    return DataType.Int;
                }
                else if (this.Type.Equals(typeof(UInt32)))
                {
                    return DataType.UnsignedInt;
                }
                else if (this.Type.Equals(typeof(Int64)))
                {
                    return DataType.BigInt;
                }
                else if (this.Type.Equals(typeof(UInt64)))
                {
                    return DataType.UnsignedBigInt;
                }
                else if (this.Type.Equals(typeof(Geometry.Point)))
                {
                    return DataType.Point;
                }
                else if (this.Type.Equals(typeof(Geometry.LineString)))
                {
                    return DataType.LineString;
                }
                else if (this.Type.Equals(typeof(Geometry.Polygon)))
                {
                    return DataType.Polygon;
                }
                else if (this.Type.Equals(typeof(Geometry.MultiPoint)))
                {
                    return DataType.MultiPoint;
                }
                else if (this.Type.Equals(typeof(Geometry.MultiLineString)))
                {
                    return DataType.MultiLineString;
                }
                else if (this.Type.Equals(typeof(Geometry.MultiPolygon)))
                {
                    return DataType.MultiPolygon;
                }
                else if (this.Type.Equals(typeof(Geometry.GeometryCollection<>)))
                {
                    return DataType.GeometryCollection;
                }
                else if (this.Type.Equals(typeof(Geometry)))
                {
                    return DataType.Geometry;
                }
                else return DataType.Int;
            }
        }
    }
}
