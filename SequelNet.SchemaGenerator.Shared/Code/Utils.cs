using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
    {
        private static string CsharpString(string value)
        {
            return (@"""" + value
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\r", "\\\r")
                        .Replace("\n", "\\\n")
                        + @"""");
        }

        private static string ProcessComputedColumn(string computed)
        {
            return "ValueWrapper.From(" + computed + ")";
        }

        public static string StripColumnName(string columnName)
        {
            columnName = columnName.Trim();
            while (columnName.Length > 0 && !Regex.IsMatch(columnName, @"^[a-zA-Z_]")) columnName = columnName.Remove(0, 1);
            columnName = Regex.Replace(columnName, @"[^a-zA-Z_0-9]", @"");
            return columnName;
        }

        public static string FirstLetterLowerCase(string name)
        {
            if (name.Length == 0) return name;
            return name.Substring(0, 1).ToLowerInvariant() + name.Remove(0, 1);
        }

        public static string ValueToDb(string varName, DalColumn dalCol)
        {
            if (string.IsNullOrEmpty(dalCol.ToDb))
            {
                return varName;
            }
            else
            {
                return string.Format(dalCol.ToDb, varName);
            }
        }

        public static string SnakeCase(string value)
        {
            var values = new List<string>();
            var matches = Regex.Matches(value, @"[^A-Z._-]+|[A-Z\d]+(?![^._-])|[A-Z\d]+(?=[A-Z])|[A-Z][^A-Z._-]*", RegexOptions.ECMAScript);
            foreach (Match match in matches)
                values.Add(match.Value);

            return string.Join("_", values.Select(x => x.ToLowerInvariant()));
        }
    }
}