using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using dg.Sql.Connector;

namespace dg.Sql
{
    public partial class Query
    {
        /// <summary>
        /// Calls a stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">Procedue to call</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query StoredProcedure(string storedProcedureName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.ExecuteStoredProcedure;
            _StoredProcedureName = storedProcedureName;
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// You can use SqlMgrFactoryBase.Factory() in order to create parameters.
        /// </summary>
        /// <param name="dbParameter">Parameter</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(DbParameter dbParameter)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(dbParameter);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(string name, object value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(name, value));
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="type">Parameter's type</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(string name, DbType type, object value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(name, type, value));
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="parameterDirection">Parameter's input/output direction</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(string name, object value, ParameterDirection parameterDirection)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(name, value, parameterDirection));
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="type">Parameter's type</param>
        /// <param name="parameterDirection">Parameter's input/output direction</param>
        /// <param name="size">Paremeter's type definition: Size</param>
        /// <param name="isNullable">Paremeter's type definition: Is nullable?</param>
        /// <param name="precision">Paremeter's type definition: Precision</param>
        /// <param name="sourceColumn">Source column</param>
        /// <param name="sourceVersion">Source version</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(
            string name, DbType type, ParameterDirection parameterDirection,
            int size, bool isNullable, byte precision, byte scale, 
            string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(name, type, parameterDirection, size, isNullable, precision, scale, sourceColumn, sourceVersion, value));
            return this;
        }
    }
}
