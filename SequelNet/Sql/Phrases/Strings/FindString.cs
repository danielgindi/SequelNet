using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class FindString : IPhrase
{
    public ValueWrapper Needle;
    public ValueWrapper Haystack;
    public ValueWrapper? StartAt = null;

    #region Constructors

    public FindString(
        ValueWrapper needle,
        ValueWrapper haystack,
        ValueWrapper? startAt = null)
    {
        this.Needle = needle;
        this.Haystack = haystack;
        this.StartAt = startAt;
    }

    public FindString(
        ValueWrapper needle,
        ValueWrapper haystack,
        int startAt)
    {
        this.Needle = needle;
        this.Haystack = haystack;
        this.StartAt = ValueWrapper.From(startAt);
    }
    
    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.BuildFindString(conn, Needle, Haystack, StartAt, relatedQuery));
    }
}
