using System;
using SequelNet.Connector;

namespace SequelNet.Phrases
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
        public bool Unquote = false;

        #region Constructors

        public JsonExtract(object value, ValueObjectType valueType, string path = "$", bool unquote = true)
        {
            this.Value = new ValueWrapper(value, valueType);
            this.Path = path;
            this.Unquote = unquote;
        }

        public JsonExtract(string tableName, string columnName, string path = "$", bool unquote = true)
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.Path = path;
            this.Unquote = unquote;
        }

        public JsonExtract(IPhrase phrase, string path = "$", bool unquote = true)
            : this(phrase, ValueObjectType.Value, path, unquote)
        {
        }

        public JsonExtract(Where where, string path = "$", bool unquote = true)
            : this(where, ValueObjectType.Value, path, unquote)
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
                        var phrase = $"JSON_EXTRACT({Value.Build(conn, relatedQuery)}, {conn.Language.PrepareValue(Path)})";

                        if (Unquote)
                            ret += 
                                $"(CASE WHEN JSON_TYPE({phrase} = 'NULL' THEN NULL" +
                                $"WHEN JSON_TYPE({phrase}) = 'STRING' THEN JSON_UNQUOTE({phrase})" +
                                $"ELSE {phrase} END)";
                        else ret += phrase;
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
