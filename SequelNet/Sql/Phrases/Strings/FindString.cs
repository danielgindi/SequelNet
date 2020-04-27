using SequelNet.Connector;

namespace SequelNet.Phrases
{
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

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            return conn.Language.BuildFindString(conn, Needle, Haystack, StartAt, relatedQuery);
        }
    }
}
