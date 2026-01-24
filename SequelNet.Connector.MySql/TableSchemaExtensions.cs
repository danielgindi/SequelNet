namespace SequelNet.Connector;

public static class TableSchemaExtensions
{
    public static void SetMySqlEngine(this TableSchema Schema, MySqlEngineType MySqlEngine)
    {
        Schema.SetTableOption("Engine", MySqlEngine.ToString());
    }

    public static MySqlEngineType GetMySqlEngine(this TableSchema Schema)
    {
        string value = Schema.GetTableOption("Engine");
        if (value == null) return MySqlEngineType.InnoDB;
        if (value == MySqlEngineType.MyISAM.ToString()) return MySqlEngineType.MyISAM;
        if (value == MySqlEngineType.ARCHIVE.ToString()) return MySqlEngineType.ARCHIVE;
        return MySqlEngineType.InnoDB;
    }
}