using System.Text;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
	{
        private static void WriteInsertMethod(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("public override void Insert(ConnectorBase conn = null, string userName = null){0}{{{0}", "\r\n");

            bool printExtraNewLine = false;

            if (!context.NoCreatedBy)
            {
                if (context.Columns.Find((DalColumn c) => c.PropertyName == "CreatedBy") != null)
                {
                    stringBuilder.AppendFormat("CreatedBy = userName;{0}", "\r\n");
                    printExtraNewLine = true;
                }
            }

            if (!context.NoCreatedOn)
            {
                if (context.Columns.Find((DalColumn c) => c.PropertyName == "CreatedOn") != null)
                {
                    stringBuilder.AppendFormat("CreatedOn = DateTime.UtcNow;{0}", "\r\n");
                    printExtraNewLine = true;
                }
            }

            if (printExtraNewLine)
            {
                stringBuilder.Append("\r\n");
            }

            if (!string.IsNullOrEmpty(context.CustomBeforeInsert))
            {
                stringBuilder.AppendFormat("{1}{0}{0}", "\r\n", context.CustomBeforeInsert);
            }
            stringBuilder.AppendFormat("Query qry = new Query(Schema);{0}", "\r\n");
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
                        stringBuilder.AppendFormat("if ({1} > 0){0}{{{0}", "\r\n", dalCol.PropertyName);
                    }
                    else if (dalCol.Type == DalColumnType.TGuid)
                    {
                        stringBuilder.AppendFormat("if ({1}.Equals(Guid.Empty)){0}{{{0}", "\r\n", dalCol.PropertyName);
                    }
                    else
                    {
                        stringBuilder.AppendFormat("if ({1} != null){0}{{{0}", "\r\n", dalCol.PropertyName);
                    }
                }

                stringBuilder.AppendFormat("qry.Insert(Columns.{1}, {2});{0}", "\r\n", dalCol.PropertyName, ValueToDb(dalCol.PropertyName, dalCol));

                if (dalCol.AutoIncrement)
                {
                    stringBuilder.AppendFormat("}}{0}", "\r\n");
                }
            }

            stringBuilder.AppendFormat("{0}object lastInsert = null;{0}if (qry.Execute(out lastInsert, conn) > 0){0}{{{0}", "\r\n");
            if (!string.IsNullOrEmpty(context.SingleColumnPrimaryKeyName))
            {
                string valueConvertorFormat = "{0}";

                DalColumn dalCol = context.Columns.Find(
                    (DalColumn c) => c.Name == context.SingleColumnPrimaryKeyName
                    || c.PropertyName == context.SingleColumnPrimaryKeyName
                );
                if (dalCol.Type == DalColumnType.TBool)
                {
                    valueConvertorFormat = "Convert.ToBoolean({0})";
                }
                else if (dalCol.Type == DalColumnType.TGuid)
                {
                    valueConvertorFormat = "new Guid({0}.ToString())";
                }
                else if (dalCol.Type == DalColumnType.TInt)
                {
                    valueConvertorFormat = "Convert.ToInt32({0})";
                }
                else if (dalCol.Type == DalColumnType.TInt8)
                {
                    valueConvertorFormat = "Convert.ToSByte({0})";
                }
                else if (dalCol.Type == DalColumnType.TInt16)
                {
                    valueConvertorFormat = "Convert.ToInt16({0})";
                }
                else if (dalCol.Type == DalColumnType.TInt32)
                {
                    valueConvertorFormat = "Convert.ToInt32({0})";
                }
                else if (dalCol.Type == DalColumnType.TInt64)
                {
                    valueConvertorFormat = "Convert.ToInt64({0})";
                }
                else if (dalCol.Type == DalColumnType.TUInt8)
                {
                    valueConvertorFormat = "Convert.ToByte({0})";
                }
                else if (dalCol.Type == DalColumnType.TUInt16)
                {
                    valueConvertorFormat = "Convert.ToUInt16({0})";
                }
                else if (dalCol.Type == DalColumnType.TUInt32)
                {
                    valueConvertorFormat = "Convert.ToUInt32({0})";
                }
                else if (dalCol.Type == DalColumnType.TUInt64)
                {
                    valueConvertorFormat = "Convert.ToUInt64({0})";
                }
                else if (dalCol.Type == DalColumnType.TDecimal || dalCol.Type == DalColumnType.TMoney)
                {
                    valueConvertorFormat = "Convert.ToDecimal({0})";
                }
                else if (dalCol.Type == DalColumnType.TDouble)
                {
                    valueConvertorFormat = "Convert.ToDouble({0})";
                }
                else if (dalCol.Type == DalColumnType.TFloat)
                {
                    valueConvertorFormat = "Convert.ToSingle({0})";
                }
                else if (dalCol.Type == DalColumnType.TDateTime)
                {
                    valueConvertorFormat = "Convert.ToDateTime({0})";
                }
                else if (dalCol.Type == DalColumnType.TDateTimeUtc)
                {
                    valueConvertorFormat = "DateTime.SpecifyKind(Convert.ToDateTime({0}),  DateTimeKind.Utc)";
                }
                else if (dalCol.Type == DalColumnType.TDateTimeLocal)
                {
                    valueConvertorFormat = "DateTime.SpecifyKind(Convert.ToDateTime({0}),  DateTimeKind.Local)";
                }
                else if (dalCol.Type == DalColumnType.TJson ||
                    dalCol.Type == DalColumnType.TJsonBinary)
                {
                    valueConvertorFormat = "(string){0}";
                }
                else if (dalCol.Type == DalColumnType.TLongText ||
                    dalCol.Type == DalColumnType.TMediumText ||
                    dalCol.Type == DalColumnType.TText ||
                    dalCol.Type == DalColumnType.TString ||
                    dalCol.Type == DalColumnType.TFixedString)
                {
                    valueConvertorFormat = "(string){0}";
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
                    valueConvertorFormat = "conn.ReadGeometry({0}) as " + dalCol.ActualType;
                }
                stringBuilder.AppendFormat("{1} = {2};{0}", "\r\n", context.SingleColumnPrimaryKeyName, string.Format(valueConvertorFormat, "(lastInsert)"));
            }

            stringBuilder.AppendFormat("MarkOld();{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("MarkAllColumnsNotMutated();{0}", "\r\n");
            }

            stringBuilder.AppendFormat("}}{0}}}{0}", "\r\n");
        }
	}
}