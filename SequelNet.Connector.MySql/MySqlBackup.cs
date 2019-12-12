using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace SequelNet.Connector
{
    public static class MySqlBackup
    {
        private const string NEW_LINE = "\r\n";
        private const string DELIMITER = "$$";

        public static async System.Threading.Tasks.Task GenerateBackup(MySqlConnector conn, Stream outputStream, BackupOptions options, CancellationToken? cancellationToken = null)
        {
            using (var writer = new StreamWriter(outputStream, Encoding.UTF8))
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

                if (options.ExportRoutines) await ExportRoutines(conn, writer, cancellationToken);
                if (options.ExportTableCreate) await ExportTableCreate(conn, writer, options.ExportTableDrop, cancellationToken);
                if (options.ExportTableData) await ExportTableData(conn, writer, options, cancellationToken);
                if (options.ExportTriggers) await ExportTriggers(conn, writer, cancellationToken);

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

            using (var reader = conn.ExecuteReader(sql))
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

        static public async System.Threading.Tasks.Task<List<string>> GetObjectList(MySqlConnector conn, DbObjectType type, CancellationToken? cancellationToken = null)
        {
            string sql;

            switch (type)
            {
                case DbObjectType.TABLE:
                    sql = "SHOW TABLES";
                    break;

                default:
                    sql = string.Format("SHOW {0} STATUS", type.ToString());
                    break;
            }

            var lstResults = new List<string>();
            using (var reader = await conn.ExecuteReaderAsync(sql, cancellationToken ?? CancellationToken.None))
            {
                while (await reader.ReadAsync(cancellationToken))
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
            string sql = string.Format(@"SELECT TABLE_NAME FROM information_schema.VIEWS WHERE TABLE_SCHEMA = SCHEMA() AND TABLE_NAME = {0}", conn.Language.PrepareValue(tableName));
            return !IsNull(conn.ExecuteScalar(sql));
        }

        static private async System.Threading.Tasks.Task ExportRoutines(MySqlConnector conn, StreamWriter writer, CancellationToken? cancellationToken = null)
        {
            var lstProcedures = await GetObjectList(conn, DbObjectType.PROCEDURE, cancellationToken);
            var lstFunctions = await GetObjectList(conn, DbObjectType.FUNCTION, cancellationToken);

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

        static private async System.Threading.Tasks.Task ExportTableCreate(MySqlConnector conn, StreamWriter writer, bool dropTables, CancellationToken? cancellationToken = null)
        {
            var lstTables = await GetObjectList(conn, DbObjectType.TABLE, cancellationToken);
            var lstViews = new List<string>();

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

        static private async System.Threading.Tasks.Task ExportTableData(MySqlConnector conn, StreamWriter writer, BackupOptions options, CancellationToken? cancellationToken = null)
        {
            var lstTables = await GetObjectList(conn, DbObjectType.TABLE, cancellationToken);

            foreach (string table in lstTables)
            {
                if (IsView(conn, table))
                {
                    continue;
                }

                var query = options.GetTableDataQuery(table);

                using (var reader = (query == null 
                    ? await conn.ExecuteReaderAsync(string.Format(@"SELECT * FROM {0}", table), cancellationToken ?? CancellationToken.None)
                    : await query.ExecuteReaderAsync(conn, cancellationToken)))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        writer.Write(string.Format(@"INSERT INTO {0} (", table));
                        for (Int32 idx = 0, count = reader.GetColumnCount(); idx < count; idx++)
                        {
                            if (idx > 0) writer.Write(@",");
                            writer.Write(conn.Language.WrapFieldName(reader.GetColumnName(idx)));
                        }
                        writer.Write(@") VALUES(");
                        for (Int32 idx = 0, count = reader.GetColumnCount(); idx < count; idx++)
                        {
                            if (idx > 0) writer.Write(@",");
                            writer.Write(conn.Language.PrepareValue(conn, reader[idx]));
                        }
                        writer.Write(string.Format(@") {0}{1}", DELIMITER, NEW_LINE));
                    }
                }
            }
        }

        static private async System.Threading.Tasks.Task ExportTriggers(MySqlConnector conn, StreamWriter writer, CancellationToken? cancellationToken = null)
        {
            using (var reader = await conn.ExecuteReaderAsync("SHOW TRIGGERS", cancellationToken ?? CancellationToken.None))
            {
                while (await reader.ReadAsync(cancellationToken))
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
