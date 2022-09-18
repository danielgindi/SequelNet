﻿using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Date : IPhrase
    {
        public int Year;
        public int Month;
        public int Day;

        #region Constructors

        public Date(int year, int month, int day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }
        
        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.FormatDate(Year, Month, Day));
        }
    }
}
