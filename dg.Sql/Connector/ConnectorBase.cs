using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;

// This class needs a little cleanup. But it's gonna break dependent code, so I'm gonna postpone it. In the meanwhile we will still use this pretty ugly code.

namespace dg.Sql.Connector
{
    public abstract class ConnectorBase : IDisposable
    {
        static Type s_ConnectorType = null;
        static public ConnectorBase NewInstance()
        {
            if (s_ConnectorType == null)
            {
                s_ConnectorType = FindConnectorType();
            }
            return (ConnectorBase)System.Activator.CreateInstance(s_ConnectorType);
        }
        static public ConnectorBase NewInstance(string connectionStringKey)
        {
            if (s_ConnectorType == null)
            {
                s_ConnectorType = FindConnectorType();
            }
            return (ConnectorBase)System.Activator.CreateInstance(s_ConnectorType, new string[] { connectionStringKey });
        }
        static private Type FindConnectorType()
        {
            Type type = null;
            try
            {
                string connector = ConfigurationManager.AppSettings[@"dg.Sql.Connector"];
                if (!string.IsNullOrEmpty(connector))
                {
                    Assembly asm = Assembly.Load(@"dg.Sql.Connector." + connector);
                    type = asm.GetType(@"dg.Sql.Connector." + connector + @"Connector");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(@"dg.ConnectorBase.FindConnectorType error: {0}", ex));
            }
            if (type == null) System.Web.Compilation.BuildManager.GetType(@"dg.Sql.Connector.MySqlConnector", false);
            return type;
        }

        virtual public SqlServiceType TYPE
        {
            get { return SqlServiceType.UNKNOWN; }
        }

        public static String GetWebsiteConnectionString(string ConnectionStringKey)
        {
            ConnectionStringSettings connString = null;

            if (ConnectionStringKey != null)
            {
                connString = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringKey];
                if (connString != null) return connString.ConnectionString;
            }

            string appConnString = ConfigurationManager.AppSettings[@"dg.Sql::ConnectionStringKey"];
            if (appConnString != null && appConnString.Length > 0) connString = System.Configuration.ConfigurationManager.ConnectionStrings[appConnString];
            if (connString != null) return connString.ConnectionString;

            appConnString = ConfigurationManager.AppSettings[@"dg.Sql::ConnectionString"];
            if (appConnString != null && appConnString.Length > 0) return appConnString;

            connString = System.Configuration.ConfigurationManager.ConnectionStrings[@"dg.Sql"];
            if (connString != null) return connString.ConnectionString;

            return System.Configuration.ConfigurationManager.ConnectionStrings[0].ConnectionString;
        }
        public static String GetWebsiteConnectionString()
        {
            return GetWebsiteConnectionString(null);
        }

        abstract public void Dispose();

        abstract public void Close();
        abstract public int ExecuteNonQuery(String strSQL);
        abstract public int ExecuteNonQuery(DbCommand command);
        abstract public object ExecuteScalar(String strSQL);
        abstract public object ExecuteScalar(DbCommand command);
        abstract public DataReaderBase ExecuteReader(String strSQL);
        abstract public DataReaderBase ExecuteReader(String strSQL, bool attachConnectionToReader);
        abstract public DataReaderBase ExecuteReader(DbCommand command);
        abstract public DataReaderBase ExecuteReader(DbCommand command, bool attachConnectionToReader);
        abstract public DataSet ExecuteDataSet(String strSQL);
        abstract public DataSet ExecuteDataSet(DbCommand command);
        abstract public int ExecuteScript(String strSQL);

        virtual public string GetVersion()
        {
            throw new NotImplementedException(@"GetVersion not implemented in connector of type " + this.GetType().Name);
        }

        abstract public object GetLastInsertID();

        abstract public bool beginTransaction();
        abstract public bool beginTransaction(IsolationLevel isolationLevel);
        abstract public bool commitTransaction();
        abstract public bool rollbackTransaction();
        abstract public bool hasTransaction { get; }
        abstract public int currentTransactions { get; }

