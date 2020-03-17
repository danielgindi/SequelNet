namespace SequelNet.Connector
{
    public struct MySql2Mode
    {
        private string _SqlMode;
        private string _Version;

        private bool? _NoBackSlashes;
        private bool? _AnsiQuotes;

        public string SqlMode
        {
            get { return _SqlMode; }
            set { _SqlMode = value; }
        }

        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
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

        public override bool Equals(object obj)
        {
            if (obj is MySql2Mode)
            {
                var m = (MySql2Mode)obj;
                return _SqlMode == m._SqlMode && _Version == m._Version;
            }

            return false;
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(MySql2Mode m1, MySql2Mode m2)
        {
            return m1.Equals(m2);
        }

        public static bool operator !=(MySql2Mode m1, MySql2Mode m2)
        {
            return !m1.Equals(m2);
        }
    }
}
