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
            public LineString(params Point[] Points)
            {
                _Points = new List<Point>(Points);
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
                if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    if (this.IsGeographyType)
                    {
                        sb.Append(@"geography::STGeomFromText('");
                    }
                    else
                    {
                        sb.Append(@"geometry::STGeomFromText('");
                    }
                }
                else if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                {
                    if (this.IsGeographyType)
                    {
                        sb.Append(@"ST_GeogFromText('");
                    }
                    else
                    {
                        sb.Append(@"ST_GeomFromText('");
                    }
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
            }
            public override void BuildValueForCollection(StringBuilder sb, ConnectorBase conn)
            {
                sb.Append(@"LINESTRING(");
                foreach (Point pt in _Points)
                {
                    sb.Append(pt.X.ToString(formatProvider));
                    sb.Append(' ');
                    sb.Append(pt.Y.ToString(formatProvider));
                }
                sb.Append(@")");
            }

            #region Common Calculation Helpers
            public const double AVERAGE_KM_PER_LATITUDE_DEGREE = 111.135;
            public const double AVERAGE_KM_PER_LONGITUDE_DEGREE_AT_40_DEGREES = 85.0;
            public const double DEGREES_TO_RADIANS = Math.PI / 180.0;

            /// <summary>
            /// Generates a bounding rect around a lat/lon coordinate, to constrain queries.
            /// Anyway this is a spherical rectangle, not a real rectangle, so it's not an exact rectangle but a rather strange rect.
            /// To prevent problems with this approach, we're changing the "radius" to a real radius of the bounding circle, which will be almost exact
            /// </summary>
            /// <param name="latitude"></param>
            /// <param name="longitude"></param>
            /// <param name="distanceInKilometers">Kinda' like radius</param>
            /// <returns></returns>
            static public LineString RectForDistanceAroundLatLon(double latitude, double longitude, double distanceInKilometers)
            {
                // Get the circle that bounds the rect. This will give us a much more accurate rect
                distanceInKilometers = (Math.Sqrt(2.0 * (distanceInKilometers * 2.0) * (distanceInKilometers * 2.0)) / 2.0);

                LineString rect = new LineString();
                double distanceLat = distanceInKilometers / AVERAGE_KM_PER_LATITUDE_DEGREE;
                double distanceLon = distanceInKilometers / (AVERAGE_KM_PER_LATITUDE_DEGREE / Math.Cos(DEGREES_TO_RADIANS * latitude));

                // This is a real rect
                //rect.Points.Add(new Point(latitude + distanceLat, longitude + distanceLon));
                //rect.Points.Add(new Point(latitude - distanceLat, longitude + distanceLon));
                //rect.Points.Add(new Point(latitude - distanceLat, longitude - distanceLon));
                //rect.Points.Add(new Point(latitude + distanceLat, longitude - distanceLon));
                //rect.Points.Add(rect.Points[0]);

                // This is enough as the wkbs are calculating the bounds and retrieves a rectangle.
                rect.Points.Add(new Point(latitude + distanceLat, longitude + distanceLon));
                rect.Points.Add(new Point(latitude - distanceLat, longitude - distanceLon));
                return rect;
            }

            /// <summary>
            /// Generates a bounding rect around a lat/lon coordinate, to constrain queries.
            /// Anyway this is a spherical rectangle, not a real rectangle, so it's not an exact rectangle but a rather strange rect.
            /// </summary>
            /// <param name="latLonPoint"></param>
            /// <param name="distanceInKilometers">Kinda' like radius</param>
            /// <returns></returns>
            static public LineString RectForDistanceAroundLatLon(Point latLonPoint, double distanceInKilometers)
            {
                return RectForDistanceAroundLatLon(latLonPoint.X, latLonPoint.Y, distanceInKilometers);
            }

            /// <summary>
            /// Generates a bounding rect around a lat/lon rect, to constrain queries.
            /// Anyway this is a spherical rectangle, not a real rectangle, so it's not an exact rectangle but a rather strange rect.
            /// To prevent problems with this approach, we're changing the "radius" to a real radius of the bounding circle, which will be almost exact
            /// </summary>
            /// <param name="lat1"></param>
            /// <param name="lon1"></param>
            /// <param name="lat2"></param>
            /// <param name="lon2"></param>
            /// <param name="distanceInKilometers">Kinda' like radius</param>
            /// <returns></returns>
            static public LineString RectForDistanceAroundRect(double lat1, double lon1, double lat2, double lon2, double distanceInKilometers)
            {
                // Get the circle that bounds the rect. This will give us a much more accurate rect
                distanceInKilometers = (Math.Sqrt(2.0 * (distanceInKilometers * 2.0) * (distanceInKilometers * 2.0)) / 2.0);

                double d = Math.Min(lat1, lat2);
                lat2 = Math.Max(lat1, lat2);
                lat1 = d;

                d = Math.Min(lon1, lon2);
                lon2 = Math.Max(lon1, lon2);
                lon1 = d;

                LineString rect = new LineString();
                double distanceLat = distanceInKilometers / AVERAGE_KM_PER_LATITUDE_DEGREE;
                double distanceLon1 = distanceInKilometers / (AVERAGE_KM_PER_LATITUDE_DEGREE / Math.Cos(DEGREES_TO_RADIANS * lat1));
                double distanceLon2 = distanceInKilometers / (AVERAGE_KM_PER_LATITUDE_DEGREE / Math.Cos(DEGREES_TO_RADIANS * lat2));

                // This is enough as the wkbs are calculating the bounds and retrieves a rectangle.
                rect.Points.Add(new Point(lat2 + distanceLat, lon2 + distanceLon2));
                rect.Points.Add(new Point(lat1 - distanceLat, lon1 - distanceLon1));
                return rect;
            }

            /// <summary>
            /// Generates a bounding rect around a lat/lon rect, to constrain queries.
            /// Anyway this is a spherical rectangle, not a real rectangle, so it's not an exact rectangle but a rather strange rect.
            /// To prevent problems with this approach, we're changing the "radius" to a real radius of the bounding circle, which will be almost exact
            /// </summary>
            /// <param name="rectCorner1"></param>
            /// <param name="rectCorner2"></param>
            /// <param name="distanceInKilometers">Kinda' like radius</param>
            /// <returns></returns>
            static public LineString RectForDistanceAroundRect(Point rectCorner1, Point rectCorner2, double distanceInKilometers)
            {
                return RectForDistanceAroundRect(rectCorner1.X, rectCorner1.Y, rectCorner2.X, rectCorner2.Y, distanceInKilometers);
            }

            #endregion
        }
    }
}
