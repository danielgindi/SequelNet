using System;
using System.Collections.Generic;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// Removes a value from a json object or array at the specified position
    /// </summary>
    public class JsonRemove : IPhrase
    {
        public ValueWrapper Document;
        public List<string> Paths;

        #region Constructors

        public JsonRemove()
        {
            Paths = new List<string>();
        }

        public JsonRemove(
            object doc, ValueObjectType docType,
            params string[] paths)
            : this()
        {
            this.Document = new ValueWrapper(doc, docType);

            foreach (var path in paths)
                this.Paths.Add(path);
        }

        public JsonRemove(
            string docTableName, string docColumnName,
            params string[] paths)
            : this()
        {
            this.Document = new ValueWrapper(docTableName, docColumnName);

            foreach (var path in paths)
                this.Paths.Add(path);
        }

        public JsonRemove(
            string docColumnName,
            params string[] paths)
            : this(null, docColumnName, paths)
        {
        }

        public JsonRemove(
            IPhrase doc,
            params string[] paths)
            : this()
        {
            this.Document = new ValueWrapper(doc);

            foreach (var path in paths)
                this.Paths.Add(path);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        ret += "JSON_REMOVE(";
                        ret += Document.Build(conn, relatedQuery);
                        foreach (var path in Paths)
                        {
                            ret += ", ";
                            ret += conn.Language.PrepareValue(path);
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