        abstract public DbTransaction Transaction { get; }
        abstract public DbConnection Connection { get; }

        virtual public void setIdentityInsert(string TableName, bool Enabled) { }

        abstract public bool checkIfTableExists(string tableName);

        public static string escapeQuotes(string strToEscape)
        {
            return strToEscape.Replace(@"'", @"''");
        }
        public virtual string fullEscape(string strToEscape)
        {
            return strToEscape.Replace(@"'", @"''");
        }
        public string prepareQuotedString(string strToEscape)
        {
            if (strToEscape == null) return @"NULL";
            else return '\'' + fullEscape(strToEscape) + '\'';
        }
        public string prepareDecimal(decimal value)
        {
            return value.ToString();
        }
        public virtual string prepareBoolean(bool value)
        {
            return value ? @"1" : @"0";
        }
        abstract public string prepareGuid(Guid value);
        public virtual string prepareString(string value)
        {
            return '\'' + fullEscape(value) + '\'';
        }
        public virtual string prepareValue(object value)
        {
            if (value == null || value is DBNull) return @"NULL";
            else if (value is string)
            {
                return prepareString((string)value);
            }
            else if (value is DateTime)
            {
                return '\'' + formatDate((DateTime)value) + '\'';
            }
            else if (value is Guid)
            {
                return prepareGuid((Guid)value);
            }
            else if (value is bool)
            {
                return prepareBoolean((bool)value);
            }
            else if (value is dg.Sql.BasePhrase)
            {
                return ((dg.Sql.BasePhrase)value).BuildPhrase(this);
            }
            else if (value is dg.Sql.Geometry)
            {
                StringBuilder sb = new StringBuilder();
                ((dg.Sql.Geometry)value).BuildValue(sb, this);
                return sb.ToString();
            }
            else return value.ToString();
        }
        abstract public string encloseFieldName(string fieldName);
        abstract public string formatDate(DateTime dateTime);
        public virtual string sqlAddPaginationAndOrdering(string strSql,
            int limit /* = 0 */, int offset /* = 0 */,
            string orderBy /* = NULL */)
        {
            int iSelect = strSql.IndexOf(@"SELECT ", StringComparison.CurrentCultureIgnoreCase);
            int iFrom = strSql.IndexOf(@" FROM ", StringComparison.CurrentCultureIgnoreCase);
            int iWhere = strSql.IndexOf(@" WHERE ", StringComparison.CurrentCultureIgnoreCase);

            string fields = strSql.Substring(iSelect + 7, iFrom - (iSelect + 7));
            string tables = strSql.Substring(iFrom + 6, iWhere < 0 ? strSql.Length - (iFrom + 6) : iWhere - (iFrom + 6));
            string where = iWhere < 0 ? string.Empty : strSql.Substring(iWhere + 7);
            string primaryKey = fields;
            iSelect = primaryKey.IndexOf(',');
            if (iSelect > 0) primaryKey = primaryKey.Substring(0, iSelect);

            return sqlAddPaginationAndOrdering(fields, primaryKey, tables, where, limit, offset, orderBy);
        }
        public virtual string sqlAddPaginationAndOrdering(
            string selectFieldsList,
            string primaryKeysList,
            string tablesList,
            string where,
            int limit /* = 0 */, int offset /* = 0 */,
            string orderBy /* = NULL */)
        {
            // The most standard technique

            tablesList = tablesList.TrimStart(new char[] { ' ' });
            if (tablesList.Length > 0) tablesList = @" FROM " + tablesList;
            where = where.TrimStart(new char[] { ' ' });
            if (where.Length > 0) where = @" WHERE " + where;
            if (orderBy == null) orderBy = string.Empty;
            orderBy = orderBy.TrimStart(new char[] { ' ' });
            if (orderBy.Length > 0) orderBy = @" ORDER BY " + orderBy;

            string sql = @"SELECT " + selectFieldsList + tablesList + where + orderBy;
            if (limit > 0) sql += @" LIMIT " + limit;
            if (offset > 0) sql += @" OFFSET " + offset;
            return sql;
        }

