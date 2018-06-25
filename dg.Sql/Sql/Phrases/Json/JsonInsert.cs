using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    /// <summary>
    /// Inserts a value to a json object or array at the specified position, not replacing existing values.
    /// </summary>
    public class JsonInsert : IPhrase
    {
        public ValueWrapper Document;
        public List<JsonPathValue> Values;

        #region Constructors

        public JsonInsert()
        {
            Values = new List<JsonPathValue>();
        }

        public JsonInsert(
            object doc, ValueObjectType docType,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(doc, docType);
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value, valueType)));
        }

        public JsonInsert(
            string docTableName, string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(docTableName, docColumnName);
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value, valueType)));
        }

        public JsonInsert(
            string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this(null, docColumnName, path, value, valueType)
        {
        }

        public JsonInsert(
            IPhrase doc,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(doc);
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value, valueType)));
        }

        public JsonInsert(
            IPhrase doc,
            string path,
            IPhrase value)
            : this()
        {
            this.Document = new ValueWrapper(doc);
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value)));
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        ret += "JSON_INSERT(";
                        ret += Document.Build(conn, relatedQuery);
                        foreach (var pair in Values)
                        {
                            ret += ", ";
                            ret += conn.PrepareValue(pair.Path);
                            ret += ", ";
                            ret += pair.Value.Build(conn, relatedQuery);
                        }
                        ret += ")";
                    }
                    break;

                default:
                    throw new NotSupportedException("JsonInsert is not supported by current DB type");
            }

            return ret;
        }
    }
}
