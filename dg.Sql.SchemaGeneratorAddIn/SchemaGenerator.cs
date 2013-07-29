using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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

			string[] strArrays;
			int k;
            object[] maxLength;
			string obj;
            string obj1;
            string obj2;
            string obj3;
            string obj4;
            string obj5;
            string obj6;
            string obj7;
            string obj8;
			string str4 = null;
			string str5 = null;
			string str6 = null;
			string str7 = null;
			string str8 = "";
			
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
				string str12 = splitLines[i];
				string str13 = str12.Trim(new char[] { ' ', '*', '\t' });
				string upper = str13.ToUpper();
				if (upper.StartsWith("@INDEX:", StringComparison.Ordinal))
				{
					string str14 = str13.Substring(7);
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
						else if (str15.ToUpper().Equals("PRIMARYKEY", StringComparison.Ordinal))
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
							string str16 = str15.Trim(new char[] { ' ', '[', ']', '\t' });
							strArrays = str16.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							for (k = 0; k < (int)strArrays.Length; k++)
							{
								string str17 = strArrays[k];
								dalIndex.Columns.Add(str17);
							}
						}
					}
					dalIndices.Add(dalIndex);
				}
				else if (upper.StartsWith("@FOREIGNKEY:", StringComparison.Ordinal))
				{
					string str18 = str13.Substring(12);
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
							strArrays = str21.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							for (k = 0; k < (int)strArrays.Length; k++)
							{
								string str22 = strArrays[k];
								dalForeignKey.Columns.Add(str22);
							}
						}
						else if (upper1.StartsWith("FOREIGNCOLUMNS[", StringComparison.Ordinal))
						{
							string str23 = str19.Substring(14);
							string str24 = str23.Trim(new char[] { ' ', '[', ']', '\t' });
							strArrays = str24.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							for (k = 0; k < (int)strArrays.Length; k++)
							{
								string str25 = strArrays[k];
								dalForeignKey.ForeignColumns.Add(str25);
							}
						}
					}
					dalForeignKeys.Add(dalForeignKey);
				}
				else if (upper.StartsWith("@BEFOREINSERT:", StringComparison.Ordinal))
				{
					str5 = str13.Substring(14).Trim();
				}
				else if (upper.StartsWith("@BEFOREUPDATE:", StringComparison.Ordinal))
				{
					str6 = str13.Substring(14).Trim();
				}
				else if (upper.StartsWith("@AFTERREAD:", StringComparison.Ordinal))
				{
					str7 = str13.Substring(11).Trim();
				}
				else if (!upper.StartsWith("@MYSQLENGINE:", StringComparison.Ordinal))
				{
					int num = str13.IndexOf(":");
					DalColumn dalColumn = new DalColumn();
					dalColumn.Name = str13.Substring(0, num).Trim();
					dalColumn.NameX = dalColumn.Name;
					if (className == dalColumn.Name)
					{
						dalColumn.Name = string.Concat(dalColumn.Name, "X");
					}
					dalColumn.IsPrimaryKey = false;
					dalColumn.IsNullable = false;
					dalColumn.AutoIncrement = false;
					dalColumn.Type = DalColumnType.TInt;
					dalColumn.DefaultValue = "null";
					dalColumn.ActualDefaultValue = "";
					dalColumn.Comment = "";
					dalColumn.EnumTypeName = "";
					str13 = str13.Substring(num + 1).Trim();
					string[] strArrays4 = str13.Split(new char[] { ';' }, StringSplitOptions.None);
					for (int m = 0; m <= (int)strArrays4.Length - 1; m++)
					{
						string str26 = strArrays4[m].Trim();
						string upper4 = str26.ToUpper();
						if (m == (int)strArrays4.Length - 1)
						{
							if (!str26.EndsWith(":") || (int)splitLines.Length <= i + 2 || !splitLines[i + 1].Trim(new char[] { ' ', '*', '\t' }).StartsWith("\"") || !splitLines[i + 2].Trim(new char[] { ' ', '*', '\t' }).StartsWith("-"))
							{
								dalColumn.Comment = str26;
							}
							else
							{
								dalColumn.Comment = str26.Remove(str26.Length - 1, 1);
								i++;
								str13 = splitLines[i];
								DalEnum dalEnum = new DalEnum();
								dalEnum.Name = str13.Trim(new char[] { ' ', '*', '\"', '\t' });
								dalColumn.EnumTypeName = dalEnum.Name;
								dalEnum.Items = new List<string>();
								while ((int)splitLines.Length > i + 1 && splitLines[i + 1].Trim(new char[] { ' ', '*', '\t' }).StartsWith("-"))
								{
									i++;
									string str27 = splitLines[i];
									str13 = str27.Trim(new char[] { ' ', '*', '-', '\t' });
									dalEnum.Items.Add(str13);
								}
								dalEnums.Add(dalEnum);
							}
						}
						else if (upper4.Equals("PRIMARY KEY", StringComparison.Ordinal))
						{
							dalColumn.IsPrimaryKey = true;
							str4 = (str4 != null ? "" : dalColumn.Name);
						}
						else if (upper4.Equals("NULLABLE", StringComparison.Ordinal))
						{
							dalColumn.IsNullable = true;
						}
						else if (upper4.Equals("AUTOINCREMENT", StringComparison.Ordinal))
						{
							dalColumn.AutoIncrement = true;
						}
						else if (upper4.Equals("NOPROPERTY", StringComparison.Ordinal))
						{
							dalColumn.NoProperty = true;
						}
						else if (upper4.StartsWith("PRECISION(", StringComparison.Ordinal))
						{
							int num1 = 0;
							int.TryParse(str26.Substring(10, str26.IndexOf(")") - 10), out num1);
							dalColumn.Precision = num1;
						}
						else if (upper4.StartsWith("SCALE(", StringComparison.Ordinal))
						{
							int num2 = 0;
							int.TryParse(str26.Substring(6, str26.IndexOf(")") - 6), out num2);
							dalColumn.Scale = num2;
						}
						else if (upper4.StartsWith("STRING(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TString;
							int num3 = 0;
							int.TryParse(str26.Substring(7, str26.IndexOf(")") - 7), out num3);
							dalColumn.MaxLength = num3;
						}
						else if (upper4.StartsWith("FIXEDSTRING(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TFixedString;
							int num4 = 0;
							int.TryParse(str26.Substring(12, str26.IndexOf(")") - 12), out num4);
							dalColumn.MaxLength = num4;
						}
						else if (upper4.Equals("TEXT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TText;
						}
						else if (upper4.StartsWith("TEXT(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TText;
							int num5 = 0;
							int.TryParse(str26.Substring(5, str26.IndexOf(")") - 5), out num5);
							dalColumn.MaxLength = num5;
						}
						else if (upper4.Equals("LONGTEXT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLongText;
						}
						else if (upper4.StartsWith("LONGTEXT(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLongText;
							int num6 = 0;
							int.TryParse(str26.Substring(9, str26.IndexOf(")") - 9), out num6);
							dalColumn.MaxLength = num6;
						}
						else if (upper4.Equals("MEDIUMTEXT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMediumText;
						}
						else if (upper4.StartsWith("MEDIUMTEXT(", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMediumText;
							int num7 = 0;
							int.TryParse(str26.Substring(11, str26.IndexOf(")") - 1), out num7);
							dalColumn.MaxLength = num7;
						}
						else if (upper4.Equals("BOOL", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TBool;
						}
						else if (upper4.Equals("GUID", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGuid;
						}
						else if (upper4.Equals("DECIMAL", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TDecimal;
						}
						else if (upper4.StartsWith("DECIMAL", StringComparison.Ordinal))
						{
							string str28 = "";
							string str29 = "";
							int num8 = -1;
							int num9 = -1;
							int num10 = -1;
							num8 = str26.IndexOf("(");
							num9 = str26.IndexOf(",");
							num10 = str26.IndexOf(")");
							if (num8 > -1 & num9 > -1)
							{
								str28 = str26.Substring(num8 + 1, num9 - num8 - 1).Trim();
								str29 = str26.Substring(num9 + 1, num10 - num9 - 1).Trim();
							}
							else if (num8 > -1)
							{
								str28 = str26.Substring(num8 + 1, num10 - num8 - 1).Trim();
							}
							if (str28.Length > 0)
							{
								dalColumn.Scale = Convert.ToInt32(str28);
							}
							if (str29.Length > 0)
							{
								dalColumn.Precision = Convert.ToInt32(str29);
							}
							dalColumn.Type = DalColumnType.TDecimal;
						}
						else if (upper4.Equals("DOUBLE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TDouble;
						}
						else if (upper4.Equals("FLOAT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TFloat;
						}
						else if (upper4.Equals("INT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt;
						}
						else if (upper4.Equals("INTEGER", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt;
						}
						else if (upper4.Equals("INT8", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt8;
						}
						else if (upper4.Equals("INT16", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt16;
						}
						else if (upper4.Equals("INT32", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt32;
						}
						else if (upper4.Equals("INT64", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TInt64;
						}
						else if (upper4.Equals("UINT8", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TUInt8;
						}
						else if (upper4.Equals("UINT16", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TUInt16;
						}
						else if (upper4.Equals("UINT32", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TUInt32;
						}
						else if (upper4.Equals("UINT64", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TUInt64;
						}
						else if (upper4.Equals("GEOMETRY", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeometry;
						}
						else if (upper4.Equals("GEOMETRYCOLLECTION", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeometryCollection;
						}
						else if (upper4.Equals("POINT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TPoint;
						}
						else if (upper4.Equals("LINESTRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLineString;
						}
						else if (upper4.Equals("POLYGON", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TPolygon;
						}
						else if (upper4.Equals("LINE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLine;
						}
						else if (upper4.Equals("CURVE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TCurve;
						}
						else if (upper4.Equals("SURFACE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TSurface;
						}
						else if (upper4.Equals("LINEARRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TLinearRing;
						}
						else if (upper4.Equals("MULTIPOINT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiPoint;
						}
						else if (upper4.Equals("MULTILINESTRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiLineString;
						}
						else if (upper4.Equals("MULTIPOLYGON", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiPolygon;
						}
						else if (upper4.Equals("MULTICURVE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiCurve;
						}
						else if (upper4.Equals("MULTISURFACE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TMultiSurface;
						}
						else if (upper4.Equals("GEOGAPHIC", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographic;
						}
						else if (upper4.Equals("GEOGAPHICCOLLECTION", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicCollection;
						}
						else if (upper4.Equals("GEOGAPHIC_POINT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicPoint;
						}
						else if (upper4.Equals("GEOGAPHIC_LINESTRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicLineString;
						}
						else if (upper4.Equals("GEOGAPHIC_POLYGON", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicPolygon;
						}
						else if (upper4.Equals("GEOGAPHIC_LINE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicLine;
						}
						else if (upper4.Equals("GEOGAPHIC_CURVE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicCurve;
						}
						else if (upper4.Equals("GEOGAPHIC_SURFACE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicSurface;
						}
						else if (upper4.Equals("GEOGAPHIC_LINEARRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicLinearRing;
						}
						else if (upper4.Equals("GEOGAPHIC_MULTIPOINT", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiPoint;
						}
						else if (upper4.Equals("GEOGAPHIC_MULTILINESTRING", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiLineString;
						}
						else if (upper4.Equals("GEOGAPHIC_MULTIPOLYGON", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiPolygon;
						}
						else if (upper4.Equals("GEOGAPHIC_MULTICURVE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiCurve;
						}
						else if (upper4.Equals("GEOGAPHIC_MULTISURFACE", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TGeographicMultiSurface;
						}
						else if (str26.Equals("DATETIME", StringComparison.Ordinal))
						{
							dalColumn.Type = DalColumnType.TDateTime;
						}
						else if (upper4.StartsWith("DEFAULT ", StringComparison.Ordinal))
						{
							dalColumn.DefaultValue = str26.Substring(8);
						}
						else if (upper4.StartsWith("ACTUALDEFAULT ", StringComparison.Ordinal))
						{
							dalColumn.ActualDefaultValue = str26.Substring(14);
						}
						else if (upper4.StartsWith("TODB ", StringComparison.Ordinal))
						{
							dalColumn.ToDb = str26.Substring(5);
						}
						else if (upper4.Equals("VIRTUAL", StringComparison.Ordinal))
						{
							dalColumn.Virtual = true;
						}
						else if (upper4.StartsWith("FROMDB ", StringComparison.Ordinal))
						{
							dalColumn.FromDb = str26.Substring(7);
						}
						else if (upper4.StartsWith("ACTUALTYPE ", StringComparison.Ordinal))
						{
							dalColumn.ActualType = str26.Substring(11);
						}
						else if (upper4.Equals("UNIQUE INDEX", StringComparison.Ordinal))
						{
							DalIndex dalIndex1 = new DalIndex();
							dalIndex1.Columns.Add(dalColumn.Name);
							dalIndex1.IndexMode = DalIndexIndexMode.Unique;
							dalIndices.Add(dalIndex1);
						}
						else if (upper4.StartsWith("FOREIGN ", StringComparison.Ordinal))
						{
							DalForeignKey dalForeignKey1 = new DalForeignKey();
							string str30 = str26.Substring(8);
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
					str8 = str13.Substring(13).Trim();
				}
			}
			if (str8.Equals("MyISAM", StringComparison.OrdinalIgnoreCase))
			{
				str8 = "MyISAM";
			}
			else if (str8.Equals("InnoDB", StringComparison.OrdinalIgnoreCase))
			{
				str8 = "InnoDB";
			}
			else if (str8.Equals("ARCHIVE", StringComparison.OrdinalIgnoreCase))
			{
				str8 = "ARCHIVE";
			}
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
				stringBuilder.AppendFormat("public static string {1} = \"{2}\";", "\r\n", dalColumn1.Name, dalColumn1.NameX);
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
					enumTypeName.ActualType = string.Concat(enumTypeName.ActualType, "?");
				}
				StringBuilder stringBuilder1 = stringBuilder;
				maxLength = new object[] { "\r\n", enumTypeName.MaxLength, enumTypeName.Precision, enumTypeName.Scale, null, null, null, null };
				object[] objArray = maxLength;
				obj = (enumTypeName.AutoIncrement ? "true" : "false");
				objArray[4] = obj;
				object[] objArray1 = maxLength;
				obj1 = (enumTypeName.IsPrimaryKey ? "true" : "false");
				objArray1[5] = obj1;
				object[] objArray2 = maxLength;
				obj2 = (enumTypeName.IsNullable ? "true" : "false");
				objArray2[6] = obj2;
				maxLength[7] = enumTypeName.DefaultValue;
				stringBuilder1.AppendFormat(", {1}, {2}, {3}, {4}, {5}, {6}, {7});{0}", maxLength);
				if (string.IsNullOrEmpty(actualType))
				{
					continue;
				}
				enumTypeName.ActualType = actualType;
			}
			stringBuilder.AppendFormat("{0}_TableSchema = schema;{0}", "\r\n");
			if (dalIndices.Count > 0)
			{
				stringBuilder.AppendFormat("{0}", "\r\n");
				foreach (DalIndex dalIndex2 in dalIndices)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					maxLength = new object[4];
					object[] objArray3 = maxLength;
					obj8 = (dalIndex2.IndexName == null ? "null" : string.Concat("\"", dalIndex2.IndexName, "\""));
					objArray3[0] = obj8;
					maxLength[1] = dalIndex2.ClusterMode.ToString();
					maxLength[2] = dalIndex2.IndexMode.ToString();
					maxLength[3] = dalIndex2.IndexType.ToString();
					stringBuilder2.AppendFormat("schema.AddIndex({0}, TableSchema.ClusterMode.{1}, TableSchema.IndexMode.{2}, TableSchema.IndexType.{3}", maxLength);
					foreach (string column in dalIndex2.Columns)
					{
						string[] strArrays5 = column.Split(new char[] { ' ' });
						string str31 = strArrays5[0];
						DalColumn dalColumn2 = dalColumns.Find((DalColumn c) => c.Name == str31);
						str31 = (dalColumn2 == null ? string.Format("\"{0}\"", str31) : string.Format("Columns.{0}", dalColumn2.Name));
						stringBuilder.AppendFormat(", {0}", str31);
						if ((int)strArrays5.Length != 2)
						{
							continue;
						}
						stringBuilder.AppendFormat(", SortDirection.{0}", strArrays5[1]);
					}
					stringBuilder.AppendFormat(");{0}", "\r\n");
				}
			}
			if (dalForeignKeys.Count > 0)
			{
				stringBuilder.AppendFormat("{0}", "\r\n");
				foreach (DalForeignKey dalForeignKey2 in dalForeignKeys)
				{
					StringBuilder stringBuilder3 = stringBuilder;
					obj7 = (dalForeignKey2.ForeignKeyName == null ? "null" : string.Concat("\"", dalForeignKey2.ForeignKeyName, "\""));
					stringBuilder3.AppendFormat("schema.AddForeignKey({0}, ", obj7);
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
			if (str8.Length > 0)
			{
				stringBuilder.AppendFormat("{0}schema.SetMySqlEngine(MySqlEngineType.{1});{0}", "\r\n", str8);
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
					StringBuilder stringBuilder4 = stringBuilder;
					obj3 = (dalColumn3.ActualDefaultValue.Length > 0 ? dalColumn3.ActualDefaultValue : dalColumn3.DefaultValue);
					stringBuilder4.AppendFormat(" = {1};{0}", "\r\n", obj3);
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
				StringBuilder stringBuilder5 = stringBuilder;
				maxLength = new object[] { "\r\n", dalColumn4.ActualType, dalColumn4.Name, null };
				object[] objArray4 = maxLength;
				obj4 = (dalColumn4.Virtual ? "virtual " : "");
				objArray4[3] = obj4;
				stringBuilder5.AppendFormat("public {3}{1} {2}{0}{{{0}get{{return _{2};}}{0}set{{_{2}=value;}}{0}}}{0}", maxLength);
			}
			stringBuilder.AppendFormat("#endregion{0}{0}#region AbstractRecord members{0}", "\r\n");
			StringBuilder stringBuilder6 = stringBuilder;
			obj5 = (str4 == null || string.IsNullOrEmpty(str4) ? "null" : str4);
			stringBuilder6.AppendFormat("public override object GetPrimaryKeyValue(){0}{{{0}return {1};{0}}}{0}{0}public override void Insert(ConnectorBase conn){0}{{{0}", "\r\n", obj5);
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
			if (str4 != null && !string.IsNullOrEmpty(str4))
			{
				string str32 = "{0}";
				List<DalColumn> dalColumns3 = dalColumns;
				DalColumn dalColumn6 = dalColumns3.Find((DalColumn c) => c.Name == str4);
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
					str32 = "Convert.ToFloat({0})";
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
					str32 = string.Concat("conn.ReadGeometry({0}) as ", dalColumn6.ActualType);
				}
				stringBuilder.AppendFormat("{1} = {2};{0}", "\r\n", str4, string.Format(str32, "(lastInsert)"));
			}
			stringBuilder.AppendFormat("MarkOld();{0}}}{0}}}{0}public override void Update(ConnectorBase conn){0}{{{0}", "\r\n");
			flag = false;
			List<DalColumn> dalColumns4 = dalColumns;
			if (dalColumns4.Find((DalColumn c) => c.Name == "ModifiedBy") != null)
			{
				stringBuilder.AppendFormat("ModifiedBy = base.CurrentSessionUserName;{0}", "\r\n");
				flag = true;
			}
			List<DalColumn> dalColumns5 = dalColumns;
			if (dalColumns5.Find((DalColumn c) => c.Name == "ModifiedOn") != null)
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
			foreach (DalColumn dalColumn8 in dalColumns)
			{
				if (!dalColumn8.IsPrimaryKey)
				{
					continue;
				}
				StringBuilder stringBuilder7 = stringBuilder;
				string name = dalColumn8.Name;
				obj6 = (flag1 ? "Where" : "AND");
				stringBuilder7.AppendFormat("qry.{2}(Columns.{1}, {1});{0}", "\r\n", name, obj6);
				flag1 = false;
			}
			stringBuilder.AppendFormat("{0}qry.Execute(conn);{0}}}{0}public override void Read(DataReaderBase reader){0}{{{0}", "\r\n");
			foreach (DalColumn dalColumn9 in dalColumns)
			{
				string fromDb = "{0}";
				string str33 = "reader[Columns.{0}]";
				if (dalColumn9.Type == DalColumnType.TBool)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToBoolean({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToBoolean({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToBoolean({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TGuid)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "GuidFromDb({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : GuidFromDb({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : GuidFromDb({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TInt || dalColumn9.Type == DalColumnType.TInt32)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToInt32({0})";
					}
					else if (dalColumn9.DefaultValue == "0")
					{
						fromDb = "Int32OrZero({0})";
					}
					else if (dalColumn9.DefaultValue != "null")
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt32({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt32({{0}})", dalColumn9.ActualType));
					}
					else
					{
						fromDb = "Int32OrNullFromDb({0})";
					}
				}
				else if (dalColumn9.Type == DalColumnType.TUInt32)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToUInt32({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt32({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt32({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TInt8)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToSByte({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToSByte({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToSByte({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TUInt8)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToByte({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToByte({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToByte({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TInt16)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToInt16({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt16({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt16({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TUInt16)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToUInt16({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt16({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt16({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TInt64)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToInt64({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToInt64({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt64({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TUInt64)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToUInt64({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToUInt64({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt64({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TDecimal)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToDecimal({0})";
					}
					else if (dalColumn9.DefaultValue == "0" || dalColumn9.DefaultValue == "0m")
					{
						fromDb = "DecimalOrZeroFromDb({0})";
					}
					else if (dalColumn9.DefaultValue != "null")
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDecimal({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDecimal({{0}})", dalColumn9.ActualType));
					}
					else
					{
						fromDb = "DecimalOrNullFromDb({0})";
					}
				}
				else if (dalColumn9.Type == DalColumnType.TDouble)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToDouble({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDouble({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDouble({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type == DalColumnType.TFloat)
				{
					if (!dalColumn9.IsNullable)
					{
						fromDb = "Convert.ToFloat({0})";
					}
					else
					{
						fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToFloat({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToFloat({{0}})", dalColumn9.ActualType));
					}
				}
				else if (dalColumn9.Type != DalColumnType.TDateTime)
				{
					if (dalColumn9.Type == DalColumnType.TLongText || dalColumn9.Type == DalColumnType.TMediumText || dalColumn9.Type == DalColumnType.TText || dalColumn9.Type == DalColumnType.TString || dalColumn9.Type == DalColumnType.TFixedString)
					{
						fromDb = (!dalColumn9.IsNullable ? "(string){0}" : "StringOrNullFromDb({0})");
					}
					else if (dalColumn9.Type == DalColumnType.TGeometry || dalColumn9.Type == DalColumnType.TGeometryCollection || dalColumn9.Type == DalColumnType.TPoint || dalColumn9.Type == DalColumnType.TLineString || dalColumn9.Type == DalColumnType.TPolygon || dalColumn9.Type == DalColumnType.TLine || dalColumn9.Type == DalColumnType.TCurve || dalColumn9.Type == DalColumnType.TSurface || dalColumn9.Type == DalColumnType.TLinearRing || dalColumn9.Type == DalColumnType.TMultiPoint || dalColumn9.Type == DalColumnType.TMultiLineString || dalColumn9.Type == DalColumnType.TMultiPolygon || dalColumn9.Type == DalColumnType.TMultiCurve || dalColumn9.Type == DalColumnType.TMultiSurface || dalColumn9.Type == DalColumnType.TGeographic || dalColumn9.Type == DalColumnType.TGeographicCollection || dalColumn9.Type == DalColumnType.TGeographicPoint || dalColumn9.Type == DalColumnType.TGeographicLineString || dalColumn9.Type == DalColumnType.TGeographicPolygon || dalColumn9.Type == DalColumnType.TGeographicLine || dalColumn9.Type == DalColumnType.TGeographicCurve || dalColumn9.Type == DalColumnType.TGeographicSurface || dalColumn9.Type == DalColumnType.TGeographicLinearRing || dalColumn9.Type == DalColumnType.TGeographicMultiPoint || dalColumn9.Type == DalColumnType.TGeographicMultiLineString || dalColumn9.Type == DalColumnType.TGeographicMultiPolygon || dalColumn9.Type == DalColumnType.TGeographicMultiCurve || dalColumn9.Type == DalColumnType.TGeographicMultiSurface)
					{
						str33 = string.Concat("reader.GetGeometry(Columns.{0}) as ", dalColumn9.ActualType);
					}
				}
				else if (!dalColumn9.IsNullable)
				{
					fromDb = "Convert.ToDateTime({0})";
				}
				else if (dalColumn9.DefaultValue == "DateTime.UtcNow")
				{
					fromDb = "DateTimeOrNow({0})";
				}
				else if (dalColumn9.DefaultValue != "null")
				{
					fromDb = (!dalColumn9.ActualType.EndsWith("?") ? "IsNull({0}) ? {1} : Convert.ToDateTime({0})" : string.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDateTime({{0}})", dalColumn9.ActualType));
				}
				else
				{
					fromDb = "DateTimeOrNullFromDb({0})";
				}
				if (!string.IsNullOrEmpty(dalColumn9.EnumTypeName))
				{
					fromDb = string.Concat("(", dalColumn9.EnumTypeName, ")", fromDb);
				}
				if (!string.IsNullOrEmpty(dalColumn9.FromDb))
				{
					fromDb = dalColumn9.FromDb;
				}
				stringBuilder.AppendFormat("{1} = {2};{0}", "\r\n", dalColumn9.Name, string.Format(fromDb, string.Format(str33, dalColumn9.Name), dalColumn9.DefaultValue));
			}
			if (!string.IsNullOrEmpty(str7))
			{
				stringBuilder.AppendFormat("{0}{1}{0}", "\r\n", str7);
			}
			stringBuilder.AppendFormat("{0}IsThisANewRecord = false;}}{0}#endregion{0}", "\r\n");
			stringBuilder.AppendFormat("{0}#region Helpers{0}", "\r\n");
			List<DalColumn> dalColumns6 = new List<DalColumn>();
			foreach (DalColumn dalColumn10 in dalColumns)
			{
				if (!dalColumn10.IsPrimaryKey)
				{
					continue;
				}
				dalColumns6.Add(dalColumn10);
			}
			foreach (DalIndex dalIndex3 in dalIndices)
			{
				if (dalIndex3.IndexMode != DalIndexIndexMode.PrimaryKey)
				{
					continue;
				}
				foreach (string column2 in dalIndex3.Columns)
				{
					List<DalColumn> dalColumns7 = dalColumns;
					DalColumn dalColumn11 = dalColumns7.Find((DalColumn c) => c.Name == column2);
					if (dalColumn11 == null)
					{
						continue;
					}
					dalColumns6.Add(dalColumn11);
				}
			}
			if (dalColumns6.Count > 0)
			{
				stringBuilder.AppendFormat("public static {1} FetchByID(", "\r\n", className);
				bool flag2 = true;
				foreach (DalColumn dalColumn12 in dalColumns6)
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
				foreach (DalColumn dalColumn13 in dalColumns6)
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
				stringBuilder.AppendFormat("public static int Delete(", "\r\n");
				flag2 = true;
				foreach (DalColumn dalColumn14 in dalColumns6)
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
				foreach (DalColumn dalColumn15 in dalColumns6)
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
				stringBuilder.AppendFormat("public static {1} FetchByID(ConnectorBase conn, ", "\r\n", className);
				flag2 = true;
				foreach (DalColumn dalColumn16 in dalColumns6)
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
				foreach (DalColumn dalColumn17 in dalColumns6)
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
				stringBuilder.AppendFormat("public static int Delete(ConnectorBase conn, ", "\r\n");
				flag2 = true;
				foreach (DalColumn dalColumn18 in dalColumns6)
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
				foreach (DalColumn dalColumn19 in dalColumns6)
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
			stringBuilder.Append("}");
			ClipboardHelper.SetClipboard(stringBuilder.ToString());
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