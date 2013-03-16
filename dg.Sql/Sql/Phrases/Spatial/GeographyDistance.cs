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
    /// D =  2R * ASIN(SQRT(POWER(SIN((FX-abs(TX)) * PI/360), 2) + COS(FX * PI/180) * COS(ABS(TX) * PI/180) * POWER(SIN((FY - TY) * PI/360), 2)))
    /// </summary>
    public class GeographyDistance : BasePhrase
    {
        string ContainingTableName;
        string ContainingLatitudeColumnName;
        string ContainingLongitudeColumnName;
        string ContainingPointColumnName;
        decimal FromLatitude;
        decimal FromLongitude;

        public GeographyDistance(
            string ContainingTableName, string ContainingLatitudeColumnName, string ContainingLongitudeColumnName,
            decimal FromLatitude, decimal FromLongitude)
        {
            this.ContainingTableName = ContainingTableName;
            this.ContainingLatitudeColumnName = ContainingLatitudeColumnName;
            this.ContainingLongitudeColumnName = ContainingLongitudeColumnName;
            this.FromLatitude = FromLatitude;
            this.FromLongitude = FromLongitude;
        }
        public GeographyDistance
            (string ContainingTableName, string ContainingPointColumnName,
            decimal FromLatitude, decimal FromLongitude)
        {
            this.ContainingTableName = null;
            this.ContainingPointColumnName = ContainingPointColumnName;
            this.FromLatitude = FromLatitude;
            this.FromLongitude = FromLongitude;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string FX, FY, TX, TY;
            if (ContainingPointColumnName != null)
            {
                if (ContainingTableName != null)
                {
                    TY = conn.encloseFieldName(ContainingTableName) + @"." + conn.encloseFieldName(ContainingPointColumnName);
                }
                else
                {
                    TY = conn.encloseFieldName(ContainingPointColumnName);
                }
                if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
                {
                    TX = TY + @".STX";
                    TY = TY + @".STY";
                }
                else // MYSQL
                {
                    TX = @"X(" + TY + @")";
                    TY = @"Y(" + TY + @")";
                }
            }
            else
            {
                if (ContainingTableName != null)
                {
                    TX = conn.encloseFieldName(ContainingTableName);
                    TY = TX + @"." + conn.encloseFieldName(ContainingLongitudeColumnName);
                    TX = TX + @"." + conn.encloseFieldName(ContainingLatitudeColumnName);
                }
                else
                {
                    TY = conn.encloseFieldName(ContainingLongitudeColumnName);
                    TX = conn.encloseFieldName(ContainingLatitudeColumnName);
                }
            }

            FX = FromLatitude.ToString(CultureInfo.InvariantCulture);
            FY = FromLongitude.ToString(CultureInfo.InvariantCulture);

            StringBuilder sb = new StringBuilder();
            sb.Append(@"12742*ASIN(SQRT(POWER(SIN((");
            sb.Append(FX);
            sb.Append(@"-ABS(");
            sb.Append(TX);
            sb.Append(@")) * PI/360), 2) + COS(");
            sb.Append(FX);
            sb.Append(@"* PI/180) * COS(ABS(");
            sb.Append(TX);
            sb.Append(@") * PI/180) * POWER(SIN((");
            sb.Append(FY);
            sb.Append(@"-");
            sb.Append(TY);
            sb.Append(@") * PI/360), 2)))");

            return sb.ToString();
        }
    }
}
