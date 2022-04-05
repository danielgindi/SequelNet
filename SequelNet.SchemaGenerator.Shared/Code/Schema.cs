using System.Text;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
	{
        private static void WriteSchema(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("private static TableSchema _Schema;{0}public struct Columns{0}{{{0}", "\r\n");
            foreach (var dalCol in context.Columns)
            {
                stringBuilder.AppendFormat("public {1} string {2} = \"{3}\";{0}", "\r\n",
                    context.StaticColumns ? @"static" : @"const", dalCol.PropertyName, dalCol.Name);
            }
            stringBuilder.AppendFormat("}}{0}", "\r\n");
            stringBuilder.AppendFormat("public override TableSchema GenerateTableSchema(){0}{{{0}if (null == _Schema){0}{{{0}TableSchema schema = new TableSchema();{0}schema.Name = @\"{1}\";{0}", "\r\n", context.SchemaName);
            if (context.DatabaseOwner != null && context.DatabaseOwner.Length > 0)
            {
                stringBuilder.AppendFormat("schema.DatabaseOwner = @\"{1}\";{0}", "\r\n", context.DatabaseOwner);
            }

            foreach (var dalCol in context.Columns)
            {
                stringBuilder.Append("schema.AddColumn(new TableSchema.Column {");
                WriteSchemaAddColumnArguments(dalCol, stringBuilder);
                stringBuilder.AppendFormat("}});{0}", "\r\n");
            }

            stringBuilder.AppendFormat("{0}_Schema = schema;{0}", "\r\n");
            if (context.Indices.Count > 0)
            {
                stringBuilder.AppendFormat("{0}", "\r\n");
                foreach (var dalIx in context.Indices)
                {
                    stringBuilder.Append("schema.AddIndex(");
                    WriteSchemaAddIndexArguments(stringBuilder, dalIx, context);
                    stringBuilder.AppendFormat(");{0}", "\r\n");
                }
            }

            if (context.ForeignKeys.Count > 0)
            {
                stringBuilder.AppendFormat("{0}", "\r\n");
                foreach (var dalFk in context.ForeignKeys)
                {
                    stringBuilder.Append("schema.AddForeignKey(");
                    WriteSchemaAddForeignKeyArguments(stringBuilder, dalFk, context);
                    stringBuilder.AppendFormat(");{0}", "\r\n");
                }
            }

            if (context.MySqlEngineName.Length > 0)
                stringBuilder.AppendFormat("{0}schema.SetMySqlEngine(MySqlEngineType.{1});{0}", "\r\n", context.MySqlEngineName);

            stringBuilder.AppendFormat("{0}}}{0}{0}return _Schema;{0}}}{0}", "\r\n");
        }

        private static void WriteSchemaAddColumnArguments(DalColumn dalCol, StringBuilder stringBuilder)
        {
            var isReferenceType = false;

            string customActualType = dalCol.ActualType;

            if (!string.IsNullOrEmpty(dalCol.EnumTypeName))
            {
                dalCol.ActualType = dalCol.EnumTypeName;
            }
            else if (dalCol.Type == DalColumnType.TBool)
            {
                dalCol.ActualType = "bool";
            }
            else if (dalCol.Type == DalColumnType.TGuid)
            {
                dalCol.ActualType = "Guid";
            }
            else if (dalCol.Type == DalColumnType.TDateTime ||
                dalCol.Type == DalColumnType.TDateTimeUtc ||
                dalCol.Type == DalColumnType.TDateTimeLocal ||
                dalCol.Type == DalColumnType.TDate)
            {
                dalCol.ActualType = "DateTime";
            }
            else if (dalCol.Type == DalColumnType.TTime)
            {
                dalCol.ActualType = "TimeSpan";
            }
            else if (dalCol.Type == DalColumnType.TInt)
            {
                dalCol.ActualType = "int";
            }
            else if (dalCol.Type == DalColumnType.TInt8)
            {
                dalCol.ActualType = "SByte";
            }
            else if (dalCol.Type == DalColumnType.TInt16)
            {
                dalCol.ActualType = "Int16";
            }
            else if (dalCol.Type == DalColumnType.TInt32)
            {
                dalCol.ActualType = "Int32";
            }
            else if (dalCol.Type == DalColumnType.TInt64)
            {
                dalCol.ActualType = "Int64";
            }
            else if (dalCol.Type == DalColumnType.TUInt8)
            {
                dalCol.ActualType = "Byte";
            }
            else if (dalCol.Type == DalColumnType.TUInt16)
            {
                dalCol.ActualType = "UInt16";
            }
            else if (dalCol.Type == DalColumnType.TUInt32)
            {
                dalCol.ActualType = "UInt32";
            }
            else if (dalCol.Type == DalColumnType.TUInt64)
            {
                dalCol.ActualType = "UInt64";
            }
            else if (dalCol.Type == DalColumnType.TString ||
                dalCol.Type == DalColumnType.TText ||
                dalCol.Type == DalColumnType.TLongText ||
                dalCol.Type == DalColumnType.TMediumText ||
                dalCol.Type == DalColumnType.TFixedString)
            {
                dalCol.ActualType = "string";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TDecimal ||
                dalCol.Type == DalColumnType.TMoney)
            {
                dalCol.ActualType = "decimal";
            }
            else if (dalCol.Type == DalColumnType.TDouble)
            {
                dalCol.ActualType = "double";
            }
            else if (dalCol.Type == DalColumnType.TFloat)
            {
                dalCol.ActualType = "float";
            }
            else if (dalCol.Type == DalColumnType.TJson ||
                dalCol.Type == DalColumnType.TJsonBinary)
            {
                dalCol.ActualType = "string";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeometry)
            {
                dalCol.ActualType = "Geometry";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeometryCollection)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TPoint)
            {
                dalCol.ActualType = "Geometry.Point";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TLineString)
            {
                dalCol.ActualType = "Geometry.LineString";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TPolygon)
            {
                dalCol.ActualType = "Geometry.Polygon";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TLine)
            {
                dalCol.ActualType = "Geometry.Line";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TCurve)
            {
                dalCol.ActualType = "Geometry";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TSurface)
            {
                dalCol.ActualType = "Geometry";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TLinearRing)
            {
                dalCol.ActualType = "Geometry";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TMultiPoint)
            {
                dalCol.ActualType = "Geometry.MultiPoint";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TMultiLineString)
            {
                dalCol.ActualType = "Geometry.MultiLineString";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TMultiPolygon)
            {
                dalCol.ActualType = "Geometry.MultiPolygon";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TMultiCurve)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TMultiSurface)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographic)
            {
                dalCol.ActualType = "Geometry";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicCollection)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicPoint)
            {
                dalCol.ActualType = "Geometry.Point";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicLineString)
            {
                dalCol.ActualType = "Geometry.LineString";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicPolygon)
            {
                dalCol.ActualType = "Geometry.Polygon";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicLine)
            {
                dalCol.ActualType = "Geometry.Line";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicCurve)
            {
                dalCol.ActualType = "Geometry";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicSurface)
            {
                dalCol.ActualType = "Geometry";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicLinearRing)
            {
                dalCol.ActualType = "Geometry";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiPoint)
            {
                dalCol.ActualType = "Geometry.MultiPoint";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiLineString)
            {
                dalCol.ActualType = "Geometry.MultiLineString";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiPolygon)
            {
                dalCol.ActualType = "Geometry.MultiPolygon";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiCurve)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiSurface)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
                isReferenceType = true;
            }
            else if (dalCol.Type == DalColumnType.TLiteral)
            {
                // Do not change it, specified by ACTUALTYPE
                isReferenceType = 
                    dalCol.ActualType != "int" &&
                    dalCol.ActualType != "Int16" &&
                    dalCol.ActualType != "Int32" &&
                    dalCol.ActualType != "Int64" &&
                    dalCol.ActualType != "float" &&
                    dalCol.ActualType != "double" &&
                    dalCol.ActualType != "decimal";
            }

            stringBuilder.AppendFormat("{0}Name = Columns.{1},",
                "\r\n",
                dalCol.PropertyName);

            stringBuilder.AppendFormat("{0}Type = typeof({1}),",
                "\r\n",
                dalCol.ActualType);

            var dataTypeString = "";

            if (dalCol.Type == DalColumnType.TDate)
            {
                dataTypeString = "DataType.Date";
            }
            else if (dalCol.Type == DalColumnType.TTime)
            {
                dataTypeString = "DataType.Time";
            }
            else if (dalCol.Type == DalColumnType.TText)
            {
                dataTypeString = "DataType.Text";
            }
            else if (dalCol.Type == DalColumnType.TLongText)
            {
                dataTypeString = "DataType.LongText";
            }
            else if (dalCol.Type == DalColumnType.TMediumText)
            {
                dataTypeString = "DataType.MediumText";
            }
            else if (dalCol.Type == DalColumnType.TFixedString)
            {
                dataTypeString = "DataType.Char";
            }
            else if (dalCol.Type == DalColumnType.TMoney)
            {
                dataTypeString = "DataType.Money";
            }
            else if (dalCol.Type == DalColumnType.TJson)
            {
                dataTypeString = "DataType.Json";
            }
            else if (dalCol.Type == DalColumnType.TJsonBinary)
            {
                dataTypeString = "DataType.JsonBinary";
            }
            else if (dalCol.Type == DalColumnType.TGeometry)
            {
                dataTypeString = "DataType.Geometry";
            }
            else if (dalCol.Type == DalColumnType.TGeometryCollection)
            {
                dataTypeString = "DataType.GeometryCollection";
            }
            else if (dalCol.Type == DalColumnType.TPoint)
            {
                dataTypeString = "DataType.Point";
            }
            else if (dalCol.Type == DalColumnType.TLineString)
            {
                dataTypeString = "DataType.LineString";
            }
            else if (dalCol.Type == DalColumnType.TPolygon)
            {
                dataTypeString = "DataType.Polygon";
            }
            else if (dalCol.Type == DalColumnType.TLine)
            {
                dataTypeString = "DataType.Line";
            }
            else if (dalCol.Type == DalColumnType.TCurve)
            {
                dataTypeString = "DataType.Curve";
            }
            else if (dalCol.Type == DalColumnType.TSurface)
            {
                dataTypeString = "DataType.Surface";
            }
            else if (dalCol.Type == DalColumnType.TLinearRing)
            {
                dataTypeString = "DataType.LinearRing";
            }
            else if (dalCol.Type == DalColumnType.TMultiPoint)
            {
                dataTypeString = "DataType.MultiPoint";
            }
            else if (dalCol.Type == DalColumnType.TMultiLineString)
            {
                dataTypeString = "DataType.MultiLineString";
            }
            else if (dalCol.Type == DalColumnType.TMultiPolygon)
            {
                dataTypeString = "DataType.MultiPolygon";
            }
            else if (dalCol.Type == DalColumnType.TMultiCurve)
            {
                dataTypeString = "DataType.MultiCurve";
            }
            else if (dalCol.Type == DalColumnType.TMultiSurface)
            {
                dataTypeString = "DataType.MultiSurface";
            }
            else if (dalCol.Type == DalColumnType.TGeographic)
            {
                dataTypeString = "DataType.Geographic";
            }
            else if (dalCol.Type == DalColumnType.TGeographicCollection)
            {
                dataTypeString = "DataType.GeographicCollection";
            }
            else if (dalCol.Type == DalColumnType.TGeographicPoint)
            {
                dataTypeString = "DataType.GeographicPoint";
            }
            else if (dalCol.Type == DalColumnType.TGeographicLineString)
            {
                dataTypeString = "DataType.GeographicLineString";
            }
            else if (dalCol.Type == DalColumnType.TGeographicPolygon)
            {
                dataTypeString = "DataType.GeographicPolygon";
            }
            else if (dalCol.Type == DalColumnType.TGeographicLine)
            {
                dataTypeString = "DataType.GeographicLine";
            }
            else if (dalCol.Type == DalColumnType.TGeographicCurve)
            {
                dataTypeString = "DataType.GeographicCurve";
            }
            else if (dalCol.Type == DalColumnType.TGeographicSurface)
            {
                dataTypeString = "DataType.GeographicSurface";
            }
            else if (dalCol.Type == DalColumnType.TGeographicLinearRing)
            {
                dataTypeString = "DataType.GeographicLinearRing";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiPoint)
            {
                dataTypeString = "DataType.GeographicMultiPoint";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiLineString)
            {
                dataTypeString = "DataType.GeographicMultiLineString";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiPolygon)
            {
                dataTypeString = "DataType.GeographicMultiPolygon";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiCurve)
            {
                dataTypeString = "DataType.GeographicMultiCurve";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiSurface)
            {
                dataTypeString = "DataType.GeographicMultiSurface";
            }
            else if (!string.IsNullOrEmpty(dalCol.EnumTypeName))
            {
                if (dalCol.Type == DalColumnType.TInt8)
                {
                    dataTypeString = "DataType.TinyInt";
                }
                else if (dalCol.Type == DalColumnType.TInt16)
                {
                    dataTypeString = "DataType.SmallInt";
                }
                else if (dalCol.Type == DalColumnType.TInt32)
                {
                    dataTypeString = "DataType.Int";
                }
                else if (dalCol.Type == DalColumnType.TInt64)
                {
                    dataTypeString = "DataType.BigInt";
                }
                else if (dalCol.Type == DalColumnType.TUInt8)
                {
                    dataTypeString = "DataType.UnsignedTinyInt";
                }
                else if (dalCol.Type == DalColumnType.TUInt16)
                {
                    dataTypeString = "DataType.UnsignedSmallInt";
                }
                else if (dalCol.Type == DalColumnType.TUInt32)
                {
                    dataTypeString = "DataType.UnsignedInt";
                }
                else if (dalCol.Type == DalColumnType.TUInt64)
                {
                    dataTypeString = "DataType.UnsignedBigInt";
                }
            }

            if (!string.IsNullOrEmpty(dataTypeString))
            {
                stringBuilder.AppendFormat("{0}DataType = {1},",
                "\r\n",
                dataTypeString);
            }

            if (!string.IsNullOrEmpty(customActualType))
            {
                dalCol.ActualType = customActualType;
                dalCol.IsCustomType = true;
            }
            else if (dalCol.IsNullable && !isReferenceType)
            {
                dalCol.ActualType += "?";
            }

            stringBuilder.AppendFormat("{0}MaxLength = {1},",
                "\r\n",
                dalCol.MaxLength);

            if (!string.IsNullOrEmpty(dalCol.LiteralType))
            {
                stringBuilder.AppendFormat("{0}LiteralType = {1},",
                    "\r\n",
                    (@"""" + dalCol.LiteralType.Replace(@"""", @"\""") + @""""));
            }

            stringBuilder.AppendFormat("{0}NumberPrecision = {1},",
                "\r\n",
                dalCol.Precision);

            stringBuilder.AppendFormat("{0}NumberScale = {1},",
                "\r\n",
                dalCol.Scale);

            if (dalCol.AutoIncrement)
            {
                stringBuilder.AppendFormat("{0}AutoIncrement = {1},",
                    "\r\n",
                    "true");
            }

            if (dalCol.IsPrimaryKey)
            {
                stringBuilder.AppendFormat("{0}IsPrimaryKey = {1},",
                    "\r\n",
                    "true");
            }

            if (dalCol.IsNullable)
            {
                stringBuilder.AppendFormat("{0}Nullable = {1},",
                    "\r\n",
                    "true");
            }

            stringBuilder.AppendFormat("{0}Default = {1},",
                "\r\n",
                dalCol.DefaultValue);

            if (!string.IsNullOrEmpty(dalCol.Computed))
            {
                stringBuilder.AppendFormat("{0}ComputedColumn = {1},",
                    "\r\n",
                    dalCol.Computed);

                stringBuilder.AppendFormat("{0}ComputedColumnStored = {1},",
                    "\r\n",
                    dalCol.ComputedStored ? "true" : "false");
            }

            if (dalCol.SRID != null)
            {
                stringBuilder.AppendFormat("{0}SRID = {1},",
                    "\r\n",
                    dalCol.SRID);
            }

            if (!string.IsNullOrEmpty(dalCol.Charset))
            {
                stringBuilder.AppendFormat("{0}Charset = {1},",
                    "\r\n",
                    CsharpString(dalCol.Charset));
            }

            if (!string.IsNullOrEmpty(dalCol.Collate))
            {
                stringBuilder.AppendFormat("{0}Collate = {1},",
                    "\r\n",
                    CsharpString(dalCol.Collate));
            }

            if (!string.IsNullOrEmpty(dalCol.Comment))
            {
                stringBuilder.AppendFormat("{0}Comment = {1},",
                    "\r\n",
                    CsharpString(dalCol.Comment));
            }
        }

        private static void WriteSchemaAddIndexArguments(StringBuilder stringBuilder, DalIndex dalIx, ScriptContext context)
        {
            object[] formatArgs = new object[4];
            formatArgs[0] = (dalIx.IndexName == null ? "null" : ("\"" + dalIx.IndexName + "\""));
            formatArgs[1] = dalIx.ClusterMode.ToString();
            formatArgs[2] = dalIx.IndexMode.ToString();
            formatArgs[3] = dalIx.IndexType.ToString();
            stringBuilder.AppendFormat("{0}, TableSchema.ClusterMode.{1}, TableSchema.IndexMode.{2}, TableSchema.IndexType.{3}", formatArgs);
            foreach (DalIndexColumn indexColumn in dalIx.Columns)
            {
                if (indexColumn.Literal)
                {
                    stringBuilder.AppendFormat(", {0}", indexColumn.Name);
                }
                else
                {
                    DalColumn dalCol = context.Columns.Find((DalColumn c) => c.Name == indexColumn.Name || c.PropertyName == indexColumn.Name);
                    string col = (dalCol == null ? $"\"{indexColumn.Name}\"" : $"Columns.{dalCol.PropertyName}");
                    stringBuilder.AppendFormat(", {0}", col);
                }

                if (!string.IsNullOrEmpty(indexColumn.SortDirection))
                {
                    stringBuilder.AppendFormat(", SortDirection.{0}", indexColumn.SortDirection);
                }
            }
        }

        private static void WriteSchemaAddForeignKeyArguments(StringBuilder stringBuilder, DalForeignKey dalFK, ScriptContext context)
        {
            stringBuilder.AppendFormat("{0}, ",
                (dalFK.ForeignKeyName == null ? "null" : ("\"" + dalFK.ForeignKeyName + "\"")));
            if (dalFK.Columns.Count <= 1)
            {
                stringBuilder.AppendFormat("{0}.Columns.{1}, ", context.ClassName, dalFK.Columns[0]);
            }
            else
            {
                stringBuilder.Append("new string[] {");
                foreach (string dalFKCol in dalFK.Columns)
                {
                    if (dalFKCol != dalFK.Columns[0])
                    {
                        stringBuilder.Append(" ,");
                    }
                    stringBuilder.AppendFormat("{0}.Columns.{1}", context.ClassName, dalFKCol);
                }
                stringBuilder.Append("}, ");
            }
            if (dalFK.ForeignTable != context.ClassName)
            {
                stringBuilder.AppendFormat("{0}.SchemaName, ", dalFK.ForeignTable);
            }
            else
            {
                stringBuilder.Append("schema.Name, ");
            }
            if (dalFK.ForeignColumns.Count <= 1)
            {
                stringBuilder.AppendFormat("{0}.Columns.{1}, ", dalFK.ForeignTable, dalFK.ForeignColumns[0]);
            }
            else
            {
                stringBuilder.Append("new string[] {");
                foreach (string foreignColumn in dalFK.ForeignColumns)
                {
                    if (foreignColumn != dalFK.ForeignColumns[0])
                    {
                        stringBuilder.Append(" ,");
                    }
                    stringBuilder.AppendFormat("{0}.Columns.{1}", dalFK.ForeignTable, foreignColumn);
                }
                stringBuilder.Append("}, ");
            }
            stringBuilder.AppendFormat("TableSchema.ForeignKeyReference.{0}, TableSchema.ForeignKeyReference.{1}", dalFK.OnDelete.ToString(), dalFK.OnUpdate.ToString());
        }
    }
}