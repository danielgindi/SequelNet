using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SequelNet.Sql.Spatial
{
    public static class WkbReader
    {
        public enum WkbByteOrder
        {
            BigEndian = 0,
            LittleEndian = 1
        };
        public enum WkbCoordinateSystem
        { 
            XY = 1,
            XYZ = 2, 
            XYM = 3,
            XYZM = 4 
        }
        public enum WkbGeometryTypes
        {
            /// <summary>
            /// Point.
            /// </summary>
            WkbPoint = 1,

            /// <summary>
            /// LineString.
            /// </summary>
            WkbLineString = 2,

            /// <summary>
            /// Polygon.
            /// </summary>
            WkbPolygon = 3,

            /// <summary>
            /// MultiPoint.
            /// </summary>
            WkbMultiPoint = 4,

            /// <summary>
            /// MultiLineString.
            /// </summary>
            WkbMultiLineString = 5,

            /// <summary>
            /// MultiPolygon.
            /// </summary>
            WkbMultiPolygon = 6,

            /// <summary>
            /// GeometryCollection.
            /// </summary>
            WkbGeometryCollection = 7,

            /// <summary>
            /// Point with Z coordinate.
            /// </summary>
            WkbPointZ = 1001,

            /// <summary>
            /// LineString with Z coordinate.
            /// </summary>
            WkbLineStringZ = 1002,

            /// <summary>
            /// Polygon with Z coordinate.
            /// </summary>
            WkbPolygonZ = 1003,

            /// <summary>
            /// MultiPoint with Z coordinate.
            /// </summary>
            WkbMultiPointZ = 1004,

            /// <summary>
            /// MultiLineString with Z coordinate.
            /// </summary>
            WkbMultiLineStringZ = 1005,

            /// <summary>
            /// MultiPolygon with Z coordinate.
            /// </summary>
            WkbMultiPolygonZ = 1006,

            /// <summary>
            /// GeometryCollection with Z coordinate.
            /// </summary>
            WkbGeometryCollectionZ = 1007,

            /// <summary>
            /// Point with M ordinate value.
            /// </summary>
            WkbPointM = 2001,

            /// <summary>
            /// LineString with M ordinate value.
            /// </summary>
            WkbLineStringM = 2002,

            /// <summary>
            /// Polygon with M ordinate value.
            /// </summary>
            WkbPolygonM = 2003,

            /// <summary>
            /// MultiPoint with M ordinate value.
            /// </summary>
            WkbMultiPointM = 2004,

            /// <summary>
            /// MultiLineString with M ordinate value.
            /// </summary>
            WkbMultiLineStringM = 2005,

            /// <summary>
            /// MultiPolygon with M ordinate value.
            /// </summary>
            WkbMultiPolygonM = 2006,

            /// <summary>
            /// GeometryCollection with M ordinate value.
            /// </summary>
            WkbGeometryCollectionM = 2007,

            /// <summary>
            /// Point with Z coordinate and M ordinate value.
            /// </summary>
            WkbPointZM = 3001,

            /// <summary>
            /// LineString with Z coordinate and M ordinate value.
            /// </summary>
            WkbLineStringZM = 3002,

            /// <summary>
            /// Polygon with Z coordinate and M ordinate value.
            /// </summary>
            WkbPolygonZM = 3003,

            /// <summary>
            /// MultiPoint with Z coordinate and M ordinate value.
            /// </summary>
            WkbMultiPointZM = 3004,

            /// <summary>
            /// MultiLineString with Z coordinate and M ordinate value.
            /// </summary>
            WkbMultiLineStringZM = 3005,

            /// <summary>
            /// MultiPolygon with Z coordinate and M ordinate value.
            /// </summary>
            WkbMultiPolygonZM = 3006,

            /// <summary>
            /// GeometryCollection with Z coordinate and M ordinate value.
            /// </summary>
            WkbGeometryCollectionZM = 3007
        };

        public static Geometry GeometryFromWkb(byte[] data)
        {
            return GeometryFromWkb(data, false);
        }
        public static Geometry GeometryFromWkb(byte[] data, bool beginsWithSRID)
        {
            using (Stream stream = new MemoryStream(data))
            {
                BinaryReader reader = null;
                WkbByteOrder byteOrder = (WkbByteOrder)(beginsWithSRID ? data[4] : stream.ReadByte());

                try
                {
                    if (byteOrder == WkbByteOrder.BigEndian)
                    {
                        reader = new BigEndianBinaryReader(stream);
                    }
                    else
                    {
                        reader = new BinaryReader(stream);
                    }

                    int? SRID = null;
                    if (beginsWithSRID)
                    {
                        SRID = reader.ReadInt32();
                        if (SRID == 0) SRID = null;
                        stream.ReadByte(); // Byte order byte
                    }

                    return Read(reader, SRID);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
        }

        private static Geometry Read(BinaryReader reader, int? PrefixedSRID)
        {
            WkbCoordinateSystem cs;
            int? SRID;
            WkbGeometryTypes geometryType = ReadGeometryType(reader, out cs, out SRID);
            if (PrefixedSRID != null && SRID == null)
            {
                SRID = PrefixedSRID;
            }

            switch (geometryType)
            {
                case WkbGeometryTypes.WkbPoint:
                    return ReadPoint(reader, SRID, WkbCoordinateSystem.XY);
                case WkbGeometryTypes.WkbPointZ:
                    return ReadPoint(reader, SRID, WkbCoordinateSystem.XYZ);
                case WkbGeometryTypes.WkbPointM:
                    return ReadPoint(reader, SRID, WkbCoordinateSystem.XYM);
                case WkbGeometryTypes.WkbPointZM:
                    return ReadPoint(reader, SRID, WkbCoordinateSystem.XYZM);
                    
                case WkbGeometryTypes.WkbLineString:
                    return ReadLineString(reader, SRID, WkbCoordinateSystem.XY);
                case WkbGeometryTypes.WkbLineStringZ:
                    return ReadLineString(reader, SRID, WkbCoordinateSystem.XYZ);
                case WkbGeometryTypes.WkbLineStringM:
                    return ReadLineString(reader, SRID, WkbCoordinateSystem.XYM);
                case WkbGeometryTypes.WkbLineStringZM:
                    return ReadLineString(reader, SRID, WkbCoordinateSystem.XYZM);
                    
                case WkbGeometryTypes.WkbPolygon:
                    return ReadPolygon(reader, SRID, WkbCoordinateSystem.XY);
                case WkbGeometryTypes.WkbPolygonZ:
                    return ReadPolygon(reader, SRID, WkbCoordinateSystem.XYZ);
                case WkbGeometryTypes.WkbPolygonM:
                    return ReadPolygon(reader, SRID, WkbCoordinateSystem.XYM);
                case WkbGeometryTypes.WkbPolygonZM:
                    return ReadPolygon(reader, SRID, WkbCoordinateSystem.XYZM);
                    
                case WkbGeometryTypes.WkbMultiPoint:
                    return ReadMultiPoint(reader, SRID, WkbCoordinateSystem.XY);
                case WkbGeometryTypes.WkbMultiPointZ:
                    return ReadMultiPoint(reader, SRID, WkbCoordinateSystem.XYZ);
                case WkbGeometryTypes.WkbMultiPointM:
                    return ReadMultiPoint(reader, SRID, WkbCoordinateSystem.XYM);
                case WkbGeometryTypes.WkbMultiPointZM:
                    return ReadMultiPoint(reader, SRID, WkbCoordinateSystem.XYZM);
                    
                case WkbGeometryTypes.WkbMultiLineString:
                    return ReadMultiLineString(reader, SRID, WkbCoordinateSystem.XY);
                case WkbGeometryTypes.WkbMultiLineStringZ:
                    return ReadMultiLineString(reader, SRID, WkbCoordinateSystem.XYZ);
                case WkbGeometryTypes.WkbMultiLineStringM:
                    return ReadMultiLineString(reader, SRID, WkbCoordinateSystem.XYM);
                case WkbGeometryTypes.WkbMultiLineStringZM:
                    return ReadMultiLineString(reader, SRID, WkbCoordinateSystem.XYZM);
                    
                case WkbGeometryTypes.WkbMultiPolygon:
                    return ReadMultiPolygon(reader, SRID, WkbCoordinateSystem.XY);
                case WkbGeometryTypes.WkbMultiPolygonZ:
                    return ReadMultiPolygon(reader, SRID, WkbCoordinateSystem.XYZ);
                case WkbGeometryTypes.WkbMultiPolygonM:
                    return ReadMultiPolygon(reader, SRID, WkbCoordinateSystem.XYM);
                case WkbGeometryTypes.WkbMultiPolygonZM:
                    return ReadMultiPolygon(reader, SRID, WkbCoordinateSystem.XYZM);
                    
                case WkbGeometryTypes.WkbGeometryCollection:
                    return ReadGeometryCollection(reader, SRID, WkbCoordinateSystem.XY);
                case WkbGeometryTypes.WkbGeometryCollectionZ:
                    return ReadGeometryCollection(reader, SRID, WkbCoordinateSystem.XYZ);
                case WkbGeometryTypes.WkbGeometryCollectionM:
                    return ReadGeometryCollection(reader, SRID, WkbCoordinateSystem.XYM);
                case WkbGeometryTypes.WkbGeometryCollectionZM:
                    return ReadGeometryCollection(reader, SRID, WkbCoordinateSystem.XYZM);
                default:
                    throw new ArgumentException("Geometry type not recognized. GeometryCode: " + geometryType);
            }
        }

        private static Geometry.Point ReadPoint(BinaryReader reader, int? SRID, WkbCoordinateSystem cs)
        {
            double x = reader.ReadDouble();
            double y = reader.ReadDouble();

            Geometry.Point pt = new Geometry.Point(x, y);
            pt.SRID = SRID;

            switch (cs)
            {
                case WkbCoordinateSystem.XY:
                    break;
                case WkbCoordinateSystem.XYZ:
                    {
                        pt.Z = (ValueWrapper)reader.ReadDouble();
                    }
                    break;
                case WkbCoordinateSystem.XYM:
                    {
                        pt.M = (ValueWrapper)reader.ReadDouble();
                    }
                    break;
                case WkbCoordinateSystem.XYZM:
                    {
                        pt.Z = (ValueWrapper)reader.ReadDouble();
                        pt.M = (ValueWrapper)reader.ReadDouble();
                    }
                    break;
                default:
                    throw new ArgumentException(String.Format("Coordinate system not supported: {0}", cs));
            }

            return pt;
        }
        private static Geometry.LineString ReadLineString(BinaryReader reader, int? SRID, WkbCoordinateSystem cs)
        {
            Int32 pointCount = reader.ReadInt32();
            Geometry.LineString ls = new Geometry.LineString(pointCount);
            ls.SRID = SRID;

            List<Geometry.Point> points = ls.Points;

            for (int j = 0; j < pointCount; j++)
            {
                points.Add(ReadPoint(reader, SRID, cs));
            }

            return ls;
        }
        private static Geometry.Polygon ReadPolygon(BinaryReader reader, int? SRID, WkbCoordinateSystem cs)
        {
            Int32 ringCount = reader.ReadInt32();
            Geometry.Polygon poly = new Geometry.Polygon(ringCount);
            poly.SRID = SRID;

            List<Geometry.LineString> holes = poly.Holes;

            if (ringCount > 0)
            {
                poly.Exterior = ReadLineString(reader, SRID, cs);
            }
            for (int j = 1; j < ringCount; j++)
            {
                holes.Add(ReadLineString(reader, SRID, cs));
            }

            return poly;
        }
        private static Geometry.MultiPoint ReadMultiPoint(BinaryReader reader, int? SRID, WkbCoordinateSystem cs)
        {
            Int32 geometryCount = reader.ReadInt32();
            Geometry.MultiPoint multi = new Geometry.MultiPoint(geometryCount);
            multi.SRID = SRID;

            List<Geometry.Point> points = multi.Geometries;

            WkbGeometryTypes geometryType;
            WkbCoordinateSystem cs2;
            int? SRID2;

            for (int j = 0; j < geometryCount; j++)
            {
                reader.ReadByte(); // Byte order byte
                geometryType = ReadGeometryType(reader, out cs2, out SRID2);
                if (geometryType != WkbGeometryTypes.WkbPoint)
                {
                    throw new ArgumentException("Point expected");
                }
                points.Add(ReadPoint(reader, SRID, cs));
            }

            return multi;
        }
        private static Geometry.MultiLineString ReadMultiLineString(BinaryReader reader, int? SRID, WkbCoordinateSystem cs)
        {
            Int32 geometryCount = reader.ReadInt32();
            Geometry.MultiLineString multi = new Geometry.MultiLineString(geometryCount);
            multi.SRID = SRID;

            List<Geometry.LineString> lines = multi.Geometries;

            WkbGeometryTypes geometryType;
            WkbCoordinateSystem cs2;
            int? SRID2;

            for (int j = 0; j < geometryCount; j++)
            {
                reader.ReadByte(); // Byte order byte
                geometryType = ReadGeometryType(reader, out cs2, out SRID2);
                if (geometryType != WkbGeometryTypes.WkbLineString)
                {
                    throw new ArgumentException("LineString expected");
                }
                lines.Add(ReadLineString(reader, SRID, cs));
            }

            return multi;
        }
        private static Geometry.MultiPolygon ReadMultiPolygon(BinaryReader reader, int? SRID, WkbCoordinateSystem cs)
        {
            Int32 geometryCount = reader.ReadInt32();
            Geometry.MultiPolygon multi = new Geometry.MultiPolygon(geometryCount);
            multi.SRID = SRID;

            List<Geometry.Polygon> polygons = multi.Geometries;

            WkbGeometryTypes geometryType;
            WkbCoordinateSystem cs2;
            int? SRID2;

            for (int j = 0; j < geometryCount; j++)
            {
                reader.ReadByte(); // Byte order byte
                geometryType = ReadGeometryType(reader, out cs2, out SRID2);
                if (geometryType != WkbGeometryTypes.WkbPolygon)
                {
                    throw new ArgumentException("Polygon expected");
                }
                polygons.Add(ReadPolygon(reader, SRID, cs));
            }

            return multi;
        }
        private static Geometry.GeometryCollection<Geometry> ReadGeometryCollection(BinaryReader reader, int? SRID, WkbCoordinateSystem cs)
        {
            Int32 geometryCount = reader.ReadInt32();
            Geometry.GeometryCollection<Geometry> multi = new Geometry.GeometryCollection<Geometry>(geometryCount);
            multi.SRID = SRID;

            List<Geometry> geometries = multi.Geometries;

            WkbGeometryTypes geometryType;
            WkbCoordinateSystem cs2;
            int? SRID2;

            for (int j = 0; j < geometryCount; j++)
            {
                reader.ReadByte(); // Byte order byte
                geometryType = ReadGeometryType(reader, out cs2, out SRID2);
                switch (geometryType)
                {
                    case WkbGeometryTypes.WkbPoint:
                    case WkbGeometryTypes.WkbPointZ:
                    case WkbGeometryTypes.WkbPointM:
                    case WkbGeometryTypes.WkbPointZM:
                        geometries.Add(ReadPoint(reader, SRID2, cs2));
                        break;

                    case WkbGeometryTypes.WkbLineString:
                    case WkbGeometryTypes.WkbLineStringZ:
                    case WkbGeometryTypes.WkbLineStringM:
                    case WkbGeometryTypes.WkbLineStringZM:
                        geometries.Add(ReadLineString(reader, SRID2, cs2));
                        break;

                    case WkbGeometryTypes.WkbPolygon:
                    case WkbGeometryTypes.WkbPolygonZ:
                    case WkbGeometryTypes.WkbPolygonM:
                    case WkbGeometryTypes.WkbPolygonZM:
                        geometries.Add(ReadPolygon(reader, SRID2, cs2));
                        break;

                    case WkbGeometryTypes.WkbMultiPoint:
                    case WkbGeometryTypes.WkbMultiPointZ:
                    case WkbGeometryTypes.WkbMultiPointM:
                    case WkbGeometryTypes.WkbMultiPointZM:
                        geometries.Add(ReadMultiPoint(reader, SRID2, cs2));
                        break;

                    case WkbGeometryTypes.WkbMultiLineString:
                    case WkbGeometryTypes.WkbMultiLineStringZ:
                    case WkbGeometryTypes.WkbMultiLineStringM:
                    case WkbGeometryTypes.WkbMultiLineStringZM:
                        geometries.Add(ReadMultiLineString(reader, SRID2, cs2));
                        break;

                    case WkbGeometryTypes.WkbMultiPolygon:
                        geometries.Add(ReadMultiPolygon(reader, SRID2, cs2));
                        break;

                    case WkbGeometryTypes.WkbGeometryCollection:
                    case WkbGeometryTypes.WkbGeometryCollectionZ:
                    case WkbGeometryTypes.WkbGeometryCollectionM:
                    case WkbGeometryTypes.WkbGeometryCollectionZM:
                        geometries.Add(ReadGeometryCollection(reader, SRID2, cs2));
                        break;

                    default:
                        throw new ArgumentException("Geometry type not recognized. GeometryCode: " + geometryType);
                }
            }

            return multi;
        }

        private static WkbGeometryTypes ReadGeometryType(BinaryReader reader, out WkbCoordinateSystem coordinateSystem, out Int32? SRID)
        {
            UInt32 type = reader.ReadUInt32();
            
            if ((type & (0x80000000 | 0x40000000)) == (0x80000000 | 0x40000000))
                coordinateSystem = WkbCoordinateSystem.XYZM;
            else if ((type & 0x80000000) == 0x80000000)
                coordinateSystem = WkbCoordinateSystem.XYZ;
            else if ((type & 0x40000000) == 0x40000000)
                coordinateSystem = WkbCoordinateSystem.XYM;
            else
                coordinateSystem = WkbCoordinateSystem.XY;

            if ((type & 0x20000000) != 0)
            {
                SRID = reader.ReadInt32();
            }
            else
            {
                SRID = null;
            }

            UInt32 ordinate = (type & 0xffff) / 1000;
            switch (ordinate)
            {
                case 1:
                    coordinateSystem = WkbCoordinateSystem.XYZ;
                    break;
                case 2:
                    coordinateSystem = WkbCoordinateSystem.XYM;
                    break;
                case 3:
                    coordinateSystem = WkbCoordinateSystem.XYZM;
                    break;
            }

            return (WkbGeometryTypes)((type & 0xffff) % 1000);
        }

        public static byte[] HexToBytes(string hex)
        {
            int byteLen = hex.Length / 2;
            byte[] bytes = new byte[byteLen];
            char chex0, chex1;
            byte hex0 = 0, hex1 = 0;
            for (int i = 0, j = 0, len = hex.Length; i < len; i += 2, j++)
            {
                chex0 = hex[i];
                chex1 = hex[i + 1];

                if (chex0 >= '0' && chex0 <= '9') hex0 = (byte)(chex0 - '0');
                else if (chex0 >= 'A' && chex0 <= 'F') hex0 = (byte)(chex0 - 'A' + 10);
                else if (chex0 >= 'a' && chex0 <= 'f') hex0 = (byte)(chex0 - 'a' + 10);
                if (chex1 >= '0' && chex1 <= '9') hex1 = (byte)(chex1 - '0');
                else if (chex1 >= 'A' && chex1 <= 'F') hex1 = (byte)(chex1 - 'A' + 10);
                else if (chex1 >= 'a' && chex1 <= 'f') hex1 = (byte)(chex1 - 'a' + 10);

                bytes[j] = (byte)((hex0 << 4) + (byte)hex1);
            }
            return bytes;
        }

        public class BigEndianBinaryReader : BinaryReader
        {
            public BigEndianBinaryReader(Stream stream) : base(stream) { }

            public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }

            public override Int16 ReadInt16()
            {
                byte[] byteArray = new byte[2];
                int iBytesRead = Read(byteArray, 0, 2);
                Debug.Assert(iBytesRead == 2);

                Array.Reverse(byteArray);
                return BitConverter.ToInt16(byteArray, 0);
            }

#pragma warning disable CS3002 // Return type is not CLS-compliant
            public override UInt16 ReadUInt16()
#pragma warning restore CS3002 // Return type is not CLS-compliant
            {
                byte[] byteArray = new byte[2];
                int iBytesRead = Read(byteArray, 0, 2);
                Debug.Assert(iBytesRead == 2);

                Array.Reverse(byteArray);
                return BitConverter.ToUInt16(byteArray, 0);
            }

            public override Int32 ReadInt32()
            {
                byte[] byteArray = new byte[4];
                int iBytesRead = Read(byteArray, 0, 4);
                Debug.Assert(iBytesRead == 4);

                Array.Reverse(byteArray);
                return BitConverter.ToInt32(byteArray, 0);
            }

#pragma warning disable CS3002 // Return type is not CLS-compliant
            public override UInt32 ReadUInt32()
#pragma warning restore CS3002 // Return type is not CLS-compliant
            {
                byte[] byteArray = new byte[4];
                int iBytesRead = Read(byteArray, 0, 4);
                Debug.Assert(iBytesRead == 4);

                Array.Reverse(byteArray);
                return BitConverter.ToUInt32(byteArray, 0);
            }

            public override Int64 ReadInt64()
            {
                byte[] byteArray = new byte[8];
                int iBytesRead = Read(byteArray, 0, 8);
                Debug.Assert(iBytesRead == 8);

                Array.Reverse(byteArray);
                return BitConverter.ToInt64(byteArray, 0);
            }

#pragma warning disable CS3002 // Return type is not CLS-compliant
            public override UInt64 ReadUInt64()
#pragma warning restore CS3002 // Return type is not CLS-compliant
            {
                byte[] byteArray = new byte[8];
                int iBytesRead = Read(byteArray, 0, 8);
                Debug.Assert(iBytesRead == 8);

                Array.Reverse(byteArray);
                return BitConverter.ToUInt64(byteArray, 0);
            }

            public override float ReadSingle()
            {
                byte[] byteArray = new byte[4];
                int iBytesRead = Read(byteArray, 0, 4);
                Debug.Assert(iBytesRead == 4);

                Array.Reverse(byteArray);
                return BitConverter.ToSingle(byteArray, 0);
            }

            public override double ReadDouble()
            {
                byte[] byteArray = new byte[8];
                int iBytesRead = Read(byteArray, 0, 8);
                Debug.Assert(iBytesRead == 8);

                Array.Reverse(byteArray);
                return BitConverter.ToDouble(byteArray, 0);
            }
        }
    }
}
