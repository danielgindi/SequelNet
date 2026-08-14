using System;
using System.Collections.Generic;

namespace SequelNet.SchemaGenerator;

/// <summary>
/// Defines the editable boundary around code generated from a SequelNet macro.
/// The macro remains the source of truth; integrations only replace text inside
/// this region.
/// </summary>
public static class GeneratedRegion
{
    public const string StartMarkerPrefix = "// <sequelnet-generated";
    public const string StartMarker = StartMarkerPrefix + ">";
    public const string EndMarker = "// </sequelnet-generated>";

    public static string BuildStartMarker(string? recordName)
    {
        return string.IsNullOrWhiteSpace(recordName)
            ? StartMarker
            : StartMarkerPrefix + " record=\"" + recordName!.Trim() + "\">";
    }

    public static bool IsStartMarker(string value)
    {
        if (value is null || !value.StartsWith(StartMarkerPrefix, StringComparison.Ordinal))
            return false;

        var suffix = value.Substring(StartMarkerPrefix.Length);
        return suffix == ">" || (suffix.Length > 1 && char.IsWhiteSpace(suffix[0]) && suffix.EndsWith(">", StringComparison.Ordinal));
    }
    public static GeneratedRegionChange CreateOrUpdateAfterMacro(string documentText, int macroEndOffset, string generatedCode, string? recordName = null)
    {
        if (documentText is null)
            throw new ArgumentNullException(nameof(documentText));
        if (generatedCode is null)
            throw new ArgumentNullException(nameof(generatedCode));
        if (macroEndOffset < 0 || macroEndOffset > documentText.Length)
            throw new ArgumentOutOfRangeException(nameof(macroEndOffset));

        var newLine = documentText.IndexOf("\r\n", StringComparison.Ordinal) >= 0 ? "\r\n" : "\n";
        var namedStartMarker = string.IsNullOrWhiteSpace(recordName) ? null : BuildStartMarker(recordName);
        var regionStart = namedStartMarker is null ? -1 : FindUniqueNamedRegionStart(documentText, namedStartMarker);
        if (regionStart < 0)
            regionStart = FindAdjacentRegionStart(documentText, macroEndOffset);

        if (regionStart < 0)
            return new GeneratedRegionChange(macroEndOffset, 0, newLine + Render(generatedCode, newLine, BuildStartMarker(recordName)));

        var regionEnd = FindMatchingRegionEnd(documentText, regionStart);
        return new GeneratedRegionChange(regionStart, regionEnd - regionStart, Render(generatedCode, newLine, BuildStartMarker(recordName)));
    }

    private static int FindUniqueNamedRegionStart(string documentText, string namedStartMarker)
    {
        var firstMatch = -1;
        var searchStart = 0;
        while (true)
        {
            var candidate = FindNextStartMarker(documentText, searchStart);
            if (candidate < 0)
                return firstMatch;

            var lineEnd = documentText.IndexOfAny(new[] { '\r', '\n' }, candidate);
            if (lineEnd < 0)
                lineEnd = documentText.Length;
            if (documentText.Substring(candidate, lineEnd - candidate).TrimEnd().Equals(namedStartMarker, StringComparison.Ordinal))
            {
                if (firstMatch >= 0)
                    throw new InvalidOperationException("Multiple SequelNet generated regions match this record name.");
                firstMatch = candidate;
            }

            searchStart = candidate + StartMarkerPrefix.Length;
        }
    }
    private static int FindAdjacentRegionStart(string documentText, int macroEndOffset)
    {
        var position = macroEndOffset;
        while (true)
        {
            while (position < documentText.Length && char.IsWhiteSpace(documentText[position]))
                position++;

            if (FindNextStartMarker(documentText, position) == position)
                return position;

            if (TrySkipDocumentationComment(documentText, ref position) ||
                TrySkipNonMacroBlockComment(documentText, ref position))
                continue;

            return -1;
        }
    }

