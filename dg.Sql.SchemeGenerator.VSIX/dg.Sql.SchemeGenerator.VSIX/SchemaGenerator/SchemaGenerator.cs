using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace dg.Sql.SchemaGenerator
{
	public class GeneratorCore
	{
		public GeneratorCore()
		{
		}

        public static string GenerateDalClass(string script)
		{
            ScriptContext context = new ScriptContext();
			
            context.ScriptLines = script.Trim(new char[] { ' ', '*', '/', '\t', '\r', '\n' }).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            ParseScript(context);

            foreach (var dalIx in context.Indices)
            {
                foreach (var column in dalIx.Columns)
                {
                    if (context.Columns.Find(x => x.NameX.Equals(column.Name)) == null && context.Columns.Find(x => x.Name.Equals(column.Name)) == null)
                    {
                        MessageBox.Show(@"Column " + column.Name + @" not found in index " + (dalIx.IndexName ?? ""));
                    }
                }
            }

            // Start building the output classes

			StringBuilder stringBuilder = new StringBuilder();

            if (context.ExportCollection)
            {
                WriteCollection(stringBuilder, context);
            }

            if (context.ExportRecord)
            {
                WriteRecord(stringBuilder, context);
            }

            // Return results
            return stringBuilder.ToString();
		}
        
        private static void ParseScript(ScriptContext context)
        {
            context.ClassName = context.ScriptLines[0].Trim(new char[] { ' ', '*', '\t' });
            context.SchemaName = context.ScriptLines[1].Trim(new char[] { ' ', '*', '\t' });

            if (context.SchemaName.Contains("."))
            {
                context.DatabaseOwner = context.SchemaName.Substring(0, context.SchemaName.IndexOf("."));
                context.SchemaName = context.SchemaName.Substring(context.SchemaName.IndexOf(".") + 1);
            }

            for (int i = 2; i <= (int)context.ScriptLines.Length - 1; i++)
            {
                string currentLine = context.ScriptLines[i];
                string currentLineTrimmed = currentLine.Trim(new char[] { ' ', '*', '\t' });

                if (currentLineTrimmed.StartsWith("@Index:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] indexArguments = currentLineTrimmed.Substring(7).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    DalIndex dalIndex = new DalIndex();
                    for (int j = 0; j <= (int)indexArguments.Length - 1; j++)
                    {
                        string arg = indexArguments[j].Trim();

                        if (arg.StartsWith("NAME(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexName = arg.Substring(5, arg.IndexOf(")") - 5);
                        }
                        else if (arg.Equals("UNIQUE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexMode = DalIndexIndexMode.Unique;
                        }
                        else if (arg.Equals("PRIMARY KEY", StringComparison.OrdinalIgnoreCase) || arg.Equals("PRIMARYKEY", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexMode = DalIndexIndexMode.PrimaryKey;
                        }
                        else if (arg.Equals("SPATIAL", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexMode = DalIndexIndexMode.Spatial;
                        }
                        else if (arg.Equals("FULLTEXT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexMode = DalIndexIndexMode.FullText;
                        }
                        else if (arg.Equals("BTREE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexType = DalIndexIndexType.BTREE;
                        }
                        else if (arg.Equals("RTREE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexType = DalIndexIndexType.RTREE;
                        }
                        else if (arg.Equals("HASH", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexType = DalIndexIndexType.HASH;
                        }
                        else if (arg.Equals("NONCLUSTERED", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.ClusterMode = DalIndexClusterMode.NonClustered;
                        }
                        else if (arg.Equals("CLUSTERED", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.ClusterMode = DalIndexClusterMode.Clustered;
                        }
                        else if (arg.StartsWith("[", StringComparison.OrdinalIgnoreCase))
                        {
                            string[] columns = arg
                                .Trim(new char[] { ' ', '[', ']', '\t' })
                                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string column in columns)
                            {
                                if (column.EndsWith(@" ASC") || column.EndsWith(@" DESC"))
                                {
                                    dalIndex.Columns.Add(new DalIndexColumn(column.Substring(0, column.LastIndexOf(' ')), column.Substring(column.LastIndexOf(' ') + 1)));
                                }
                                else
                                {
                                    dalIndex.Columns.Add(new DalIndexColumn(column));
                                }
                            }
                        }
                    }

                    if (dalIndex.IndexMode == DalIndexIndexMode.PrimaryKey && dalIndex.Columns.Count == 1)
                    {
                        context.SingleColumnPrimaryKeyName = (context.SingleColumnPrimaryKeyName != null ? "" : dalIndex.Columns[0].Name);
                    }
                    context.Indices.Add(dalIndex);
                }
                else if (currentLineTrimmed.StartsWith("@ForeignKey:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] foreignKeyArguments = currentLineTrimmed.Substring(12).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    DalForeignKey dalForeignKey = new DalForeignKey();
                    for (int l = 0; l <= (int)foreignKeyArguments.Length - 1; l++)
                    {
                        string arg = foreignKeyArguments[l].Trim();

                        if (arg.StartsWith("NAME(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalForeignKey.ForeignKeyName = arg.Substring(5, arg.IndexOf(")") - 5);
                        }
                        else if (arg.StartsWith("FOREIGNTABLE(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalForeignKey.ForeignTable = arg.Substring(13, arg.IndexOf(")") - 13);
                        }
                        else if (arg.StartsWith("ONUPDATE(", StringComparison.OrdinalIgnoreCase))
                        {
                            switch ((arg.Substring(9, arg.IndexOf(")") - 9)).ToUpper())
                            {
                                case "RESTRICT":
                                    dalForeignKey.OnUpdate = DalForeignKeyReference.Restrict;
                                    break;
                                case "CASCADE":
                                    dalForeignKey.OnUpdate = DalForeignKeyReference.Cascade;
                                    break;
                                case "SETNULL":
                                case "SET NULL":
                                    dalForeignKey.OnUpdate = DalForeignKeyReference.SetNull;
                                    break;
                                case "NOACTION":
                                    dalForeignKey.OnUpdate = DalForeignKeyReference.NoAction;
                                    break;
                                default:
                                    dalForeignKey.OnUpdate = DalForeignKeyReference.None;
                                    break;
                            }
                        }
                        else if (arg.StartsWith("ONDELETE(", StringComparison.OrdinalIgnoreCase))
                        {
                            switch ((arg.Substring(9, arg.IndexOf(")") - 9)).ToUpper())
                            {
                                case "RESTRICT":
                                    dalForeignKey.OnDelete = DalForeignKeyReference.Restrict;
                                    break;
                                case "CASCADE":
                                    dalForeignKey.OnDelete = DalForeignKeyReference.Cascade;
                                    break;
                                case "SETNULL":
                                case "SET NULL":
                                    dalForeignKey.OnDelete = DalForeignKeyReference.SetNull;
                                    break;
                                case "NOACTION":
                                    dalForeignKey.OnDelete = DalForeignKeyReference.NoAction;
                                    break;
                                default:
                                    dalForeignKey.OnDelete = DalForeignKeyReference.None;
                                    break;
                            }
                        }
                        else if (arg.StartsWith("COLUMNS[", StringComparison.OrdinalIgnoreCase))
                        {
                            string columns = arg.Substring(7).Trim(new char[] { ' ', '[', ']', '\t' });
                            string[] strArrays = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int k = 0; k < strArrays.Length; k++)
                            {
                                dalForeignKey.Columns.Add(strArrays[k]);
                            }
                        }
                        else if (arg.StartsWith("FOREIGNCOLUMNS[", StringComparison.OrdinalIgnoreCase))
                        {
                            string columns = arg.Substring(14).Trim(new char[] { ' ', '[', ']', '\t' });
                            string[] strArrays = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int k = 0; k < strArrays.Length; k++)
                            {
                                dalForeignKey.ForeignColumns.Add(strArrays[k]);
                            }
                        }
                    }
                    context.ForeignKeys.Add(dalForeignKey);
                }
                else if (currentLineTrimmed.StartsWith("@BeforeInsert:", StringComparison.OrdinalIgnoreCase))
                {
                    context.CustomBeforeInsert = currentLineTrimmed.Substring(14).Trim();
                }
                else if (currentLineTrimmed.StartsWith("@BeforeUpdate:", StringComparison.OrdinalIgnoreCase))
                {
                    context.CustomBeforeUpdate = currentLineTrimmed.Substring(14).Trim();
                }
                else if (currentLineTrimmed.StartsWith("@AfterRead:", StringComparison.OrdinalIgnoreCase))
                {
                    context.CustomAfterRead = currentLineTrimmed.Substring(11).Trim();
                }
                else if (currentLineTrimmed.StartsWith("@StaticColumns", StringComparison.OrdinalIgnoreCase))
                {
                    context.StaticColumns = true;
                }
                else if (currentLineTrimmed.StartsWith("@OmitCollection", StringComparison.OrdinalIgnoreCase))
                {
                    context.ExportCollection = false;
                }
                else if (currentLineTrimmed.StartsWith("@AtomicUpdates", StringComparison.OrdinalIgnoreCase))
                {
                    context.AtomicUpdates = true;
                }
                else if (currentLineTrimmed.StartsWith("@InsertAutoIncrement", StringComparison.OrdinalIgnoreCase))
                {
                    context.InsertAutoIncrement = true;
                }
                else if (!currentLineTrimmed.StartsWith("@MySqlEngine:", StringComparison.OrdinalIgnoreCase))
                {
                    int startPos = currentLineTrimmed.IndexOf(":");
                    DalColumn dalColumn = new DalColumn();
                    dalColumn.Name = currentLineTrimmed.Substring(0, startPos).Trim();
                    dalColumn.NameX = StripColumnName(dalColumn.Name);
                    if (context.ClassName == dalColumn.Name || dalColumn.Name == "Columns")
                    {
                        dalColumn.Name += "X";
                    }
                    dalColumn.IsPrimaryKey = false;
                    dalColumn.IsNullable = false;
                    dalColumn.AutoIncrement = false;
                    dalColumn.Type = DalColumnType.TInt;
                    dalColumn.DefaultValue = "null";
                    dalColumn.ActualDefaultValue = "";
                    dalColumn.Comment = "";
                    dalColumn.EnumTypeName = "";
                    currentLineTrimmed = currentLineTrimmed.Substring(startPos + 1).Trim();
                    string[] columnKeywords = currentLineTrimmed.Split(new char[] { ';' }, StringSplitOptions.None);
                    for (int m = 0; m <= (int)columnKeywords.Length - 1; m++)
                    {
                        string columnKeyword = columnKeywords[m].Trim();
                        if (m == (int)columnKeywords.Length - 1)
                        {
                            if (!columnKeyword.EndsWith(":") || 
                                (int)context.ScriptLines.Length <= i + 2 || 
                                !context.ScriptLines[i + 1].Trim(new char[] { ' ', '*', '\t' }).StartsWith("\"") || 
                                !context.ScriptLines[i + 2].Trim(new char[] { ' ', '*', '\t' }).StartsWith("-"))
                            {
                                dalColumn.Comment = columnKeyword;
                            }
                            else
                            {
                                dalColumn.Comment = columnKeyword.Remove(columnKeyword.Length - 1, 1);
                                i++;
                                currentLineTrimmed = context.ScriptLines[i];
                                DalEnum dalEnum = new DalEnum();
                                dalEnum.Name = currentLineTrimmed.Trim(new char[] { ' ', '*', '\"', '\t' });
                                dalColumn.EnumTypeName = dalEnum.Name;
                                dalEnum.Items = new List<string>();
                                while ((int)context.ScriptLines.Length > i + 1 && 
                                    context.ScriptLines[i + 1].Trim(new char[] { ' ', '*', '\t' }).StartsWith("-"))
                                {
                                    i++;
                                    currentLineTrimmed = context.ScriptLines[i].Trim(new char[] { ' ', '*', '-', '\t' });
                                    dalEnum.Items.Add(currentLineTrimmed);
                                }
                                context.Enums.Add(dalEnum);
                            }
                        }
                        else if (columnKeyword.Equals("PRIMARY KEY", StringComparison.OrdinalIgnoreCase) || 
                            columnKeyword.Equals("PRIMARYKEY", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.IsPrimaryKey = true;
                            context.SingleColumnPrimaryKeyName = (context.SingleColumnPrimaryKeyName != null ? "" : dalColumn.Name);
                        }
                        else if (columnKeyword.Equals("NULLABLE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.IsNullable = true;
                        }
                        else if (columnKeyword.Equals("AUTOINCREMENT", StringComparison.OrdinalIgnoreCase) ||
                            columnKeyword.Equals("AUTO_INCREMENT", StringComparison.OrdinalIgnoreCase) ||
                            columnKeyword.Equals("AUTO INCREMENT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.AutoIncrement = true;
                        }
                        else if (columnKeyword.Equals("NoProperty", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.NoProperty = true;
                        }
                        else if (columnKeyword.Equals("NoSave", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.NoSave = true;
                        }
                        else if (columnKeyword.StartsWith("PRECISION(", StringComparison.OrdinalIgnoreCase))
                        {
                            int precision = 0;
                            int.TryParse(columnKeyword.Substring(10, columnKeyword.IndexOf(")") - 10), out precision);
                            dalColumn.Precision = precision;
                        }
                        else if (columnKeyword.StartsWith("SCALE(", StringComparison.OrdinalIgnoreCase))
                        {
                            int scale = 0;
                            int.TryParse(columnKeyword.Substring(6, columnKeyword.IndexOf(")") - 6), out scale);
                            dalColumn.Scale = scale;
                        }
                        else if (columnKeyword.StartsWith("LITERALTYPE ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TLiteral;
                            dalColumn.LiteralType = columnKeyword.Substring(12).Trim();
                        }
                        else if (columnKeyword.StartsWith("STRING(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TString;
                            string maxLength = columnKeyword.Substring(7, columnKeyword.IndexOf(")") - 7);
                            if (maxLength == "MAX")
                            {
                                dalColumn.MaxLength = -1;
                            }
                            else
                            {
                                int iMaxLength = 0;
                                int.TryParse(maxLength, out iMaxLength);
                                dalColumn.MaxLength = iMaxLength;
                            }
                        }
                        else if (columnKeyword.StartsWith("FIXEDSTRING(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TString;
                            string maxLength = columnKeyword.Substring(12, columnKeyword.IndexOf(")") - 12);
                            if (maxLength == "MAX")
                            {
                                dalColumn.MaxLength = -1;
                            }
                            else
                            {
                                int iMaxLength = 0;
                                int.TryParse(maxLength, out iMaxLength);
                                dalColumn.MaxLength = iMaxLength;
                            }
                        }
                        else if (columnKeyword.Equals("TEXT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TText;
                        }
                        else if (columnKeyword.StartsWith("TEXT(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TText;
                            int maxLength = 0;
                            int.TryParse(columnKeyword.Substring(5, columnKeyword.IndexOf(")") - 5), out maxLength);
                            dalColumn.MaxLength = maxLength;
                        }
                        else if (columnKeyword.Equals("LONGTEXT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TLongText;
                        }
                        else if (columnKeyword.StartsWith("LONGTEXT(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TLongText;
                            int maxLength = 0;
                            int.TryParse(columnKeyword.Substring(9, columnKeyword.IndexOf(")") - 9), out maxLength);
                            dalColumn.MaxLength = maxLength;
                        }
                        else if (columnKeyword.Equals("MEDIUMTEXT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TMediumText;
                        }
                        else if (columnKeyword.StartsWith("MEDIUMTEXT(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TMediumText;
                            int maxLength = 0;
                            int.TryParse(columnKeyword.Substring(11, columnKeyword.IndexOf(")") - 1), out maxLength);
                            dalColumn.MaxLength = maxLength;
                        }
                        else if (columnKeyword.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TBool;
                        }
                        else if (columnKeyword.Equals("GUID", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGuid;
                        }
                        else if (columnKeyword.Equals("DECIMAL", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TDecimal;
                        }
                        else if (columnKeyword.Equals("MONEY", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TMoney;
                        }
                        else if (columnKeyword.StartsWith("DECIMAL", StringComparison.OrdinalIgnoreCase) |
                            columnKeyword.StartsWith("MONEY", StringComparison.OrdinalIgnoreCase))
                        {
                            string precision = "";
                            string scale = "";
                            int leftPartIndex = columnKeyword.IndexOf("(");
                            int commaIndex = columnKeyword.IndexOf(",");
                            int rightParIndex = columnKeyword.IndexOf(")");
                            if (leftPartIndex > -1 & commaIndex > -1)
                            {
                                precision = columnKeyword.Substring(leftPartIndex + 1, commaIndex - leftPartIndex - 1).Trim();
                                scale = columnKeyword.Substring(commaIndex + 1, rightParIndex - commaIndex - 1).Trim();
                            }
                            else if (leftPartIndex > -1)
                            {
                                precision = columnKeyword.Substring(leftPartIndex + 1, rightParIndex - leftPartIndex - 1).Trim();
                            }
                            if (precision.Length > 0)
                            {
                                dalColumn.Precision = Convert.ToInt32(precision);
                            }
                            if (scale.Length > 0)
                            {
                                dalColumn.Scale = Convert.ToInt32(scale);
                            }
                            if (columnKeyword.StartsWith("MONEY", StringComparison.OrdinalIgnoreCase))
                            {
                                dalColumn.Type = DalColumnType.TMoney;
                            }
                            else
                            {
                                dalColumn.Type = DalColumnType.TDecimal;
                            }
                        }
                        else if (columnKeyword.Equals("DOUBLE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TDouble;
                        }
                        else if (columnKeyword.Equals("FLOAT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TFloat;
                        }
                        else if (columnKeyword.Equals("INT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TInt;
                        }
                        else if (columnKeyword.Equals("INTEGER", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TInt;
                        }
                        else if (columnKeyword.Equals("INT8", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TInt8;
                        }
                        else if (columnKeyword.Equals("INT16", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TInt16;
                        }
                        else if (columnKeyword.Equals("INT32", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TInt32;
                        }
                        else if (columnKeyword.Equals("INT64", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TInt64;
                        }
                        else if (columnKeyword.Equals("UINT8", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TUInt8;
                        }
                        else if (columnKeyword.Equals("UINT16", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TUInt16;
                        }
                        else if (columnKeyword.Equals("UINT32", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TUInt32;
                        }
                        else if (columnKeyword.Equals("UINT64", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TUInt64;
                        }
                        else if (columnKeyword.Equals("GEOMETRY", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeometry;
                        }
                        else if (columnKeyword.Equals("GEOMETRYCOLLECTION", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeometryCollection;
                        }
                        else if (columnKeyword.Equals("POINT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TPoint;
                        }
                        else if (columnKeyword.Equals("LINESTRING", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TLineString;
                        }
                        else if (columnKeyword.Equals("POLYGON", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TPolygon;
                        }
                        else if (columnKeyword.Equals("LINE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TLine;
                        }
                        else if (columnKeyword.Equals("CURVE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TCurve;
                        }
                        else if (columnKeyword.Equals("SURFACE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TSurface;
                        }
                        else if (columnKeyword.Equals("LINEARRING", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TLinearRing;
                        }
                        else if (columnKeyword.Equals("MULTIPOINT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TMultiPoint;
                        }
                        else if (columnKeyword.Equals("MULTILINESTRING", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TMultiLineString;
                        }
                        else if (columnKeyword.Equals("MULTIPOLYGON", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TMultiPolygon;
                        }
                        else if (columnKeyword.Equals("MULTICURVE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TMultiCurve;
                        }
                        else if (columnKeyword.Equals("MULTISURFACE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TMultiSurface;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographic;
                        }
                        else if (columnKeyword.Equals("GEOGAPHICCOLLECTION", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicCollection;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_POINT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicPoint;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_LINESTRING", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicLineString;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_POLYGON", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicPolygon;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_LINE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicLine;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_CURVE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicCurve;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_SURFACE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicSurface;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_LINEARRING", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicLinearRing;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_MULTIPOINT", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicMultiPoint;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_MULTILINESTRING", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicMultiLineString;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_MULTIPOLYGON", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicMultiPolygon;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_MULTICURVE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicMultiCurve;
                        }
                        else if (columnKeyword.Equals("GEOGAPHIC_MULTISURFACE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TGeographicMultiSurface;
                        }
                        else if (columnKeyword.Equals("DATETIME", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TDateTime;
                        }
                        else if (columnKeyword.StartsWith("Default ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.DefaultValue = columnKeyword.Substring(8);
                        }
                        else if (columnKeyword.StartsWith("ActualDefault ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.ActualDefaultValue = columnKeyword.Substring(14);
                        }
                        else if (columnKeyword.StartsWith("ToDB ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.ToDb = columnKeyword.Substring(5);
                        }
                        else if (columnKeyword.Equals("Virtual", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Virtual = true;
                        }
                        else if (columnKeyword.StartsWith("FromDB ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.FromDb = columnKeyword.Substring(7);
                        }
                        else if (columnKeyword.StartsWith("ActualType ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.ActualType = columnKeyword.Substring(11);
                        }
                        else if (columnKeyword.StartsWith("ColumnName ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Name = columnKeyword.Substring(11);
                        }
                        else if (columnKeyword.StartsWith("PropertyName ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.NameX = columnKeyword.Substring(13);
                        }
                        else if (columnKeyword.Equals("Unique Index", StringComparison.OrdinalIgnoreCase) ||
                            columnKeyword.Equals("Unique", StringComparison.OrdinalIgnoreCase))
                        {
                            DalIndex dalIx = new DalIndex();
                            dalIx.Columns.Add(new DalIndexColumn(dalColumn.Name));
                            dalIx.IndexMode = DalIndexIndexMode.Unique;
                            context.Indices.Add(dalIx);
                        }
                        else if (columnKeyword.StartsWith("Foreign ", StringComparison.OrdinalIgnoreCase))
                        {
                            DalForeignKey dalFk = new DalForeignKey();
                            string str30 = columnKeyword.Substring(8);
                            dalFk.ForeignTable = str30.Substring(0, str30.IndexOf("."));
                            dalFk.ForeignColumns.Add(str30.Substring(str30.IndexOf(".") + 1));
                            dalFk.Columns.Add(dalColumn.Name);
                            context.ForeignKeys.Add(dalFk);
                        }
                        else if (columnKeyword.StartsWith("Charset ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Charset = columnKeyword.Substring(8);
                        }
                        else if (columnKeyword.StartsWith("Collate ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Collate = columnKeyword.Substring(8);
                        }
                    }
                    if (dalColumn.IsPrimaryKey & dalColumn.Type == DalColumnType.TInt)
                    {
                        dalColumn.Type = DalColumnType.TInt64;
                    }
                    context.Columns.Add(dalColumn);
                }
                else
                {
                    context.MySqlEngineName = currentLineTrimmed.Substring(13).Trim();
                }
            }
            if (context.MySqlEngineName.Equals("MyISAM", StringComparison.OrdinalIgnoreCase))
            {
                context.MySqlEngineName = "MyISAM";
            }
            else if (context.MySqlEngineName.Equals("InnoDB", StringComparison.OrdinalIgnoreCase))
            {
                context.MySqlEngineName = "InnoDB";
            }
            else if (context.MySqlEngineName.Equals("ARCHIVE", StringComparison.OrdinalIgnoreCase))
            {
                context.MySqlEngineName = "ARCHIVE";
            }
        }

        private static void WriteCollection(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("public partial class {1}Collection : AbstractRecordList<{1}, {1}Collection> {{{0}}}{0}{0}", "\r\n", context.ClassName);
            foreach (DalEnum dalEn in context.Enums)
            {
                stringBuilder.AppendFormat("public enum {1}{0}{{{0}", "\r\n", dalEn.Name);
                foreach (string item in dalEn.Items)
                {
                    stringBuilder.AppendFormat("{1},{0}", "\r\n", item);
                }
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");
            }
        }

        private static void WriteRecord(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("public partial class {1} : AbstractRecord<{1}>{0}{{{0}", "\r\n", context.ClassName);

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("#region Static Constructor{0}", "\r\n");
                stringBuilder.AppendFormat("static {1}(){0}", "\r\n", context.ClassName);
                stringBuilder.AppendFormat("{{{0}", "\r\n", context.ClassName);
                stringBuilder.AppendFormat("AtomicUpdates = true;{0}", "\r\n");
                stringBuilder.AppendFormat("}}{0}", "\r\n");
                stringBuilder.AppendFormat("#endregion{0}{0}", "\r\n");
            }

            #region Table Schema

            stringBuilder.AppendFormat("#region Table Schema{0}", "\r\n");

            stringBuilder.AppendFormat("private static TableSchema _TableSchema;{0}public struct Columns{0}{{{0}", "\r\n");
            foreach (DalColumn dalCol in context.Columns)
            {
                stringBuilder.AppendFormat("public {1} string {2} = \"{3}\";", "\r\n", context.StaticColumns ? @"static" : @"const", dalCol.Name, dalCol.NameX);
                if (!string.IsNullOrEmpty(dalCol.Comment))
                {
                    stringBuilder.AppendFormat(" // {1}", "\r\n", dalCol.Comment);
                }
                stringBuilder.Append("\r\n");
            }
            stringBuilder.AppendFormat("}}{0}", "\r\n");
            stringBuilder.AppendFormat("public override TableSchema GetTableSchema(){0}{{{0}if (null == _TableSchema){0}{{{0}TableSchema schema = new TableSchema();{0}schema.SchemaName = @\"{1}\";{0}", "\r\n", context.SchemaName);
            if (context.DatabaseOwner != null && context.DatabaseOwner.Length > 0)
            {
                stringBuilder.AppendFormat("schema.DatabaseOwner = @\"{1}\";{0}", "\r\n", context.DatabaseOwner);
            }

            foreach (DalColumn dalCol in context.Columns)
            {
                stringBuilder.Append("schema.AddColumn(");
                WriteSchemaAddColumnArguments(dalCol, stringBuilder);
                stringBuilder.AppendFormat(");{0}", "\r\n");
            }

            // Create a list of all columns that participate in the Primary Key
            List<DalColumn> primaryKeyColumns = new List<DalColumn>();
            foreach (DalColumn dalCol in context.Columns)
            {
                if (!dalCol.IsPrimaryKey) continue;
                primaryKeyColumns.Add(dalCol);
            }
            foreach (DalIndex dalIx in context.Indices)
            {
                if (dalIx.IndexMode != DalIndexIndexMode.PrimaryKey) continue;
                foreach (DalIndexColumn indexColumn in dalIx.Columns)
                {
                    DalColumn column = context.Columns.Find((DalColumn c) => c.NameX == indexColumn.Name);
                    if (column == null) continue;
                    primaryKeyColumns.Add(column);
                }
            }

            stringBuilder.AppendFormat("{0}_TableSchema = schema;{0}", "\r\n");
            if (context.Indices.Count > 0)
            {
                stringBuilder.AppendFormat("{0}", "\r\n");
                foreach (DalIndex dalIx in context.Indices)
                {
                    stringBuilder.Append("schema.AddIndex(");
                    WriteSchemaAddIndexArguments(stringBuilder, dalIx, context);
                    stringBuilder.AppendFormat(");{0}", "\r\n");
                }
            }

            if (context.ForeignKeys.Count > 0)
            {
                stringBuilder.AppendFormat("{0}", "\r\n");
                foreach (DalForeignKey dalFK in context.ForeignKeys)
                {
                    stringBuilder.Append("schema.AddForeignKey(");
                    WriteSchemaAddForeignKeyArguments(stringBuilder, dalFK, context);
                    stringBuilder.AppendFormat(");{0}", "\r\n");
                }
            }
            if (context.MySqlEngineName.Length > 0)
            {
                stringBuilder.AppendFormat("{0}schema.SetMySqlEngine(MySqlEngineType.{1});{0}", "\r\n", context.MySqlEngineName);
            }
            stringBuilder.AppendFormat("{0}}}{0}{0}return _TableSchema;{0}}}{0}", "\r\n");

            stringBuilder.AppendFormat("#endregion{0}", "\r\n");

            #endregion

            #region Private Members

            stringBuilder.AppendFormat("{0}#region Private Members{0}", "\r\n");
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
                stringBuilder.AppendFormat(" _{0}", dalColumn.Name);
                if ((dalColumn.DefaultValue == "null" || dalColumn.ActualDefaultValue.Length > 0 & (dalColumn.ActualDefaultValue == "null")) && dalColumn.IsNullable)
                {
                    stringBuilder.AppendFormat(" = {1};{0}", "\r\n",
                        (dalColumn.ActualDefaultValue.Length > 0 ? dalColumn.ActualDefaultValue : dalColumn.DefaultValue));
                }
                else if (defaultValue != null)
                {
                    stringBuilder.AppendFormat(" = {1};{0}", "\r\n", defaultValue);
                }
                else
                {
                    stringBuilder.AppendFormat(";{0}", "\r\n");
                }
            }
            stringBuilder.AppendFormat("#endregion{0}", "\r\n");

            #endregion

            #region Properties

            stringBuilder.AppendFormat("{0}#region Properties{0}", "\r\n");

            foreach (DalColumn dalCol in context.Columns)
            {
                if (dalCol.NoProperty)
                {
                    continue;
                }
                object[] formatArgs = new object[] { "\r\n", dalCol.ActualType, dalCol.Name, null };
                formatArgs[3] = (dalCol.Virtual ? "virtual " : "");
                stringBuilder.AppendFormat("public {3}{1} {2}{0}{{{0}", formatArgs);
                stringBuilder.AppendFormat("get{{ return _{2}; }}{0}", formatArgs);
                if (context.AtomicUpdates)
                {
                    stringBuilder.AppendFormat("set{{ _{2}=value; MarkColumnDirty(Columns.{2}); }}{0}", formatArgs);
                }
                else
                {
                    stringBuilder.AppendFormat("set{{_{2}=value;}}{0}", formatArgs);
                }
                stringBuilder.AppendFormat("}}{0}", formatArgs);
            }
            stringBuilder.AppendFormat("#endregion{0}", "\r\n");

            #endregion

            #region AbstractRecord members

            stringBuilder.AppendFormat("{0}#region AbstractRecord members{0}", "\r\n");

            // GetPrimaryKeyValue() function
            stringBuilder.AppendFormat("public override object GetPrimaryKeyValue(){0}{{{0}return {1};{0}}}{0}{0}", "\r\n",
                string.IsNullOrEmpty(context.SingleColumnPrimaryKeyName) ? "null" : context.SingleColumnPrimaryKeyName);

            // Insert() method
            stringBuilder.AppendFormat("public override void Insert(ConnectorBase conn){0}{{{0}", "\r\n");

            bool printExtraNewLine = false;
            if (context.Columns.Find((DalColumn c) => c.Name == "CreatedBy") != null)
            {
                stringBuilder.AppendFormat("CreatedBy = base.CurrentSessionUserName;{0}", "\r\n");
                printExtraNewLine = true;
            }
            if (context.Columns.Find((DalColumn c) => c.Name == "CreatedOn") != null)
            {
                stringBuilder.AppendFormat("CreatedOn = DateTime.UtcNow;{0}", "\r\n");
                printExtraNewLine = true;
            }
            if (printExtraNewLine)
            {
                stringBuilder.Append("\r\n");
            }

            if (!string.IsNullOrEmpty(context.CustomBeforeInsert))
            {
                stringBuilder.AppendFormat("{1}{0}{0}", "\r\n", context.CustomBeforeInsert);
            }
            stringBuilder.AppendFormat("Query qry = new Query(TableSchema);{0}", "\r\n");
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
                        stringBuilder.AppendFormat("if ({1} > 0){0}{{{0}", "\r\n", dalCol.Name);
                    }
                    else if (dalCol.Type == DalColumnType.TGuid)
                    {
                        stringBuilder.AppendFormat("if ({1}.Equals(Guid.Empty)){0}{{{0}", "\r\n", dalCol.Name);
                    }
                    else
                    {
                        stringBuilder.AppendFormat("if ({1} != null){0}{{{0}", "\r\n", dalCol.Name);
                    }
                }

                if (string.IsNullOrEmpty(dalCol.ToDb))
                {
                    stringBuilder.AppendFormat("qry.Insert(Columns.{1}, {1});{0}", "\r\n", dalCol.Name);
                }
                else
                {
                    stringBuilder.AppendFormat("qry.Insert(Columns.{1}, {2});{0}", "\r\n", dalCol.Name, string.Format(dalCol.ToDb, dalCol.Name));
                }

                if (dalCol.AutoIncrement)
                {
                    stringBuilder.AppendFormat("}}{0}", "\r\n");
                }
            }

            stringBuilder.AppendFormat("{0}object lastInsert = null;{0}if (qry.Execute(conn, out lastInsert) > 0){0}{{{0}", "\r\n");
            if (!string.IsNullOrEmpty(context.SingleColumnPrimaryKeyName))
            {
                string valueConvertorFormat = "{0}";

                DalColumn dalCol = context.Columns.Find((DalColumn c) => c.Name == context.SingleColumnPrimaryKeyName);
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
                else if (dalCol.Type == DalColumnType.TLongText || dalCol.Type == DalColumnType.TMediumText || dalCol.Type == DalColumnType.TText || dalCol.Type == DalColumnType.TString || dalCol.Type == DalColumnType.TFixedString)
                {
                    valueConvertorFormat = "(string){0}";
                }
                else if (dalCol.Type == DalColumnType.TGeometry || dalCol.Type == DalColumnType.TGeometryCollection || dalCol.Type == DalColumnType.TPoint || dalCol.Type == DalColumnType.TLineString || dalCol.Type == DalColumnType.TPolygon || dalCol.Type == DalColumnType.TLine || dalCol.Type == DalColumnType.TCurve || dalCol.Type == DalColumnType.TSurface || dalCol.Type == DalColumnType.TLinearRing || dalCol.Type == DalColumnType.TMultiPoint || dalCol.Type == DalColumnType.TMultiLineString || dalCol.Type == DalColumnType.TMultiPolygon || dalCol.Type == DalColumnType.TMultiCurve || dalCol.Type == DalColumnType.TMultiSurface || dalCol.Type == DalColumnType.TGeographic || dalCol.Type == DalColumnType.TGeographicCollection || dalCol.Type == DalColumnType.TGeographicPoint || dalCol.Type == DalColumnType.TGeographicLineString || dalCol.Type == DalColumnType.TGeographicPolygon || dalCol.Type == DalColumnType.TGeographicLine || dalCol.Type == DalColumnType.TGeographicCurve || dalCol.Type == DalColumnType.TGeographicSurface || dalCol.Type == DalColumnType.TGeographicLinearRing || dalCol.Type == DalColumnType.TGeographicMultiPoint || dalCol.Type == DalColumnType.TGeographicMultiLineString || dalCol.Type == DalColumnType.TGeographicMultiPolygon || dalCol.Type == DalColumnType.TGeographicMultiCurve || dalCol.Type == DalColumnType.TGeographicMultiSurface)
                {
                    valueConvertorFormat = "conn.ReadGeometry({0}) as " + dalCol.ActualType;
                }
                stringBuilder.AppendFormat("{1} = {2};{0}", "\r\n", context.SingleColumnPrimaryKeyName, string.Format(valueConvertorFormat, "(lastInsert)"));
            }

            stringBuilder.AppendFormat("MarkOld();{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("MarkAllColumnsNotDirty();{0}", "\r\n");
            }

            stringBuilder.AppendFormat("}}{0}}}{0}", "\r\n");

            // Update() method
            stringBuilder.AppendFormat("public override void Update(ConnectorBase conn){0}{{{0}", "\r\n");

            bool hasModifiedBy = context.Columns.Find((DalColumn c) => c.Name == "ModifiedBy") != null;
            bool hasModifiedOn = context.Columns.Find((DalColumn c) => c.Name == "ModifiedOn") != null;

            if (context.AtomicUpdates && (hasModifiedBy || hasModifiedOn))
            {
                stringBuilder.AppendFormat(@"if (HasDirtyColumns()){0}{{{0}", "\r\n");
            }
            if (context.Columns.Find((DalColumn c) => c.Name == "ModifiedBy") != null)
            {
                stringBuilder.AppendFormat("ModifiedBy = base.CurrentSessionUserName;{0}", "\r\n");
            }
            if (context.Columns.Find((DalColumn c) => c.Name == "ModifiedOn") != null)
            {
                stringBuilder.AppendFormat("ModifiedOn = DateTime.UtcNow;{0}", "\r\n");
            }
            if (context.AtomicUpdates && (hasModifiedBy || hasModifiedOn))
            {
                stringBuilder.AppendFormat(@"}}{0}", "\r\n");
            }

            if (hasModifiedBy || hasModifiedOn)
            {
                stringBuilder.Append("\r\n");
            }
            if (!string.IsNullOrEmpty(context.CustomBeforeUpdate))
            {
                stringBuilder.AppendFormat("{1}{0}{0}", "\r\n", context.CustomBeforeUpdate);
            }

            stringBuilder.AppendFormat("Query qry = new Query(TableSchema);{0}", "\r\n");
            foreach (DalColumn dalCol in context.Columns)
            {
                if (dalCol.AutoIncrement || dalCol.NoSave)
                {
                    continue;
                }

                if (context.AtomicUpdates)
                {
                    stringBuilder.AppendFormat(@"if (IsColumnDirty(Columns.{1})){0}{{{0}", "\r\n", dalCol.Name);
                }

                if (string.IsNullOrEmpty(dalCol.ToDb))
                {
                    stringBuilder.AppendFormat("qry.Update(Columns.{1}, {1});{0}", "\r\n", dalCol.Name);
                }
                else
                {
                    stringBuilder.AppendFormat("qry.Update(Columns.{1}, {2});{0}", "\r\n", dalCol.Name, string.Format(dalCol.ToDb, dalCol.Name));
                }

                if (context.AtomicUpdates)
                {
                    stringBuilder.AppendFormat(@"}}{0}{0}", "\r\n", dalCol.Name);
                }
            }

            bool flag1 = true;
            foreach (DalColumn dalCol in primaryKeyColumns)
            {
                stringBuilder.AppendFormat("qry.{2}(Columns.{1}, {1});{0}", "\r\n", dalCol.Name, (flag1 ? "Where" : "AND"));
                flag1 = false;
            }

            stringBuilder.AppendFormat("{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("if (qry.HasInsertsOrUpdates){0}{{{0}", "\r\n");
            }
            stringBuilder.AppendFormat("qry.Execute(conn);{0}", "\r\n");
            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("}}{0}", "\r\n");
                stringBuilder.AppendFormat("{0}MarkAllColumnsNotDirty();{0}", "\r\n");
            }

            stringBuilder.AppendFormat("}}{0}{0}", "\r\n");


            // Read() method
            stringBuilder.AppendFormat("public override void Read(DataReaderBase reader){0}{{{0}", "\r\n");
            foreach (DalColumn dalCol in context.Columns)
            {
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
                else if (dalCol.Type != DalColumnType.TDateTime)
                {
                    if (dalCol.Type == DalColumnType.TLongText || dalCol.Type == DalColumnType.TMediumText || dalCol.Type == DalColumnType.TText || dalCol.Type == DalColumnType.TString || dalCol.Type == DalColumnType.TFixedString)
                    {
                        fromDb = (!dalCol.IsNullable ? "(string){0}" : "StringOrNullFromDb({0})");
                    }
                    else if (dalCol.Type == DalColumnType.TGeometry || dalCol.Type == DalColumnType.TGeometryCollection || dalCol.Type == DalColumnType.TPoint || dalCol.Type == DalColumnType.TLineString || dalCol.Type == DalColumnType.TPolygon || dalCol.Type == DalColumnType.TLine || dalCol.Type == DalColumnType.TCurve || dalCol.Type == DalColumnType.TSurface || dalCol.Type == DalColumnType.TLinearRing || dalCol.Type == DalColumnType.TMultiPoint || dalCol.Type == DalColumnType.TMultiLineString || dalCol.Type == DalColumnType.TMultiPolygon || dalCol.Type == DalColumnType.TMultiCurve || dalCol.Type == DalColumnType.TMultiSurface || dalCol.Type == DalColumnType.TGeographic || dalCol.Type == DalColumnType.TGeographicCollection || dalCol.Type == DalColumnType.TGeographicPoint || dalCol.Type == DalColumnType.TGeographicLineString || dalCol.Type == DalColumnType.TGeographicPolygon || dalCol.Type == DalColumnType.TGeographicLine || dalCol.Type == DalColumnType.TGeographicCurve || dalCol.Type == DalColumnType.TGeographicSurface || dalCol.Type == DalColumnType.TGeographicLinearRing || dalCol.Type == DalColumnType.TGeographicMultiPoint || dalCol.Type == DalColumnType.TGeographicMultiLineString || dalCol.Type == DalColumnType.TGeographicMultiPolygon || dalCol.Type == DalColumnType.TGeographicMultiCurve || dalCol.Type == DalColumnType.TGeographicMultiSurface)
                    {
                        fromReader = "reader.GetGeometry(Columns.{0}) as " + dalCol.ActualType;
                    }
                }
                else if (!dalCol.IsNullable)
                {
                    fromDb = "Convert.ToDateTime({0})";
                }
                else if (dalCol.DefaultValue == "DateTime.UtcNow")
                {
                    fromDb = "DateTimeOrNow({0})";
                }
                else if (dalCol.DefaultValue != "null")
                {
                    fromDb = (!dalCol.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDateTime({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDateTime({{0}})", dalCol.ActualType));
                }
                else
                {
                    fromDb = "DateTimeOrNullFromDb({0})";
                }
                if (!string.IsNullOrEmpty(dalCol.EnumTypeName))
                {
                    fromDb = "(" + dalCol.EnumTypeName + ")" + fromDb;
                }
                if (!string.IsNullOrEmpty(dalCol.FromDb))
                {
                    fromDb = dalCol.FromDb;
                }
                stringBuilder.AppendFormat("{1} = {2};{0}", "\r\n", dalCol.Name, string.Format(fromDb, string.Format(fromReader, dalCol.Name), dalCol.DefaultValue));
            }
            if (!string.IsNullOrEmpty(context.CustomAfterRead))
            {
                stringBuilder.AppendFormat("{0}{1}{0}", "\r\n", context.CustomAfterRead);
            }

            stringBuilder.AppendFormat("{0}MarkOld();{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("MarkAllColumnsNotDirty();{0}", "\r\n");
            }

            stringBuilder.AppendFormat("}}{0}", "\r\n");

            stringBuilder.AppendFormat("#endregion{0}", "\r\n");

            #endregion

            #region Helpers

            stringBuilder.AppendFormat("{0}#region Helpers{0}", "\r\n");
            if (primaryKeyColumns.Count > 0)
            {
                // FetchByID(...) function
                stringBuilder.AppendFormat("public static {1} FetchByID(", "\r\n", context.ClassName);
                bool first = true;
                foreach (DalColumn dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    stringBuilder.AppendFormat("{0} {1}", dalCol.ActualType, dalCol.Name);
                }
                stringBuilder.AppendFormat("){0}{{{0}", "\r\n");

                stringBuilder.AppendFormat("Query qry = new Query(TableSchema){0}", "\r\n");
                first = true;
                foreach (DalColumn dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {1})", "\r\n", dalCol.Name);
                    }
                    else
                    {
                        stringBuilder.AppendFormat(".Where(Columns.{0}, {0})", dalCol.Name);
                        first = false;
                    }
                }
                stringBuilder.AppendFormat(";{0}using (DataReaderBase reader = qry.ExecuteReader()){0}{{{0}if (reader.Read()){0}{{{0}{1} item = new {1}();{0}item.Read(reader);{0}return item;{0}}}{0}}}{0}return null;{0}}}{0}{0}", "\r\n", context.ClassName);

                // Delete() function
                stringBuilder.AppendFormat("public static int Delete(", "\r\n");
                first = true;
                foreach (DalColumn dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    stringBuilder.AppendFormat("{0} {1}", dalCol.ActualType, dalCol.Name);
                }
                stringBuilder.AppendFormat("){0}{{{0}", "\r\n");

                stringBuilder.AppendFormat("Query qry = new Query(TableSchema){0}", "\r\n");
                first = true;
                foreach (DalColumn dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {1})", "\r\n", dalCol.Name);
                    }
                    else
                    {
                        stringBuilder.AppendFormat(".Delete().Where(Columns.{0}, {0})", dalCol.Name);
                        first = false;
                    }
                }
                stringBuilder.AppendFormat(";{0}return qry.Execute();{0}}}{0}{0}", "\r\n");

                // FetchByID(..., ConnectorBase conn = null) function
                stringBuilder.AppendFormat("public static {1} FetchByID(", "\r\n", context.ClassName);
                first = true;
                foreach (DalColumn dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    stringBuilder.AppendFormat("{0} {1}", dalCol.ActualType, dalCol.Name);
                }
                stringBuilder.AppendFormat(", ConnectorBase conn = null){0}{{{0}", "\r\n");

                stringBuilder.AppendFormat("Query qry = new Query(TableSchema){0}", "\r\n");
                first = true;
                foreach (DalColumn dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {1})", "\r\n", dalCol.Name);
                    }
                    else
                    {
                        stringBuilder.AppendFormat(".Where(Columns.{0}, {0})", dalCol.Name);
                        first = false;
                    }
                }
                stringBuilder.AppendFormat(";{0}using (DataReaderBase reader = qry.ExecuteReader(conn)){0}{{{0}if (reader.Read()){0}{{{0}{1} item = new {1}();{0}item.Read(reader);{0}return item;{0}}}{0}}}{0}return null;{0}}}{0}{0}", "\r\n", context.ClassName);

                // Delete(..., ConnectorBase conn = null) function
                stringBuilder.AppendFormat("public static int Delete(", "\r\n");
                first = true;
                foreach (DalColumn dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    stringBuilder.AppendFormat("{0} {1}", dalCol.ActualType, dalCol.Name);
                }
                stringBuilder.AppendFormat(", ConnectorBase conn = null){0}{{{0}", "\r\n");

                stringBuilder.AppendFormat("Query qry = new Query(TableSchema){0}", "\r\n");
                first = true;
                foreach (DalColumn dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {1})", "\r\n", dalCol.Name);
                    }
                    else
                    {
                        stringBuilder.AppendFormat(".Delete().Where(Columns.{0}, {0})", dalCol.Name);
                        first = false;
                    }
                }
                stringBuilder.AppendFormat(";{0}return qry.Execute(conn);{0}}}{0}", "\r\n");
            }
            stringBuilder.AppendFormat("#endregion{0}", "\r\n");

            #endregion

            // End of class
            stringBuilder.Append("}");
        }

        private static void WriteSchemaAddColumnArguments(DalColumn dalCol, StringBuilder stringBuilder)
        {
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
            else if (dalCol.Type == DalColumnType.TDateTime)
            {
                dalCol.ActualType = "DateTime";
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
            else if (dalCol.Type == DalColumnType.TString || dalCol.Type == DalColumnType.TText || dalCol.Type == DalColumnType.TLongText || dalCol.Type == DalColumnType.TMediumText || dalCol.Type == DalColumnType.TFixedString)
            {
                dalCol.ActualType = "string";
            }
            else if (dalCol.Type == DalColumnType.TDecimal || dalCol.Type == DalColumnType.TMoney)
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
            else if (dalCol.Type == DalColumnType.TGeometry)
            {
                dalCol.ActualType = "Geometry";
            }
            else if (dalCol.Type == DalColumnType.TGeometryCollection)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
            }
            else if (dalCol.Type == DalColumnType.TPoint)
            {
                dalCol.ActualType = "Geometry.Point";
            }
            else if (dalCol.Type == DalColumnType.TLineString)
            {
                dalCol.ActualType = "Geometry.LineString";
            }
            else if (dalCol.Type == DalColumnType.TPolygon)
            {
                dalCol.ActualType = "Geometry.Polygon";
            }
            else if (dalCol.Type == DalColumnType.TLine)
            {
                dalCol.ActualType = "Geometry.Line";
            }
            else if (dalCol.Type == DalColumnType.TCurve)
            {
                dalCol.ActualType = "Geometry";
            }
            else if (dalCol.Type == DalColumnType.TSurface)
            {
                dalCol.ActualType = "Geometry";
            }
            else if (dalCol.Type == DalColumnType.TLinearRing)
            {
                dalCol.ActualType = "Geometry";
            }
            else if (dalCol.Type == DalColumnType.TMultiPoint)
            {
                dalCol.ActualType = "Geometry.MultiPoint";
            }
            else if (dalCol.Type == DalColumnType.TMultiLineString)
            {
                dalCol.ActualType = "Geometry.MultiLineString";
            }
            else if (dalCol.Type == DalColumnType.TMultiPolygon)
            {
                dalCol.ActualType = "Geometry.MultiPolygon";
            }
            else if (dalCol.Type == DalColumnType.TMultiCurve)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
            }
            else if (dalCol.Type == DalColumnType.TMultiSurface)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
            }
            else if (dalCol.Type == DalColumnType.TGeographic)
            {
                dalCol.ActualType = "Geometry";
            }
            else if (dalCol.Type == DalColumnType.TGeographicCollection)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
            }
            else if (dalCol.Type == DalColumnType.TGeographicPoint)
            {
                dalCol.ActualType = "Geometry.Point";
            }
            else if (dalCol.Type == DalColumnType.TGeographicLineString)
            {
                dalCol.ActualType = "Geometry.LineString";
            }
            else if (dalCol.Type == DalColumnType.TGeographicPolygon)
            {
                dalCol.ActualType = "Geometry.Polygon";
            }
            else if (dalCol.Type == DalColumnType.TGeographicLine)
            {
                dalCol.ActualType = "Geometry.Line";
            }
            else if (dalCol.Type == DalColumnType.TGeographicCurve)
            {
                dalCol.ActualType = "Geometry";
            }
            else if (dalCol.Type == DalColumnType.TGeographicSurface)
            {
                dalCol.ActualType = "Geometry";
            }
            else if (dalCol.Type == DalColumnType.TGeographicLinearRing)
            {
                dalCol.ActualType = "Geometry";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiPoint)
            {
                dalCol.ActualType = "Geometry.MultiPoint";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiLineString)
            {
                dalCol.ActualType = "Geometry.MultiLineString";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiPolygon)
            {
                dalCol.ActualType = "Geometry.MultiPolygon";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiCurve)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiSurface)
            {
                dalCol.ActualType = "Geometry.GeometryCollection";
            }
            else if (dalCol.Type == DalColumnType.TLiteral)
            {
                // Do not change it, specified by ACTUALTYPE
            }

            stringBuilder.AppendFormat("Columns.{0}, typeof({1})", dalCol.Name, dalCol.ActualType);

            if (dalCol.Type == DalColumnType.TText)
            {
                stringBuilder.Append(", DataType.Text");
            }
            else if (dalCol.Type == DalColumnType.TLongText)
            {
                stringBuilder.Append(", DataType.LongText");
            }
            else if (dalCol.Type == DalColumnType.TMediumText)
            {
                stringBuilder.Append(", DataType.MediumText");
            }
            else if (dalCol.Type == DalColumnType.TFixedString)
            {
                stringBuilder.Append(", DataType.Char");
            }
            else if (dalCol.Type == DalColumnType.TMoney)
            {
                stringBuilder.Append(", DataType.Money");
            }
            else if (dalCol.Type == DalColumnType.TGeometry)
            {
                stringBuilder.Append(", DataType.Geometry");
            }
            else if (dalCol.Type == DalColumnType.TGeometryCollection)
            {
                stringBuilder.Append(", DataType.GeometryCollection");
            }
            else if (dalCol.Type == DalColumnType.TPoint)
            {
                stringBuilder.Append(", DataType.Point");
            }
            else if (dalCol.Type == DalColumnType.TLineString)
            {
                stringBuilder.Append(", DataType.LineString");
            }
            else if (dalCol.Type == DalColumnType.TPolygon)
            {
                stringBuilder.Append(", DataType.Polygon");
            }
            else if (dalCol.Type == DalColumnType.TLine)
            {
                stringBuilder.Append(", DataType.Line");
            }
            else if (dalCol.Type == DalColumnType.TCurve)
            {
                stringBuilder.Append(", DataType.Curve");
            }
            else if (dalCol.Type == DalColumnType.TSurface)
            {
                stringBuilder.Append(", DataType.Surface");
            }
            else if (dalCol.Type == DalColumnType.TLinearRing)
            {
                stringBuilder.Append(", DataType.LinearRing");
            }
            else if (dalCol.Type == DalColumnType.TMultiPoint)
            {
                stringBuilder.Append(", DataType.MultiPoint");
            }
            else if (dalCol.Type == DalColumnType.TMultiLineString)
            {
                stringBuilder.Append(", DataType.MultiLineString");
            }
            else if (dalCol.Type == DalColumnType.TMultiPolygon)
            {
                stringBuilder.Append(", DataType.MultiPolygon");
            }
            else if (dalCol.Type == DalColumnType.TMultiCurve)
            {
                stringBuilder.Append(", DataType.MultiCurve");
            }
            else if (dalCol.Type == DalColumnType.TMultiSurface)
            {
                stringBuilder.Append(", DataType.MultiSurface");
            }
            else if (dalCol.Type == DalColumnType.TGeographic)
            {
                stringBuilder.Append(", DataType.Geographic");
            }
            else if (dalCol.Type == DalColumnType.TGeographicCollection)
            {
                stringBuilder.Append(", DataType.GeographicCollection");
            }
            else if (dalCol.Type == DalColumnType.TGeographicPoint)
            {
                stringBuilder.Append(", DataType.GeographicPoint");
            }
            else if (dalCol.Type == DalColumnType.TGeographicLineString)
            {
                stringBuilder.Append(", DataType.GeographicLineString");
            }
            else if (dalCol.Type == DalColumnType.TGeographicPolygon)
            {
                stringBuilder.Append(", DataType.GeographicPolygon");
            }
            else if (dalCol.Type == DalColumnType.TGeographicLine)
            {
                stringBuilder.Append(", DataType.GeographicLine");
            }
            else if (dalCol.Type == DalColumnType.TGeographicCurve)
            {
                stringBuilder.Append(", DataType.GeographicCurve");
            }
            else if (dalCol.Type == DalColumnType.TGeographicSurface)
            {
                stringBuilder.Append(", DataType.GeographicSurface");
            }
            else if (dalCol.Type == DalColumnType.TGeographicLinearRing)
            {
                stringBuilder.Append(", DataType.GeographicLinearRing");
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiPoint)
            {
                stringBuilder.Append(", DataType.GeographicMultiPoint");
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiLineString)
            {
                stringBuilder.Append(", DataType.GeographicMultiLineString");
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiPolygon)
            {
                stringBuilder.Append(", DataType.GeographicMultiPolygon");
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiCurve)
            {
                stringBuilder.Append(", DataType.GeographicMultiCurve");
            }
            else if (dalCol.Type == DalColumnType.TGeographicMultiSurface)
            {
                stringBuilder.Append(", DataType.GeographicMultiSurface");
            }
            else if (!string.IsNullOrEmpty(dalCol.EnumTypeName))
            {
                if (dalCol.Type == DalColumnType.TInt8)
                {
                    stringBuilder.Append(", DataType.TinyInt");
                }
                else if (dalCol.Type == DalColumnType.TInt16)
                {
                    stringBuilder.Append(", DataType.SmallInt");
                }
                else if (dalCol.Type == DalColumnType.TInt32)
                {
                    stringBuilder.Append(", DataType.Int");
                }
                else if (dalCol.Type == DalColumnType.TInt64)
                {
                    stringBuilder.Append(", DataType.BigInt");
                }
                else if (dalCol.Type == DalColumnType.TUInt8)
                {
                    stringBuilder.Append(", DataType.UnsignedTinyInt");
                }
                else if (dalCol.Type == DalColumnType.TUInt16)
                {
                    stringBuilder.Append(", DataType.UnsignedSmallInt");
                }
                else if (dalCol.Type == DalColumnType.TUInt32)
                {
                    stringBuilder.Append(", DataType.UnsignedInt");
                }
                else if (dalCol.Type == DalColumnType.TUInt64)
                {
                    stringBuilder.Append(", DataType.UnsignedBigInt");
                }
            }
            
            if (!string.IsNullOrEmpty(customActualType))
            {
                dalCol.ActualType = customActualType;
            }
            else if (dalCol.IsNullable && dalCol.ActualType != "string")
            {
                dalCol.ActualType += "?";
            }

            stringBuilder.AppendFormat(", {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                dalCol.MaxLength,
                string.IsNullOrEmpty(dalCol.LiteralType) ? "null" : (@"""" + dalCol.LiteralType.Replace(@"""", @"""""") + @""""),
                dalCol.Precision,
                dalCol.Scale,
                dalCol.AutoIncrement ? "true" : "false",
                dalCol.IsPrimaryKey ? "true" : "false",
                dalCol.IsNullable ? "true" : "false",
                dalCol.DefaultValue);

            if (!string.IsNullOrEmpty(dalCol.Charset) || !string.IsNullOrEmpty(dalCol.Collate))
            {
                stringBuilder.AppendFormat(@", {0}, {1}",
                    string.IsNullOrEmpty(dalCol.Charset) ? "null" : (@"""" + dalCol.Charset + @""""),
                    string.IsNullOrEmpty(dalCol.Collate) ? "null" : (@"""" + dalCol.Collate + @""""));
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
                DalColumn dalCol = context.Columns.Find((DalColumn c) => c.Name == indexColumn.Name);
                string col = (dalCol == null ? string.Format("\"{0}\"", indexColumn.Name) : string.Format("Columns.{0}", dalCol.Name));
                stringBuilder.AppendFormat(", {0}", col);
                if (string.IsNullOrEmpty(indexColumn.SortDirection))
                {
                    continue;
                }
                stringBuilder.AppendFormat(", SortDirection.{0}", indexColumn.SortDirection);
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
                stringBuilder.AppendFormat("{0}.TableSchema.SchemaName, ", dalFK.ForeignTable);
            }
            else
            {
                stringBuilder.Append("schema.SchemaName, ");
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

        public static string StripColumnName(string columnName)
        {
            columnName = columnName.Trim();
            while (columnName.Length > 0 && !Regex.IsMatch(columnName, @"^[a-zA-Z_]")) columnName = columnName.Remove(0, 1);
            columnName = Regex.Replace(columnName, @"[^a-zA-Z_0-9]", @"");
            return columnName;
        }
	}

    public class ScriptContext
    {
        public string[] ScriptLines;

        public string ClassName = null;
        public string SchemaName = null;
        public string DatabaseOwner = null;

        public List<DalColumn> Columns = new List<DalColumn>();
        public List<DalIndex> Indices = new List<DalIndex>();
        public List<DalForeignKey> ForeignKeys = new List<DalForeignKey>();
        public List<DalEnum> Enums = new List<DalEnum>();

        public bool StaticColumns = false;
        public bool ExportRecord = true;
        public bool ExportCollection = true;
        public bool AtomicUpdates = false;
        public bool InsertAutoIncrement = false;

        public string SingleColumnPrimaryKeyName = null;
        public string CustomBeforeInsert = null;
        public string CustomBeforeUpdate = null;
        public string CustomAfterRead = null;
        public string MySqlEngineName = "";
    }
}