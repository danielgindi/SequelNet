using System.Runtime.InteropServices;

namespace SequelNet
{
    public enum DataType : int
    {
        /// <summary>
        /// Automatic
        /// </summary>
        None = 0,

        /// <summary>
        /// Automatic, resolving from the Type
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Boolean
        /// </summary>
        Boolean,

        /// <summary>
        /// DateTime
        /// </summary>
        DateTime,

        /// <summary>
        /// Date
        /// </summary>
        Date,

        /// <summary>
        /// Time
        /// </summary>
        Time,

        /// <summary>
        /// Guid
        /// </summary>
        Guid,

        /// <summary>
        /// byte[]
        /// </summary>
        Blob,

        /// <summary>
        /// Float
        /// </summary>
        Float,

        /// <summary>
        /// Double
        /// </summary>
        Double,

        /// <summary>
        /// Decimal
        /// </summary>
        Decimal,

        /// <summary>
        /// Money
        /// </summary>
        Money,

        /// <summary>
        /// Numeric
        /// </summary>
        Numeric,

        /// <summary>
        /// SByte
        /// </summary>
        TinyInt,

        /// <summary>
        /// Byte
        /// </summary>
        UnsignedTinyInt,

        /// <summary>
        /// Int16
        /// </summary>
        SmallInt,

        /// <summary>
        /// UInt16
        /// </summary>
        UnsignedSmallInt,

        /// <summary>
        /// Int32
        /// </summary>
        Int,

        /// <summary>
        /// UInt32
        /// </summary>
        UnsignedInt,

        /// <summary>
        /// Int64
        /// </summary>
        BigInt,

        /// <summary>
        /// UInt64
        /// </summary>
        UnsignedBigInt,

        /// <summary>
        /// String with size limit, up to 255. Larger than that - will use Text and truncate locally.
        /// </summary>
        VarChar,

        /// <summary>
        /// String with fixed size, up to 255.
        /// </summary>
        Char,

        /// <summary>
        /// String (up to 65,536 bytes)
        /// </summary>
        Text,

        /// <summary>
        /// String (up to 16,777,215 bytes - ~16MB)
        /// </summary>
        MediumText,

        /// <summary>
        /// String (up to 4,294,967,296 bytes - 4GB)
        /// </summary>
        LongText,

        /// <summary>
        /// A JSON column
        /// </summary>
        Json,

        /// <summary>
        /// A JSON (in binary storage format) column.
        /// This may have performance improvements on some DBs.
        /// And may support a different subset of JSON operations.
        /// </summary>
        JsonBinary,

        /// <summary>
        /// Geometry base class
        /// </summary>
        Geometry,

        /// <summary>
        /// Geometry collection
        /// </summary>
        GeometryCollection,

        /// <summary>
        /// Point
        /// </summary>
        Point,

        /// <summary>
        /// Line string which is two or more points
        /// </summary>
        LineString,

        /// <summary>
        /// Polygon which is two or more points
        /// </summary>
        Polygon,

        /// <summary>
        /// Line
        /// </summary>
        Line,

        /// <summary>
        /// Curve
        /// </summary>
        Curve,

        /// <summary>
        /// Surface
        /// </summary>
        Surface,

        /// <summary>
        /// Linear ring
        /// </summary>
        LinearRing,

        /// <summary>
        /// Point collection
        /// </summary>
        MultiPoint,

        /// <summary>
        /// LineString collection
        /// </summary>
        MultiLineString,

        /// <summary>
        /// Polygon collection
        /// </summary>
        MultiPolygon,

        /// <summary>
        /// Curve collection
        /// </summary>
        MultiCurve,

        /// <summary>
        /// Surface collection
        /// </summary>
        MultiSurface,

        /// <summary>
        /// Geographic base class
        /// </summary>
        Geographic,

        /// <summary>
        /// Geographic collection
        /// </summary>
        GeographicCollection,

        /// <summary>
        /// Geographic Point
        /// </summary>
        GeographicPoint,

        /// <summary>
        /// Geographic Line string which is two or more points
        /// </summary>
        GeographicLineString,

        /// <summary>
        /// Geographic Polygon which is two or more points
        /// </summary>
        GeographicPolygon,

        /// <summary>
        /// Geographic Line
        /// </summary>
        GeographicLine,

        /// <summary>
        /// Geographic Curve
        /// </summary>
        GeographicCurve,

        /// <summary>
        /// Geographic Surface
        /// </summary>
        GeographicSurface,

        /// <summary>
        /// Geographic Linear ring
        /// </summary>
        GeographicLinearRing,

        /// <summary>
        /// Geographic Point collection
        /// </summary>
        GeographicMultiPoint,

        /// <summary>
        /// Geographic LineString collection
        /// </summary>
        GeographicMultiLineString,

        /// <summary>
        /// Geographic Polygon collection
        /// </summary>
        GeographicMultiPolygon,

        /// <summary>
        /// Geographic Curve collection
        /// </summary>
        GeographicMultiCurve,

        /// <summary>
        /// Geographic Surface collection
        /// </summary>
        GeographicMultiSurface,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DataTypeDef
    {
        [FieldOffset(0)]
        public DataType Type;

        [FieldOffset(4)]
        public int MaxLength;

        /// <summary>
        /// Number of digits in the number (excluding the decimal period)
        /// </summary>
        [FieldOffset(4)]
        public short Precision;

        /// <summary>
        /// Number of digits to the right of the decimal period
        /// </summary>
        [FieldOffset(6)]
        public short Scale;

        [FieldOffset(4)]
        public int? SRID;
    }
}