    private static int FindNextStartMarker(string documentText, int searchStart)
    {
        var position = searchStart;
        while (true)
        {
            position = documentText.IndexOf(StartMarkerPrefix, position, StringComparison.Ordinal);
            if (position < 0)
                return -1;

            var lineEnd = documentText.IndexOfAny(new[] { '\r', '\n' }, position);
            if (lineEnd < 0)
                lineEnd = documentText.Length;
            if (IsAtLineStart(documentText, position) &&
                IsStartMarker(documentText.Substring(position, lineEnd - position).TrimEnd()))
                return position;

            position += StartMarkerPrefix.Length;
        }
    }

    private static bool IsAtLineStart(string documentText, int position)
    {
        for (var index = position - 1; index >= 0; index--)
        {
            var character = documentText[index];
            if (character == '\r' || character == '\n')
                return true;
            if (!char.IsWhiteSpace(character))
                return false;
        }
        return true;
    }
    private static bool TrySkipDocumentationComment(string documentText, ref int position)
    {
        if (documentText.IndexOf("///", position, StringComparison.Ordinal) != position)
            return false;

        var lineEnd = documentText.IndexOfAny(new[] { '\r', '\n' }, position);
        position = lineEnd < 0 ? documentText.Length : lineEnd;
        return true;
    }

    private static bool TrySkipNonMacroBlockComment(string documentText, ref int position)
    {
        if (documentText.IndexOf("/*", position, StringComparison.Ordinal) != position)
            return false;

        var commentEnd = documentText.IndexOf("*/", position + 2, StringComparison.Ordinal);
        if (commentEnd < 0 || IsMacroShapedComment(documentText.Substring(position + 2, commentEnd - position - 2)))
            return false;

        position = commentEnd + 2;
        return true;
    }

    private static bool IsMacroShapedComment(string content)
    {
        var meaningfulLines = new List<string>();
        foreach (var line in content.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n'))
        {
            var trimmed = line.TrimStart();
            if (trimmed.StartsWith("*", StringComparison.Ordinal))
                trimmed = trimmed.Substring(1).TrimStart();
            trimmed = trimmed.Trim();
            if (trimmed.Length > 0)
                meaningfulLines.Add(trimmed);
        }

        if (meaningfulLines.Count < 3)
            return false;

        for (var i = 2; i < meaningfulLines.Count; i++)
        {
            var line = meaningfulLines[i];
            if (!line.StartsWith("@", StringComparison.Ordinal) && line.IndexOf(':') > 0)
                return true;
        }
        return false;
    }

    private static int FindMatchingRegionEnd(string documentText, int regionStart)
    {
        var depth = 1;
        var searchStart = regionStart + StartMarkerPrefix.Length;

        while (true)
        {
            var nextStart = FindNextStartMarker(documentText, searchStart);
            var nextEnd = documentText.IndexOf(EndMarker, searchStart, StringComparison.Ordinal);
            if (nextEnd < 0)
                throw new InvalidOperationException("The SequelNet generated region is missing its end marker.");

            if (nextStart >= 0 && nextStart < nextEnd)
            {
                depth++;
                searchStart = nextStart + StartMarkerPrefix.Length;
                continue;
            }

            depth--;
            searchStart = nextEnd + EndMarker.Length;
            if (depth == 0)
                return searchStart;
        }
    }

    private static string Render(string generatedCode, string newLine, string startMarker)
    {
        var normalizedCode = generatedCode
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Replace("\n", newLine)
            .TrimEnd('\r', '\n');

        return startMarker + newLine + normalizedCode + newLine + EndMarker;
    }
}

public readonly struct GeneratedRegionChange
{
    public GeneratedRegionChange(int start, int length, string text)
    {
        Start = start;
        Length = length;
        Text = text;
    }

    public int Start { get; }
    public int Length { get; }
    public string Text { get; }
}