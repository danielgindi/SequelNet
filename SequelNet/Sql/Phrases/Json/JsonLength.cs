using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// Returns the length of a json object/array
    /// </summary>
    public class JsonLength : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors
        
        public JsonLength(object value, ValueObjectType valueType)
        {
            this.Value = ValueWrapper.Make(value, valueType);
        }

        public JsonLength(string tableName, string columnName)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public JsonLength(string columnName)
            : this(null, columnName)
        {
        }

        public JsonLength(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public JsonLength(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        sb.Append("JSON_LENGTH(");
                        sb.Append(Value.Build(conn, relatedQuery));
                        sb.Append(")");
                    }
                    break;

                case ConnectorBase.SqlServiceType.POSTGRESQL:
                    {
                        sb.Append("json_array_length(");
                        sb.Append(Value.Build(conn, relatedQuery));
                        sb.Append(")");
                    }
                    break;

                default:
                    throw new NotSupportedException("JsonLength is not supported by current DB type");
            }
        }
    }
}
