﻿using System;
using System.Text;
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

        public JsonExtract(object value, ValueObjectType valueType, string path, bool unquote = true)
        {
            this.Value = ValueWrapper.Make(value, valueType);
            this.Path = path;
            this.Unquote = unquote;
        }

        public JsonExtract(string tableName, string columnName, string path, bool unquote = true)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Path = path;
            this.Unquote = unquote;
        }

        public JsonExtract(string columnName, string path, bool unquote = true)
            : this(null, columnName, path, unquote)
        {
        }

        public JsonExtract(IPhrase phrase, string path, bool unquote = true)
            : this(phrase, ValueObjectType.Value, path, unquote)
        {
        }

        public JsonExtract(Where where, string path, bool unquote = true)
            : this(where, ValueObjectType.Value, path, unquote)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            switch (conn.TYPE)
            {
                case ConnectorBase.SqlServiceType.MYSQL:
                    {
                        var phrase = $"JSON_EXTRACT({Value.Build(conn, relatedQuery)}, {conn.Language.PrepareValue(Path)})";

                        if (Unquote)
                        {
                            sb.Append(
                                $"(CASE WHEN JSON_TYPE({phrase}) = 'NULL' THEN NULL" +
                                $" WHEN JSON_TYPE({phrase}) = 'STRING' THEN JSON_UNQUOTE({phrase})" +
                                $" ELSE {phrase} END)"
                            );
                        }
                        else sb.Append(phrase);
                    }
                    break;

                case ConnectorBase.SqlServiceType.MSSQL:
                    {
                        sb.Append("JSON_VALUE(");
                        sb.Append(Value.Build(conn, relatedQuery));
                        sb.Append(", ");
                        sb.Append(conn.Language.PrepareValue(Path));
                        sb.Append(")");
                    }
                    break;

                case ConnectorBase.SqlServiceType.POSTGRESQL:
                    { // No support for returning "self". Postgres works with actual json Objects.
                        var parts = JsonPathValue.PathParts(Path);

                        if (parts.Count > 0 && parts[0] == "$")
                        {
                            parts.RemoveAt(0);
                        }

                        sb.Append("json_extract_path_text(");
                        sb.Append(Value.Build(conn, relatedQuery));
                        foreach (var part in parts)
                        {
                            sb.Append(", " + conn.Language.PrepareValue(part));
                        }
                        sb.Append(")");
                    }
                    break;

                default:
                    throw new NotSupportedException("JsonExtract is not supported by current DB type");
            }
        }
    }
}
