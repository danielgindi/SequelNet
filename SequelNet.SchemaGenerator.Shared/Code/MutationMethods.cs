using System.Collections.Generic;
using System.Text;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    private static void WriteMutationMethods(StringBuilder stringBuilder, ScriptContext context)
    {
        var customMutatedColumns = context.Columns.FindAll(x => !string.IsNullOrEmpty(x.IsMutatedProperty));
        if (customMutatedColumns.Count > 0)
        {
            AppendLine(stringBuilder);
            AppendLine(stringBuilder, "#region Mutated");
            AppendLine(stringBuilder);

            // MarkColumnMutated
            AppendLine(stringBuilder, "public override void MarkColumnMutated(string column)");
            AppendLine(stringBuilder, "{");
            AppendLine(stringBuilder, "base.MarkColumnMutated(column);");
            AppendLine(stringBuilder);
            foreach (var dalCol in customMutatedColumns)
            {
                AppendLine(stringBuilder, $"if (column == Columns.{dalCol.PropertyName} && {dalCol.PropertyName} != null) {dalCol.PropertyName}.{dalCol.IsMutatedProperty} = true;");
            }
            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder);

            // MarkColumnNotMutated
            AppendLine(stringBuilder, "public override void MarkColumnNotMutated(string column)");
            AppendLine(stringBuilder, "{");
            AppendLine(stringBuilder, "base.MarkColumnNotMutated(column);");
            AppendLine(stringBuilder);
            foreach (var dalCol in customMutatedColumns)
            {
                AppendLine(stringBuilder, $"if (column == Columns.{dalCol.PropertyName} && {dalCol.PropertyName} != null) {dalCol.PropertyName}.{dalCol.IsMutatedProperty} = false;");
            }
            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder);

            // MarkAllColumnsNotMutated
            AppendLine(stringBuilder, "public override void MarkAllColumnsNotMutated()");
            AppendLine(stringBuilder, "{");
            AppendLine(stringBuilder, "base.MarkAllColumnsNotMutated();");
            AppendLine(stringBuilder);
            foreach (var dalCol in customMutatedColumns)
            {
                AppendLine(stringBuilder, $"if ({dalCol.PropertyName} != null) {dalCol.PropertyName}.{dalCol.IsMutatedProperty} = false;");
            }
            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder);

            // IsColumnMutated
            AppendLine(stringBuilder, "public override bool IsColumnMutated(string column)");
            AppendLine(stringBuilder, "{");
            AppendLine(stringBuilder, "if (base.IsColumnMutated(column)) return true;");
            AppendLine(stringBuilder);
            AppendLine(stringBuilder, "switch (column)");
            AppendLine(stringBuilder, "{");
            foreach (var dalCol in customMutatedColumns)
            {
                AppendLine(stringBuilder, $"case Columns.{dalCol.PropertyName}:");
                AppendLine(stringBuilder, $"if ({dalCol.PropertyName} != null && {dalCol.PropertyName}.{dalCol.IsMutatedProperty}) return true;");
                AppendLine(stringBuilder, "break;");
            }
            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder);
            AppendLine(stringBuilder, "return false;");
            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder);

            // HasMutatedColumns
            AppendLine(stringBuilder, "public override bool HasMutatedColumns()");
            AppendLine(stringBuilder, "{");
            AppendLine(stringBuilder, "if (base.HasMutatedColumns()) return true;");
            foreach (var dalCol in customMutatedColumns)
            {
                AppendLine(stringBuilder, $"if ({dalCol.PropertyName} != null && {dalCol.PropertyName}.{dalCol.IsMutatedProperty}) return true;");
            }
            AppendLine(stringBuilder, "return false;");
            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder);

            // GetMutatedColumnNamesSet
            AppendLine(stringBuilder, "public override HashSet<string> GetMutatedColumnNamesSet()");
            AppendLine(stringBuilder, "{");
            AppendLine(stringBuilder, "  var set = new HashSet<string>(base.GetMutatedColumnNamesSet());");
            foreach (var dalCol in customMutatedColumns)
            {
                AppendLine(stringBuilder, $"if ({dalCol.PropertyName} != null && {dalCol.PropertyName}.{dalCol.IsMutatedProperty}) set.Add(Columns.{dalCol.PropertyName});");
            }
            AppendLine(stringBuilder, "return set;");
            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder);

            AppendLine(stringBuilder, "#endregion");
        }
    }
}