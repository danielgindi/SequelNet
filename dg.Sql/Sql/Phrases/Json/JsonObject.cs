using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    /// <summary>
    /// Creates a json object.
    /// </summary>
    public class JsonObject : IPhrase
    {
        public Dictionary<string, ValueWrapper> Values;

        #region Constructors

        public JsonObject()
        {
            this.Values = new Dictionary<string, ValueWrapper>();
        }

        public JsonObject(Dictionary<string, ValueWrapper> values)
        {
            this.Values = values == null
                ? new Dictionary<string, ValueWrapper>()
                : new Dictionary<string, ValueWrapper>(values);
        }

        public JsonObject(Dictionary<string, object> values)
        {
            this.Values = new Dictionary<string, ValueWrapper>();
            if (values != null)
            {
                foreach (var pair in values)
                {
                    Values.Add(pair.Key, ValueWrapper.From(pair.Value, ValueObjectType.Value));
                }
            }
        }

        public JsonObject(Dictionary<string, string> values)
        {
            this.Values = new Dictionary<string, ValueWrapper>();
            if (values != null)
            {
                foreach (var pair in values)
                {
                    Values.Add(pair.Key, ValueWrapper.From(pair.Value, ValueObjectType.Value));
                }
            }
        }

        public JsonObject(Dictionary<string, Int64> values)
        {
            this.Values = new Dictionary<string, ValueWrapper>();
            if (values != null)
            {
                foreach (var pair in values)
                {
                    Values.Add(pair.Key, ValueWrapper.From(pair.Value, ValueObjectType.Value));
                }
            }
        }

        public JsonObject(Dictionary<string, Int32> values)
        {
            this.Values = new Dictionary<string, ValueWrapper>();
            if (values != null)
            {
                foreach (var pair in values)
                {
                    Values.Add(pair.Key, ValueWrapper.From(pair.Value, ValueObjectType.Value));
                }
            }
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        var sb = new StringBuilder();
                        sb.Append("JSON_OBJECT(");

                        bool first = true;
                        foreach (var val in Values)
                        {
                            if (first) first = false;
                            else sb.Append(",");

                            sb.Append(conn.PrepareValue(val.Key));
                            sb.Append(",");
                            sb.Append(val.Value.Build(conn, relatedQuery));
                        }

                        sb.Append(")");

                        return sb.ToString();
                    }

                case ConnectorBase.SqlServiceType.POSTGRESQL:
                    {
                        var sb = new StringBuilder();
                        sb.Append("json_build_object(");

                        bool first = true;
                        foreach (var val in Values)
                        {
                            if (first) first = false;
                            else sb.Append(",");

                            sb.Append(conn.PrepareValue(val.Key));
                            sb.Append(",");
                            sb.Append(val.Value.Build(conn, relatedQuery));
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
