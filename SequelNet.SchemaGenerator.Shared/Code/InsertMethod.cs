using System.Text;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    private static void WriteGetInsertQuery(StringBuilder stringBuilder, ScriptContext context)
    {
        AppendLine(stringBuilder, "public override Query GetInsertQuery()" + NewLine + "{" );

        bool printExtraNewLine = false;

        if (!context.NoCreatedOn)
        {
            if (context.Columns.Find((DalColumn c) => c.PropertyName == "CreatedOn") != null)
            {
                AppendLine(stringBuilder, "CreatedOn = DateTime.UtcNow;");
                printExtraNewLine = true;
            }
        }

        if (printExtraNewLine)
        {
            AppendLine(stringBuilder);
        }

        AppendLine(stringBuilder, "Query qry = new Query(Schema);");
        AppendLine(stringBuilder);

        foreach (DalColumn dalCol in context.Columns)
        {
            if ((dalCol.AutoIncrement && !context.InsertAutoIncrement) || dalCol.NoSave)
            {
                continue;
            }

            if (dalCol.AutoIncrement)
            {
                if (dalCol.Type == DalColumnType.TInt ||
                    dalCol.Type == DalColumnType.TInt8 ||
                    dalCol.Type == DalColumnType.TInt16 ||
                    dalCol.Type == DalColumnType.TInt32 ||
                    dalCol.Type == DalColumnType.TInt64 ||
                    dalCol.Type == DalColumnType.TUInt8 ||
                    dalCol.Type == DalColumnType.TUInt16 ||
                    dalCol.Type == DalColumnType.TUInt32 ||
                    dalCol.Type == DalColumnType.TUInt64)
                {
                    AppendLine(stringBuilder, $"if ({dalCol.PropertyName} > 0)");
                    AppendLine(stringBuilder, "{");
                }
                else if (dalCol.Type == DalColumnType.TGuid)
                {
                    AppendLine(stringBuilder, $"if ({dalCol.PropertyName}.Equals(Guid.Empty))");
                    AppendLine(stringBuilder, "{");
                }
                else
                {
                    AppendLine(stringBuilder, $"if ({dalCol.PropertyName} != null)");
                    AppendLine(stringBuilder, "{");
                }
            }

            AppendLine(stringBuilder, $"qry.Insert(Columns.{dalCol.PropertyName}, {ValueToDb(dalCol.PropertyName!, dalCol)});");

            if (dalCol.AutoIncrement)
            {
                AppendLine(stringBuilder, "}");
            }
        }

        if (!string.IsNullOrEmpty(context.CustomAfterInsertQuery))
        {
            AppendLine(stringBuilder, context.CustomAfterInsertQuery!);
        }

        AppendLine(stringBuilder);
        AppendLine(stringBuilder, "return qry;");

        AppendLine(stringBuilder, "}");
    }

    private static string GetLastInsertValueConvertFormat(ScriptContext context)
    {
        string valueConvertorFormat = "{0}";

        DalColumn dalCol = context.Columns.Find(
            (DalColumn c) => c.Name == context.SingleColumnPrimaryKeyName
            || c.PropertyName == context.SingleColumnPrimaryKeyName
        );

        if (context.NullableEnabled && !dalCol.IsNullable)
        {
            valueConvertorFormat += "!";
        }

        if (dalCol.Type == DalColumnType.TBool)
        {
            valueConvertorFormat = $"Convert.ToBoolean({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TGuid)
        {
            valueConvertorFormat = $"new Guid({valueConvertorFormat}.ToString())";
        }
        else if (dalCol.Type == DalColumnType.TInt)
        {
            valueConvertorFormat = $"Convert.ToInt32({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TInt8)
        {
            valueConvertorFormat = $"Convert.ToSByte({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TInt16)
        {
            valueConvertorFormat = $"Convert.ToInt16({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TInt32)
        {
            valueConvertorFormat = $"Convert.ToInt32({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TInt64)
        {
            valueConvertorFormat = $"Convert.ToInt64({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TUInt8)
        {
            valueConvertorFormat = $"Convert.ToByte({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TUInt16)
        {
            valueConvertorFormat = $"Convert.ToUInt16({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TUInt32)
        {
            valueConvertorFormat = $"Convert.ToUInt32({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TUInt64)
        {
            valueConvertorFormat = $"Convert.ToUInt64({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TDecimal || dalCol.Type == DalColumnType.TMoney)
        {
            valueConvertorFormat = $"Convert.ToDecimal({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TDouble)
        {
            valueConvertorFormat = $"Convert.ToDouble({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TFloat)
        {
            valueConvertorFormat = $"Convert.ToSingle({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TDateTime)
        {
            valueConvertorFormat = $"Convert.ToDateTime({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TDateTimeUtc)
        {
            valueConvertorFormat = $"DateTime.SpecifyKind(Convert.ToDateTime({valueConvertorFormat}),  DateTimeKind.Utc)";
        }
        else if (dalCol.Type == DalColumnType.TDateTimeLocal)
        {
            valueConvertorFormat = $"DateTime.SpecifyKind(Convert.ToDateTime({valueConvertorFormat}),  DateTimeKind.Local)";
        }
        else if (dalCol.Type == DalColumnType.TDateTimeOffset)
        {
            valueConvertorFormat = $"(DateTimeOffset)DateTime.SpecifyKind(Convert.ToDateTime({valueConvertorFormat}),  DateTimeKind.Utc)";
        }
        else if (dalCol.Type == DalColumnType.TDate)
        {
            valueConvertorFormat = $"Convert.ToDateTime({valueConvertorFormat})";
        }
        else if (dalCol.Type == DalColumnType.TTime)
        {
            valueConvertorFormat = $"({{0}}) is TimeSpan ? (TimeSpan)({valueConvertorFormat}) : TimeSpan.Parse((string)({valueConvertorFormat}))";
        }
        else if (dalCol.Type == DalColumnType.TJson ||
            dalCol.Type == DalColumnType.TJsonBinary)
        {
            valueConvertorFormat = $"(string){valueConvertorFormat}";
        }
        else if (dalCol.Type == DalColumnType.TLongText ||
            dalCol.Type == DalColumnType.TMediumText ||
            dalCol.Type == DalColumnType.TText ||
            dalCol.Type == DalColumnType.TString ||
            dalCol.Type == DalColumnType.TFixedString)
        {
            valueConvertorFormat = $"(string){valueConvertorFormat}";
        }
        else if (dalCol.Type == DalColumnType.TGeometry
            || dalCol.Type == DalColumnType.TGeometryCollection
            || dalCol.Type == DalColumnType.TPoint
            || dalCol.Type == DalColumnType.TLineString
            || dalCol.Type == DalColumnType.TPolygon
            || dalCol.Type == DalColumnType.TLine
            || dalCol.Type == DalColumnType.TCurve
            || dalCol.Type == DalColumnType.TSurface
            || dalCol.Type == DalColumnType.TLinearRing
            || dalCol.Type == DalColumnType.TMultiPoint
            || dalCol.Type == DalColumnType.TMultiLineString
            || dalCol.Type == DalColumnType.TMultiPolygon
            || dalCol.Type == DalColumnType.TMultiCurve
            || dalCol.Type == DalColumnType.TMultiSurface
            || dalCol.Type == DalColumnType.TGeographic
            || dalCol.Type == DalColumnType.TGeographicCollection
            || dalCol.Type == DalColumnType.TGeographicPoint
            || dalCol.Type == DalColumnType.TGeographicLineString
            || dalCol.Type == DalColumnType.TGeographicPolygon
            || dalCol.Type == DalColumnType.TGeographicLine
            || dalCol.Type == DalColumnType.TGeographicCurve
            || dalCol.Type == DalColumnType.TGeographicSurface
            || dalCol.Type == DalColumnType.TGeographicLinearRing
            || dalCol.Type == DalColumnType.TGeographicMultiPoint
            || dalCol.Type == DalColumnType.TGeographicMultiLineString
            || dalCol.Type == DalColumnType.TGeographicMultiPolygon
            || dalCol.Type == DalColumnType.TGeographicMultiCurve
            || dalCol.Type == DalColumnType.TGeographicMultiSurface)
        {
            var (actualType, effectiveType, isReferenceType) = GetClrTypeName(dalCol, context);
            valueConvertorFormat = $"conn.ReadGeometry({valueConvertorFormat}) as " + effectiveType;
        }

        return valueConvertorFormat;
    }

    private static void WriteSetPrimaryKeyValueMethod(StringBuilder stringBuilder, ScriptContext context)
    {
        if (!string.IsNullOrEmpty(context.SingleColumnPrimaryKeyName))
        {
            var nullabilitySign = context.NullableEnabled ? "?" : "";

            AppendLine(stringBuilder, $"public override void SetPrimaryKeyValue(object{nullabilitySign} value)");
            AppendLine(stringBuilder, "{");
            AppendLine(stringBuilder,
                $"{context.SingleColumnPrimaryKeyName} = {string.Format(GetLastInsertValueConvertFormat(context), "value")};");
            AppendLine(stringBuilder, "}");
        }
    }

    private static bool WriteInsertMethod(StringBuilder stringBuilder, ScriptContext context)
    {
        bool hasInsertMethod = !string.IsNullOrEmpty(context.CustomBeforeInsert);

        if (hasInsertMethod)
        {
            AppendLine(stringBuilder, "public override void Insert(ConnectorBase conn = null)");
            AppendLine(stringBuilder, "{");

            if (!string.IsNullOrEmpty(context.CustomBeforeInsert))
            {
                AppendLine(stringBuilder, context.CustomBeforeInsert!);
                AppendLine(stringBuilder);
            }

            AppendLine(stringBuilder, "super.Insert(conn);");

            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder, "}");
        }

        return hasInsertMethod;
    }

    private static bool WriteInsertAsyncMethod(StringBuilder stringBuilder, ScriptContext context)
    {
        bool hasInsertMethod = !string.IsNullOrEmpty(context.CustomBeforeInsert);

        if (hasInsertMethod)
        {
            AppendLine(stringBuilder, "public override Task InsertAsync(ConnectorBase conn = null, CancellationToken? cancellationToken = null)");
            AppendLine(stringBuilder, "{");

            if (!string.IsNullOrEmpty(context.CustomBeforeInsert))
            {
                AppendLine(stringBuilder, context.CustomBeforeInsert!);
                AppendLine(stringBuilder);
            }

            AppendLine(stringBuilder, "super.InsertAsync(conn, cancellationToken);");

            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder, "}");
        }

        return hasInsertMethod;
    }
}