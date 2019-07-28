﻿using System;
using System.Collections.Generic;
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
        public enum SqlServiceType
        {
            UNKNOWN = 0,
            MYSQL = 1,
            MSSQL = 2,
            MSACCESS = 3,
            POSTGRESQL = 4
        }

        #region Instancing

        private DbConnection _Connection = null;

        static Type s_ConnectorType = null;
        static public ConnectorBase NewInstance()
        {
            if (s_ConnectorType == null)
            {
                s_ConnectorType = FindConnectorType();
            }
            return (ConnectorBase)Activator.CreateInstance(s_ConnectorType);
        }

        static public ConnectorBase NewInstance(string connectionStringKey)
        {
            if (s_ConnectorType == null)
            {
                s_ConnectorType = FindConnectorType();
            }
            return (ConnectorBase)Activator.CreateInstance(s_ConnectorType, new string[] { connectionStringKey });
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

            return type;
        }

        public static string FindConnectionString(string connectionStringKey)
        {
            ConnectionStringSettings connString = null;

            if (connectionStringKey != null)
            {
                connString = ConfigurationManager.ConnectionStrings[connectionStringKey];
                if (connString != null) return connString.ConnectionString;
            }

            string appConnString = ConfigurationManager.AppSettings[@"dg.Sql::ConnectionStringKey"];
            if (appConnString != null && appConnString.Length > 0) connString = ConfigurationManager.ConnectionStrings[appConnString];
            if (connString != null) return connString.ConnectionString;

            appConnString = ConfigurationManager.AppSettings[@"dg.Sql::ConnectionString"];
            if (appConnString != null && appConnString.Length > 0) return appConnString;

            connString = ConfigurationManager.ConnectionStrings[@"dg.Sql"];
            if (connString != null) return connString.ConnectionString;

            return ConfigurationManager.ConnectionStrings[0].ConnectionString;
        }

        public static string FindConnectionString()
        {
            return FindConnectionString(null);
        }

        abstract public FactoryBase Factory
        {
            get;
        }

        public DbConnection Connection
        {
            get { return _Connection; }
            protected set { _Connection = value; }
        }

        private static LanguageFactory _LanguageFactory = new LanguageFactory();

        public virtual LanguageFactory Language => _LanguageFactory;

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
            // Now clean up Native Resources (Pointers)
        }

        public virtual void Close()
        {
            try
            {
                if (_Connection != null && _Connection.State != ConnectionState.Closed)
                {
                    try
                    {
                        while (HasTransaction)
                            RollbackTransaction();
                    }
                    catch { /*ignore errors here*/ }

                    _Connection.Close();
                }
            }
            catch { }
            if (_Connection != null) _Connection.Dispose();
            _Connection = null;
        }

        #endregion

        #region Executing
        
        public virtual int ExecuteNonQuery(DbCommand command)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;
            return command.ExecuteNonQuery();
        }
        
        public virtual object ExecuteScalar(DbCommand command)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;
            return command.ExecuteScalar();
        }

        public virtual DataReaderBase ExecuteReader(DbCommand command, bool attachCommandToReader = false, bool attachConnectionToReader = false)
        {
            try
            {
                if (Connection.State != ConnectionState.Open) Connection.Open();

                command.Connection = Connection;
                command.Transaction = Transaction;

                return new DataReaderBase(
                    command.ExecuteReader(),
                    attachCommandToReader ? command : null,
                    attachConnectionToReader ? this : null);
            }
            catch (Exception ex)
            {
                if (attachCommandToReader && command != null)
                    command.Dispose();

                if (attachConnectionToReader && Connection != null)
                    Connection.Dispose();

                throw ex;
            }
        }
        
        public virtual DataSet ExecuteDataSet(DbCommand command)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();
            command.Connection = Connection;
            command.Transaction = Transaction;

            var dataSet = new DataSet();

            using (var adapter = Factory.NewDataAdapter(command))
            {
                adapter.Fill(dataSet);
            }

            return dataSet;
        }

        public virtual int ExecuteNonQuery(string querySql)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();

            using (var command = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public virtual object ExecuteScalar(string querySql)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();

            using (var command = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return command.ExecuteScalar();
            }
        }

        public virtual DataReaderBase ExecuteReader(string querySql)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();

            var command = Factory.NewCommand(querySql, Connection, Transaction);
            return ExecuteReader(command, true);
        }

        public virtual DataReaderBase ExecuteReader(string querySql, bool attachConnectionToReader)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();

            var command = Factory.NewCommand(querySql, Connection, Transaction);
            return ExecuteReader(command, true, attachConnectionToReader);
        }

        public virtual DataSet ExecuteDataSet(string querySql)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();

            using (var cmd = Factory.NewCommand(querySql, Connection, Transaction))
            {
                return ExecuteDataSet(cmd);
            }
        }

        public abstract int ExecuteScript(string querySql);

        #endregion

        #region Utilities

        virtual public string GetVersion()
        {
            throw new NotImplementedException(@"GetVersion not implemented in connector of type " + this.GetType().Name);
        }

        virtual public bool SupportsSelectPaging()
        {
            return false;
        }

        abstract public object GetLastInsertID();

        virtual public void SetIdentityInsert(string tableName, bool enabled) { }

        abstract public bool CheckIfTableExists(string tableName);

        #endregion

        #region Transactions
        
        private DbTransaction _Transaction = null;
        private Stack<DbTransaction> _Transactions = null;

        public virtual bool BeginTransaction()
        {
            try
            {
                if (_Connection.State != ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction();
                if (_Transactions == null) _Transactions = new Stack<DbTransaction>(1);
                _Transactions.Push(_Transaction);
                return (_Transaction != null);
            }
            catch (DbException) { }
            return false;
        }

        public virtual bool BeginTransaction(IsolationLevel IsolationLevel)
        {
            try
            {
                if (_Connection.State != ConnectionState.Open) _Connection.Open();
                _Transaction = _Connection.BeginTransaction(IsolationLevel);
                if (_Transactions == null) _Transactions = new Stack<DbTransaction>(1);
                _Transactions.Push(_Transaction);
            }
            catch (DbException) { return false; }
            return (_Transaction != null);
        }

        public virtual bool CommitTransaction()
        {
            if (_Transaction == null) return false;
            else
            {
                _Transactions.Pop();

                try
                {
                    _Transaction.Commit();
                }
                catch (DbException) { return false; }

                if (_Transactions.Count > 0) _Transaction = _Transactions.Peek();
                else _Transaction = null;
                return true;
            }
        }

        public virtual bool RollbackTransaction()
        {
            if (_Transaction == null) return false;
            else
            {
                _Transactions.Pop();

                try
                {
                    _Transaction.Rollback();
                }
                catch (DbException) { return false; }

                if (_Transactions.Count > 0) _Transaction = _Transactions.Peek();
                else _Transaction = null;
                return true;
            }
        }

        public virtual bool HasTransaction
        {
            get { return _Transaction != null; }
        }

        public virtual int CurrentTransactions
        {
            get { return _Transactions == null ? 0 : _Transactions.Count; }
        }

        public DbTransaction Transaction
        {
            get { return _Transaction; }
        }

        #endregion

        #region DB Mutex

        public enum SqlMutexOwner
        {
            /// <summary>
            /// Used to declare the mutex owned by the session, and expires when the session ends.
            /// </summary>
            Session,

            /// <summary>
            /// Used to declare the mutex owned by the transaction, and expires when the transaction ends.
            /// If <value>Transaction</value> is specified, then the lock must be acquired within a transaction.
            /// 
            /// * Not supported by MySql
            /// </summary>
            Transaction
        }

        /// <summary>
        /// Creates a mutex on the DB server.
        /// </summary>
        /// <param name="lockName">Unique name for the lock</param>
        /// <param name="owner">The owner of the lock. Partial support in different db servers.</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="dbPrincipal">The user that has permissions to the lock.</param>
        /// <returns><value>true</value> if the lock was acquired, <value>false</value> if failed or timed out.</returns>
        public virtual bool GetLock(string lockName, TimeSpan timeout, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            throw new NotImplementedException(@"GetLock");
        }

        /// <summary>
        /// Releases a mutex on the DB server.
        /// </summary>
        /// <param name="lockName">Unique name for the lock</param>
        /// <param name="owner">The owner of the lock. Partial support in different db servers.</param>
        /// <param name="dbPrincipal">The user that has permissions to the lock.</param>
        /// <returns><value>true</value> if the lock was release, <value>false</value> if failed, not exists or timed out.</returns>
        public virtual bool ReleaseLock(string lockName, SqlMutexOwner owner = SqlMutexOwner.Session, string dbPrincipal = null)
        {
            throw new NotImplementedException(@"ReleaseLock");
        }

        #endregion
    }
}
