using System.Text;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
	{
        private static void WriteValueStoredVariables(StringBuilder stringBuilder, ScriptContext context)
        {
            foreach (DalColumn dalColumn in context.Columns)
            {
                if (!dalColumn.NoProperty)
                {
                    stringBuilder.Append("internal ");
                }
                string defaultValue = null;
                defaultValue = dalColumn.DefaultValue;
                if (string.IsNullOrEmpty(defaultValue) || defaultValue == "null")
                {
                    if (!string.IsNullOrEmpty(dalColumn.EnumTypeName))
                    {
                        defaultValue = null;
                    }
                    else if (dalColumn.Type == DalColumnType.TBool)
                    {
                        defaultValue = "false";
                    }
                    else if (dalColumn.Type == DalColumnType.TGuid)
                    {
                        defaultValue = "Guid.Empty";
                    }
                    else if (dalColumn.Type == DalColumnType.TDateTime)
                    {
                        defaultValue = "DateTime.UtcNow";
                    }
                    else if (dalColumn.Type == DalColumnType.TDateTimeUtc)
                    {
                        defaultValue = "DateTime.UtcNow";
                    }
                    else if (dalColumn.Type == DalColumnType.TDateTimeLocal)
                    {
                        defaultValue = "DateTime.Now";
                    }
                    else if (dalColumn.Type == DalColumnType.TDate)
                    {
                        defaultValue = "DateTime.UtcNow";
                    }
                    else if (dalColumn.Type == DalColumnType.TTime)
                    {
                        defaultValue = "TimeSpan.Zero";
                    }
                    else if (dalColumn.Type == DalColumnType.TInt)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TInt8)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TInt16)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TInt32)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TInt64)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TUInt8)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TUInt16)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TUInt32)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TUInt64)
                    {
                        defaultValue = "0";
                    }
                    else if (dalColumn.Type == DalColumnType.TString || dalColumn.Type == DalColumnType.TText || dalColumn.Type == DalColumnType.TLongText || dalColumn.Type == DalColumnType.TMediumText || dalColumn.Type == DalColumnType.TFixedString)
                    {
                        defaultValue = "string.Empty";
                    }
                    else if (dalColumn.Type == DalColumnType.TDecimal || dalColumn.Type == DalColumnType.TMoney)
                    {
                        defaultValue = "0m";
                    }
                    else if (dalColumn.Type == DalColumnType.TDouble)
                    {
                        defaultValue = "0d";
                    }
                    else if (dalColumn.Type == DalColumnType.TFloat)
                    {
                        defaultValue = "0f";
                    }
                    else if (dalColumn.Type == DalColumnType.TJson || dalColumn.Type == DalColumnType.TJsonBinary)
                    {
                        defaultValue = "null";
                    }
                }
                if (dalColumn.ActualDefaultValue.Length > 0)
                {
                    defaultValue = dalColumn.ActualDefaultValue;
                }
                if (dalColumn.NoProperty)
                {
                    continue;
                }
                stringBuilder.Append(dalColumn.ActualType);
                stringBuilder.AppendFormat(" _{0}", dalColumn.PropertyName);
                if ((dalColumn.DefaultValue == "null" || dalColumn.ActualDefaultValue.Length > 0 & (dalColumn.ActualDefaultValue == "null")) && dalColumn.IsNullable)
                {
                    stringBuilder.AppendFormat(" = {1};{0}", "\r\n",
                        (dalColumn.ActualDefaultValue.Length > 0 ? dalColumn.ActualDefaultValue : dalColumn.DefaultValue));
                }
                else if (defaultValue != null)
                {
                    stringBuilder.AppendFormat(" = {1};{0}{0}", "\r\n", defaultValue);
                }
                else
                {
                    stringBuilder.AppendFormat(";{0}{0}", "\r\n");
                }
            }
        }
	}
}