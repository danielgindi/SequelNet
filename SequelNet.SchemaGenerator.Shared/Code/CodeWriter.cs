using System;
using System.Text;

namespace SequelNet.SchemaGenerator;

internal sealed class CodeWriter
{
    private readonly StringBuilder _sb;
    private int _indentLevel;

    public CodeWriter(StringBuilder sb)
    {
        _sb = sb ?? throw new ArgumentNullException(nameof(sb));
    }

    public void Indent() => _indentLevel++;

    public void Unindent()
    {
        if (_indentLevel > 0)
            _indentLevel--;
    }

    public void AppendLine(string? text = null)
    {
        if (text is null)
        {
            _sb.AppendLine();
            return;
        }

        if (text.Length > 0)
        {
            _sb.Append(' ', _indentLevel * 4);
        }

        _sb.AppendLine(text);
    }

    public void Append(string text) => _sb.Append(text);

    public override string ToString() => _sb.ToString();
}