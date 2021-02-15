using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// General formula is:
    /// R = 6371 (km, earth's avg. radius)
    /// FX = From latitude
    /// FY = From longitude
    /// TX = To latitude
    /// TY = To longitude
    /// D =  2R * ASIN(SQRT(POWER(SIN((FX-TX) * PI/360), 2) + COS(FX * PI/180) * COS(TX * PI/180) * POWER(SIN((FY - TY) * PI/360), 2)))
    /// meters = D * 1000
    /// 
    /// This will return value in meters
    /// </summary>
    public class GeographySphericalDistanceMath : IPhrase
    {
        public ValueWrapper FromLatitude;
        public ValueWrapper FromLongitude;
        public ValueWrapper ToLatitude;
        public ValueWrapper ToLongitude;

        public GeographySphericalDistanceMath(
            ValueWrapper fromLat, ValueWrapper fromLng, 
            ValueWrapper toLat, ValueWrapper toLng)
        {
            this.FromLatitude = fromLat;
            this.FromLongitude = fromLng;
            this.ToLatitude = toLat;
            this.ToLongitude = toLng;
        }
        
        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            var fx = FromLatitude.Build(conn, relatedQuery);
            var fy = FromLongitude.Build(conn, relatedQuery);
            var tx = ToLatitude.Build(conn, relatedQuery);
            var ty = ToLongitude.Build(conn, relatedQuery);
            
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
            sb.Append(@" * 1000.0"); // Return in meters
        }
    }
}
