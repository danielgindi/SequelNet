using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using System.Globalization;

namespace dg.Sql.Phrases
{
    /// <summary>
    /// General formula is:
    /// R = 6371 (km, earth's avg. radius)
    /// FX = From latitude
    /// FY = From longitude
    /// TX = To latitude
    /// TY = To longitude
    /// D =  2R * ASIN(SQRT(POWER(SIN((FX-TX) * PI/360), 2) + COS(FX * PI/180) * COS(TX * PI/180) * POWER(SIN((FY - TY) * PI/360), 2)))
    /// </summary>
    public class GeographyDistance : IPhrase
    {
        string FromTableName;
        string FromLatitudeColumnName;
        string FromLongitudeColumnName;
        string FromPointColumnName;

        decimal? ToLatitude;
        decimal? ToLongitude;
        string ToTableName;
        string ToLatitudeColumnName;
        string ToLongitudeColumnName;
        string ToPointColumnName;

        public GeographyDistance(
            string fromTableName, string fromLatitudeColumnName, string fromLongitudeColumnName,
            decimal toLatitude, decimal toLongitude)
        {
            this.FromTableName = fromTableName;
            this.FromLatitudeColumnName = fromLatitudeColumnName;
            this.FromLongitudeColumnName = fromLongitudeColumnName;
            this.ToLatitude = toLatitude;
            this.ToLongitude = toLongitude;
        }

        public GeographyDistance(
            string fromTableName, string fromPointColumnName,
            decimal toLatitude, decimal toLongitude)
        {
            this.FromTableName = fromTableName;
            this.FromPointColumnName = fromPointColumnName;
            this.ToLatitude = toLatitude;
            this.ToLongitude = toLongitude;
        }

        public GeographyDistance(
            string fromTableName, string fromLatitudeColumnName, string fromLongitudeColumnName,
            string toTableName, string toLatitudeColumnName, string toLongitudeColumnName)
        {
            this.FromTableName = fromTableName;
            this.FromLatitudeColumnName = fromLatitudeColumnName;
            this.FromLongitudeColumnName = fromLongitudeColumnName;
            this.ToTableName = toTableName;
            this.ToLatitudeColumnName = toLatitudeColumnName;
            this.ToLongitudeColumnName = toLongitudeColumnName;
        }

        public GeographyDistance(
            string fromTableName, string fromPointColumnName,
            string toTableName, string toLatitudeColumnName, string toLongitudeColumnName)
        {
            this.FromTableName = fromTableName;
            this.FromPointColumnName = fromPointColumnName;
            this.ToTableName = toTableName;
            this.ToLatitudeColumnName = toLatitudeColumnName;
            this.ToLongitudeColumnName = toLongitudeColumnName;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string fx, fy, tx, ty;

            GenerateXY(conn, FromTableName, FromPointColumnName, FromLatitudeColumnName, FromLongitudeColumnName, null, null, out fx, out fy);
            GenerateXY(conn, ToTableName, ToPointColumnName, ToLatitudeColumnName, ToLongitudeColumnName, ToLatitude, ToLongitude, out tx, out ty);
            
            StringBuilder sb = new StringBuilder();
            sb.Append(@"12742.0*ASIN(SQRT(POWER(SIN(((");
            sb.Append(fx);
            sb.Append(@")-(");
            sb.Append(tx);
            sb.Append(@")) * PI()/360.0), 2) + COS(");
            sb.Append(fx);
            sb.Append(@"* PI()/180.0) * COS((");
            sb.Append(tx);
            sb.Append(@") * PI()/180.0) * POWER(SIN((");
            sb.Append(fy);
            sb.Append(@"-");
            sb.Append(ty);
            sb.Append(@") * PI()/360.0), 2)))");

            return sb.ToString();
        }

        private static void GenerateXY(
            ConnectorBase conn, 
            string tableName, string pointName, string latName, string lonName,
            decimal? lat, decimal? lon,
            out string x, out string y)
        {
            if (pointName != null)
            {
                string pt;

                if (tableName != null)
                {
                    pt = conn.EncloseFieldName(tableName) + @"." + conn.EncloseFieldName(pointName);
                }
                else
                {
                    pt = conn.EncloseFieldName(pointName);
                }

                if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    x = pt + @".STX";
                    y = pt + @".STY";
                }
                else
                {
                    if (conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                    {
                        x = @"ST_X(" + pt + @")";
                        y = @"ST_Y(" + pt + @")";
                    }
                    else // MYSQL
                    {
                        x = @"X(" + pt + @")";
                        y = @"Y(" + pt + @")";
                    }
                }
            }
            else
            {
                if (latName != null)
                {
                    x = (tableName != null ? conn.EncloseFieldName(tableName) + "." : "")
                        + conn.EncloseFieldName(latName);
                }
                else
                {
                    x = (lat ?? 0m).ToString(CultureInfo.InvariantCulture);
                }

                if (lonName != null)
                {
                    y = (tableName != null ? conn.EncloseFieldName(tableName) + "." : "")
                        + conn.EncloseFieldName(lonName);
                }
                else
                {
                    y = (lon ?? 0m).ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    }
}
