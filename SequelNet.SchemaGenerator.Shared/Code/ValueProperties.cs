using System.Text;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    private static void WriteValueProperties(StringBuilder stringBuilder, ScriptContext context)
    {
        foreach (DalColumn dalCol in context.Columns)
        {
            if (dalCol.NoProperty)
            {
                continue;
            }
            var virtualPrefix = dalCol.VirtualProp ? "virtual " : "";

            if (!string.IsNullOrEmpty(dalCol.Comment))
            {
                AppendLine(stringBuilder, "/// <summary>");
                AppendLine(stringBuilder, "/// " + dalCol.Comment!.Replace("\r\n", NewLine + "/// ").Replace("\r", NewLine + "/// ").Replace("\n", NewLine + "/// "));
                AppendLine(stringBuilder, "/// </summary>");
            }

            if (context.ComponentModel)
            {
                if (dalCol.IsPrimaryKey || context.GetPrimaryKeyColumns().Contains(dalCol))
                {
                    AppendLine(stringBuilder, "[System.ComponentModel.DataAnnotations.Key]");
                }

                if (dalCol.MaxLength > 0 &&
                    (dalCol.Type == DalColumnType.TString || dalCol.Type == DalColumnType.TFixedString /*|| dalCol.Type == DalColumnType.TBlob*/))
                {
                    AppendLine(stringBuilder, $"[System.ComponentModel.DataAnnotations.MaxLength({dalCol.MaxLength})]");
                }

                if (!dalCol.IsNullable)
                {
                    AppendLine(stringBuilder, "[System.ComponentModel.DataAnnotations.Required]");
                }
            }

            var (actualType, effectiveType, isReferenceType) = GetClrTypeName(dalCol, context);

            AppendLine(stringBuilder, $"public {virtualPrefix}{effectiveType} {dalCol.PropertyName}");
            AppendLine(stringBuilder, "{");
            AppendLine(stringBuilder, $"get {{ return _{dalCol.PropertyName}; }}");
            if (context.AtomicUpdates && dalCol.Computed == null)
            {
                AppendLine(stringBuilder, $"set{{ _{dalCol.PropertyName} = value; MarkColumnMutated(Columns.{dalCol.PropertyName}); }}");
            }
            else
            {
                AppendLine(stringBuilder, $"set{{ _{dalCol.PropertyName} = value; }}");
            }
            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder);
        }
    }
}