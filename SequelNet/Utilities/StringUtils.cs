using System;

namespace SequelNet;

public static class StringUtils
{
    internal static string UnescapeStringLiteral(string str)
    {
        string unescaped = "";

        bool escaped = false;
        for (int i = 0, len = str.Length; i < len; i++)
        {
            char c = str[i];
            if (escaped)
            {
                switch (c)
                {
                    case '0': unescaped += '\0'; break;
                    case 'a': unescaped += '\a'; break;
                    case 'b': unescaped += '\b'; break;
                    case 'f': unescaped += '\f'; break;
                    case 'n': unescaped += '\n'; break;
                    case 'r': unescaped += '\r'; break;
                    case 't': unescaped += '\t'; break;
                    case 'v': unescaped += '\v'; break;
                    default: unescaped += c; break;
                }
                escaped = false;
                continue;
            }

            if (c == '\\')
            {
                escaped = true;
                continue;
            }
            
            unescaped += c;
        }

        return unescaped;
    }

    private const string HexChars = "0123456789ABCDEF";

    public static string ToHex(ReadOnlySpan<byte> bytes)
    {
        char[] chars = new char[bytes.Length * 2];

        int i = 0;
        foreach (byte b in bytes)
        {
            chars[i++] = HexChars[b >> 4];
            chars[i++] = HexChars[b & 0xF];
        }

        return new string(chars);
    }
}
