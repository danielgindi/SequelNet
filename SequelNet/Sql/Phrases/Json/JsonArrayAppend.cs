using System;
using System.Collections.Generic;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// Append a value to a json array
    /// </summary>
    public class JsonArrayAppend : IPhrase
    {
        public ValueWrapper Document;
        public string Path;
        public List<ValueWrapper> Values;

        #region Constructors

        public JsonArrayAppend()
        {
            Values = new List<ValueWrapper>();
        }

        public JsonArrayAppend(
            object doc, ValueObjectType docType,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(doc, docType);
            this.Path = path;
            this.Values.Add(new ValueWrapper(value, valueType));
        }

        public JsonArrayAppend(
            string docTableName, string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(docTableName, docColumnName);
            this.Path = path;
            this.Values.Add(new ValueWrapper(value, valueType));
        }

        public JsonArrayAppend(
            string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this(null, docColumnName, path, value, valueType)
        {
        }

        public JsonArrayAppend(
            IPhrase doc,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = new ValueWrapper(doc);
            this.Path = path;
            this.Values.Add(new ValueWrapper(value, valueType));
        }

        public JsonArrayAppend(
            IPhrase doc,
            string path,
            IPhrase value)
            : this()
        {
            this.Document = new ValueWrapper(doc);
            this.Path = path;
            this.Values.Add(new ValueWrapper(value));
        }

        public JsonArrayAppend(
            IPhrase doc,
            string path,
            params ValueWrapper[] values)
            : this()
        {
            this.Document = new ValueWrapper(doc);
            this.Path = path;
            this.Values.AddRange(values);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        ret += "JSON_ARRAY_APPEND(";
                        ret += Document.Build(conn, relatedQuery);
                        for (int i = 0, len = Values.Count; i < len; i++)
                        {
                            ret += ", ";
                            ret += conn.Language.PrepareValue(Path);
                            ret += ", ";
                            ret += Values[i].Build(conn, relatedQuery);
                        }
                        ret += ")";
                    }
                    break;

                default:
                    throw new NotSupportedException("JsonArrayAppend is not supported by current DB type");
            }

            return ret;
        }
    }
}
