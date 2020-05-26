using System;
using System.Collections.Generic;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// Sets a value to a json object or array at the specified position
    /// </summary>
    public class JsonSet : IPhrase
    {
        public ValueWrapper Document;
        public List<JsonPathValue> Values;

        #region Constructors

        public JsonSet()
        {
            Values = new List<JsonPathValue>();
        }

        public JsonSet(
            ValueWrapper doc,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = doc;
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value, valueType)));
        }

        public JsonSet(
            object doc, ValueObjectType docType,
            string path,
            object value, ValueObjectType valueType)
            : this(ValueWrapper.From(doc, docType), path, value, valueType)
        {
        }

        public JsonSet(
            string docTableName, string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this(ValueWrapper.Column(docTableName, docColumnName), path, value, valueType)
        {
        }

        public JsonSet(
            string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this(ValueWrapper.Column(docColumnName), path, value, valueType)
        {
        }

        public JsonSet(
            IPhrase doc,
            string path,
            object value, ValueObjectType valueType)
            : this(ValueWrapper.From(doc), path, value, valueType)
        {
        }

        public JsonSet(
            IPhrase doc,
            string path,
            IPhrase value)
            : this()
        {
            this.Document = ValueWrapper.From(doc);
            this.Values.Add(JsonPathValue.From(path, new ValueWrapper(value)));
        }

        public JsonSet(
            ValueWrapper doc,
            params JsonPathValue[] pathValues)
            : this()
        {
            this.Document = doc;

            foreach (var pair in pathValues)
                this.Values.Add(pair);
        }

        public JsonSet(
            IPhrase doc,
            params JsonPathValue[] pathValues)
            : this(ValueWrapper.From(doc), pathValues)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        ret += "JSON_SET(";
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
                    throw new NotSupportedException("JsonSet is not supported by current DB type");
            }

            return ret;
        }
    }
}
