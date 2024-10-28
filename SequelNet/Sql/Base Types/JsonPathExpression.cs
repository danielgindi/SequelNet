using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace SequelNet;

public class JsonPathExpression
{
    private bool _IsPathCompiled = false;
    private string? _Path = null;
    private List<Part>? _Parts = null;

    #region Constructors

    public JsonPathExpression(string path)
    {
        this._Path = path;
        this._IsPathCompiled = true;
    }

    public JsonPathExpression(params Part[] parts)
    {
        this._Parts = parts.ToList();
        this._IsPathCompiled = false;
    }

    public JsonPathExpression(IEnumerable<Part> parts)
    {
        this._Parts = parts.ToList();
        this._IsPathCompiled = false;
    }

    #endregion

    #region Accessors

    public bool IsEmpty()
    {
        if (_IsPathCompiled)
        {
            return string.IsNullOrEmpty(_Path) || _Path == "$";
        }
        else
        {
            return _Parts == null || _Parts.Count == 0 ||
                (_Parts.Count == 1 &&
                _Parts[0].Value.Type == ValueObjectType.Value &&
                _Parts[0].Value.Value as string == "$");
        }
    }

    public ValueWrapper GetPath()
    {
        if (this._IsPathCompiled)
        {
            return ValueWrapper.From(this._Path!);
        }

        if (_Parts != null && _Parts.Any(x => x.Value.Type == ValueObjectType.Value))
        {
            var sb = new StringBuilder("$");

            var first = true;

            foreach (var part in _Parts!)
            {
                if (first)
                {
                    first = false;
                    if (part.Value.Type == ValueObjectType.Value && part.Value.Value as string == "$")
                        continue;
                }

                if (part.Indexed)
                    sb.Append("[");
                else sb.Append(".");

                var v = part.Value.Value!.ToString()!;

                // test if the value is a simple property accessor name
                if (v.Length == 0 ||
                    (!part.Indexed && (
                        char.IsDigit(v[0]) ||
                        !v.All(x => char.IsLetterOrDigit(x) || x == '_')
                    )) ||
                    (part.Indexed && !v.All(char.IsDigit)))
                {
                    sb.Append('\"');
                    sb.Append(v.Replace("\\", "\\\\").Replace("\"", "\\\""));
                    sb.Append('\"');
                }
                else
                {
                    sb.Append(v);
                }

                if (part.Indexed)
                    sb.Append("]");
            }

            _Path = sb.ToString();

            return ValueWrapper.From(this._Path);
        }
        else
        {
            var parts = new List<ValueWrapper>
            {
                ValueWrapper.From("$")
            };

            var first = true;

            if (_Parts != null)
            {
                foreach (var part in _Parts)
                {
                    if (first)
                    {
                        first = false;
                        if (part.Value.Type == ValueObjectType.Value && part.Value.Value as string == "$")
                            continue;
                    }

                    if (part.Indexed)
                        parts.Add(ValueWrapper.From("["));
                    else parts.Add(ValueWrapper.From("."));

                    if (part.Value.Type == ValueObjectType.Value)
                    {
                        var v = part.Value.Value!.ToString()!;

                        // test if the value is a simple property accessor name
                        if (v.Length == 0 ||
                            (!part.Indexed && (
                                char.IsDigit(v[0]) ||
                                !v.All(x => char.IsLetterOrDigit(x) || x == '_')
                            )) ||
                            (part.Indexed && !v.All(char.IsDigit)))
                        {
                            parts.Add(ValueWrapper.From(
                                PhraseHelper.Replace(
                                    ValueWrapper.From(
                                        PhraseHelper.Replace(ValueWrapper.From(v), ValueWrapper.From("\\"), ValueWrapper.From("\\\\"))
                                    ),
                                    ValueWrapper.From("\""), ValueWrapper.From("\\\"")
                                )
                            ));
                        }
                        else
                        {
                            parts.Add(ValueWrapper.From(v));
                        }
                    }
                    else
                    {
                        if (part.Indexed)
                        {
                            parts.Add(part.Value);
                        }
                        else
                        {
                            parts.Add(ValueWrapper.From(
                                PhraseHelper.Replace(
                                    ValueWrapper.From(
                                        PhraseHelper.Replace(part.Value, ValueWrapper.From("\\"), ValueWrapper.From("\\\\"))
                                    ),
                                    ValueWrapper.From("\""), ValueWrapper.From("\\\"")
                                )
                            ));
                        }
                    }

                    if (part.Indexed)
                        parts.Add(ValueWrapper.From("]"));
                }
            }

            return ValueWrapper.From(PhraseHelper.Concat(parts.ToArray()));
        }
    }

    public List<Part> GetParts()
    {
        if (this._Parts != null)
        {
            return _Parts.ToList();
        }
        else
        {
            _Parts = GetPathParts(this._Path ?? "$");
            return _Parts;
        }
    }

    public void SetPath(string path)
    {
        this._Path = path;
        this._Parts = null;
        this._IsPathCompiled = true;
    }

    public void SetParts(IEnumerable<Part> parts)
    {
        this._Parts = parts.ToList();
        this._Path = null;
        this._IsPathCompiled = false;
    }

    #endregion

    public struct Part
    {
        public bool Indexed;
        public ValueWrapper Value;

        public static Part Root()
        {
            return new Part { Value = ValueWrapper.From("$") };
        }

        public static Part Property(string name)
        {
            return new Part { Value = ValueWrapper.From(name), Indexed = false };
        }

        public static Part IndexAt(int index)
        {
            return new Part { Value = ValueWrapper.From(index), Indexed = true };
        }
    }

    #region Utility

    public static List<Part> GetPathParts(string path)
    {
        var parts = new List<Part>();

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
                            part = StringUtils.UnescapeStringLiteral(part);
                            if (inArray && part.All(char.IsDigit))
                            {
                                parts.Add(new Part { Value = ValueWrapper.From(int.Parse(part)), Indexed = true });
                            }
                            else
                            {
                                parts.Add(new Part { Value = ValueWrapper.From(part), Indexed = inArray });
                            }
                            isQuoted = false;
                            inProp = false;

                            if (inArray)
                            {
                                inArray = false;

                                do
                                {
                                    i++;
                                } while (i < len && path[i] != ']');
                            }
                            break;
                    }
                }

                continue;
            }

            if (inArray)
            {
                if (c == ']')
                {
                    if (inArray && part.All(char.IsDigit))
                    {
                        parts.Add(new Part { Value = ValueWrapper.From(int.Parse(part)), Indexed = true });
                    }
                    else
                    {
                        parts.Add(new Part { Value = ValueWrapper.From(part), Indexed = true });
                    }
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
                    parts.Add(new Part { Value = ValueWrapper.From(part), Indexed = false });
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
            parts.Add(new Part { Value = ValueWrapper.From(part), Indexed = false });
        }

        return parts;
    }

    #endregion
}
