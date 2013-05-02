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
        /// <param name="StoredProcedureName">Procedue to call</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query StoredProcedure(string StoredProcedureName)
        {
            ClearSelect();
            ClearOrderBy();
            ClearGroupBy();
            ClearInsertAndUpdate();
            ClearStoredProcedureParameters();
            this.QueryMode = QueryMode.ExecuteStoredProcedure;
            _StoredProcedureName = StoredProcedureName;
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// You can use SqlMgrFactoryBase.Factory() in order to create parameters.
        /// </summary>
        /// <param name="DbParameter">Parameter</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(DbParameter DbParameter)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(DbParameter);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <param name="Value">Parameter value</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(string Name, object Value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(Name, Value));
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <param name="DbType">Parameter's type</param>
        /// <param name="Value">Parameter value</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(string Name, DbType Type, object Value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(Name, Type, Value));
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <param name="Value">Parameter value</param>
        /// <param name="ParameterDirection">Parameter's input/output direction</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(string Name, object Value, ParameterDirection ParameterDirection)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(Name, Value, ParameterDirection));
            return this;
        }

        /// <summary>
        /// Adds a parameter to the Stored Procedure execution.
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <param name="DbType">Parameter's type</param>
        /// <param name="ParameterDirection">Parameter's input/output direction</param>
        /// <param name="Size">Paremeter's type definition: Size</param>
        /// <param name="IsNullable">Paremeter's type definition: Is nullable?</param>
        /// <param name="Precision">Paremeter's type definition: Precision</param>
        /// <param name="SourceColumn">Source column</param>
        /// <param name="SourceVersion">Source version</param>
        /// <param name="Value">Parameter value</param>
        /// <returns>Current <typeparamref name="Query"/> object</returns>
        public Query AddStoredProcedureParameter(string Name, DbType Type, ParameterDirection ParameterDirection, int Size, bool IsNullable, byte Precision, byte Scale, string SourceColumn, DataRowVersion SourceVersion, object Value)
        {
            if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameter>();
            _StoredProcedureParameters.Add(FactoryBase.Factory().NewParameter(Name, Type, ParameterDirection, Size, IsNullable, Precision, Scale, SourceColumn, SourceVersion, Value));
            return this;
        }
    }
}
