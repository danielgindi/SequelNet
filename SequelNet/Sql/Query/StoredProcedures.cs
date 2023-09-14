using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using SequelNet.Connector;

namespace SequelNet;

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
        if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameterWrapper>();
        _StoredProcedureParameters.Add(new DbParameterWrapper {
            Parameter = dbParameter,
        });
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
        if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameterWrapper>();
        _StoredProcedureParameters.Add(new DbParameterWrapper
        {
            ParameterName = name,
            Value = value,
        });
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
        if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameterWrapper>();
        _StoredProcedureParameters.Add(new DbParameterWrapper
        {
            ParameterName = name,
            Value = value,
            DbType = type,
        });
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
        if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameterWrapper>();
        _StoredProcedureParameters.Add(new DbParameterWrapper
        {
            ParameterName = name,
            Value = value,
            Direction = parameterDirection,
        });
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
        if (_StoredProcedureParameters == null) _StoredProcedureParameters = new List<DbParameterWrapper>();
        _StoredProcedureParameters.Add(new DbParameterWrapper
        {
            ParameterName = name,
            DbType = type,
            Value = value,
            Direction = parameterDirection,
            Size = size,
            IsNullable = isNullable,
            Precision = precision,
            Scale = scale,
            SourceColumn = sourceColumn,
            SourceVersion = sourceVersion,
        });
        return this;
    }

    private class DbParameterWrapper
    {
        public DbParameter Parameter { get; set; }
        public DbType? DbType { get; set; }
        public ParameterDirection? Direction { get; set; }
        public bool? IsNullable { get; set; }
        public string ParameterName { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Size { get; set; }
        public string SourceColumn { get; set; }
        public bool? SourceColumnNullMapping { get; set; }
        public DataRowVersion? SourceVersion { get; set; }
        public object Value { get; set; }

        public DbParameter Build(IConnectorFactory factory)
        {
            if (Parameter != null)
            {
                return Parameter;
            }
            else
            {
                var param = factory.NewParameter(ParameterName, Value);

                if (DbType != null) param.DbType = DbType.Value;
                if (Direction != null) param.Direction = Direction.Value;
                if (IsNullable != null) param.IsNullable = IsNullable.Value;
                if (Precision != null) param.Precision = Precision.Value;
                if (Scale != null) param.Scale = Scale.Value;
                if (Size != null) param.Size = Size.Value;
                if (SourceColumn != null) param.SourceColumn = SourceColumn;
                if (SourceColumnNullMapping != null) param.SourceColumnNullMapping = SourceColumnNullMapping.Value;
                if (SourceVersion != null) param.SourceVersion = SourceVersion.Value;

                return param;
            }
        }
    }
}
