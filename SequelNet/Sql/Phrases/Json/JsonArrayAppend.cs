using System;
using System.Collections.Generic;
using System.Text;
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
            this.Document = ValueWrapper.Make(doc, docType);
            this.Path = path;
            this.Values.Add(ValueWrapper.Make(value, valueType));
        }

        public JsonArrayAppend(
            string docTableName, string docColumnName,
            string path,
            object value, ValueObjectType valueType)
            : this()
        {
            this.Document = ValueWrapper.Column(docTableName, docColumnName);
            this.Path = path;
            this.Values.Add(ValueWrapper.Make(value, valueType));
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
            this.Document = ValueWrapper.From(doc);
            this.Path = path;
            this.Values.Add(ValueWrapper.Make(value, valueType));
        }

        public JsonArrayAppend(
            IPhrase doc,
            string path,
            IPhrase value)
            : this()
        {
            this.Document = ValueWrapper.From(doc);
            this.Path = path;
            this.Values.Add(ValueWrapper.From(value));
        }

        public JsonArrayAppend(
            IPhrase doc,
            string path,
            params ValueWrapper[] values)
            : this()
        {
            this.Document = ValueWrapper.From(doc);
            this.Path = path;
            this.Values.AddRange(values);
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        sb.Append("JSON_ARRAY_APPEND(");
                        sb.Append(Document.Build(conn, relatedQuery));
                        for (int i = 0, len = Values.Count; i < len; i++)
                        {
                            sb.Append(", ");
                            sb.Append(conn.Language.PrepareValue(Path));
                            sb.Append(", ");
                            sb.Append(Values[i].Build(conn, relatedQuery));
                        }
                        sb.Append(")");
                    }
                    break;

                default:
                    throw new NotSupportedException("JsonArrayAppend is not supported by current DB type");
            }
        }
    }
}
