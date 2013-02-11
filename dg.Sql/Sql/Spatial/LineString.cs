using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using System.Globalization;

namespace dg.Sql
{
    public abstract partial class Geometry
    {
        public class LineString : Geometry
        {
            private List<Point> _Points;

            public LineString()
            {
                _Points = new List<Point>();
            }
            public LineString(params Point[] pt)
            {
                _Points = new List<Point>(pt);
            }
            public LineString(int Capacity)
            {
                _Points = new List<Point>(Capacity);
            }

            public Point this[int i]
            {
                get { return _Points[i]; }
                set { _Points[i] = value; }
            }

            public List<Point> Points
            {
                get
                {
                    return _Points;
                }
                set
                {
                    _Points = value;
                }
            }

            public override bool IsEmpty
            {
                get
                {
                    return _Points.Count == 0;
                }
            }
            public override bool IsValid
            {
                get
                {
                    if (_Points.Count < 2) return false;
                    foreach (Point point in _Points)
                    {
                        if (!point.IsValid) return false;
                    }
                    return true;
                }
            }
            public bool IsValidRing
            {
                get
                {
                    if (!IsClosedRing) return false;
                    if (_Points.Count < 4) return false;
                    foreach (Point point in _Points)
                    {
                        if (!point.IsValid) return false;
                    }
                    return true;
                }
            }
            public virtual bool IsClosed
            {
                get
                {
                    if (IsEmpty) return false;
                    Geometry.Point FirstPoint = _Points[0];
                    Geometry.Point LastPoint = _Points[_Points.Count - 1];
                    return FirstPoint.X == LastPoint.X && FirstPoint.Y == LastPoint.Y;
                }
            }
            public virtual bool IsClosedRing
            {
                get
                {
                    if (IsEmpty) return true;
                    Geometry.Point FirstPoint = _Points[0];
                    Geometry.Point LastPoint = _Points[_Points.Count - 1];
                    return FirstPoint.X == LastPoint.X && FirstPoint.Y == LastPoint.Y;
                }
            }

            /// <summary>
            /// Returns the value of the angle between the first <see cref="Point" /> and the last <see cref="Point" />
            /// </summary>
            public double Angle
            {
                get
                {
                    if (_Points.Count == 0) return Double.NaN;
                    Geometry.Point FirstPoint = _Points[0];
                    Geometry.Point LastPoint = _Points[_Points.Count - 1];
                    double deltaX = LastPoint.X - FirstPoint.X;
                    double deltaY = LastPoint.Y - FirstPoint.Y;
                    double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double angleRAD = Math.Asin(Math.Abs(LastPoint.Y - FirstPoint.Y) / length);
                    double angle = (angleRAD * 180) / Math.PI;

                    if (((FirstPoint.X < LastPoint.X) && (FirstPoint.Y > LastPoint.Y)) ||
                         ((FirstPoint.X > LastPoint.X) && (FirstPoint.Y < LastPoint.Y)))
                        angle = 360 - angle;
                    return angle;
                }
            }
            
            static IFormatProvider formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            public override void BuildValue(StringBuilder sb, ConnectorBase conn)
            {
#if !USE_EWKT
                if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    sb.Append(@"geography::STGeomFromText('");
                }
                else
                {
                    sb.Append(@"GeomFromText('");
                }

                sb.Append(@"LINESTRING(");

                bool first = true;
                foreach (Point pt in _Points)
                {
                    if (first) first = false; else sb.Append(',');
                    sb.Append(pt.X.ToString(formatProvider));
                    sb.Append(' ');
                    sb.Append(pt.Y.ToString(formatProvider));
                }

                if (SRID != null)
                {
                    sb.Append(@")',");
                    sb.Append(SRID.Value);
                    sb.Append(')');
                }
                else
                {
                    sb.Append(@")')");
                }
#else
                sb.Append(@"GeomFromEwkt('");
                if (SRID != null)
                {
                    sb.Append(@"SRID=");
                    sb.Append(SRID);
                    sb.Append(';');
                }

                BuildValueForCollection(sb, conn);

                sb.Append(@"')");
#endif
            }
            public override void BuildValueForCollection(StringBuilder sb, ConnectorBase conn)
            {
#if !USE_EWKT
                sb.Append(@"LINESTRING(");
                foreach (Point pt in _Points)
                {
                    sb.Append(pt.X.ToString(formatProvider));
                    sb.Append(' ');
                    sb.Append(pt.Y.ToString(formatProvider));
                }
                sb.Append(@")");
#else
                bool HasM = false, HasZ = false;
                if (_Points.Count > 0)
                {
                    HasM = _Points[0].M != null;
                    HasZ = _Points[0].Z != null;
                }

                bool first = true;
                if (!HasZ && HasM)
                {
                    sb.Append(@"LINESTRINGM(");
                    foreach (Point pt in _Points)
                    {
                        if (first) first = false; else sb.Append(',');
                        sb.Append(pt.X.ToString(formatProvider));
                        sb.Append(' ');
                        sb.Append(pt.Y.ToString(formatProvider));
                        sb.Append(' ');
                        sb.Append(pt.M.Value.ToString(formatProvider));
                    }
                }
                else
                {
                    sb.Append(@"LINESTRING(");
                    foreach (Point pt in _Points)
                    {
                        if (first) first = false; else sb.Append(',');
                        sb.Append(pt.X.ToString(formatProvider));
                        sb.Append(' ');
                        sb.Append(pt.Y.ToString(formatProvider));
                        if (HasM)
                        {
                            sb.Append(' ');
                            sb.Append(pt.Z.Value.ToString(formatProvider));
                            if (HasZ)
                            {
                                sb.Append(' ');
                                sb.Append(pt.M.Value.ToString(formatProvider));
                            }
                        }
                    }
                }
                sb.Append(')');
#endif
            }

            #region Common Calculation Helpers
            public const double AVERAGE_KM_PER_DEGREE = 111.135;
            public const double DEGREES_TO_RADIANS = Math.PI / 180;
            static public LineString RectForDistanceAroundLatLon(double latitude, double longitude, double distanceInKilometers)
            {
                LineString rect = new LineString();
                double distanceLat = distanceInKilometers / AVERAGE_KM_PER_DEGREE;
                double distanceLon = distanceInKilometers / (AVERAGE_KM_PER_DEGREE / Math.Cos(DEGREES_TO_RADIANS * latitude));
                rect.Points.Add(new Point(latitude + distanceLat, longitude + distanceLon));
                rect.Points.Add(new Point(latitude - distanceLat, longitude - distanceLon));
                return rect;
            }
            static public LineString RectForDistanceAroundLatLon(Point latLonPoint, double distanceInKilometers)
            {
                LineString rect = new LineString();
                double distanceLat = distanceInKilometers / AVERAGE_KM_PER_DEGREE;
                double distanceLon = distanceInKilometers / (AVERAGE_KM_PER_DEGREE / Math.Cos(DEGREES_TO_RADIANS * latLonPoint.X));
                rect.Points.Add(new Point(latLonPoint.X + distanceLat, latLonPoint.Y + distanceLon));
                rect.Points.Add(new Point(latLonPoint.X - distanceLat, latLonPoint.Y - distanceLon));
                return rect;
            }
            #endregion
        }
    }
}
