using System;
using System.Text;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    private static void WriteReadMethod(StringBuilder stringBuilder, ScriptContext context)
    {
        // Read() method
        AppendLine(stringBuilder, "public override void Read(DataReader reader)");
        AppendLine(stringBuilder, "{");
        foreach (DalColumn dalCol in context.Columns)
        {
            if (dalCol.NoRead) continue;

            string fromDb = "{0}";
            string fromReader = "reader[Columns.{0}]";

            if (dalCol.Type == DalColumnType.TBool ||
                dalCol.Type == DalColumnType.TGuid ||
                dalCol.Type == DalColumnType.TInt ||
                dalCol.Type == DalColumnType.TInt8 ||
                dalCol.Type == DalColumnType.TUInt8 ||
                dalCol.Type == DalColumnType.TInt16 ||
                dalCol.Type == DalColumnType.TUInt16 ||
                dalCol.Type == DalColumnType.TInt32 ||
                dalCol.Type == DalColumnType.TUInt32 ||
                dalCol.Type == DalColumnType.TInt64 ||
                dalCol.Type == DalColumnType.TUInt64 ||
                dalCol.Type == DalColumnType.TDecimal ||
                dalCol.Type == DalColumnType.TMoney ||
                dalCol.Type == DalColumnType.TDouble ||
                dalCol.Type == DalColumnType.TFloat)
            {
                BuildReaderStatement(dalCol, context, ref fromReader, ref fromDb);
            }
            else if (dalCol.Type == DalColumnType.TJson
                || dalCol.Type == DalColumnType.TJsonBinary)
            {
                if (dalCol.IsNullable)
                    fromReader = "reader.GetStringOrNull(Columns.{0})";
                else fromDb = "(string){0}";
            }
            else if (dalCol.Type == DalColumnType.TLongText ||
                dalCol.Type == DalColumnType.TMediumText ||
                dalCol.Type == DalColumnType.TText ||
                dalCol.Type == DalColumnType.TString ||
                dalCol.Type == DalColumnType.TFixedString)
            {
                if (dalCol.IsNullable)
                    fromReader = "reader.GetStringOrNull(Columns.{0})";
                else fromDb = "(string){0}";
            }
            else if (dalCol.Type == DalColumnType.TGeometry ||
                dalCol.Type == DalColumnType.TGeometryCollection ||
                dalCol.Type == DalColumnType.TPoint ||
                dalCol.Type == DalColumnType.TLineString ||
                dalCol.Type == DalColumnType.TPolygon ||
                dalCol.Type == DalColumnType.TLine ||
                dalCol.Type == DalColumnType.TCurve ||
                dalCol.Type == DalColumnType.TSurface ||
                dalCol.Type == DalColumnType.TLinearRing ||
                dalCol.Type == DalColumnType.TMultiPoint ||
                dalCol.Type == DalColumnType.TMultiLineString ||
                dalCol.Type == DalColumnType.TMultiPolygon ||
                dalCol.Type == DalColumnType.TMultiCurve ||
                dalCol.Type == DalColumnType.TMultiSurface ||
                dalCol.Type == DalColumnType.TGeographic ||
                dalCol.Type == DalColumnType.TGeographicCollection ||
                dalCol.Type == DalColumnType.TGeographicPoint ||
                dalCol.Type == DalColumnType.TGeographicLineString ||
                dalCol.Type == DalColumnType.TGeographicPolygon ||
                dalCol.Type == DalColumnType.TGeographicLine ||
                dalCol.Type == DalColumnType.TGeographicCurve ||
                dalCol.Type == DalColumnType.TGeographicSurface ||
                dalCol.Type == DalColumnType.TGeographicLinearRing ||
                dalCol.Type == DalColumnType.TGeographicMultiPoint ||
                dalCol.Type == DalColumnType.TGeographicMultiLineString ||
                dalCol.Type == DalColumnType.TGeographicMultiPolygon ||
                dalCol.Type == DalColumnType.TGeographicMultiCurve ||
                dalCol.Type == DalColumnType.TGeographicMultiSurface)
            {
                var (actualType, effectiveType, isReferenceType) = GetClrTypeName(dalCol, context);
                fromReader = "reader.GetGeometry(Columns.{0}) as " + actualType;
            }

            else if (dalCol.Type == DalColumnType.TDateTime ||
                dalCol.Type == DalColumnType.TDateTimeUtc ||
                dalCol.Type == DalColumnType.TDateTimeLocal ||
                dalCol.Type == DalColumnType.TDateTimeOffset ||
                dalCol.Type == DalColumnType.TDate)
            {
                fromReader = "reader.GetDateTime";

                if (dalCol.Type == DalColumnType.TDateTimeUtc)
                {
                    fromReader += "Utc";
                }
                else if (dalCol.Type == DalColumnType.TDateTimeLocal)
                {
                    fromReader += "Local";
                }
                else if (dalCol.Type == DalColumnType.TDateTimeOffset)
                {
                    fromReader += "Offset";
                }

                if (dalCol.IsNullable)
                {
                    fromReader += "OrNull";
                }

                fromReader += "(Columns.{0})";

                if (dalCol.IsNullable &&
                    !string.IsNullOrEmpty(dalCol.DefaultValue) &&
                    dalCol.DefaultValue != "null")
                {
                    fromReader += " ?? " + dalCol.DefaultValue;
                }
            }

            else if (dalCol.Type == DalColumnType.TTime)
            {
                fromReader = "reader.GetTimeSpan";

                if (dalCol.IsNullable)
                {
                    fromReader += "OrNull";
                }

                fromReader += "(Columns.{0})";

                if (dalCol.IsNullable &&
                    !string.IsNullOrEmpty(dalCol.DefaultValue) &&
                    dalCol.DefaultValue != "null")
                {
                    fromReader += " ?? " + dalCol.DefaultValue;
                }
            }

            if (!string.IsNullOrEmpty(dalCol.EnumTypeName))
            {
                if (dalCol.IsNullable)
                {
                    fromDb = $"reader.IsDBNull(Columns.{dalCol.PropertyName}) ? ({dalCol.EnumTypeName}?)null : ({dalCol.EnumTypeName}){fromDb}";
                }
                else
                {
                    fromDb = $"({dalCol.EnumTypeName}){fromDb}";
                }
            }

            if (!string.IsNullOrEmpty(dalCol.FromDb))
            {
                fromDb = dalCol.FromDb!;
            }

            AppendLine(stringBuilder, $"{dalCol.PropertyName} = {string.Format(fromDb, string.Format(fromReader, dalCol.PropertyName), dalCol.DefaultValue, dalCol.PropertyName)};");
        }

        if (!string.IsNullOrEmpty(context.CustomAfterRead))
        {
            AppendLine(stringBuilder);
            AppendLine(stringBuilder, context.CustomAfterRead!);
            AppendLine(stringBuilder);
        }

        AppendLine(stringBuilder);
        AppendLine(stringBuilder, "MarkOld();");

        if (context.AtomicUpdates)
        {
            AppendLine(stringBuilder, "MarkAllColumnsNotMutated();");
        }

        AppendLine(stringBuilder, "}");
    }

    private static void BuildReaderStatement(DalColumn col, ScriptContext context, ref string fromReader, ref string fromDb)
    {
        var typeName = "";

        switch (col.Type)
        {
            case DalColumnType.TInt: typeName = "Int32"; break;
            case DalColumnType.TInt8: typeName = "SByte"; break;
            case DalColumnType.TInt16: typeName = "Int16"; break;
            case DalColumnType.TInt32: typeName = "Int32"; break;
            case DalColumnType.TInt64: typeName = "Int64"; break;
            case DalColumnType.TUInt8: typeName = "Byte"; break;
            case DalColumnType.TUInt16: typeName = "UInt16"; break;
            case DalColumnType.TUInt32: typeName = "UInt32"; break;
            case DalColumnType.TUInt64: typeName = "UInt64"; break;
            case DalColumnType.TFloat: typeName = "Float"; break;
            case DalColumnType.TDouble: typeName = "Double"; break;
            case DalColumnType.TDecimal: typeName = "Decimal"; break;
            case DalColumnType.TMoney: typeName = "Decimal"; break;
            case DalColumnType.TBool: typeName = "Boolean"; break;
            case DalColumnType.TGuid: typeName = "Guid"; break;
        }

        var (actualType, effectiveType, isReferenceType) = GetClrTypeName(col, context);

        if (col.IsNullable || effectiveType?.EndsWith("?") == true)
        {
            if (col.IsCustomType)
            {
                fromReader = $"({effectiveType})reader.Get{typeName}OrNull(Columns.{{0}})";
            }
            else
            {
                fromReader = $"reader.Get{typeName}OrNull(Columns.{{0}})";

                if (!col.IsNullable && !string.IsNullOrEmpty(col.ActualDefaultValue))
                {
                    fromReader = fromReader + " ?? " + col.ActualDefaultValue!.Replace("{", "{{").Replace("}", "}}");
                }
            }
        }
        else
        {
            if (col.IsCustomType)
            {
                fromReader = $"({effectiveType})reader.Get{typeName}(Columns.{{0}})";
            }
            else
            {
                fromReader = $"reader.Get{typeName}(Columns.{{0}})";
            }
        }
    }
}