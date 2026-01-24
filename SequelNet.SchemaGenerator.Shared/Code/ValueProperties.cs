using System.Text;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

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
            object[] formatArgs = new object[] { "\r\n", dalCol.ActualType, dalCol.PropertyName, null };
            formatArgs[3] = (dalCol.VirtualProp ? "virtual " : "");

            if (!string.IsNullOrEmpty(dalCol.Comment))
            {
                stringBuilder.AppendFormat("/// <summary>{0}/// {1}{0}/// </summary>{0}", "\r\n",
                    dalCol.Comment.Replace("\r\n", "/// ").Replace("\r", "/// ").Replace("\n", "/// "));
            }

            if (context.ComponentModel)
            {
                if (dalCol.IsPrimaryKey || context.GetPrimaryKeyColumns().Contains(dalCol))
                {
                    stringBuilder.AppendFormat("[System.ComponentModel.DataAnnotationsKey]{0}", formatArgs);
                }

                if (dalCol.MaxLength > 0 &&
                    (dalCol.Type == DalColumnType.TString || dalCol.Type == DalColumnType.TFixedString /*|| dalCol.Type == DalColumnType.TBlob*/))
                {
                    stringBuilder.AppendFormat("[System.ComponentModel.DataAnnotationsMaxLength({1})]{0}", formatArgs, dalCol.MaxLength);
                }

                if (!dalCol.IsNullable)
                {
                    stringBuilder.AppendFormat("[System.ComponentModel.DataAnnotationsRequired]{0}", formatArgs);
                }
            }

            stringBuilder.AppendFormat("public {3}{1} {2}{0}{{{0}", formatArgs);
            stringBuilder.AppendFormat("get {{ return _{2}; }}{0}", formatArgs);
            if (context.AtomicUpdates && dalCol.Computed == null)
            {
                stringBuilder.AppendFormat("set{{ _{2} = value; MarkColumnMutated(Columns.{2}); }}{0}", formatArgs);
            }
            else
            {
                stringBuilder.AppendFormat("set{{ _{2} = value; }}{0}", formatArgs);
            }
            stringBuilder.AppendFormat("}}{0}{0}", formatArgs);
        }
    }
}