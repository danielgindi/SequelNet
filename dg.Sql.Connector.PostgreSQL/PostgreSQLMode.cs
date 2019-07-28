namespace dg.Sql.Connector
{
    public struct PostgreSQLMode
    {
        public string Version;
        public bool StandardConformingStrings; // Which is the default in earlier versions, for backwards compatibility
        public bool BackslashQuote; // Relevant only for non-standard-conforming string literals

        public override bool Equals(object obj)
        {
            if (obj is PostgreSQLMode)
            {
                var m = (PostgreSQLMode)obj;
                return StandardConformingStrings == m.StandardConformingStrings &&
                    BackslashQuote == m.BackslashQuote &&
                    Version == m.Version;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(PostgreSQLMode m1, PostgreSQLMode m2)
        {
            return m1.Equals(m2);
        }

        public static bool operator !=(PostgreSQLMode m1, PostgreSQLMode m2)
        {
            return !m1.Equals(m2);
        }
    }
}
