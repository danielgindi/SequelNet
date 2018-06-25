using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    /// <summary>
    /// Append a value to a json array at the specified position
    /// </summary>
    public class JsonArrayInsert : IPhrase
    {
        public ValueWrapper Document;
        public List<JsonPathValue> Values;

        #region Constructors

        public JsonArrayInsert()
        {
            Values = new List<JsonPathValue>();
        }

        public JsonArrayInsert(
            object doc, ValueObjectType docType,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(doc, docType);
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value, valueType)));
        }

        public JsonArrayInsert(
            string docTableName, string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(docTableName, docColumnName);
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value, valueType)));
        }

        public JsonArrayInsert(
            string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this(null, docColumnName, path, value, valueType)
        {
        }

        public JsonArrayInsert(
            IPhrase doc,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(doc);
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value, valueType)));
        }

        public JsonArrayInsert(
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
                        ret += "JSON_ARRAY_INSERT(";
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
                    throw new NotSupportedException("JsonArrayInsert is not supported by current DB type");
            }

            return ret;
        }
    }
}
