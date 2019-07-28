using System.Text;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace dg.Sql.SchemaGenerator
{
    public partial class GeneratorCore
	{
        private static void WriteReadMethod(StringBuilder stringBuilder, ScriptContext context)
        {
            // Read() method
            stringBuilder.AppendFormat("public override void Read(DataReaderBase reader){0}{{{0}", "\r\n");
            foreach (DalColumn dalCol in context.Columns)
            {
                if (dalCol.NoRead) continue;

                string fromDb = "{0}";
                string fromReader = "reader[Columns.{0}]";
                if (dalCol.Type == DalColumnType.TBool)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToBoolean({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToBoolean({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToBoolean({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TGuid)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "GuidFromDb({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : GuidFromDb({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : GuidFromDb({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TInt || dalCol.Type == DalColumnType.TInt32)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToInt32({0})";
                    }
                    else if (dalCol.DefaultValue == "0")
                    {
                        fromDb = "Int32OrZero({0})";
                    }
                    else if (dalCol.DefaultValue != "null")
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt32({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt32({{0}})", dalCol.ActualType));
                    }
                    else
                    {
                        fromDb = "Int32OrNullFromDb({0})";
                    }
                }
                else if (dalCol.Type == DalColumnType.TUInt32)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToUInt32({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt32({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt32({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TInt8)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToSByte({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToSByte({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToSByte({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TUInt8)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToByte({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToByte({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToByte({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TInt16)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToInt16({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt16({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt16({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TUInt16)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToUInt16({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt16({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt16({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TInt64)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToInt64({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt64({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt64({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TUInt64)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToUInt64({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt64({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt64({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TDecimal || dalCol.Type == DalColumnType.TMoney)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToDecimal({0})";
                    }
                    else if (dalCol.DefaultValue == "0" || dalCol.DefaultValue == "0m")
                    {
                        fromDb = "DecimalOrZeroFromDb({0})";
                    }
                    else if (dalCol.DefaultValue != "null")
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDecimal({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDecimal({{0}})", dalCol.ActualType));
                    }
                    else
                    {
                        fromDb = "DecimalOrNullFromDb({0})";
                    }
                }
                else if (dalCol.Type == DalColumnType.TDouble)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToDouble({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDouble({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDouble({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TFloat)
                {
                    if (!dalCol.IsNullable)
                    {
                        fromDb = "Convert.ToSingle({0})";
                    }
                    else
                    {
                        fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToSingle({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToSingle({{0}})", dalCol.ActualType));
                    }
                }
                else if (dalCol.Type == DalColumnType.TJson 
                    || dalCol.Type == DalColumnType.TJsonBinary)
                {
                    fromDb = (!dalCol.IsNullable ? "(string){0}" : "StringOrNullFromDb({0})");
                }
                else if (dalCol.Type == DalColumnType.TLongText ||
                    dalCol.Type == DalColumnType.TMediumText ||
                    dalCol.Type == DalColumnType.TText ||
                    dalCol.Type == DalColumnType.TString ||
                    dalCol.Type == DalColumnType.TFixedString)
                {
                    fromDb = (!dalCol.IsNullable ? "(string){0}" : "StringOrNullFromDb({0})");
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
                    fromReader = "reader.GetGeometry(Columns.{0}) as " + dalCol.ActualType;
                }

                else if (dalCol.Type == DalColumnType.TDateTime ||
                    dalCol.Type == DalColumnType.TDateTimeUtc ||
                    dalCol.Type == DalColumnType.TDateTimeLocal)
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

                else if (dalCol.Type == DalColumnType.TJson ||
                    dalCol.Type == DalColumnType.TJsonBinary)
                {
                    fromDb = (!dalCol.IsNullable ? "(string){0}" : "StringOrNullFromDb({0})");
                }

                if (!string.IsNullOrEmpty(dalCol.EnumTypeName))
                {
                    fromDb = "(" + dalCol.EnumTypeName + ")" + fromDb;
                }

                if (!string.IsNullOrEmpty(dalCol.FromDb))
                {
                    fromDb = dalCol.FromDb;
                }

                stringBuilder.AppendFormat("{1} = {2};{0}", "\r\n", dalCol.PropertyName, string.Format(fromDb, string.Format(fromReader, dalCol.PropertyName), dalCol.DefaultValue, dalCol.PropertyName));
            }

            if (!string.IsNullOrEmpty(context.CustomAfterRead))
            {
                stringBuilder.AppendFormat("{0}{1}{0}", "\r\n", context.CustomAfterRead);
            }

            stringBuilder.AppendFormat("{0}MarkOld();{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("MarkAllColumnsNotMutated();{0}", "\r\n");
            }

            stringBuilder.AppendFormat("}}{0}", "\r\n");
        }
	}
}