using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Time : IPhrase
    {
        public int Hours;
        public int Minutes;
        public int Seconds;
        public int Milliseconds;

        #region Constructors

        public Time(int hours, int minutes, int seconds, int milliseconds = 0)
        {
            this.Hours = hours;
            this.Minutes = minutes;
            this.Seconds = seconds;
            this.Milliseconds = milliseconds;
        }
        
        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.FormatTime(Hours, Minutes, Seconds, Milliseconds));
        }
    }
}
