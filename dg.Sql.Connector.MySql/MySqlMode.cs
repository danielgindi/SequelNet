namespace dg.Sql.Connector
{
    public class MySqlMode
    {
        private string _SqlMode = null;
        private bool? _NoBackSlashes = null;
        private bool? _AnsiQuotes = null;

        public string SqlMode
        {
            get { return _SqlMode; }
            set { _SqlMode = value; }
        }

        public bool NoBackSlashes
        {
            get
            {
                if (_NoBackSlashes == null)
                {
                    if (_SqlMode == null) return false;
                    _NoBackSlashes = _SqlMode.IndexOf("NO_BACKSLASH_ESCAPES") != -1;
                }
                return _NoBackSlashes == true;
            }
            set { _NoBackSlashes = value; }
        }

        public bool AnsiQuotes
        {
            get
            {
                if (_AnsiQuotes == null)
                {
                    if (_SqlMode == null) return false;
                    _AnsiQuotes = _SqlMode.IndexOf("ANSI_QUOTES") != -1;
                }
                return _AnsiQuotes == true;
            }
            set { _AnsiQuotes = value; }
        }
    }
}
