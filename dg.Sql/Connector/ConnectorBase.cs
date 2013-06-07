using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

// This class needs a little cleanup. But it's gonna break dependent code, so I'm gonna postpone it. In the meanwhile we will still use this pretty ugly code.

namespace dg.Sql.Connector
{
    public abstract class ConnectorBase : IDisposable
    {
        public enum SqlServiceType
        {
            UNKNOWN = 0,
            MYSQL = 1,
            MSSQL = 2,
            MSACCESS = 3
        }

        #region Instancing

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

        virtual public SqlServiceType TYPE
        {
            get { return SqlServiceType.UNKNOWN; }
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

        public static string FindConnectionString(string ConnectionStringKey)
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

        public static string FindConnectionString()
        {
            return FindConnectionString(null);
        }

        abstract public DbConnection Connection { get; }

        #endregion

        #region Closing / Disposing

        abstract public void Dispose();
        abstract public void Close();

        #endregion

        #region Executing

        abstract public int ExecuteNonQuery(string QuerySql);
        abstract public int ExecuteNonQuery(DbCommand Command);
        abstract public object ExecuteScalar(string QuerySql);
        abstract public object ExecuteScalar(DbCommand Command);
        abstract public DataReaderBase ExecuteReader(string QuerySql);
        abstract public DataReaderBase ExecuteReader(string QuerySql, bool AttachConnectionToReader);
        abstract public DataReaderBase ExecuteReader(DbCommand Command);
        abstract public DataReaderBase ExecuteReader(DbCommand Command, bool AttachConnectionToReader);
        abstract public DataSet ExecuteDataSet(string QuerySql);
        abstract public DataSet ExecuteDataSet(DbCommand Command);
        abstract public int ExecuteScript(string QuerySql);

        #endregion

        #region Utilities

        virtual public string GetVersion()
        {
            throw new NotImplementedException(@"GetVersion not implemented in connector of type " + this.GetType().Name);
        }

        abstract public object GetLastInsertID();

        virtual public void SetIdentityInsert(string TableName, bool Enabled) { }

        abstract public bool CheckIfTableExists(string TableName);

        #endregion

        #region Transactions

        abstract public bool BeginTransaction();
        abstract public bool BeginTransaction(IsolationLevel IsolationLevel);
        abstract public bool CommitTransaction();
        abstract public bool RollbackTransaction();
        abstract public bool HasTransaction { get; }
        abstract public int CurrentTransactions { get; }

        abstract public DbTransaction Transaction { get; }

        #endregion

        #region Preparing values for SQL

        abstract public string EncloseFieldName(string FieldName);

        public virtual string EscapeString(string Value)
        {
            return Value.Replace(@"'", @"''");
        }
        public virtual string PrepareValue(decimal Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
        public virtual string PrepareValue(float Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
        public virtual string PrepareValue(double Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
        public virtual string PrepareValue(bool Value)
        {
            return Value ? @"1" : @"0";
        }
        abstract public string PrepareValue(Guid Value);
        public virtual string PrepareValue(string Value)
        {
            if (Value == null) return @"NULL";
            else return '\'' + EscapeString(Value) + '\'';
        }
        public virtual string PrepareValue(object Value)
        {
            if (Value == null || Value is DBNull) return @"NULL";
            else if (Value is string)
            {
                return PrepareValue((string)Value);
            }
            else if (Value is DateTime)
            {
                return '\'' + FormatDate((DateTime)Value) + '\'';
            }
            else if (Value is Guid)
            {
                return PrepareValue((Guid)Value);
            }
            else if (Value is bool)
            {
                return PrepareValue((bool)Value);
            }
            else if (Value is decimal)
            { // Must be formatted specifically, to avoid decimal separator confusion
                return PrepareValue((decimal)Value);
            }
            else if (Value is float)
            {
             // Must be formatted specifically, to avoid decimal separator confusion
                return PrepareValue((float)Value);
            }
            else if (Value is double)
            { // Must be formatted specifically, to avoid decimal separator confusion
                return PrepareValue((double)Value);
            }
            else if (Value is dg.Sql.BasePhrase)
            {
                return ((dg.Sql.BasePhrase)Value).BuildPhrase(this);
            }
            else if (Value is dg.Sql.Geometry)
            {
                StringBuilder sb = new StringBuilder();
                ((dg.Sql.Geometry)Value).BuildValue(sb, this);
                return sb.ToString();
            }
            else return Value.ToString();
        }

        abstract public string FormatDate(DateTime DateTime);

        public virtual string EscapeLike(string LikeExpression)
        {
            return LikeExpression.Replace(@"\", @"\\").Replace(@"%", @"\%");
        }

        #endregion

        #region Reading values from SQL

        /// <summary>
        /// Gets the value of the specified column in Geometry type given the column name.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column in Geometry type.</returns>
        /// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found</exception>
        public virtual Geometry ReadGeometry(object Value)
        {
            throw new NotImplementedException(@"ReadGeometry not implemented for this connector");
        }

        #endregion

        #region Engine-specific keywords

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

        public virtual string func_YEAR(string Date)
        {
            return @"YEAR(" + Date + ")";
        }
        public virtual string func_MONTH(string Date)
        {
            return @"MONTH(" + Date + ")";
        }
        public virtual string func_DAY(string Date)
        {
            return @"DAY(" + Date + ")";
        }
        public virtual string func_HOUR(string Date)
        {
            return @"HOUR(" + Date + ")";
        }
        public virtual string func_MINUTE(string Date)
        {
            return @"MINUTE(" + Date + ")";
        }
        public virtual string func_SECOND(string Date)
        {
            return @"SECONDS(" + Date + ")";
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
        
        #endregion

        #region Legacy, backwards compatibility

        [Obsolete("GetWebsiteConnectionString is deprecated, please use FindConnectionString instead.")]
        public static String GetWebsiteConnectionString(string ConnectionStringKey)
        {
            return FindConnectionString(ConnectionStringKey);
        }

        [Obsolete("GetWebsiteConnectionString is deprecated, please use FindConnectionString instead.")]
        public static String GetWebsiteConnectionString()
        {
            return FindConnectionString();
        }

        [Obsolete("beginTransaction is deprecated, please use BeginTransaction instead.")]
        [CLSCompliant(false)]
        public bool beginTransaction()
        {
            return BeginTransaction();
        }

        [Obsolete("beginTransaction is deprecated, please use BeginTransaction instead.")]
        [CLSCompliant(false)]
        public bool beginTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction(isolationLevel);
        }

        [Obsolete("commitTransaction is deprecated, please use CommitTransaction instead.")]
        [CLSCompliant(false)]
        public bool commitTransaction()
        {
            return CommitTransaction();
        }

        [Obsolete("rollbackTransaction is deprecated, please use RollbackTransaction instead.")]
        [CLSCompliant(false)]
        public bool rollbackTransaction()
        {
            return RollbackTransaction();
        }

        [Obsolete("hasTransaction is deprecated, please use HasTransaction instead.")]
        [CLSCompliant(false)]
        public bool hasTransaction { get { return HasTransaction; } }

        [Obsolete("currentTransactions is deprecated, please use CurrentTransactions instead.")]
        [CLSCompliant(false)]
        public int currentTransactions { get { return CurrentTransactions; } }


        [Obsolete("setIdentityInsert is deprecated, please use SetIdentityInsert instead.")]
        [CLSCompliant(false)]
        public void setIdentityInsert(string TableName, bool Enabled)
        {
            SetIdentityInsert(TableName, Enabled);
        }

        [Obsolete("checkIfTableExists is deprecated, please use CheckIfTableExists instead.")]
        [CLSCompliant(false)]
        public bool checkIfTableExists(string TableName)
        {
            return CheckIfTableExists(TableName);
        }

        [Obsolete("fullEscape is deprecated, please use EscapeString instead.")]
        [CLSCompliant(false)]
        public virtual string fullEscape(string Value)
        {
            return EscapeString(Value);
        }

        [Obsolete("prepareQuotedString is deprecated, please use PrepareValue instead.")]
        [CLSCompliant(false)]
        public string prepareQuotedString(string Value)
        {
            return PrepareValue(Value);
        }

        [Obsolete("prepareValue is deprecated, please use PrepareValue instead.")]
        [CLSCompliant(false)]
        public string prepareValue(object Value)
        {
            return PrepareValue(Value);
        }

        [Obsolete("encloseFieldName is deprecated, please use EncloseFieldName instead.")]
        [CLSCompliant(false)]
        public string encloseFieldName(string FieldName)
        {
            return EncloseFieldName(FieldName);
        }

        [Obsolete("formatDate is deprecated, please use FormatDate instead.")]
        [CLSCompliant(false)]
        public string formatDate(DateTime DateTime)
        {
            return FormatDate(DateTime);
        }

        #endregion
    }
}
