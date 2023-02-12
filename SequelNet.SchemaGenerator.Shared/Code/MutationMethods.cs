using System.Collections.Generic;
using System.Text;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
	{
        private static void WriteMutationMethods(StringBuilder stringBuilder, ScriptContext context)
        {
            var customMutatedColumns = context.Columns.FindAll(x => !string.IsNullOrEmpty(x.IsMutatedProperty));
            if (customMutatedColumns.Count > 0)
            {
                stringBuilder.AppendFormat("{0}#region Mutated{0}{0}", "\r\n");

                // MarkColumnMutated
                stringBuilder.AppendFormat("public override void MarkColumnMutated(string column){0}{{{0}", "\r\n");
                stringBuilder.AppendFormat("base.MarkColumnMutated(column);{0}{0}", "\r\n");
                foreach (var dalCol in customMutatedColumns)
                {
                    stringBuilder.AppendFormat("if (column == Columns.{1} && {2} != null) {2}.{3} = true;{0}", "\r\n", dalCol.PropertyName, dalCol.PropertyName, dalCol.IsMutatedProperty);
                }
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");

                // MarkColumnNotMutated
                stringBuilder.AppendFormat("public override void MarkColumnNotMutated(string column){0}{{{0}", "\r\n");
                stringBuilder.AppendFormat("base.MarkColumnNotMutated(column);{0}{0}", "\r\n");
                foreach (var dalCol in customMutatedColumns)
                {
                    stringBuilder.AppendFormat("if (column == Columns.{1} && {2} != null) {2}.{3} = false;{0}", "\r\n", dalCol.PropertyName, dalCol.PropertyName, dalCol.IsMutatedProperty);
                }
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");

                // MarkAllColumnsNotMutated
                stringBuilder.AppendFormat("public override void MarkAllColumnsNotMutated(){0}{{{0}", "\r\n");
                stringBuilder.AppendFormat("base.MarkAllColumnsNotMutated();{0}{0}", "\r\n");
                foreach (var dalCol in customMutatedColumns)
                {
                    stringBuilder.AppendFormat("if ({1} != null) {1}.{2} = false;{0}", "\r\n", dalCol.PropertyName, dalCol.IsMutatedProperty);
                }
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");

                // IsColumnMutated
                stringBuilder.AppendFormat("public override bool IsColumnMutated(string column){0}{{{0}", "\r\n");
                stringBuilder.AppendFormat("if (base.IsColumnMutated(column)) return true;{0}{0}", "\r\n");
                stringBuilder.AppendFormat("switch (column){0}{{{0}", "\r\n");
                foreach (var dalCol in customMutatedColumns)
                {
                    stringBuilder.AppendFormat("case Columns.{1}:{0}if ({2} != null && {2}.{3}) return true;{0}break;{0}", "\r\n", dalCol.PropertyName, dalCol.PropertyName, dalCol.IsMutatedProperty);
                }
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");
                stringBuilder.AppendFormat("return false;{0}", "\r\n");
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");

                // HasMutatedColumns
                stringBuilder.AppendFormat("public override bool HasMutatedColumns(){0}{{{0}", "\r\n");
                stringBuilder.AppendFormat("if (base.HasMutatedColumns()) return true;{0}", "\r\n");
                foreach (var dalCol in customMutatedColumns)
                {
                    stringBuilder.AppendFormat("if ({1} != null && {1}.{2}) return true;{0}", "\r\n", dalCol.PropertyName, dalCol.IsMutatedProperty);
                }
                stringBuilder.AppendFormat("return false;{0}", "\r\n");
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");

                // GetMutatedColumnNamesSet
                stringBuilder.AppendFormat("public override HashSet<string> GetMutatedColumnNamesSet(){0}{{{0}", "\r\n");
                stringBuilder.AppendFormat("  var set = new HashSet<string>(base.GetMutatedColumnNamesSet());{0}", "\r\n");
                stringBuilder.AppendFormat("  return base.GetMutatedColumnNamesSet();{0}", "\r\n");
                foreach (var dalCol in customMutatedColumns)
                {
                    stringBuilder.AppendFormat("if ({1} != null && {1}.{2}) set.Add(Columns.{1});{0}", "\r\n", dalCol.PropertyName, dalCol.IsMutatedProperty);
                }
                stringBuilder.AppendFormat("return set;{0}", "\r\n");
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");

                stringBuilder.AppendFormat("#endregion{0}", "\r\n");
            }
        }
	}
}