using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace dg.Sql.SchemaGeneratorAddIn
{
	public class SchemaGenerator
	{
		public SchemaGenerator()
		{
		}

		public static void GenerateDalClass(DTE2 application)
		{
			Document activeDocument = application.ActiveDocument;
			TextDocument textDocument = activeDocument.Object() as TextDocument;
			string selectionText = textDocument.Selection.Text;
            
			string className = null;
			string schemaName = null;
			string databaseOwner = null;

			List<DalColumn> dalColumns = new List<DalColumn>();
			List<DalIndex> dalIndices = new List<DalIndex>();
			List<DalForeignKey> dalForeignKeys = new List<DalForeignKey>();
			List<DalEnum> dalEnums = new List<DalEnum>();

            bool staticColumns = false;

            object[] maxLength;
			string singleColumnPrimaryKeyName = null;
			string str5 = null;
			string str6 = null;
			string str7 = null;
			string mySqlEngineName = "";
			
			string[] splitLines = selectionText.Trim(new char[] { ' ', '*', '/', '\t', '\r', '\n' }).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			className = splitLines[0].Trim(new char[] { ' ', '*', '\t' });
			schemaName = splitLines[1].Trim(new char[] { ' ', '*', '\t' });
			if (schemaName.Contains("."))
			{
				databaseOwner = schemaName.Substring(0, schemaName.IndexOf("."));
				schemaName = schemaName.Substring(schemaName.IndexOf(".") + 1);
			}
			for (int i = 2; i <= (int)splitLines.Length - 1; i++)
			{
				string currentLine = splitLines[i];
				string currentLineTrimmed = currentLine.Trim(new char[] { ' ', '*', '\t' });
				string currentLineTrimmedUpper = currentLineTrimmed.ToUpper();
				if (currentLineTrimmedUpper.StartsWith("@INDEX:", StringComparison.Ordinal))
				{
					string str14 = currentLineTrimmed.Substring(7);
					string[] strArrays2 = str14.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					DalIndex dalIndex = new DalIndex();
					for (int j = 0; j <= (int)strArrays2.Length - 1; j++)
					{
						string str15 = strArrays2[j].Trim();
						if (str15.ToUpper().StartsWith("NAME(", StringComparison.Ordinal))
						{
							dalIndex.IndexName = str15.Substring(5, str15.IndexOf(")") - 5);
						}
						else if (str15.ToUpper().Equals("UNIQUE", StringComparison.Ordinal))
						{
							dalIndex.IndexMode = DalIndexIndexMode.Unique;
						}
                        else if (str15.ToUpper().Equals("PRIMARY KEY", StringComparison.Ordinal) || str15.ToUpper().Equals("PRIMARYKEY", StringComparison.Ordinal))
						{
							dalIndex.IndexMode = DalIndexIndexMode.PrimaryKey;
						}
						else if (str15.ToUpper().Equals("SPATIAL", StringComparison.Ordinal))
						{
							dalIndex.IndexMode = DalIndexIndexMode.Spatial;
						}
						else if (str15.ToUpper().Equals("FULLTEXT", StringComparison.Ordinal))
						{
							dalIndex.IndexMode = DalIndexIndexMode.FullText;
						}
						else if (str15.ToUpper().Equals("BTREE", StringComparison.Ordinal))
						{
							dalIndex.IndexType = DalIndexIndexType.BTREE;
						}
						else if (str15.ToUpper().Equals("RTREE", StringComparison.Ordinal))
						{
							dalIndex.IndexType = DalIndexIndexType.RTREE;
						}
						else if (str15.ToUpper().Equals("HASH", StringComparison.Ordinal))
						{
							dalIndex.IndexType = DalIndexIndexType.HASH;
						}
						else if (str15.ToUpper().Equals("NONCLUSTERED", StringComparison.Ordinal))
						{
							dalIndex.ClusterMode = DalIndexClusterMode.NonClustered;
						}
						else if (str15.ToUpper().Equals("CLUSTERED", StringComparison.Ordinal))
						{
							dalIndex.ClusterMode = DalIndexClusterMode.Clustered;
						}
						else if (str15.ToUpper().StartsWith("[", StringComparison.Ordinal))
						{
                            string[] columns = str15
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
                        singleColumnPrimaryKeyName = (singleColumnPrimaryKeyName != null ? "" : dalIndex.Columns[0].Name);
                    }
					dalIndices.Add(dalIndex);
				}
				else if (currentLineTrimmedUpper.StartsWith("@FOREIGNKEY:", StringComparison.Ordinal))
				{
					string str18 = currentLineTrimmed.Substring(12);
					string[] strArrays3 = str18.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					DalForeignKey dalForeignKey = new DalForeignKey();
					for (int l = 0; l <= (int)strArrays3.Length - 1; l++)
					{
						string str19 = strArrays3[l].Trim();
						string upper1 = str19.ToUpper();
						if (upper1.StartsWith("NAME(", StringComparison.Ordinal))
						{
							dalForeignKey.ForeignKeyName = str19.Substring(5, str19.IndexOf(")") - 5);
						}
						else if (upper1.StartsWith("FOREIGNTABLE(", StringComparison.Ordinal))
						{
							dalForeignKey.ForeignTable = str19.Substring(13, str19.IndexOf(")") - 13);
						}
						else if (upper1.StartsWith("ONUPDATE(", StringComparison.Ordinal))
						{
                            switch ((str19.Substring(9, str19.IndexOf(")") - 9)).ToUpper())
                            {
	                            case "RESTRICT":
		                            dalForeignKey.OnUpdate = DalForeignKeyReference.Restrict;
		                            break;
	                            case "CASCADE":
		                            dalForeignKey.OnUpdate = DalForeignKeyReference.Cascade;
		                            break;
	                            case "SETNULL":
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
						else if (upper1.StartsWith("ONDELETE(", StringComparison.Ordinal))
						{
                            switch ((str19.Substring(9, str19.IndexOf(")") - 9)).ToUpper())
                            {
	                            case "RESTRICT":
		                            dalForeignKey.OnDelete = DalForeignKeyReference.Restrict;
		                            break;
	                            case "CASCADE":
		                            dalForeignKey.OnDelete = DalForeignKeyReference.Cascade;
		                            break;
	                            case "SETNULL":
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
						else if (upper1.StartsWith("COLUMNS[", StringComparison.Ordinal))
						{
							string str20 = str19.Substring(7);
							string str21 = str20.Trim(new char[] { ' ', '[', ']', '\t' });
                            string[] strArrays = str21.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							for (int k = 0; k < strArrays.Length; k++)
							{
								string str22 = strArrays[k];
								dalForeignKey.Columns.Add(str22);
							}
						}
						else if (upper1.StartsWith("FOREIGNCOLUMNS[", StringComparison.Ordinal))
						{
							string str23 = str19.Substring(14);
							string str24 = str23.Trim(new char[] { ' ', '[', ']', '\t' });
							string[] strArrays = str24.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							for (int k = 0; k < strArrays.Length; k++)
							{
								string str25 = strArrays[k];
								dalForeignKey.ForeignColumns.Add(str25);
							}
						}
					}
					dalForeignKeys.Add(dalForeignKey);
				}
				else if (currentLineTrimmedUpper.StartsWith("@BEFOREINSERT:", StringComparison.Ordinal))
				{
					str5 = currentLineTrimmed.Substring(14).Trim();
				}
				else if (currentLineTrimmedUpper.StartsWith("@BEFOREUPDATE:", StringComparison.Ordinal))
				{
					str6 = currentLineTrimmed.Substring(14).Trim();
				}
				else if (currentLineTrimmedUpper.StartsWith("@AFTERREAD:", StringComparison.Ordinal))
				{
					str7 = currentLineTrimmed.Substring(11).Trim();
                }
                else if (currentLineTrimmedUpper.StartsWith("@STATICCOLUMNS", StringComparison.Ordinal))
                {
                    staticColumns = true;
                }
				else if (!currentLineTrimmedUpper.StartsWith("@MYSQLENGINE:", StringComparison.Ordinal))
				{
					int startPos = currentLineTrimmed.IndexOf(":");
					DalColumn dalColumn = new DalColumn();
					dalColumn.Name = currentLineTrimmed.Substring(0, startPos).Trim();
					dalColumn.NameX = StripColumnName(dalColumn.Name);
					if (className == dalColumn.Name || dalColumn.Name == "Columns")
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
						string columnKeywordUpper = columnKeyword.ToUpper();
						if (m == (int)columnKeywords.Length - 1)
						{
							if (!columnKeyword.EndsWith(":") || (int)splitLines.Length <= i + 2 || !splitLines[i + 1].Trim(new char[] { ' ', '*', '\t' }).StartsWith("\"") || !splitLines[i + 2].Trim(new char[] { ' ', '*', '\t' }).StartsWith("-"))
							{
								dalColumn.Comment = columnKeyword;
							}
							else
							{
								dalColumn.Comment = columnKeyword.Remove(columnKeyword.Length - 1, 1);
								i++;
								currentLineTrimmed = splitLines[i];
								DalEnum dalEnum = new DalEnum();
								dalEnum.Name = currentLineTrimmed.Trim(new char[] { ' ', '*', '\"', '\t' });
								dalColumn.EnumTypeName = dalEnum.Name;
								dalEnum.Items = new List<string>();
								while ((int)splitLines.Length > i + 1 && splitLines[i + 1].Trim(new char[] { ' ', '*', '\t' }).StartsWith("-"))
								{
									i++;
									string str27 = splitLines[i];
									currentLineTrimmed = str27.Trim(new char[] { ' ', '*', '-', '\t' });
									dalEnum.Items.Add(currentLineTrimmed);
								}
								dalEnums.Add(dalEnum);
							}
						}
                        else if (columnKeywordUpper.Equals("PRIMARY KEY", StringComparison.Ordinal) || columnKeywordUpper.Equals("PRIMARYKEY", StringComparison.Ordinal))
						{
							dalColumn.IsPrimaryKey = true;
							singleColumnPrimaryKeyName = (singleColumnPrimaryKeyName != null ? "" : dalColumn.Name);
						}
						else if (columnKeywordUpper.Equals("NULLABLE", StringComparison.Ordinal))
						{
							dalColumn.IsNullable = true;
						}
						else if (columnKeywordUpper.Equals("AUTOINCREMENT", StringComparison.Ordinal))
						{
							dalColumn.AutoIncrement = true;
						}
						else if (columnKeywordUpper.Equals("NOPROPERTY", StringComparison.Ordinal))
						{
							dalColumn.NoProperty = true;
						}
						else if (columnKeywordUpper.StartsWith("PRECISION(", StringComparison.Ordinal))
						{
							int num1 = 0;
							int.TryParse(columnKeyword.Substring(10, columnKeyword.IndexOf(")") - 10), out num1);
							dalColumn.Precision = num1;
						}
						else if (columnKeywordUpper.StartsWith("SCALE(", StringComparison.Ordinal))
						{
							int num2 = 0;
							int.TryParse(columnKeyword.Substring(6, columnKeyword.IndexOf(")") - 6), out num2);
							dalColumn.Scale = num2;
						}
						else if (columnKeywordUpper.StartsWith("STRING(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TString;
							int num3 = 0;
							int.TryParse(columnKeyword.Substring(7, columnKeyword.IndexOf(")") - 7), out num3);
							dalColumn.MaxLength = num3;
						}
						else if (columnKeywordUpper.StartsWith("FIXEDSTRING(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TFixedString;
							int num4 = 0;
							int.TryParse(columnKeyword.Substring(12, columnKeyword.IndexOf(")") - 12), out num4);
							dalColumn.MaxLength = num4;
						}
						else if (columnKeywordUpper.Equals("TEXT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TText;
						}
						else if (columnKeywordUpper.StartsWith("TEXT(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TText;
							int num5 = 0;
							int.TryParse(columnKeyword.Substring(5, columnKeyword.IndexOf(")") - 5), out num5);
							dalColumn.MaxLength = num5;
						}
						else if (columnKeywordUpper.Equals("LONGTEXT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLongText;
						}
						else if (columnKeywordUpper.StartsWith("LONGTEXT(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLongText;
							int num6 = 0;
							int.TryParse(columnKeyword.Substring(9, columnKeyword.IndexOf(")") - 9), out num6);
							dalColumn.MaxLength = num6;
						}
						else if (columnKeywordUpper.Equals("MEDIUMTEXT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMediumText;
						}
						else if (columnKeywordUpper.StartsWith("MEDIUMTEXT(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMediumText;
							int num7 = 0;
							int.TryParse(columnKeyword.Substring(11, columnKeyword.IndexOf(")") - 1), out num7);
							dalColumn.MaxLength = num7;
						}
						else if (columnKeywordUpper.Equals("BOOL", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TBool;
						}
						else if (columnKeywordUpper.Equals("GUID", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGuid;
						}
						else if (columnKeywordUpper.Equals("DECIMAL", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TDecimal;
						}
						else if (columnKeywordUpper.StartsWith("DECIMAL", StringComparison.Ordinal))
						{
							string str28 = "";
							string str29 = "";
							int num8 = -1;
							int num9 = -1;
							int num10 = -1;
							num8 = columnKeyword.IndexOf("(");
							num9 = columnKeyword.IndexOf(",");
							num10 = columnKeyword.IndexOf(")");
							if (num8 > -1 & num9 > -1)
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
							dalColumn.Type = DalColumnType.TDecimal;
						}
						else if (columnKeywordUpper.Equals("DOUBLE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TDouble;
						}
						else if (columnKeywordUpper.Equals("FLOAT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TFloat;
						}
						else if (columnKeywordUpper.Equals("INT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt;
						}
						else if (columnKeywordUpper.Equals("INTEGER", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt;
						}
						else if (columnKeywordUpper.Equals("INT8", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt8;
						}
						else if (columnKeywordUpper.Equals("INT16", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt16;
						}
						else if (columnKeywordUpper.Equals("INT32", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt32;
						}
						else if (columnKeywordUpper.Equals("INT64", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt64;
						}
						else if (columnKeywordUpper.Equals("UINT8", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TUInt8;
						}
						else if (columnKeywordUpper.Equals("UINT16", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TUInt16;
						}
						else if (columnKeywordUpper.Equals("UINT32", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TUInt32;
						}
						else if (columnKeywordUpper.Equals("UINT64", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TUInt64;
						}
						else if (columnKeywordUpper.Equals("GEOMETRY", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeometry;
						}
						else if (columnKeywordUpper.Equals("GEOMETRYCOLLECTION", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeometryCollection;
						}
						else if (columnKeywordUpper.Equals("POINT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TPoint;
						}
						else if (columnKeywordUpper.Equals("LINESTRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLineString;
						}
						else if (columnKeywordUpper.Equals("POLYGON", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TPolygon;
						}
						else if (columnKeywordUpper.Equals("LINE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLine;
						}
						else if (columnKeywordUpper.Equals("CURVE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TCurve;
						}
						else if (columnKeywordUpper.Equals("SURFACE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TSurface;
						}
						else if (columnKeywordUpper.Equals("LINEARRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLinearRing;
						}
						else if (columnKeywordUpper.Equals("MULTIPOINT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiPoint;
						}
						else if (columnKeywordUpper.Equals("MULTILINESTRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiLineString;
						}
						else if (columnKeywordUpper.Equals("MULTIPOLYGON", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiPolygon;
						}
						else if (columnKeywordUpper.Equals("MULTICURVE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiCurve;
						}
						else if (columnKeywordUpper.Equals("MULTISURFACE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiSurface;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographic;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHICCOLLECTION", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicCollection;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_POINT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicPoint;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_LINESTRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicLineString;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_POLYGON", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicPolygon;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_LINE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicLine;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_CURVE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicCurve;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_SURFACE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicSurface;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_LINEARRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicLinearRing;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_MULTIPOINT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiPoint;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_MULTILINESTRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiLineString;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_MULTIPOLYGON", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiPolygon;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_MULTICURVE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiCurve;
						}
						else if (columnKeywordUpper.Equals("GEOGAPHIC_MULTISURFACE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiSurface;
						}
						else if (columnKeyword.Equals("DATETIME", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TDateTime;
						}
						else if (columnKeywordUpper.StartsWith("DEFAULT ", StringComparison.Ordinal))
						{
							dalColumn.DefaultValue = columnKeyword.Substring(8);
						}
						else if (columnKeywordUpper.StartsWith("ACTUALDEFAULT ", StringComparison.Ordinal))
						{
							dalColumn.ActualDefaultValue = columnKeyword.Substring(14);
						}
						else if (columnKeywordUpper.StartsWith("TODB ", StringComparison.Ordinal))
						{
							dalColumn.ToDb = columnKeyword.Substring(5);
						}
						else if (columnKeywordUpper.Equals("VIRTUAL", StringComparison.Ordinal))
						{
							dalColumn.Virtual = true;
						}
						else if (columnKeywordUpper.StartsWith("FROMDB ", StringComparison.Ordinal))
						{
							dalColumn.FromDb = columnKeyword.Substring(7);
                        }
                        else if (columnKeywordUpper.StartsWith("ACTUALTYPE ", StringComparison.Ordinal))
                        {
                            dalColumn.ActualType = columnKeyword.Substring(11);
                        }
                        else if (columnKeywordUpper.StartsWith("COLUMNNAME ", StringComparison.Ordinal))
                        {
                            dalColumn.Name = columnKeyword.Substring(11);
                        }
						else if (columnKeywordUpper.Equals("UNIQUE INDEX", StringComparison.Ordinal))
						{
							DalIndex index = new DalIndex();
                            index.Columns.Add(new DalIndexColumn(dalColumn.Name));
                            index.IndexMode = DalIndexIndexMode.Unique;
                            dalIndices.Add(index);
						}
						else if (columnKeywordUpper.StartsWith("FOREIGN ", StringComparison.Ordinal))
						{
							DalForeignKey dalForeignKey1 = new DalForeignKey();
							string str30 = columnKeyword.Substring(8);
							dalForeignKey1.ForeignTable = str30.Substring(0, str30.IndexOf("."));
							dalForeignKey1.ForeignColumns.Add(str30.Substring(str30.IndexOf(".") + 1));
							dalForeignKey1.Columns.Add(dalColumn.Name);
							dalForeignKeys.Add(dalForeignKey1);
						}
					}
					if (dalColumn.IsPrimaryKey & dalColumn.Type == DalColumnType.TInt)
					{
						dalColumn.Type = DalColumnType.TInt64;
					}
					dalColumns.Add(dalColumn);
				}
				else
				{
					mySqlEngineName = currentLineTrimmed.Substring(13).Trim();
				}
			}
			if (mySqlEngineName.Equals("MyISAM", StringComparison.OrdinalIgnoreCase))
			{
				mySqlEngineName = "MyISAM";
			}
			else if (mySqlEngineName.Equals("InnoDB", StringComparison.OrdinalIgnoreCase))
			{
				mySqlEngineName = "InnoDB";
			}
			else if (mySqlEngineName.Equals("ARCHIVE", StringComparison.OrdinalIgnoreCase))
			{
				mySqlEngineName = "ARCHIVE";
			}

            foreach (var index in dalIndices)
            {
                foreach (var column in index.Columns)
                {
                    if (dalColumns.Find(x => x.NameX.Equals(column.Name)) == null && dalColumns.Find(x => x.Name.Equals(column.Name)) == null)
                    {
                        MessageBox.Show(@"Column " + column.Name + @" not found in index " + (index.IndexName ?? ""));
                    }
                }
            }

            // Start building the output classes

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("public partial class {1}Collection : AbstractRecordList<{1}, {1}Collection> {{{0}}}{0}{0}", "\r\n", className);
			foreach (DalEnum dalEnum1 in dalEnums)
			{
				stringBuilder.AppendFormat("public enum {1}{0}{{{0}", "\r\n", dalEnum1.Name);
				foreach (string item in dalEnum1.Items)
				{
					stringBuilder.AppendFormat("{1},{0}", "\r\n", item);
				}
				stringBuilder.AppendFormat("}}{0}{0}", "\r\n");
			}
			stringBuilder.AppendFormat("public partial class {1} : AbstractRecord<{1}>{0}{{{0}#region Table Schema{0}private static TableSchema _TableSchema;{0}public struct Columns{0}{{{0}", "\r\n", className);
			foreach (DalColumn dalColumn1 in dalColumns)
			{
                stringBuilder.AppendFormat("public {1} string {2} = \"{3}\";", "\r\n", staticColumns ? @"static" : @"const", dalColumn1.Name, dalColumn1.NameX);
				if (!string.IsNullOrEmpty(dalColumn1.Comment))
				{
					stringBuilder.AppendFormat(" // {1}", "\r\n", dalColumn1.Comment);
				}
				stringBuilder.Append("\r\n");
			}
			stringBuilder.AppendFormat("}}{0}", "\r\n");
			stringBuilder.AppendFormat("public override TableSchema GetTableSchema(){0}{{{0}if (null == _TableSchema){0}{{{0}TableSchema schema = new TableSchema();{0}schema.SchemaName = @\"{1}\";{0}", "\r\n", schemaName);
			if (databaseOwner != null && databaseOwner.Length > 0)
			{
				stringBuilder.AppendFormat("schema.DatabaseOwner = @\"{1}\";{0}", "\r\n", databaseOwner);
			}
			foreach (DalColumn enumTypeName in dalColumns)
			{
				string actualType = enumTypeName.ActualType;
				stringBuilder.AppendFormat("schema.AddColumn(Columns.{1}, ", "\r\n", enumTypeName.Name);
				if (!string.IsNullOrEmpty(enumTypeName.EnumTypeName))
				{
					enumTypeName.ActualType = enumTypeName.EnumTypeName;
				}
				else if (enumTypeName.Type == DalColumnType.TBool)
				{
					enumTypeName.ActualType = "bool";
				}
				else if (enumTypeName.Type == DalColumnType.TGuid)
				{
					enumTypeName.ActualType = "Guid";
				}
				else if (enumTypeName.Type == DalColumnType.TDateTime)
				{
					enumTypeName.ActualType = "DateTime";
				}
				else if (enumTypeName.Type == DalColumnType.TInt)
				{
					enumTypeName.ActualType = "int";
				}
				else if (enumTypeName.Type == DalColumnType.TInt8)
				{
					enumTypeName.ActualType = "SByte";
				}
				else if (enumTypeName.Type == DalColumnType.TInt16)
				{
					enumTypeName.ActualType = "Int16";
				}
				else if (enumTypeName.Type == DalColumnType.TInt32)
				{
					enumTypeName.ActualType = "Int32";
				}
				else if (enumTypeName.Type == DalColumnType.TInt64)
				{
					enumTypeName.ActualType = "Int64";
				}
				else if (enumTypeName.Type == DalColumnType.TUInt8)
				{
					enumTypeName.ActualType = "Byte";
				}
				else if (enumTypeName.Type == DalColumnType.TUInt16)
				{
					enumTypeName.ActualType = "UInt16";
				}
				else if (enumTypeName.Type == DalColumnType.TUInt32)
				{
					enumTypeName.ActualType = "UInt32";
				}
				else if (enumTypeName.Type == DalColumnType.TUInt64)
				{
					enumTypeName.ActualType = "UInt64";
				}
				else if (enumTypeName.Type == DalColumnType.TString || enumTypeName.Type == DalColumnType.TText || enumTypeName.Type == DalColumnType.TLongText || enumTypeName.Type == DalColumnType.TMediumText || enumTypeName.Type == DalColumnType.TFixedString)
				{
					enumTypeName.ActualType = "string";
				}
				else if (enumTypeName.Type == DalColumnType.TDecimal)
				{
					enumTypeName.ActualType = "decimal";
				}
				else if (enumTypeName.Type == DalColumnType.TDouble)
				{
					enumTypeName.ActualType = "double";
				}
				else if (enumTypeName.Type == DalColumnType.TFloat)
				{
					enumTypeName.ActualType = "float";
				}
				else if (enumTypeName.Type == DalColumnType.TGeometry)
				{
					enumTypeName.ActualType = "Geometry";
				}
				else if (enumTypeName.Type == DalColumnType.TGeometryCollection)
				{
					enumTypeName.ActualType = "Geometry.GeometryCollection";
				}
				else if (enumTypeName.Type == DalColumnType.TPoint)
				{
					enumTypeName.ActualType = "Geometry.Point";
				}
				else if (enumTypeName.Type == DalColumnType.TLineString)
				{
					enumTypeName.ActualType = "Geometry.LineString";
				}
				else if (enumTypeName.Type == DalColumnType.TPolygon)
				{
					enumTypeName.ActualType = "Geometry.Polygon";
				}
				else if (enumTypeName.Type == DalColumnType.TLine)
				{
					enumTypeName.ActualType = "Geometry.Line";
				}
				else if (enumTypeName.Type == DalColumnType.TCurve)
				{
					enumTypeName.ActualType = "Geometry";
				}
				else if (enumTypeName.Type == DalColumnType.TSurface)
				{
					enumTypeName.ActualType = "Geometry";
				}
				else if (enumTypeName.Type == DalColumnType.TLinearRing)
				{
					enumTypeName.ActualType = "Geometry";
				}
				else if (enumTypeName.Type == DalColumnType.TMultiPoint)
				{
					enumTypeName.ActualType = "Geometry.MultiPoint";
				}
				else if (enumTypeName.Type == DalColumnType.TMultiLineString)
				{
					enumTypeName.ActualType = "Geometry.MultiLineString";
				}
				else if (enumTypeName.Type == DalColumnType.TMultiPolygon)
				{
					enumTypeName.ActualType = "Geometry.MultiPolygon";
				}
				else if (enumTypeName.Type == DalColumnType.TMultiCurve)
				{
					enumTypeName.ActualType = "Geometry.GeometryCollection";
				}
				else if (enumTypeName.Type == DalColumnType.TMultiSurface)
				{
					enumTypeName.ActualType = "Geometry.GeometryCollection";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographic)
				{
					enumTypeName.ActualType = "Geometry";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicCollection)
				{
					enumTypeName.ActualType = "Geometry.GeometryCollection";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicPoint)
				{
					enumTypeName.ActualType = "Geometry.Point";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicLineString)
				{
					enumTypeName.ActualType = "Geometry.LineString";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicPolygon)
				{
					enumTypeName.ActualType = "Geometry.Polygon";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicLine)
				{
					enumTypeName.ActualType = "Geometry.Line";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicCurve)
				{
					enumTypeName.ActualType = "Geometry";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicSurface)
				{
					enumTypeName.ActualType = "Geometry";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicLinearRing)
				{
					enumTypeName.ActualType = "Geometry";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiPoint)
				{
					enumTypeName.ActualType = "Geometry.MultiPoint";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiLineString)
				{
					enumTypeName.ActualType = "Geometry.MultiLineString";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiPolygon)
				{
					enumTypeName.ActualType = "Geometry.MultiPolygon";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiCurve)
				{
					enumTypeName.ActualType = "Geometry.GeometryCollection";
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiSurface)
				{
					enumTypeName.ActualType = "Geometry.GeometryCollection";
				}
				stringBuilder.AppendFormat("typeof({0})", enumTypeName.ActualType);
				if (enumTypeName.Type == DalColumnType.TText)
				{
					stringBuilder.Append(", DataType.Text");
				}
				else if (enumTypeName.Type == DalColumnType.TLongText)
				{
					stringBuilder.Append(", DataType.LongText");
				}
				else if (enumTypeName.Type == DalColumnType.TMediumText)
				{
					stringBuilder.Append(", DataType.MediumText");
				}
				else if (enumTypeName.Type == DalColumnType.TFixedString)
				{
					stringBuilder.Append(", DataType.Char");
				}
				else if (enumTypeName.Type == DalColumnType.TGeometry)
				{
					stringBuilder.Append(", DataType.Geometry");
				}
				else if (enumTypeName.Type == DalColumnType.TGeometryCollection)
				{
					stringBuilder.Append(", DataType.GeometryCollection");
				}
				else if (enumTypeName.Type == DalColumnType.TPoint)
				{
					stringBuilder.Append(", DataType.Point");
				}
				else if (enumTypeName.Type == DalColumnType.TLineString)
				{
					stringBuilder.Append(", DataType.LineString");
				}
				else if (enumTypeName.Type == DalColumnType.TPolygon)
				{
					stringBuilder.Append(", DataType.Polygon");
				}
				else if (enumTypeName.Type == DalColumnType.TLine)
				{
					stringBuilder.Append(", DataType.Line");
				}
				else if (enumTypeName.Type == DalColumnType.TCurve)
				{
					stringBuilder.Append(", DataType.Curve");
				}
				else if (enumTypeName.Type == DalColumnType.TSurface)
				{
					stringBuilder.Append(", DataType.Surface");
				}
				else if (enumTypeName.Type == DalColumnType.TLinearRing)
				{
					stringBuilder.Append(", DataType.LinearRing");
				}
				else if (enumTypeName.Type == DalColumnType.TMultiPoint)
				{
					stringBuilder.Append(", DataType.MultiPoint");
				}
				else if (enumTypeName.Type == DalColumnType.TMultiLineString)
				{
					stringBuilder.Append(", DataType.MultiLineString");
				}
				else if (enumTypeName.Type == DalColumnType.TMultiPolygon)
				{
					stringBuilder.Append(", DataType.MultiPolygon");
				}
				else if (enumTypeName.Type == DalColumnType.TMultiCurve)
				{
					stringBuilder.Append(", DataType.MultiCurve");
				}
				else if (enumTypeName.Type == DalColumnType.TMultiSurface)
				{
					stringBuilder.Append(", DataType.MultiSurface");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographic)
				{
					stringBuilder.Append(", DataType.Geographic");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicCollection)
				{
					stringBuilder.Append(", DataType.GeographicCollection");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicPoint)
				{
					stringBuilder.Append(", DataType.GeographicPoint");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicLineString)
				{
					stringBuilder.Append(", DataType.GeographicLineString");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicPolygon)
				{
					stringBuilder.Append(", DataType.GeographicPolygon");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicLine)
				{
					stringBuilder.Append(", DataType.GeographicLine");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicCurve)
				{
					stringBuilder.Append(", DataType.GeographicCurve");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicSurface)
				{
					stringBuilder.Append(", DataType.GeographicSurface");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicLinearRing)
				{
					stringBuilder.Append(", DataType.GeographicLinearRing");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiPoint)
				{
					stringBuilder.Append(", DataType.GeographicMultiPoint");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiLineString)
				{
					stringBuilder.Append(", DataType.GeographicMultiLineString");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiPolygon)
				{
					stringBuilder.Append(", DataType.GeographicMultiPolygon");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiCurve)
				{
					stringBuilder.Append(", DataType.GeographicMultiCurve");
				}
				else if (enumTypeName.Type == DalColumnType.TGeographicMultiSurface)
				{
					stringBuilder.Append(", DataType.GeographicMultiSurface");
				}
				else if (!string.IsNullOrEmpty(enumTypeName.EnumTypeName))
				{
					if (enumTypeName.Type == DalColumnType.TInt8)
					{
						stringBuilder.Append(", DataType.TinyInt");
					}
					else if (enumTypeName.Type == DalColumnType.TInt16)
					{
						stringBuilder.Append(", DataType.SmallInt");
					}
					else if (enumTypeName.Type == DalColumnType.TInt32)
					{
						stringBuilder.Append(", DataType.Int");
					}
					else if (enumTypeName.Type == DalColumnType.TInt64)
					{
						stringBuilder.Append(", DataType.BigInt");
					}
					else if (enumTypeName.Type == DalColumnType.TUInt8)
					{
						stringBuilder.Append(", DataType.UnsignedTinyInt");
					}
					else if (enumTypeName.Type == DalColumnType.TUInt16)
					{
						stringBuilder.Append(", DataType.UnsignedSmallInt");
					}
					else if (enumTypeName.Type == DalColumnType.TUInt32)
					{
						stringBuilder.Append(", DataType.UnsignedInt");
					}
					else if (enumTypeName.Type == DalColumnType.TUInt64)
					{
						stringBuilder.Append(", DataType.UnsignedBigInt");
					}
				}
				if (enumTypeName.IsNullable && enumTypeName.ActualType != "string")
				{
					enumTypeName.ActualType += "?";
				}
				maxLength = new object[] { "\r\n", enumTypeName.MaxLength, enumTypeName.Precision, enumTypeName.Scale, null, null, null, null };
				object[] objArray = maxLength;
				objArray[4] = (enumTypeName.AutoIncrement ? "true" : "false");
				object[] objArray1 = maxLength;
				objArray1[5] = (enumTypeName.IsPrimaryKey ? "true" : "false");
				object[] objArray2 = maxLength;
				objArray2[6] = (enumTypeName.IsNullable ? "true" : "false");
				maxLength[7] = enumTypeName.DefaultValue;
                stringBuilder.AppendFormat(", {1}, {2}, {3}, {4}, {5}, {6}, {7});{0}", maxLength);
				if (string.IsNullOrEmpty(actualType))
				{
					continue;
				}
				enumTypeName.ActualType = actualType;
			}

            // Create a list of all columns that participate in the Primary Key
            List<DalColumn> primaryKeyColumns = new List<DalColumn>();
            foreach (DalColumn column in dalColumns)
            {
                if (!column.IsPrimaryKey) continue;
                primaryKeyColumns.Add(column);
            }
            foreach (DalIndex index in dalIndices)
            {
                if (index.IndexMode != DalIndexIndexMode.PrimaryKey) continue;
                foreach (DalIndexColumn indexColumn in index.Columns)
                {
                    DalColumn column = dalColumns.Find((DalColumn c) => c.NameX == indexColumn.Name);
                    if (column == null) continue;
                    primaryKeyColumns.Add(column);
                }
            }

			stringBuilder.AppendFormat("{0}_TableSchema = schema;{0}", "\r\n");
			if (dalIndices.Count > 0)
			{
				stringBuilder.AppendFormat("{0}", "\r\n");
				foreach (DalIndex dalIndex2 in dalIndices)
				{
					maxLength = new object[4];
					object[] objArray3 = maxLength;
					objArray3[0] = (dalIndex2.IndexName == null ? "null" : ("\"" + dalIndex2.IndexName + "\""));
					maxLength[1] = dalIndex2.ClusterMode.ToString();
					maxLength[2] = dalIndex2.IndexMode.ToString();
					maxLength[3] = dalIndex2.IndexType.ToString();
                    stringBuilder.AppendFormat("schema.AddIndex({0}, TableSchema.ClusterMode.{1}, TableSchema.IndexMode.{2}, TableSchema.IndexType.{3}", maxLength);
                    foreach (DalIndexColumn indexColumn in dalIndex2.Columns)
                    {
                        DalColumn dalColumn2 = dalColumns.Find((DalColumn c) => c.Name == indexColumn.Name);
                        string col = (dalColumn2 == null ? string.Format("\"{0}\"", indexColumn.Name) : string.Format("Columns.{0}", dalColumn2.Name));
                        stringBuilder.AppendFormat(", {0}", col);
                        if (string.IsNullOrEmpty(indexColumn.SortDirection))
						{
							continue;
						}
                        stringBuilder.AppendFormat(", SortDirection.{0}", indexColumn.SortDirection);
					}
					stringBuilder.AppendFormat(");{0}", "\r\n");
				}
			}
			if (dalForeignKeys.Count > 0)
			{
				stringBuilder.AppendFormat("{0}", "\r\n");
				foreach (DalForeignKey dalForeignKey2 in dalForeignKeys)
				{
                    stringBuilder.AppendFormat("schema.AddForeignKey({0}, ", 
                        (dalForeignKey2.ForeignKeyName == null ? "null" : ("\"" + dalForeignKey2.ForeignKeyName + "\"")));
					if (dalForeignKey2.Columns.Count <= 1)
					{
						stringBuilder.AppendFormat("{0}.Columns.{1}, ", className, dalForeignKey2.Columns[0]);
					}
					else
					{
						stringBuilder.Append("new string[] {");
						foreach (string column1 in dalForeignKey2.Columns)
						{
							if (column1 != dalForeignKey2.Columns[0])
							{
								stringBuilder.Append(" ,");
							}
							stringBuilder.AppendFormat("{0}.Columns.{1}", className, column1);
						}
						stringBuilder.Append("}, ");
					}
					if (dalForeignKey2.ForeignTable != className)
					{
						stringBuilder.AppendFormat("{0}.TableSchema.SchemaName, ", dalForeignKey2.ForeignTable);
					}
					else
					{
						stringBuilder.Append("schema.SchemaName, ");
					}
					if (dalForeignKey2.ForeignColumns.Count <= 1)
					{
						stringBuilder.AppendFormat("{0}.Columns.{1}, ", dalForeignKey2.ForeignTable, dalForeignKey2.ForeignColumns[0]);
					}
					else
					{
						stringBuilder.Append("new string[] {");
						foreach (string foreignColumn in dalForeignKey2.ForeignColumns)
						{
							if (foreignColumn != dalForeignKey2.ForeignColumns[0])
							{
								stringBuilder.Append(" ,");
							}
							stringBuilder.AppendFormat("{0}.Columns.{1}", dalForeignKey2.ForeignTable, foreignColumn);
						}
						stringBuilder.Append("}, ");
					}
					stringBuilder.AppendFormat("TableSchema.ForeignKeyReference.{1}, TableSchema.ForeignKeyReference.{2});{0}", "\r\n", dalForeignKey2.OnDelete.ToString(), dalForeignKey2.OnUpdate.ToString());
				}
			}
			if (mySqlEngineName.Length > 0)
			{
				stringBuilder.AppendFormat("{0}schema.SetMySqlEngine(MySqlEngineType.{1});{0}", "\r\n", mySqlEngineName);
			}
			stringBuilder.AppendFormat("{0}}}{0}{0}return _TableSchema;{0}}}{0}#endregion{0}{0}#region Private Members{0}", "\r\n");
			foreach (DalColumn dalColumn3 in dalColumns)
			{
				if (!dalColumn3.NoProperty)
				{
					stringBuilder.Append("internal ");
				}
				string defaultValue = null;
				defaultValue = dalColumn3.DefaultValue;
				if (string.IsNullOrEmpty(defaultValue) || defaultValue == "null")
				{
					if (!string.IsNullOrEmpty(dalColumn3.EnumTypeName))
					{
						defaultValue = null;
					}
					else if (dalColumn3.Type == DalColumnType.TBool)
					{
						defaultValue = "false";
					}
					else if (dalColumn3.Type == DalColumnType.TGuid)
					{
						defaultValue = "Guid.Empty";
					}
					else if (dalColumn3.Type == DalColumnType.TDateTime)
					{
						defaultValue = "DateTime.UtcNow";
					}
					else if (dalColumn3.Type == DalColumnType.TInt)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TInt8)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TInt16)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TInt32)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TInt64)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TUInt8)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TUInt16)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TUInt32)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TUInt64)
					{
						defaultValue = "0";
					}
					else if (dalColumn3.Type == DalColumnType.TString || dalColumn3.Type == DalColumnType.TText || dalColumn3.Type == DalColumnType.TLongText || dalColumn3.Type == DalColumnType.TMediumText || dalColumn3.Type == DalColumnType.TFixedString)
					{
						defaultValue = "string.Empty";
					}
					else if (dalColumn3.Type == DalColumnType.TDecimal)
					{
						defaultValue = "0m";
					}
					else if (dalColumn3.Type == DalColumnType.TDouble)
					{
						defaultValue = "0d";
					}
					else if (dalColumn3.Type == DalColumnType.TFloat)
					{
						defaultValue = "0f";
					}
				}
				if (dalColumn3.ActualDefaultValue.Length > 0)
				{
					defaultValue = dalColumn3.ActualDefaultValue;
				}
				if (dalColumn3.NoProperty)
				{
					continue;
				}
				stringBuilder.Append(dalColumn3.ActualType);
				stringBuilder.AppendFormat(" _{0}", dalColumn3.Name);
				if ((dalColumn3.DefaultValue == "null" || dalColumn3.ActualDefaultValue.Length > 0 & (dalColumn3.ActualDefaultValue == "null")) && dalColumn3.IsNullable)
				{
                    stringBuilder.AppendFormat(" = {1};{0}", "\r\n", 
                        (dalColumn3.ActualDefaultValue.Length > 0 ? dalColumn3.ActualDefaultValue : dalColumn3.DefaultValue));
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
			stringBuilder.AppendFormat("#endregion{0}{0}#region Properties{0}", "\r\n");
			foreach (DalColumn dalColumn4 in dalColumns)
			{
				if (dalColumn4.NoProperty)
				{
					continue;
				}
				maxLength = new object[] { "\r\n", dalColumn4.ActualType, dalColumn4.Name, null };
				object[] objArray4 = maxLength;
				objArray4[3] = (dalColumn4.Virtual ? "virtual " : "");
                stringBuilder.AppendFormat("public {3}{1} {2}{0}{{{0}get{{return _{2};}}{0}set{{_{2}=value;}}{0}}}{0}", maxLength);
			}
			stringBuilder.AppendFormat("#endregion{0}{0}#region AbstractRecord members{0}", "\r\n");

            // GetPrimaryKeyValue() function
            stringBuilder.AppendFormat("public override object GetPrimaryKeyValue(){0}{{{0}return {1};{0}}}{0}{0}", "\r\n", 
                string.IsNullOrEmpty(singleColumnPrimaryKeyName) ? "null" : singleColumnPrimaryKeyName);

            // Insert() method
            stringBuilder.AppendFormat("public override void Insert(ConnectorBase conn){0}{{{0}", "\r\n");
			bool flag = false;
			List<DalColumn> dalColumns1 = dalColumns;
			if (dalColumns1.Find((DalColumn c) => c.Name == "CreatedBy") != null)
			{
				stringBuilder.AppendFormat("CreatedBy = base.CurrentSessionUserName;{0}", "\r\n");
				flag = true;
			}
			List<DalColumn> dalColumns2 = dalColumns;
			if (dalColumns2.Find((DalColumn c) => c.Name == "CreatedOn") != null)
			{
				stringBuilder.AppendFormat("CreatedOn = DateTime.UtcNow;{0}", "\r\n");
				flag = true;
			}
			if (flag)
			{
				stringBuilder.Append("\r\n");
			}
			if (!string.IsNullOrEmpty(str5))
			{
				stringBuilder.AppendFormat("{1}{0}{0}", "\r\n", str5);
			}
			stringBuilder.AppendFormat("Query qry = new Query(TableSchema);{0}", "\r\n");
			foreach (DalColumn dalColumn5 in dalColumns)
			{
				if (dalColumn5.AutoIncrement)
				{
					continue;
				}
				if (string.IsNullOrEmpty(dalColumn5.ToDb))
				{
					stringBuilder.AppendFormat("qry.Insert(Columns.{1}, {1});{0}", "\r\n", dalColumn5.Name);
				}
				else
				{
					stringBuilder.AppendFormat("qry.Insert(Columns.{1}, {2});{0}", "\r\n", dalColumn5.Name, string.Format(dalColumn5.ToDb, dalColumn5.Name));
				}
			}
			stringBuilder.AppendFormat("{0}object lastInsert = null;{0}if (qry.Execute(conn, out lastInsert) > 0){0}{{{0}", "\r\n");
			if (!string.IsNullOrEmpty(singleColumnPrimaryKeyName))
			{
				string str32 = "{0}";
				List<DalColumn> dalColumns3 = dalColumns;
				DalColumn dalColumn6 = dalColumns3.Find((DalColumn c) => c.Name == singleColumnPrimaryKeyName);
				if (dalColumn6.Type == DalColumnType.TBool)
				{
					str32 = "Convert.ToBoolean({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TGuid)
				{
					str32 = "new Guid({0}.ToString())";
				}
				else if (dalColumn6.Type == DalColumnType.TInt)
				{
					str32 = "Convert.ToInt32({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TInt8)
				{
					str32 = "Convert.ToSByte({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TInt16)
				{
					str32 = "Convert.ToInt16({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TInt32)
				{
					str32 = "Convert.ToInt32({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TInt64)
				{
					str32 = "Convert.ToInt64({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TUInt8)
				{
					str32 = "Convert.ToByte({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TUInt16)
				{
					str32 = "Convert.ToUInt16({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TUInt32)
				{
					str32 = "Convert.ToUInt32({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TUInt64)
				{
					str32 = "Convert.ToUInt64({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TDecimal)
				{
					str32 = "Convert.ToDecimal({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TDouble)
				{
					str32 = "Convert.ToDouble({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TFloat)
				{
					str32 = "Convert.ToSingle({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TDateTime)
				{
					str32 = "Convert.ToDateTime({0})";
				}
				else if (dalColumn6.Type == DalColumnType.TLongText || dalColumn6.Type == DalColumnType.TMediumText || dalColumn6.Type == DalColumnType.TText || dalColumn6.Type == DalColumnType.TString || dalColumn6.Type == DalColumnType.TFixedString)
				{
					str32 = "(string){0}";
				}
				else if (dalColumn6.Type == DalColumnType.TGeometry || dalColumn6.Type == DalColumnType.TGeometryCollection || dalColumn6.Type == DalColumnType.TPoint || dalColumn6.Type == DalColumnType.TLineString || dalColumn6.Type == DalColumnType.TPolygon || dalColumn6.Type == DalColumnType.TLine || dalColumn6.Type == DalColumnType.TCurve || dalColumn6.Type == DalColumnType.TSurface || dalColumn6.Type == DalColumnType.TLinearRing || dalColumn6.Type == DalColumnType.TMultiPoint || dalColumn6.Type == DalColumnType.TMultiLineString || dalColumn6.Type == DalColumnType.TMultiPolygon || dalColumn6.Type == DalColumnType.TMultiCurve || dalColumn6.Type == DalColumnType.TMultiSurface || dalColumn6.Type == DalColumnType.TGeographic || dalColumn6.Type == DalColumnType.TGeographicCollection || dalColumn6.Type == DalColumnType.TGeographicPoint || dalColumn6.Type == DalColumnType.TGeographicLineString || dalColumn6.Type == DalColumnType.TGeographicPolygon || dalColumn6.Type == DalColumnType.TGeographicLine || dalColumn6.Type == DalColumnType.TGeographicCurve || dalColumn6.Type == DalColumnType.TGeographicSurface || dalColumn6.Type == DalColumnType.TGeographicLinearRing || dalColumn6.Type == DalColumnType.TGeographicMultiPoint || dalColumn6.Type == DalColumnType.TGeographicMultiLineString || dalColumn6.Type == DalColumnType.TGeographicMultiPolygon || dalColumn6.Type == DalColumnType.TGeographicMultiCurve || dalColumn6.Type == DalColumnType.TGeographicMultiSurface)
				{
					str32 = "conn.ReadGeometry({0}) as " + dalColumn6.ActualType;
				}
				stringBuilder.AppendFormat("{1} = {2};{0}", "\r\n", singleColumnPrimaryKeyName, string.Format(str32, "(lastInsert)"));
			}
            stringBuilder.AppendFormat("MarkOld();{0}}}{0}}}{0}", "\r\n");

            // Update() method
            stringBuilder.AppendFormat("public override void Update(ConnectorBase conn){0}{{{0}", "\r\n");
			flag = false;
			if (dalColumns.Find((DalColumn c) => c.Name == "ModifiedBy") != null)
			{
				stringBuilder.AppendFormat("ModifiedBy = base.CurrentSessionUserName;{0}", "\r\n");
				flag = true;
			}
			if (dalColumns.Find((DalColumn c) => c.Name == "ModifiedOn") != null)
			{
				stringBuilder.AppendFormat("ModifiedOn = DateTime.UtcNow;{0}", "\r\n");
				flag = true;
			}
			if (flag)
			{
				stringBuilder.Append("\r\n");
			}
			if (!string.IsNullOrEmpty(str6))
			{
				stringBuilder.AppendFormat("{1}{0}{0}", "\r\n", str6);
			}
			stringBuilder.AppendFormat("Query qry = new Query(TableSchema);{0}", "\r\n");
			foreach (DalColumn dalColumn7 in dalColumns)
			{
				if (dalColumn7.AutoIncrement)
				{
					continue;
				}
				if (string.IsNullOrEmpty(dalColumn7.ToDb))
				{
					stringBuilder.AppendFormat("qry.Update(Columns.{1}, {1});{0}", "\r\n", dalColumn7.Name);
				}
				else
				{
					stringBuilder.AppendFormat("qry.Update(Columns.{1}, {2});{0}", "\r\n", dalColumn7.Name, string.Format(dalColumn7.ToDb, dalColumn7.Name));
				}
			}
			bool flag1 = true;
            foreach (DalColumn column in primaryKeyColumns)
			{
                stringBuilder.AppendFormat("qry.{2}(Columns.{1}, {1});{0}", "\r\n", column.Name, (flag1 ? "Where" : "AND"));
				flag1 = false;
			}
            stringBuilder.AppendFormat("{0}qry.Execute(conn);{0}}}{0}", "\r\n");

            // Read() method
            stringBuilder.AppendFormat("public override void Read(DataReaderBase reader){0}{{{0}", "\r\n");
			foreach (DalColumn column in dalColumns)
			{
				string fromDb = "{0}";
				string fromReader = "reader[Columns.{0}]";
				if (column.Type == DalColumnType.TBool)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToBoolean({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToBoolean({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToBoolean({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TGuid)
				{
					if (!column.IsNullable)
					{
						fromDb = "GuidFromDb({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : GuidFromDb({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : GuidFromDb({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TInt || column.Type == DalColumnType.TInt32)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToInt32({0})";
					}
					else if (column.DefaultValue == "0")
					{
						fromDb = "Int32OrZero({0})";
					}
					else if (column.DefaultValue != "null")
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt32({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt32({{0}})", column.ActualType));
					}
					else
					{
						fromDb = "Int32OrNullFromDb({0})";
					}
				}
				else if (column.Type == DalColumnType.TUInt32)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToUInt32({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt32({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt32({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TInt8)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToSByte({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToSByte({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToSByte({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TUInt8)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToByte({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToByte({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToByte({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TInt16)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToInt16({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt16({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt16({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TUInt16)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToUInt16({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt16({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt16({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TInt64)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToInt64({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt64({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt64({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TUInt64)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToUInt64({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt64({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt64({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TDecimal)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToDecimal({0})";
					}
					else if (column.DefaultValue == "0" || column.DefaultValue == "0m")
					{
						fromDb = "DecimalOrZeroFromDb({0})";
					}
					else if (column.DefaultValue != "null")
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDecimal({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDecimal({{0}})", column.ActualType));
					}
					else
					{
						fromDb = "DecimalOrNullFromDb({0})";
					}
				}
				else if (column.Type == DalColumnType.TDouble)
				{
					if (!column.IsNullable)
					{
						fromDb = "Convert.ToDouble({0})";
					}
					else
					{
						fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDouble({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDouble({{0}})", column.ActualType));
					}
				}
				else if (column.Type == DalColumnType.TFloat)
				{
					if (!column.IsNullable)
					{
                        fromDb = "Convert.ToSingle({0})";
					}
					else
					{
                        fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToSingle({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToSingle({{0}})", column.ActualType));
					}
				}
				else if (column.Type != DalColumnType.TDateTime)
				{
					if (column.Type == DalColumnType.TLongText || column.Type == DalColumnType.TMediumText || column.Type == DalColumnType.TText || column.Type == DalColumnType.TString || column.Type == DalColumnType.TFixedString)
					{
						fromDb = (!column.IsNullable ? "(string){0}" : "StringOrNullFromDb({0})");
					}
					else if (column.Type == DalColumnType.TGeometry || column.Type == DalColumnType.TGeometryCollection || column.Type == DalColumnType.TPoint || column.Type == DalColumnType.TLineString || column.Type == DalColumnType.TPolygon || column.Type == DalColumnType.TLine || column.Type == DalColumnType.TCurve || column.Type == DalColumnType.TSurface || column.Type == DalColumnType.TLinearRing || column.Type == DalColumnType.TMultiPoint || column.Type == DalColumnType.TMultiLineString || column.Type == DalColumnType.TMultiPolygon || column.Type == DalColumnType.TMultiCurve || column.Type == DalColumnType.TMultiSurface || column.Type == DalColumnType.TGeographic || column.Type == DalColumnType.TGeographicCollection || column.Type == DalColumnType.TGeographicPoint || column.Type == DalColumnType.TGeographicLineString || column.Type == DalColumnType.TGeographicPolygon || column.Type == DalColumnType.TGeographicLine || column.Type == DalColumnType.TGeographicCurve || column.Type == DalColumnType.TGeographicSurface || column.Type == DalColumnType.TGeographicLinearRing || column.Type == DalColumnType.TGeographicMultiPoint || column.Type == DalColumnType.TGeographicMultiLineString || column.Type == DalColumnType.TGeographicMultiPolygon || column.Type == DalColumnType.TGeographicMultiCurve || column.Type == DalColumnType.TGeographicMultiSurface)
					{
						fromReader = "reader.GetGeometry(Columns.{0}) as " + column.ActualType;
					}
				}
				else if (!column.IsNullable)
				{
					fromDb = "Convert.ToDateTime({0})";
				}
				else if (column.DefaultValue == "DateTime.UtcNow")
				{
					fromDb = "DateTimeOrNow({0})";
				}
				else if (column.DefaultValue != "null")
				{
					fromDb = (!column.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDateTime({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDateTime({{0}})", column.ActualType));
				}
				else
				{
					fromDb = "DateTimeOrNullFromDb({0})";
				}
				if (!string.IsNullOrEmpty(column.EnumTypeName))
				{
					fromDb = "(" + column.EnumTypeName + ")" + fromDb;
				}
				if (!string.IsNullOrEmpty(column.FromDb))
				{
					fromDb = column.FromDb;
				}
				stringBuilder.AppendFormat("{1} = {2};{0}", "\r\n", column.Name, string.Format(fromDb, string.Format(fromReader, column.Name), column.DefaultValue));
			}
			if (!string.IsNullOrEmpty(str7))
			{
				stringBuilder.AppendFormat("{0}{1}{0}", "\r\n", str7);
			}
            stringBuilder.AppendFormat("{0}IsThisANewRecord = false;}}{0}", "\r\n");

            stringBuilder.AppendFormat("#endregion{0}", "\r\n");

            // Helpers
			stringBuilder.AppendFormat("{0}#region Helpers{0}", "\r\n");
			if (primaryKeyColumns.Count > 0)
			{
                // FetchByID(...) function
				stringBuilder.AppendFormat("public static {1} FetchByID(", "\r\n", className);
				bool flag2 = true;
				foreach (DalColumn dalColumn12 in primaryKeyColumns)
				{
					if (!flag2)
					{
						stringBuilder.Append(", ");
					}
					else
					{
						flag2 = false;
					}
					stringBuilder.AppendFormat("{0} {1}", dalColumn12.ActualType, dalColumn12.Name);
				}
				stringBuilder.AppendFormat("){0}{{Query qry = new Query(TableSchema){0}", "\r\n");
				flag2 = true;
				foreach (DalColumn dalColumn13 in primaryKeyColumns)
				{
					if (!flag2)
					{
						stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {1})", "\r\n", dalColumn13.Name);
					}
					else
					{
						stringBuilder.AppendFormat(".Where(Columns.{0}, {0})", dalColumn13.Name);
						flag2 = false;
					}
				}
                stringBuilder.AppendFormat(";{0}using (DataReaderBase reader = qry.ExecuteReader()){0}{{{0}if (reader.Read()){0}{{{0}{1} item = new {1}();{0}item.Read(reader);{0}return item;{0}}}{0}}}{0}return null;{0}}}{0}{0}", "\r\n", className);

                // Delete() function
				stringBuilder.AppendFormat("public static int Delete(", "\r\n");
				flag2 = true;
				foreach (DalColumn dalColumn14 in primaryKeyColumns)
				{
					if (!flag2)
					{
						stringBuilder.Append(", ");
					}
					else
					{
						flag2 = false;
					}
					stringBuilder.AppendFormat("{0} {1}", dalColumn14.ActualType, dalColumn14.Name);
				}
				stringBuilder.AppendFormat("){0}{{Query qry = new Query(TableSchema){0}", "\r\n");
				flag2 = true;
				foreach (DalColumn dalColumn15 in primaryKeyColumns)
				{
					if (!flag2)
					{
						stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {1})", "\r\n", dalColumn15.Name);
					}
					else
					{
						stringBuilder.AppendFormat(".Delete().Where(Columns.{0}, {0})", dalColumn15.Name);
						flag2 = false;
					}
				}
				stringBuilder.AppendFormat(";{0}return qry.Execute();{0}}}{0}", "\r\n");

                // FetchByID(ConnectorBase, ...) function
				stringBuilder.AppendFormat("public static {1} FetchByID(ConnectorBase conn, ", "\r\n", className);
				flag2 = true;
				foreach (DalColumn dalColumn16 in primaryKeyColumns)
				{
					if (!flag2)
					{
						stringBuilder.Append(", ");
					}
					else
					{
						flag2 = false;
					}
					stringBuilder.AppendFormat("{0} {1}", dalColumn16.ActualType, dalColumn16.Name);
				}
				stringBuilder.AppendFormat("){0}{{Query qry = new Query(TableSchema){0}", "\r\n");
				flag2 = true;
				foreach (DalColumn dalColumn17 in primaryKeyColumns)
				{
					if (!flag2)
					{
						stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {1})", "\r\n", dalColumn17.Name);
					}
					else
					{
						stringBuilder.AppendFormat(".Where(Columns.{0}, {0})", dalColumn17.Name);
						flag2 = false;
					}
				}
                stringBuilder.AppendFormat(";{0}using (DataReaderBase reader = qry.ExecuteReader(conn)){0}{{{0}if (reader.Read()){0}{{{0}{1} item = new {1}();{0}item.Read(reader);{0}return item;{0}}}{0}}}{0}return null;{0}}}{0}{0}", "\r\n", className);

                // Delete(ConnectorBase, ...) function
				stringBuilder.AppendFormat("public static int Delete(ConnectorBase conn, ", "\r\n");
				flag2 = true;
				foreach (DalColumn dalColumn18 in primaryKeyColumns)
				{
					if (!flag2)
					{
						stringBuilder.Append(", ");
					}
					else
					{
						flag2 = false;
					}
					stringBuilder.AppendFormat("{0} {1}", dalColumn18.ActualType, dalColumn18.Name);
				}
				stringBuilder.AppendFormat("){0}{{Query qry = new Query(TableSchema){0}", "\r\n");
				flag2 = true;
				foreach (DalColumn dalColumn19 in primaryKeyColumns)
				{
					if (!flag2)
					{
						stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {1})", "\r\n", dalColumn19.Name);
					}
					else
					{
						stringBuilder.AppendFormat(".Delete().Where(Columns.{0}, {0})", dalColumn19.Name);
						flag2 = false;
					}
				}
				stringBuilder.AppendFormat(";{0}return qry.Execute(conn);{0}}}{0}", "\r\n");
			}
			stringBuilder.AppendFormat("#endregion{0}", "\r\n");

            // End of class
			stringBuilder.Append("}");

            // Copy result to ClipBoard
			ClipboardHelper.SetClipboard(stringBuilder.ToString());
		}

        public static string StripColumnName(string columnName)
        {
            columnName = columnName.Trim();
            while (columnName.Length > 0 && !Regex.IsMatch(columnName, @"^[a-zA-Z_]")) columnName = columnName.Remove(0, 1);
            columnName = Regex.Replace(columnName, @"[^a-zA-Z_0-9]", @"");
            return columnName;
        }

		public static bool HasSelection(DTE2 application)
		{
			Document activeDocument = application.ActiveDocument;
			if (activeDocument == null) return false;
			TextDocument textDocument = activeDocument.Object() as TextDocument;
			if (textDocument == null) return false;
			TextSelection selection = textDocument.Selection;
			if (selection == null || selection.Text == null || selection.Text.Length == 0) return false; 
			return true;
		}
	}
}