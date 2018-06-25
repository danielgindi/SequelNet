using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    /// <summary>
    /// Creates a json array.
    /// </summary>
    public class JsonArray : IPhrase
    {
        public List<ValueWrapper> Values;

        #region Constructors

        public JsonArray()
        {
            this.Values = new List<ValueWrapper>();
        }

        public JsonArray(params ValueWrapper[] values)
        {
            this.Values = new List<ValueWrapper>(values);
        }

        public JsonArray(IEnumerable<ValueWrapper> values)
        {
            this.Values = values == null ? new List<ValueWrapper>() : new List<ValueWrapper>(values);
        }

        public JsonArray(params object[] values)
        {
            this.Values = new List<ValueWrapper>(values.Length);
            foreach (var val in values)
                this.Values.Add(ValueWrapper.From(val, ValueObjectType.Value));
        }

        public JsonArray(params string[] values)
        {
            this.Values = new List<ValueWrapper>(values.Length);
            foreach (var val in values)
                this.Values.Add(ValueWrapper.From(val, ValueObjectType.Value));
        }

        public JsonArray(params Int64[] values)
        {
            this.Values = new List<ValueWrapper>(values.Length);
            foreach (var val in values)
                this.Values.Add(ValueWrapper.From(val, ValueObjectType.Value));
        }

        public JsonArray(params Int32[] values)
        {
            this.Values = new List<ValueWrapper>(values.Length);
            foreach (var val in values)
                this.Values.Add(ValueWrapper.From(val, ValueObjectType.Value));
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        var sb = new StringBuilder();
                        sb.Append("JSON_ARRAY(");
                        bool first = true;
                        foreach (var val in Values)
                        {
                            if (first) first = false;
                            else sb.Append(",");
                            sb.Append(val.Build(conn, relatedQuery));
                        }
                        sb.Append(")");

                        return sb.ToString();
                    }

                case ConnectorBase.SqlServiceType.POSTGRESQL:
                    {
                        var sb = new StringBuilder();
                        sb.Append("json_build_array(");
                        bool first = true;
                        foreach (var val in Values)
                        {
                            if (first) first = false;
                            else sb.Append(",");
                            sb.Append(val.Build(conn, relatedQuery));
                        }
                        sb.Append(")");

                        return sb.ToString();
                    }

                default:
                    throw new NotSupportedException("JsonExtract is not supported by current DB type");
            }
        }
    }
}
