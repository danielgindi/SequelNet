using System.Collections.Generic;

namespace SequelNet
{
    public class JsonPathValue
    {
        public string Path;
        public ValueWrapper Value;

        #region Constructors

        public JsonPathValue()
        {
            this.Value = new ValueWrapper();
        }

        public JsonPathValue(string path, string tableName, string columnName)
        {
            this.Path = path;
            this.Value = ValueWrapper.Column(tableName, columnName);
        }

        public JsonPathValue(string path, string column)
        {
            this.Path = path;
            this.Value = ValueWrapper.Column(column);
        }

        public JsonPathValue(string path, object value, ValueObjectType type)
        {
            this.Path = path;
            this.Value = ValueWrapper.Make(value, type);
        }

        public JsonPathValue(string path, IPhrase value)
        {
            this.Path = path;
            this.Value = ValueWrapper.From(value);
        }

        public JsonPathValue(string path, ValueWrapper value)
        {
            this.Path = path;
            this.Value = value;
        }

        #endregion

        #region Convenience

        public static JsonPathValue From(string path, string tableName, string columnName)
        {
            return new JsonPathValue(path, tableName, columnName);
        }

        public static JsonPathValue From(string path, string column)
        {
            return new JsonPathValue(path, column);
        }

        public static JsonPathValue From(string path, object value, ValueObjectType type)
        {
            return new JsonPathValue(path, value, type);
        }
        
        public static JsonPathValue From(string path, IPhrase value)
        {
            return new JsonPathValue(path, value);
        }

        public static JsonPathValue From(string path, ValueWrapper value)
        {
            return new JsonPathValue(path, value);
        }

        #endregion

        #region Utility

        public static List<string> GetPathParts(string path)
        {
            List<string> parts = new List<string>();

            bool inProp = true; // Starts with a $
            bool isEscaped = false;
            bool inArray = false;
            bool isQuoted = false;
            string part = "";

            for (int i = 0, len = path.Length; i < len; i++)
            {
                char c = path[i];

                if (isQuoted)
                {
                    if (isEscaped || c != '"')
                    {
                        part += c;
                    }

                    if (isEscaped)
                    {
                        isEscaped = false;
                    }
                    else
                    {
                        switch (c)
                        {
                            case '\\':
                                isEscaped = true;
                                break;

                            case '"':
                                parts.Add(StringUtils.UnescapeStringLiteral(part));
                                isQuoted = false;
                                inProp = false;
                                break;
                        }
                    }

                    continue;
                }

                if (inArray)
                {
                    if (c == ']')
                    {
                        parts.Add(part);
                        inArray = false;
                    }
                    else
                    {
                        part += c;
                    }

                    continue;
                }

                if (inProp)
                {
                    if (c == '.' || c == '[')
                    {
                        parts.Add(part);
                        inProp = false;
                    }
                    else
                    {
                        if (c == '"' && part.Length == 0)
                        {
                            isQuoted = true;
                            isEscaped = false;
                        }
                        else
                        {
                            part += c;
                        }

                        continue;
                    }
                }

                switch (c)
                {
                    case '.':
                        part = "";
                        inProp = true;
                        break;

                    case '[':
                        part = "";
                        inArray = true;
                        break;
                }
            }

            if (inProp)
            {
                parts.Add(part);
            }

            return parts;
        }

        #endregion
    }
}
