namespace SequelNet.Connector;

public struct MsSqlVersion
{
    public int MajorVersion;
    private string _Version;
    public string Level;
    public string Edition;

    public string Version
    {
        get { return _Version; }
        set 
        {
            _Version = value;

            int periodIndex = _Version.IndexOf('.');
            int.TryParse(periodIndex == -1 ? _Version : _Version.Substring(0, periodIndex), out var major);
            MajorVersion = major;
        }
    }

    public bool SupportsOffset
    {
        get
        {
            return MajorVersion >= 11; // Since Sql Server 2012
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is MsSqlVersion)
        {
            var m = (MsSqlVersion)obj;
            return _Version == m._Version &&
                Level == m.Level &&
                Edition == m.Edition;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(MsSqlVersion m1, MsSqlVersion m2)
    {
        return m1.Equals(m2);
    }

    public static bool operator !=(MsSqlVersion m1, MsSqlVersion m2)
    {
        return !m1.Equals(m2);
    }
}
