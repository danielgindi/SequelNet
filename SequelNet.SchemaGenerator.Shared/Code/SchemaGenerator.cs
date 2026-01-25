using System;
using System.Collections.Generic;
using System.Text;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    public GeneratorCore()
    {
    }

    public static string GenerateDalClass(string script, Action<string> onWarning)
    {
        var result = GenerateDalClass(script);

        foreach (var warning in result.Warnings)
        {
            onWarning(warning);
        }

        return result.Code;
    }

    public static GenerateDalClassResult GenerateDalClass(string script)
    {
        var context = Parse(script);

        var warnings = Validate(context);

        Normalize(context);

        var code = Render(context);

        return new GenerateDalClassResult(code, warnings, context);
    }

    private static ScriptContext Parse(string script)
    {
        var context = new ScriptContext();

        // Avoid repeated allocations and keep trimming rules explicit.
        var scriptLines = script
            .Trim(new[] { ' ', '*', '/', '\t', '\r', '\n' })
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        ParseScript(context, scriptLines);

        return context;
    }

    private static List<string> Validate(ScriptContext context)
    {
        // Column name matching is case-sensitive (Ordinal).
        var knownColumnNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var c in context.Columns)
        {
            if (!string.IsNullOrEmpty(c.Name))
                knownColumnNames.Add(c.Name!);

            if (!string.IsNullOrEmpty(c.PropertyName))
                knownColumnNames.Add(c.PropertyName!);
        }

        var warnings = new List<string>();

        foreach (var dalIx in context.Indices)
        {
            foreach (var column in dalIx.Columns)
            {
                if (column.Literal)
                    continue;

                if (!string.IsNullOrEmpty(column.Name) && !knownColumnNames.Contains(column.Name!))
                {
                    warnings.Add($"Column {column.Name} not found in index {dalIx.IndexName ?? ""}");
                }
            }
        }

        return warnings;
    }

    private static void Normalize(ScriptContext context)
    {
        if (!context.SnakeColumnNames)
            return;

        foreach (var column in context.Columns)
        {
            if (column.HasCustomName)
                continue;

            if (!string.IsNullOrEmpty(column.Name))
                column.Name = SnakeCase(column.Name!);
        }
    }

    private static string Render(ScriptContext context)
    {
        // Start building the output classes
        var stringBuilder = new StringBuilder();

        if (context.ExportCollection)
        {
            WriteCollection(stringBuilder, context);
        }

        if (context.ExportRecord)
        {
            WriteRecord(stringBuilder, context);
        }

        return stringBuilder.ToString();
    }

    public sealed class GenerateDalClassResult
    {
        public GenerateDalClassResult(string code, IReadOnlyList<string> warnings, ScriptContext context)
        {
            Code = code;
            Warnings = warnings;
            Context = context;
        }

        public string Code { get; }

        public IReadOnlyList<string> Warnings { get; }

        public ScriptContext Context { get; }
    }

    private static void WriteCollection(StringBuilder stringBuilder, ScriptContext context)
    {
        var w = new CodeWriter(stringBuilder);

        w.AppendLine($"public partial class {context.ClassName}Collection : AbstractRecordList<{context.ClassName}, {context.ClassName}Collection>");
        w.AppendLine("{");
        w.AppendLine("}");
        w.AppendLine();

        foreach (DalEnum dalEn in context.Enums)
        {
            w.AppendLine($"public enum {dalEn.Name}");
            w.AppendLine("{");
            w.Indent();

            foreach (var item in dalEn.Items!)
            {
                w.AppendLine(item + ",");
            }

            w.Unindent();
            w.AppendLine("}");
            w.AppendLine();
        }
    }

    private static void WriteRecord(StringBuilder stringBuilder, ScriptContext context)
    {
        var w = new CodeWriter(stringBuilder);

        var nullabilitySign = context.NullableEnabled ? "?" : "";

        w.AppendLine($"public partial class {context.ClassName} : AbstractRecord<{context.ClassName}>");
        w.AppendLine("{");

        if (context.AtomicUpdates)
        {
            w.AppendLine("#region Static Constructor");
            w.AppendLine();
            w.AppendLine($"static {context.ClassName}()");
            w.AppendLine("{");
            w.Indent();
            w.AppendLine("AtomicUpdates = true;");
            w.Unindent();
            w.AppendLine("}");
            w.AppendLine();
            w.AppendLine("#endregion");
            w.AppendLine();
        }

        w.AppendLine("#region Table Schema");
        w.AppendLine();
        WriteSchema(stringBuilder, context);
        w.AppendLine();
        w.AppendLine("#endregion");
        w.AppendLine();

        w.AppendLine("#region Private Members");
        w.AppendLine();
        WriteValueStoredVariables(stringBuilder, context);
        w.AppendLine();
        w.AppendLine("#endregion");
        w.AppendLine();

        w.AppendLine("#region Properties");
        w.AppendLine();
        WriteValueProperties(stringBuilder, context);
        w.AppendLine();
        w.AppendLine("#endregion");
        w.AppendLine();

        w.AppendLine("#region AbstractRecord members");
        w.AppendLine();

        w.AppendLine($"public override object{nullabilitySign} GetPrimaryKeyValue()");
        w.AppendLine("{");
        w.Indent();
        w.AppendLine($"return {(string.IsNullOrEmpty(context.SingleColumnPrimaryKeyName) ? "null" : context.SingleColumnPrimaryKeyName)};");
        w.Unindent();
        w.AppendLine("}");
        w.AppendLine();

        WriteSetPrimaryKeyValueMethod(stringBuilder, context);
        w.AppendLine();
        WriteGetInsertQuery(stringBuilder, context);
        w.AppendLine();
        WriteGetUpdateQuery(stringBuilder, context);

        if (WriteUpdateMethod(stringBuilder, context))
            w.AppendLine();

        if (WriteUpdateAsyncMethod(stringBuilder, context))
            w.AppendLine();

        w.AppendLine();

        if (WriteInsertMethod(stringBuilder, context))
            w.AppendLine();

        if (WriteInsertAsyncMethod(stringBuilder, context))
            w.AppendLine();

        WriteReadMethod(stringBuilder, context);
        w.AppendLine();
        w.AppendLine("#endregion");

        WriteMutationMethods(stringBuilder, context);

        w.AppendLine();
        w.AppendLine("#region Helpers");
        w.AppendLine();
        WriteFetchMethods(stringBuilder, context);
        w.AppendLine();
        w.AppendLine("#endregion");

        w.AppendLine("}");
    }
}