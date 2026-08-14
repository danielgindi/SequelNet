using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SequelNet.SchemaGenerator.VSIX
{
    internal sealed class MacroLocation
    {
        public MacroLocation(int start, int end, string script)
        {
            Start = start;
            End = end;
            Script = script;
            RecordName = MacroLocator.GetRecordName(script);
        }

        public int Start { get; }
        public int End { get; }
        public string Script { get; }
        public string RecordName { get; }
    }

    internal static class MacroLocator
    {
        private static readonly Regex BlockCommentRegex = new Regex(@"/\*[\s\S]*?\*/", RegexOptions.Compiled);

        public static string Normalize(string text)
        {
            var trimmed = text.Trim();
            if (trimmed.StartsWith("/*", StringComparison.Ordinal) && trimmed.EndsWith("*/", StringComparison.Ordinal))
                return NormalizeBlockComment(trimmed.Substring(2, trimmed.Length - 4));

            var lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            var hasContent = false;
            var allLineComments = true;
            foreach (var line in lines)
            {
                if (line.Trim().Length == 0)
                    continue;

                hasContent = true;
                if (!line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                {
                    allLineComments = false;
                    break;
                }
            }

            if (!hasContent || !allLineComments)
                return trimmed;

            var result = new StringBuilder();
            foreach (var line in lines)
            {
                var uncommented = line.TrimStart();
                if (uncommented.StartsWith("//", StringComparison.Ordinal))
                {
                    uncommented = uncommented.Substring(2);
                    if (uncommented.StartsWith("/", StringComparison.Ordinal))
                        uncommented = uncommented.Substring(1);
                    if (uncommented.StartsWith(" ", StringComparison.Ordinal))
                        uncommented = uncommented.Substring(1);
                }

                result.AppendLine(uncommented);
            }
            return result.ToString().Trim();
        }

        public static string GetRecordName(string script)
        {
            foreach (var line in script.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n'))
            {
                var trimmed = line.Trim();
                if (trimmed.Length > 0)
                    return trimmed;
            }
            return string.Empty;
        }
        public static MacroLocation Find(string documentText, int cursorOffset)
        {
            var candidates = new List<MacroLocation>();
            foreach (Match match in BlockCommentRegex.Matches(documentText))
                AddIfMacro(candidates, match.Index, match.Index + match.Length, NormalizeBlockComment(match.Value.Substring(2, match.Value.Length - 4)));

            var groupStart = -1;
            var groupEnd = -1;
            var offset = 0;
            while (offset < documentText.Length)
            {
                var newlineLength = 0;
                var lineEnd = documentText.IndexOfAny(new[] { '\r', '\n' }, offset);
                if (lineEnd < 0)
                    lineEnd = documentText.Length;
                else if (documentText[lineEnd] == '\r' && lineEnd + 1 < documentText.Length && documentText[lineEnd + 1] == '\n')
                    newlineLength = 2;
                else
                    newlineLength = 1;

                var line = documentText.Substring(offset, lineEnd - offset);
                if (IsGeneratedRegionMarker(line) || IsDocumentationComment(line))
                {
                    if (groupStart >= 0)
                    {
                        AddIfMacro(candidates, groupStart, groupEnd, Normalize(documentText.Substring(groupStart, groupEnd - groupStart)));
                        groupStart = -1;
                    }
                }
                else if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                {
                    if (groupStart < 0)
                        groupStart = offset;
                    groupEnd = lineEnd;
                }
                else if (groupStart >= 0)
                {
                    AddIfMacro(candidates, groupStart, groupEnd, Normalize(documentText.Substring(groupStart, groupEnd - groupStart)));
                    groupStart = -1;
                }
                offset = lineEnd + newlineLength;
            }
            if (groupStart >= 0)
                AddIfMacro(candidates, groupStart, groupEnd, Normalize(documentText.Substring(groupStart, groupEnd - groupStart)));

            MacroLocation atCursor = null;
            foreach (var candidate in candidates)
            {
                if (cursorOffset < candidate.Start || cursorOffset > candidate.End)
                    continue;
                if (atCursor != null)
                    return null;
                atCursor = candidate;
            }
            return atCursor ?? (candidates.Count == 1 ? candidates[0] : null);
        }

        private static void AddIfMacro(ICollection<MacroLocation> candidates, int start, int end, string script)
        {
            var lines = script.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            var meaningfulLines = new List<string>();
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.Length > 0)
                    meaningfulLines.Add(trimmed);
            }

            if (meaningfulLines.Count < 3)
                return;

            for (var i = 2; i < meaningfulLines.Count; i++)
            {
                var line = meaningfulLines[i];
                if (!line.StartsWith("@", StringComparison.Ordinal) && line.IndexOf(':') > 0)
                {
                    candidates.Add(new MacroLocation(start, end, script));
                    return;
                }
            }
        }

        private static bool IsDocumentationComment(string line)
        {
            return line.TrimStart().StartsWith("///", StringComparison.Ordinal);
        }

        private static bool IsGeneratedRegionMarker(string line)
        {
            var trimmed = line.Trim();
            return GeneratedRegion.IsStartMarker(trimmed) ||
                trimmed.Equals(GeneratedRegion.EndMarker, StringComparison.Ordinal);
        }
        private static string NormalizeBlockComment(string content)
        {
            var result = new StringBuilder();
            var lines = content.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.TrimStart();
                if (trimmed.StartsWith("*", StringComparison.Ordinal))
                {
                    trimmed = trimmed.Substring(1);
                    if (trimmed.StartsWith(" ", StringComparison.Ordinal))
                        trimmed = trimmed.Substring(1);
                }
                result.AppendLine(trimmed);
            }
            return result.ToString().Trim();
        }
    }
}