        public virtual string EscapeLike(string expression)
        {
            return expression.Replace(@"\", @"\\").Replace(@"%", @"\%");
        }

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column name.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found</exception>
        public virtual Geometry ReadGeometry(object value)
        {
            throw new NotImplementedException(@"ReadGeometry not implemented for this connector");
        }

        public virtual string func_UTC_NOW
        {
            get { return @"NOW()"; }
        }
        public virtual string func_LOWER
        {
            get { return @"LOWER"; }
        }
        public virtual string func_UPPER
        {
            get { return @"UPPER"; }
        }

        public virtual string func_YEAR(string date)
        {
            return @"YEAR(" + date + ")";
        }
        public virtual string func_MONTH(string date)
        {
            return @"MONTH(" + date + ")";
        }
        public virtual string func_DAY(string date)
        {
            return @"DAY(" + date + ")";
        }
        public virtual string func_HOUR(string date)
        {
            return @"HOUR(" + date + ")";
        }
        public virtual string func_MINUTE(string date)
        {
            return @"MINUTE(" + date + ")";
        }
        public virtual string func_SECOND(string date)
        {
            return @"SECONDS(" + date + ")";
        }

        public virtual string type_AUTOINCREMENT { get { return @"AUTOINCREMENT"; } }

        public virtual string type_TINYINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDTINYINT { get { return @"INT"; } }
        public virtual string type_SMALLINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDSMALLINT { get { return @"INT"; } }
        public virtual string type_INT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDINT { get { return @"INT"; } }
        public virtual string type_BIGINT { get { return @"INT"; } }
        public virtual string type_UNSIGNEDBIGINT { get { return @"INT"; } }
        public virtual string type_NUMERIC { get { return @"NUMERIC"; } }
        public virtual string type_DECIMAL { get { return @"DECIMAL"; } }
        public virtual string type_MONEY { get { return @"DECIMAL"; } }
        public virtual string type_VARCHAR { get { return @"NVARCHAR"; } }
        public virtual string type_CHAR { get { return @"NCHAR"; } }
        public virtual string type_TEXT { get { return @"NTEXT"; } }
        public virtual string type_MEDIUMTEXT { get { return @"NTEXT"; } }
        public virtual string type_LONGTEXT { get { return @"NTEXT"; } }
        public virtual string type_BOOLEAN { get { return @"BOOLEAN"; } }
        public virtual string type_DATETIME { get { return @"DATETIME"; } }
        public virtual string type_BLOB { get { return @"BLOB"; } }
        public virtual string type_GUID { get { return @"GUID"; } }

        public virtual string type_GEOMETRY { get { return @"GEOMETRY"; } }
        public virtual string type_GEOMETRYCOLLECTION { get { return @"GEOMETRYCOLLECTION"; } }
        public virtual string type_POINT { get { return @"POINT"; } }
        public virtual string type_LINESTRING { get { return @"LINESTRING"; } }
        public virtual string type_POLYGON { get { return @"POLYGON"; } }
        public virtual string type_LINE { get { return @"LINE"; } }
        public virtual string type_CURVE { get { return @"CURVE"; } }
        public virtual string type_SURFACE { get { return @"SURFACE"; } }
        public virtual string type_LINEARRING { get { return @"LINEARRING"; } }
        public virtual string type_MULTIPOINT { get { return @"MULTIPOINT"; } }
        public virtual string type_MULTILINESTRING { get { return @"MULTILINESTRING"; } }
        public virtual string type_MULTIPOLYGON { get { return @"MULTIPOLYGON"; } }
        public virtual string type_MULTICURVE { get { return @"MULTICURVE"; } }
        public virtual string type_MULTISURFACE { get { return @"MULTISURFACE"; } }

        public enum SqlServiceType
        {
            UNKNOWN = 0,
            MYSQL = 1,
            MSSQL = 2,
            MSACCESS = 3
        }
    }
}
