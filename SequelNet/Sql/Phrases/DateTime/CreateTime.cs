using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class CreateTime : IPhrase
{
    public int Hours;
    public int Minutes;
    public int Seconds;
    public int Milliseconds;

    #region Constructors

    public CreateTime(int hours, int minutes, int seconds, int milliseconds = 0)
    {
        this.Hours = hours;
        this.Minutes = minutes;
        this.Seconds = seconds;
        this.Milliseconds = milliseconds;
    }
    
    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.FormatCreateTime(Hours, Minutes, Seconds, Milliseconds));
    }
}
