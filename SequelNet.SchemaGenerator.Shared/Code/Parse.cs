using Codenet.Text;
using System;
using System.Collections.Generic;
using System.Linq;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
    {

        private static void ParseScript(ScriptContext context, string[] scriptLines)
        {
            context.ClassName = scriptLines[0].Trim(new char[] { ' ', '*', '\t' });
            context.SchemaName = scriptLines[1].Trim(new char[] { ' ', '*', '\t' });

            if (context.SchemaName.Contains("."))
            {
                context.DatabaseOwner = context.SchemaName.Substring(0, context.SchemaName.IndexOf(".")).Trim();
                context.SchemaName = context.SchemaName.Substring(context.SchemaName.IndexOf(".") + 1).Trim();
            }

            for (int i = 2; i < scriptLines.Length; i++)
            {
                string currentLine = scriptLines[i];
                while (currentLine.EndsWith("\\") && i + 1 < scriptLines.Length)
                {
                    i++;
                    currentLine = currentLine.Substring(0, currentLine.Length - 1).Trim(new char[] { ' ', '*', '\t' }) + scriptLines[i].Trim(new char[] { ' ', '*', '\t' });
                }

                string currentLineTrimmed = currentLine.Trim(new char[] { ' ', '*', '\t' });

                if (currentLineTrimmed.StartsWith("@Index:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] indexArguments = currentLineTrimmed.Substring(7)
                        .SplitWithEscape(';', '\\')
                        .Select(x => x?.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToArray();

                    DalIndex dalIndex = new DalIndex();
                    for (int j = 0; j <= (int)indexArguments.Length - 1; j++)
                    {
                        string arg = indexArguments[j].Trim();

                        if (arg.StartsWith("NAME(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalIndex.IndexName = arg.Substring(5, arg.IndexOf(")") - 5).Trim();
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
                            var columns = arg
                                .Trim(new char[] { ' ', '[', ']', '\t' })
                                .SplitWithEscape(',', '\\')
                                .Select(x => x?.Trim())
                                .Where(x => !string.IsNullOrEmpty(x))
                                .ToArray();

                            foreach (string column in columns)
                            {
                                string columnName = column;
                                string direction = null;

                                if (column.EndsWith(" ASC") || column.EndsWith(" DESC"))
                                {
                                    direction = column.Substring(column.LastIndexOf(' ') + 1);
                                    columnName = column.Substring(0, column.LastIndexOf(' ')).Trim();
                                }

                                dalIndex.Columns.Add(new DalIndexColumn(
                                    columnName,
                                    columnName.Contains("."),
                                    direction));
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
                            dalForeignKey.ForeignKeyName = arg.Substring(5, arg.IndexOf(")") - 5).Trim();
                        }
                        else if (arg.StartsWith("FOREIGNTABLE(", StringComparison.OrdinalIgnoreCase))
                        {
                            dalForeignKey.ForeignTable = arg.Substring(13, arg.IndexOf(")") - 13).Trim();
                        }
                        else if (arg.StartsWith("ONUPDATE(", StringComparison.OrdinalIgnoreCase))
                        {
                            switch ((arg.Substring(9, arg.IndexOf(")") - 9)).ToUpper().Trim())
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
                            switch ((arg.Substring(9, arg.IndexOf(")") - 9)).ToUpper().Trim())
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
                                dalForeignKey.Columns.Add(strArrays[k].Trim());
                            }
                        }
                        else if (arg.StartsWith("FOREIGNCOLUMNS[", StringComparison.OrdinalIgnoreCase))
                        {
                            string columns = arg.Substring(14).Trim(new char[] { ' ', '[', ']', '\t' });
                            string[] strArrays = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int k = 0; k < strArrays.Length; k++)
                            {
                                dalForeignKey.ForeignColumns.Add(strArrays[k].Trim());
                            }
                        }
                    }
                    context.ForeignKeys.Add(dalForeignKey);
                }
                else if (currentLineTrimmed.StartsWith("@BeforeInsert:", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(context.CustomBeforeInsert))
                        context.CustomBeforeInsert = context.CustomBeforeInsert + "\n";
                    context.CustomBeforeInsert = (context.CustomBeforeInsert ?? "") + currentLineTrimmed.Substring(14).Trim();
                }
                else if (currentLineTrimmed.StartsWith("@AfterInsertQuery:", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(context.CustomAfterInsertQuery))
                        context.CustomAfterInsertQuery = context.CustomAfterInsertQuery + "\n";
                    context.CustomAfterInsertQuery = (context.CustomAfterInsertQuery ?? "") + currentLineTrimmed.Substring(18).Trim();
                }
                else if (currentLineTrimmed.StartsWith("@BeforeUpdate:", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(context.CustomBeforeUpdate))
                        context.CustomBeforeUpdate = context.CustomBeforeUpdate + "\n";
                    context.CustomBeforeUpdate = (context.CustomBeforeUpdate ?? "") + currentLineTrimmed.Substring(14).Trim();
                }
                else if (currentLineTrimmed.StartsWith("@AfterUpdateQuery:", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(context.CustomAfterUpdateQuery))
                        context.CustomAfterUpdateQuery = context.CustomAfterUpdateQuery + "\n";
                    context.CustomAfterUpdateQuery = (context.CustomAfterUpdateQuery ?? "") + currentLineTrimmed.Substring(18).Trim();
                }
                else if (currentLineTrimmed.StartsWith("@AfterRead:", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(context.CustomAfterRead))
                        context.CustomAfterRead = context.CustomAfterRead + "\n";
                    context.CustomAfterRead = (context.CustomAfterRead ?? "") + currentLineTrimmed.Substring(11).Trim();
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
                else if (currentLineTrimmed.StartsWith("@SnakeColumnNames", StringComparison.OrdinalIgnoreCase))
                {
                    context.SnakeColumnNames = true;
                }
                else if (currentLineTrimmed.StartsWith("@NullableEnabled", StringComparison.OrdinalIgnoreCase) ||
                    currentLineTrimmed.StartsWith("@NullableEnable", StringComparison.OrdinalIgnoreCase))
                {
                    context.NullableEnabled = true;
                }
                else if (currentLineTrimmed.StartsWith("@InsertAutoIncrement", StringComparison.OrdinalIgnoreCase))
                {
                    context.InsertAutoIncrement = true;
                }
                else if (currentLineTrimmed.StartsWith("@NoCreatedOn", StringComparison.OrdinalIgnoreCase))
                {
                    context.NoCreatedOn = true;
                }
                else if (currentLineTrimmed.StartsWith("@NoModifiedOn", StringComparison.OrdinalIgnoreCase))
                {
                    context.NoModifiedOn = true;
                }
                else if (currentLineTrimmed.StartsWith("@MySqlEngine:", StringComparison.OrdinalIgnoreCase))
                {
                    context.MySqlEngineName = currentLineTrimmed.Substring(13).Trim();
                }
                else if (currentLineTrimmed.StartsWith("@ComponentModel", StringComparison.OrdinalIgnoreCase))
                {
                    context.ComponentModel = true;
                }
                else
                {
                    int startPos = currentLineTrimmed.IndexOf(":");
                    DalColumn dalColumn = new DalColumn();
                    dalColumn.Name = currentLineTrimmed.Substring(0, startPos).Trim();
                    dalColumn.PropertyName = StripColumnName(dalColumn.Name);

                    if (context.ClassName == dalColumn.PropertyName || dalColumn.PropertyName == "Columns")
                    {
                        dalColumn.PropertyName += "X";
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
                                (int)scriptLines.Length <= i + 2 ||
                                !scriptLines[i + 1].Trim(new char[] { ' ', '*', '\t' }).StartsWith("\"") ||
                                !scriptLines[i + 2].Trim(new char[] { ' ', '*', '\t' }).StartsWith("-"))
                            {
                                dalColumn.Comment = columnKeyword;
                            }
                            else
                            {
                                dalColumn.Comment = columnKeyword.Remove(columnKeyword.Length - 1, 1);
                                i++;
                                currentLineTrimmed = scriptLines[i];
                                DalEnum dalEnum = new DalEnum();
                                dalEnum.Name = currentLineTrimmed.Trim(new char[] { ' ', '*', '\"', '\t' });
                                dalColumn.EnumTypeName = dalEnum.Name;
                                dalEnum.Items = new List<string>();
                                while ((int)scriptLines.Length > i + 1 &&
                                    scriptLines[i + 1].Trim(new char[] { ' ', '*', '\t' }).StartsWith("-"))
                                {
                                    i++;
                                    currentLineTrimmed = scriptLines[i].Trim(new char[] { ' ', '*', '-', '\t' });
                                    dalEnum.Items.Add(currentLineTrimmed);
                                }
                                context.Enums.Add(dalEnum);
                            }
                        }
                        else if (columnKeyword.Equals("PRIMARY KEY", StringComparison.OrdinalIgnoreCase) ||
                            columnKeyword.Equals("PRIMARYKEY", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.IsPrimaryKey = true;
                            context.SingleColumnPrimaryKeyName = (context.SingleColumnPrimaryKeyName != null ? "" : dalColumn.PropertyName);
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
                        else if (columnKeyword.Equals("NoRead", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.NoRead = true;
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
                            dalColumn.Type = DalColumnType.TFixedString;
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
                        else if (columnKeyword.Equals("JSON", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TJson;
                        }
                        else if (columnKeyword.Equals("JSON_BINARY", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TJsonBinary;
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
                        else if (columnKeyword.Equals("DATETIME_UTC", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TDateTimeUtc;
                        }
                        else if (columnKeyword.Equals("DATETIME_LOCAL", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TDateTimeLocal;
                        }
                        else if (columnKeyword.Equals("DATETIMEOFFSET", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TDateTimeOffset;
                        }
                        else if (columnKeyword.Equals("DATE", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TDate;
                        }
                        else if (columnKeyword.Equals("TIME", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Type = DalColumnType.TTime;
                        }
                        else if (columnKeyword.StartsWith("Default ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.DefaultValue = columnKeyword.Substring(8).Trim();
                            dalColumn.HasDefault = !string.IsNullOrEmpty(dalColumn.DefaultValue);
                        }
                        else if (columnKeyword.StartsWith("ActualDefault ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.ActualDefaultValue = columnKeyword.Substring(14).Trim();
                        }
                        else if (columnKeyword.StartsWith("ToDB ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.ToDb = columnKeyword.Substring(5).Trim();
                        }
                        else if (columnKeyword.Equals("VirtualProp", StringComparison.OrdinalIgnoreCase) ||
                            /* deprecated */ columnKeyword.Equals("Virtual", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.VirtualProp = true;
                        }
                        else if (columnKeyword.StartsWith("FromDB ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.FromDb = columnKeyword.Substring(7).Trim();
                        }
                        else if (columnKeyword.StartsWith("ActualType ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.ActualType = columnKeyword.Substring(11).Trim();
                        }
                        else if (columnKeyword.StartsWith("Computed ", StringComparison.OrdinalIgnoreCase))
                        {
                            var computed = columnKeyword.Substring(9).Trim();
                            var isStored = computed.EndsWith(" STORED", StringComparison.OrdinalIgnoreCase);
                            if (isStored)
                            {
                                computed = computed.Remove(computed.Length - 7, 7);
                            }

                            dalColumn.Computed = ProcessComputedColumn(computed);
                            dalColumn.ComputedStored = isStored;
                            dalColumn.NoSave = true;
                        }
                        else if (columnKeyword.StartsWith("ColumnName ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.HasCustomName = true;
                            dalColumn.Name = columnKeyword.Substring(11).Trim();
                        }
                        else if (columnKeyword.StartsWith("PropertyName ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.PropertyName = columnKeyword.Substring(13).Trim();
                        }
                        else if (columnKeyword.Equals("Unique Index", StringComparison.OrdinalIgnoreCase) ||
                            columnKeyword.Equals("Unique", StringComparison.OrdinalIgnoreCase))
                        {
                            DalIndex dalIx = new DalIndex();
                            dalIx.Columns.Add(new DalIndexColumn(dalColumn.Name, false, null));
                            dalIx.IndexMode = DalIndexIndexMode.Unique;
                            context.Indices.Add(dalIx);
                        }
                        else if (columnKeyword.StartsWith("Foreign ", StringComparison.OrdinalIgnoreCase))
                        {
                            DalForeignKey dalFk = new DalForeignKey();
                            string str30 = columnKeyword.Substring(8).Trim();
                            dalFk.ForeignTable = str30.Substring(0, str30.IndexOf(".")).Trim();
                            dalFk.ForeignColumns.Add(str30.Substring(str30.IndexOf(".") + 1).Trim());
                            dalFk.Columns.Add(dalColumn.Name);
                            context.ForeignKeys.Add(dalFk);
                        }
                        else if (columnKeyword.StartsWith("IsMutatedProperty ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.IsMutatedProperty = columnKeyword.Substring(18).Trim();
                        }
                        else if (columnKeyword.StartsWith("Charset ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Charset = columnKeyword.Substring(8).Trim();
                        }
                        else if (columnKeyword.StartsWith("Collate ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.Collate = columnKeyword.Substring(8).Trim();
                        }
                        else if (columnKeyword.StartsWith("SRID ", StringComparison.OrdinalIgnoreCase))
                        {
                            dalColumn.SRID = Convert.ToInt32(columnKeyword.Substring(5).Trim());
                        }
                    }
                    if (dalColumn.IsPrimaryKey & dalColumn.Type == DalColumnType.TInt)
                    {
                        dalColumn.Type = DalColumnType.TInt64;
                    }
                    context.Columns.Add(dalColumn);
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
    }
}