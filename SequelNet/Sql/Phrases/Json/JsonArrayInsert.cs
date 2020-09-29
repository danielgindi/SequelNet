using System;
using System.Collections.Generic;
using SequelNet.Connector;

namespace SequelNet.Phrases
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
            this.Document = ValueWrapper.Make(doc, docType);
            this.Values.Add(JsonPathValue.From(path, ValueWrapper.Make(value, valueType)));
        }

        public JsonArrayInsert(
            string docTableName, string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = ValueWrapper.Column(docTableName, docColumnName);
            this.Values.Add(JsonPathValue.From(path, ValueWrapper.Make(value, valueType)));
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
            this.Document = ValueWrapper.From(doc);
            this.Values.Add(JsonPathValue.From(path, ValueWrapper.Make(value, valueType)));
        }

        public JsonArrayInsert(
            IPhrase doc,
            string path,
            IPhrase value)
            : this()
        {
            this.Document = ValueWrapper.From(doc);
            this.Values.Add(JsonPathValue.From(path, ValueWrapper.From(value)));
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
                            ret += conn.Language.PrepareValue(pair.Path);
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
