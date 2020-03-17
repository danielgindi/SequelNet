namespace SequelNet.Connector
{
    public static class TableSchemaExtensions
    {
        public static void SetMySqlEngine(this TableSchema Schema, MySql2EngineType MySqlEngine)
        {
            Schema.SetTableOption("Engine", MySqlEngine.ToString());
        }

        public static MySql2EngineType GetMySqlEngine(this TableSchema Schema)
        {
            string value = Schema.GetTableOption("Engine");
            if (value == null) return MySql2EngineType.InnoDB;
            if (value == MySql2EngineType.MyISAM.ToString()) return MySql2EngineType.MyISAM;
            if (value == MySql2EngineType.ARCHIVE.ToString()) return MySql2EngineType.ARCHIVE;
            return MySql2EngineType.InnoDB;
        }
    }
}