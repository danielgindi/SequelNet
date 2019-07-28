namespace dg.Sql.Connector
{
    public class MsSqlVersion
    {
        private bool? _SupportsOffset = null;

        private int _MajorVersion = 0;
        private string _Version = null;
        private string _Level = null;
        private string _Edition = null;

        public string Version
        {
            get { return _Version; }
            set 
            {
                _Version = value;

                int periodIndex = _Version.IndexOf('.');
                int.TryParse(periodIndex == -1 ? _Version : _Version.Substring(0, periodIndex), out _MajorVersion);
            }
        }

        public int MajorVersion
        {
            get { return _MajorVersion; }
            set { _MajorVersion = value; }
        }

        public string Level
        {
            get { return _Level; }
            set { _Level = value; }
        }

        public string Edition
        {
            get { return _Edition; }
            set { _Edition = value; }
        }

        public bool SupportsOffset
        {
            get 
            {
                if (_SupportsOffset == null)
                {
                    _SupportsOffset = MajorVersion >= 11; // Since Sql Server 2012
                }
                return _SupportsOffset.Value;
            }
            set 
            {
                _SupportsOffset = value;
            }
        }
    }
}
