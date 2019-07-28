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
        public PointWrapper From;
        public PointWrapper To;

        public GeographyDistance(PointWrapper from, PointWrapper to)
        {
            this.From = from;
            this.To = to;
        }
        
        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string fx, fy, tx, ty;

            GenerateXY(conn, From, out fx, out fy);
            GenerateXY(conn, To, out tx, out ty);
            
            StringBuilder sb = new StringBuilder();
            sb.Append(@"12742.0 * ASIN(SQRT(POWER(SIN(((");
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
            PointWrapper point,
            out string x, out string y)
        {
            if (point.PointColumnName != null)
            {
                string pt;

                if (point.LatitudeTableName != null)
                {
                    pt = conn.Language.WrapFieldName(point.LatitudeTableName) + @"." + conn.Language.WrapFieldName(point.PointColumnName);
                }
                else
                {
                    pt = conn.Language.WrapFieldName(point.PointColumnName);
                }

                x = conn.Language.ST_X(pt);
                y = conn.Language.ST_Y(pt);
            }
            else
            {
                if (point.LatitudeColumnName != null)
                {
                    x = (point.LatitudeTableName != null ? conn.Language.WrapFieldName(point.LatitudeTableName) + "." : "")
                        + conn.Language.WrapFieldName(point.LatitudeColumnName);
                }
                else
                {
                    x = (point.Latitude ?? 0m).ToString(CultureInfo.InvariantCulture);
                }

                if (point.LongitudeColumnName != null)
                {
                    y = (point.LongitudeTableName != null ? conn.Language.WrapFieldName(point.LongitudeTableName) + "." : "")
                        + conn.Language.WrapFieldName(point.LongitudeColumnName);
                }
                else
                {
                    y = (point.Longitude ?? 0m).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public class PointWrapper
        {
            public decimal? Latitude;
            public decimal? Longitude;

            public string LatitudeTableName;
            public string LatitudeColumnName;
            public string LongitudeTableName;
            public string LongitudeColumnName;

            public string PointColumnName;

            public PointWrapper(string tableName, string latitudeColumnName, string longitudeColumnName)
            {
                this.LatitudeTableName = tableName;
                this.LatitudeColumnName = latitudeColumnName;
                this.LongitudeTableName = tableName;
                this.LongitudeColumnName = longitudeColumnName;
            }

            public PointWrapper(
                string latitudeTableName, string latitudeColumnName,
                string longitudeTableName, string longitudeColumnName)
            {
                this.LatitudeTableName = latitudeTableName;
                this.LatitudeColumnName = latitudeColumnName;
                this.LongitudeTableName = longitudeTableName;
                this.LongitudeColumnName = longitudeColumnName;
            }

            public PointWrapper(string tableName, string pointColumnName)
            {
                this.LatitudeTableName = LongitudeTableName = tableName;
                this.PointColumnName = pointColumnName;
            }

            public PointWrapper(decimal? latitude, decimal? longitude)
            {
                this.Latitude = latitude;
                this.Longitude = longitude;
            }
        }
    }
}
