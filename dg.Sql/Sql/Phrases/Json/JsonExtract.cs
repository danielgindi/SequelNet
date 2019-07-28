using System;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    /// <summary>
    /// Extract a member of a JSON object/array.
    /// The path consists of $ sign, . period, [] for array indexes, and member names.
    /// The dollar sign($) represents the context item.
    /// The property path is a set of path steps.
    /// Path steps can contain the following elements and operators.
    /// 
    ///     Key names:
    ///         If the key name starts with a dollar sign or contains special characters such as spaces, surround it with double quotes.
    ///         For example, $.name and $."first name".
    ///         
    ///     Array elements:
    ///         For example, $.product[3]. Arrays are zero-based.
    ///         
    ///     The dot operator (.) indicates a member of an object.
    /// </summary>
    public class JsonExtract : IPhrase
    {
        public ValueWrapper Value;
        public string Path = "$";

        #region Constructors
        
        public JsonExtract(object value, ValueObjectType valueType, string path = "$")
        {
            this.Value = new ValueWrapper(value, valueType);
            this.Path = path;
        }

        public JsonExtract(string tableName, string columnName, string path = "$")
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.Path = path;
        }

        public JsonExtract(string columnName, string path = "$")
            : this(null, columnName, path)
        {
        }

        public JsonExtract(IPhrase phrase, string path = "$")
            : this(phrase, ValueObjectType.Value, path)
        {
        }

        public JsonExtract(Where where, string path = "$")
            : this(where, ValueObjectType.Value, path)
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
                        ret += "JSON_UNQUOTE(JSON_EXTRACT(";
                        ret += Value.Build(conn, relatedQuery);
                        ret += ", ";
                        ret += conn.Language.PrepareValue(Path);
                        ret += "))";
                    }
                    break;

                case ConnectorBase.SqlServiceType.MSSQL:
                    {
                        ret += "JSON_VALUE(";
                        ret += Value.Build(conn, relatedQuery);
                        ret += ", ";
                        ret += conn.Language.PrepareValue(Path);
                        ret += ")";
                    }
                    break;

                case ConnectorBase.SqlServiceType.POSTGRESQL:
                    { // No support for returning "self". Postgres works with actual json Objects.
                        var parts = JsonPathValue.PathParts(Path);

                        if (parts.Count > 0 && parts[0] == "$")
                        {
                            parts.RemoveAt(0);
                        }

                        ret += "json_extract_path_text(";
                        ret += Value.Build(conn, relatedQuery);
                        foreach (var part in parts)
                        {
                            ret += ", " + conn.Language.PrepareValue(part);
                        }
                        ret += ")";
                    }
                    break;

                default:
                    throw new NotSupportedException("JsonExtract is not supported by current DB type");
            }

            return ret;
        }
    }
}
