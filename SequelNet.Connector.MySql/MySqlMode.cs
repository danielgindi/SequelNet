namespace SequelNet.Connector;

public struct MySqlMode
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
        set
        {
            _Version = value;
            ParsedVersion = ParseVersion(value);
        }
    }

    /// <summary>
    /// Whether the detected server version can be used for feature checks.
    /// </summary>
    public bool IsVersionKnown => ParsedVersion != null;

    internal System.Version ParsedVersion { get; private set; }

    private static System.Version ParseVersion(string rawVersion)
    {
        if (string.IsNullOrWhiteSpace(rawVersion))
            return null;

        var numericLength = 0;
        while (numericLength < rawVersion.Length &&
            (char.IsDigit(rawVersion[numericLength]) || rawVersion[numericLength] == '.'))
        {
            numericLength++;
        }

        var numericVersion = rawVersion.Substring(0, numericLength).TrimEnd('.');
        return System.Version.TryParse(numericVersion, out var version) ? version : null;
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
        if (obj is MySqlMode)
        {
            var m = (MySqlMode)obj;
            return _SqlMode == m._SqlMode && _Version == m._Version;
        }

        return false;
    }
    
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(MySqlMode m1, MySqlMode m2)
    {
        return m1.Equals(m2);
    }

    public static bool operator !=(MySqlMode m1, MySqlMode m2)
    {
        return !m1.Equals(m2);
    }
}
