using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace dg.Sql.Connector
{
    public static class MySqlBackup
    {
        private const string NEW_LINE = "\r\n";
        private const string DELIMITER = "$$";

        public static void GenerateBackup(MySqlConnector conn, Stream outputStream, BackupOptions options)
        {
            using (StreamWriter writer = new StreamWriter(outputStream, Encoding.UTF8))
            {
                if (options.BOM)
                {
                    outputStream.Write(System.Text.Encoding.UTF8.GetPreamble(), 0, System.Text.Encoding.UTF8.GetPreamble().Length);
                }
                writer.Write(string.Format(@"DELIMITER {0}{1}", DELIMITER, NEW_LINE));
                writer.Write(string.Format(@"SET FOREIGN_KEY_CHECKS=0 {0}{1}", DELIMITER, NEW_LINE));
                writer.Write(string.Format(@"SET SQL_MODE=""NO_AUTO_VALUE_ON_ZERO"" {0}{1}", DELIMITER, NEW_LINE));
                writer.Write(string.Format(@"SET AUTOCOMMIT=0 {0}{1}", DELIMITER, NEW_LINE));
                if (options.WrapInTransaction) writer.Write(string.Format(@"START TRANSACTION {0}{1}", DELIMITER, NEW_LINE));

                if (options.ExportRoutines) ExportRoutines(conn, writer);
                if (options.ExportTableCreate) ExportTableCreate(conn, writer, options.ExportTableDrop);
                if (options.ExportTableData) ExportTableData(conn, writer, options);
                if (options.ExportTriggers) ExportTriggers(conn, writer);

                writer.Write(string.Format(@"SET FOREIGN_KEY_CHECKS=1 {0}{1}", DELIMITER, NEW_LINE));
                if (options.WrapInTransaction) writer.Write(string.Format(@"COMMIT {0}{1}", DELIMITER, NEW_LINE));
            }
        }

        public class BackupOptions
        {
            public bool BOM;
            public bool ExportRoutines;
            public bool ExportTableDrop;
            public bool ExportTableCreate;
            public bool ExportTableData;
            public bool ExportTriggers;
            public bool WrapInTransaction;

            private Dictionary<string, Query> tableDataQueries = new Dictionary<string, Query>();

            public void SetTableDataQuery(string tableName, Query query)
            {
                if (query == null)
                {
                    tableDataQueries.Remove(tableName);
                }
                else
                {
                    tableDataQueries[tableName] = query;
                }
            }

            public Query GetTableDataQuery(string tableName)
            {
                return tableDataQueries.ContainsKey(tableName) ? tableDataQueries[tableName] : null;
            }
        }

        public enum DbObjectType
        {
            PROCEDURE,
            FUNCTION,
            EVENT,
            VIEW,
            TABLE
        }

        static public string GetObjectCreate(MySqlConnector conn, DbObjectType type, string objectName, bool ifNotExists)
        {
            string resultColumn = "";
            switch (type)
            {
                case DbObjectType.PROCEDURE:
                    resultColumn = "Create Procedure";
                    break;
                case DbObjectType.FUNCTION:
                    resultColumn = "Create Function";
                    break;
                case DbObjectType.EVENT:
                    resultColumn = "Create Event";
                    break;
                case DbObjectType.VIEW:
                    resultColumn = "Create View";
                    break;
                case DbObjectType.TABLE:
                    resultColumn = "Create Table";
                    break;
            }

            string sql = string.Format(@"SHOW CREATE {0} {1}", type.ToString(), objectName);

            using (DataReaderBase reader = conn.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    string create = StringFromDb(reader[resultColumn]);

                    if (ifNotExists && create != null && create.StartsWith(@"CREATE TABLE"))
                    {
                        create = @"CREATE TABLE IF NOT EXISTS" + create.Substring(@"CREATE TABLE".Length);
                    }

                    return create;
                }
            }
            return null;
        }

        static public List<string> GetObjectList(MySqlConnector conn, DbObjectType type)
        {
            string sql = null;
            switch (type)
            {
                case DbObjectType.TABLE:
                    sql = @"SHOW TABLES";
                    break;
                default:
                    sql = string.Format(@"SHOW {0} STATUS", type.ToString());
                    break;
            }
            List<string> lstResults = new List<string>();
            using (DataReaderBase reader = conn.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    switch (type)
                    {
                        case DbObjectType.TABLE:
                            lstResults.Add(reader.GetStringOrEmpty(0));
                            break;
                        default:
                            lstResults.Add((string)reader[@"Name"]);
                            break;
                    }
                }
            }
            return lstResults;
        }

        static public bool IsView(MySqlConnector conn, string tableName)
        {
            string sql = string.Format(@"SELECT TABLE_NAME FROM information_schema.VIEWS WHERE TABLE_SCHEMA = SCHEMA() AND TABLE_NAME = {0}", conn.PrepareValue(tableName));
            return !IsNull(conn.ExecuteScalar(sql));
        }

        static private void ExportRoutines(MySqlConnector conn, StreamWriter writer)
        {
            List<string> lstProcedures = GetObjectList(conn, DbObjectType.PROCEDURE);
            List<string> lstFunctions = GetObjectList(conn, DbObjectType.FUNCTION);

            foreach (string proc in lstProcedures)
            {
                writer.Write(string.Format(@"DROP PROCEDURE IF EXISTS {2} {0}{1}", DELIMITER, NEW_LINE, proc));
                writer.Write(string.Format(@"{2} {0}{1}", DELIMITER, NEW_LINE, GetObjectCreate(conn, DbObjectType.PROCEDURE, proc, false)));
            }
            foreach (string func in lstFunctions)
            {
                writer.Write(string.Format(@"DROP FUNCTION IF EXISTS {2} {0}{1}", DELIMITER, NEW_LINE, func));
                writer.Write(string.Format(@"{2} {0}{1}", DELIMITER, NEW_LINE, GetObjectCreate(conn, DbObjectType.FUNCTION, func, false)));
            }
        }

        static private void ExportTableCreate(MySqlConnector conn, StreamWriter writer, bool dropTables)
        {
            List<string> lstTables = GetObjectList(conn, DbObjectType.TABLE);
            List<string> lstViews = new List<string>();

            foreach (string table in lstTables)
            {
                if (IsView(conn, table))
                {
                    lstViews.Add(table);
                    continue;
                }

                if (dropTables)
                {
                    writer.Write(string.Format(@"DROP TABLE IF EXISTS {2} {0}{1}", DELIMITER, NEW_LINE, table));
                }

                string create = GetObjectCreate(conn, DbObjectType.TABLE, table, !dropTables);
                writer.Write(string.Format(@"{2} {0}{1}", DELIMITER, NEW_LINE, create));
            }

            foreach (string view in lstViews)
            {
                if (dropTables)
                {
                    writer.Write(string.Format(@"DROP VIEW IF EXISTS {2} {0}{1}", DELIMITER, NEW_LINE, view));
                }

                string create = GetObjectCreate(conn, DbObjectType.VIEW, view, !dropTables);
                writer.Write(string.Format(@"{2} {0}{1}", DELIMITER, NEW_LINE, create));
            }
        }

        static private void ExportTableData(MySqlConnector conn, StreamWriter writer, BackupOptions options)
        {
            List<string> lstTables = GetObjectList(conn, DbObjectType.TABLE);

            foreach (string table in lstTables)
            {
                if (IsView(conn, table))
                {
                    continue;
                }

                Query query = options.GetTableDataQuery(table);

                using (DataReaderBase reader = (query == null ? conn.ExecuteReader(string.Format(@"SELECT * FROM {0}", table)) : query.ExecuteReader(conn)))
                {
                    while (reader.Read())
                    {
                        writer.Write(string.Format(@"INSERT INTO {0} (", table));
                        for (Int32 idx = 0, count = reader.GetColumnCount(); idx < count; idx++)
                        {
                            if (idx > 0) writer.Write(@",");
                            writer.Write(conn.EncloseFieldName(reader.GetColumnName(idx)));
                        }
                        writer.Write(@") VALUES(");
                        for (Int32 idx = 0, count = reader.GetColumnCount(); idx < count; idx++)
                        {
                            if (idx > 0) writer.Write(@",");
                            writer.Write(conn.PrepareValue(reader[idx]));
                        }
                        writer.Write(string.Format(@") {0}{1}", DELIMITER, NEW_LINE));
                    }
                }
            }
        }

        static private void ExportTriggers(MySqlConnector conn, StreamWriter writer)
        {
            using (DataReaderBase reader = conn.ExecuteReader(@"SHOW TRIGGERS"))
            {
                while (reader.Read())
                {
                    writer.WriteLine(@"CREATE TRIGGER {0} ", StringFromDb(reader[@"Trigger"]));
                    writer.WriteLine(@"{0} {1} ON {2} ", StringFromDb(reader[@"Timing"]), StringFromDb(reader[@"Event"]), StringFromDb(reader[@"Table"]));
                    writer.WriteLine(@"FOR EACH ROW ");
                    writer.WriteLine(@"{0} {1}{2}", StringFromDb(reader[@"Statement"]), DELIMITER, NEW_LINE);
                }
            }
        }

        static private bool IsNull(this object value)
        {
            if (value == null) return true;
            if (value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull) return true;
            if (value is DBNull) return true;
            else return false;
        }

        static private string StringFromDb(object str)
        {
            if (IsNull(str)) return null;

            string result = null;
            if (str is byte[]) result = Encoding.UTF8.GetString((byte[])str);
            else result = (string)str;
            return result;
        }
    }
}
