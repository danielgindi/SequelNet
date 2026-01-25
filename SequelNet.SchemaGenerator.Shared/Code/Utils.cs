using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    internal const string NewLine = "\r\n";

    internal static void AppendLine(System.Text.StringBuilder stringBuilder, string line = "")
    {
        stringBuilder.Append(line);
        stringBuilder.Append(NewLine);
    }

    internal static void AppendInit(System.Text.StringBuilder stringBuilder, string memberName, string valueExpression)
    {
        stringBuilder.AppendFormat("{0}{1} = {2},",
            "\r\n",
            memberName,
            valueExpression);
    }

    internal static void AppendInitIfNotEmpty(System.Text.StringBuilder stringBuilder, string memberName, string? valueExpression)
    {
        if (string.IsNullOrEmpty(valueExpression))
            return;

        AppendInit(stringBuilder, memberName, valueExpression!);
    }

    internal static void AppendInitIfTrue(System.Text.StringBuilder stringBuilder, string memberName, bool condition)
    {
        if (!condition)
            return;

        AppendInit(stringBuilder, memberName, "true");
    }

    internal static string ToCsharpStringLiteral(string value)
    {
        return CsharpString(value);
    }

    internal static (string? TypeName, string? EffectiveType, bool IsReferenceType) GetClrTypeName(DalColumn dalCol, ScriptContext context)
    {
        (string? typeName, bool isReferenceType) typePair;

        if (!string.IsNullOrEmpty(dalCol.EnumTypeName))
            typePair = (dalCol.EnumTypeName, false);
        else typePair = dalCol.Type switch
        {
            DalColumnType.TBool => ("bool", false),
            DalColumnType.TGuid => ("Guid", false),

            DalColumnType.TDateTime or DalColumnType.TDateTimeUtc or DalColumnType.TDateTimeLocal or DalColumnType.TDate => ("DateTime", false),
            DalColumnType.TDateTimeOffset => ("DateTimeOffset", false),
            DalColumnType.TTime => ("TimeSpan", false),

            DalColumnType.TInt => ("int", false),
            DalColumnType.TInt8 => ("SByte", false),
            DalColumnType.TInt16 => ("Int16", false),
            DalColumnType.TInt32 => ("Int32", false),
            DalColumnType.TInt64 => ("Int64", false),

            DalColumnType.TUInt8 => ("Byte", false),
            DalColumnType.TUInt16 => ("UInt16", false),
            DalColumnType.TUInt32 => ("UInt32", false),
            DalColumnType.TUInt64 => ("UInt64", false),

            DalColumnType.TString or DalColumnType.TText or DalColumnType.TLongText or DalColumnType.TMediumText or DalColumnType.TFixedString => ("string", true),

            DalColumnType.TDecimal or DalColumnType.TMoney => ("decimal", false),
            DalColumnType.TDouble => ("double", false),
            DalColumnType.TFloat => ("float", false),

            DalColumnType.TJson or DalColumnType.TJsonBinary => ("string", true),

            DalColumnType.TGeometry => ("Geometry", true),
            DalColumnType.TGeometryCollection => ("Geometry.GeometryCollection", true),
            DalColumnType.TPoint => ("Geometry.Point", true),
            DalColumnType.TLineString => ("Geometry.LineString", true),
            DalColumnType.TPolygon => ("Geometry.Polygon", true),
            DalColumnType.TLine => ("Geometry.Line", true),
            DalColumnType.TCurve => ("Geometry", true),
            DalColumnType.TSurface => ("Geometry", true),
            DalColumnType.TLinearRing => ("Geometry", true),

            DalColumnType.TMultiPoint => ("Geometry.MultiPoint", true),
            DalColumnType.TMultiLineString => ("Geometry.MultiLineString", true),
            DalColumnType.TMultiPolygon => ("Geometry.MultiPolygon", true),
            DalColumnType.TMultiCurve => ("Geometry.GeometryCollection", true),
            DalColumnType.TMultiSurface => ("Geometry.GeometryCollection", true),

            DalColumnType.TGeographic => ("Geometry", true),
            DalColumnType.TGeographicCollection => ("Geometry.GeometryCollection", true),
            DalColumnType.TGeographicPoint => ("Geometry.Point", true),
            DalColumnType.TGeographicLineString => ("Geometry.LineString", true),
            DalColumnType.TGeographicPolygon => ("Geometry.Polygon", true),
            DalColumnType.TGeographicLine => ("Geometry.Line", true),
            DalColumnType.TGeographicCurve => ("Geometry", true),
            DalColumnType.TGeographicSurface => ("Geometry", true),
            DalColumnType.TGeographicLinearRing => ("Geometry", true),
            DalColumnType.TGeographicMultiPoint => ("Geometry.MultiPoint", true),
            DalColumnType.TGeographicMultiLineString => ("Geometry.MultiLineString", true),
            DalColumnType.TGeographicMultiPolygon => ("Geometry.MultiPolygon", true),
            DalColumnType.TGeographicMultiCurve => ("Geometry.GeometryCollection", true),
            DalColumnType.TGeographicMultiSurface => ("Geometry.GeometryCollection", true),

            _ => (dalCol.ActualType, false)
        };

        if (!string.IsNullOrEmpty(dalCol.ActualType))
            typePair.typeName = dalCol.ActualType;

        string? fullTypeName = typePair.typeName;

        if (dalCol.IsNullable && (!typePair.isReferenceType || context.NullableEnabled))
        {
            fullTypeName += "?";
        }

        return (typePair.typeName, fullTypeName, typePair.isReferenceType);
    }

    internal static string? GetSchemaDataTypeLiteral(DalColumn dalCol)
    {
        return dalCol.Type switch
        {
            DalColumnType.TDate => "DataType.Date",
            DalColumnType.TTime => "DataType.Time",

            DalColumnType.TText => "DataType.Text",
            DalColumnType.TLongText => "DataType.LongText",
            DalColumnType.TMediumText => "DataType.MediumText",
            DalColumnType.TFixedString => "DataType.Char",

            DalColumnType.TMoney => "DataType.Money",

            DalColumnType.TJson => "DataType.Json",
            DalColumnType.TJsonBinary => "DataType.JsonBinary",

            DalColumnType.TGeometry => "DataType.Geometry",
            DalColumnType.TGeometryCollection => "DataType.GeometryCollection",
            DalColumnType.TPoint => "DataType.Point",
            DalColumnType.TLineString => "DataType.LineString",
            DalColumnType.TPolygon => "DataType.Polygon",
            DalColumnType.TLine => "DataType.Line",
            DalColumnType.TCurve => "DataType.Curve",
            DalColumnType.TSurface => "DataType.Surface",
            DalColumnType.TLinearRing => "DataType.LinearRing",

            DalColumnType.TMultiPoint => "DataType.MultiPoint",
            DalColumnType.TMultiLineString => "DataType.MultiLineString",
            DalColumnType.TMultiPolygon => "DataType.MultiPolygon",
            DalColumnType.TMultiCurve => "DataType.MultiCurve",
            DalColumnType.TMultiSurface => "DataType.MultiSurface",

            DalColumnType.TGeographic => "DataType.Geographic",
            DalColumnType.TGeographicCollection => "DataType.GeographicCollection",
            DalColumnType.TGeographicPoint => "DataType.GeographicPoint",
            DalColumnType.TGeographicLineString => "DataType.GeographicLineString",
            DalColumnType.TGeographicPolygon => "DataType.GeographicPolygon",
            DalColumnType.TGeographicLine => "DataType.GeographicLine",
            DalColumnType.TGeographicCurve => "DataType.GeographicCurve",
            DalColumnType.TGeographicSurface => "DataType.GeographicSurface",
            DalColumnType.TGeographicLinearRing => "DataType.GeographicLinearRing",
            DalColumnType.TGeographicMultiPoint => "DataType.GeographicMultiPoint",
            DalColumnType.TGeographicMultiLineString => "DataType.GeographicMultiLineString",
            DalColumnType.TGeographicMultiPolygon => "DataType.GeographicMultiPolygon",
            DalColumnType.TGeographicMultiCurve => "DataType.GeographicMultiCurve",
            DalColumnType.TGeographicMultiSurface => "DataType.GeographicMultiSurface",

            _ => null
        };
    }

    internal static string? GetEnumUnderlyingDataTypeLiteral(DalColumn dalCol)
    {
        if (string.IsNullOrEmpty(dalCol.EnumTypeName))
            return null;

        return dalCol.Type switch
        {
            DalColumnType.TInt8 => "DataType.TinyInt",
            DalColumnType.TInt16 => "DataType.SmallInt",
            DalColumnType.TInt32 => "DataType.Int",
            DalColumnType.TInt64 => "DataType.BigInt",

            DalColumnType.TUInt8 => "DataType.UnsignedTinyInt",
            DalColumnType.TUInt16 => "DataType.UnsignedSmallInt",
            DalColumnType.TUInt32 => "DataType.UnsignedInt",
            DalColumnType.TUInt64 => "DataType.UnsignedBigInt",

            _ => null
        };
    }

    internal static Dictionary<string, string> BuildPropertyNameByColumnKeyMap(IEnumerable<DalColumn> columns)
    {
        // Case-sensitive mapping to preserve existing behavior.
        var propertyNameByColumnKey = new Dictionary<string, string>(System.StringComparer.Ordinal);

        foreach (var c in columns)
        {
            // Prefer first occurrence if duplicates exist.
            if (!string.IsNullOrEmpty(c.Name) && !propertyNameByColumnKey.ContainsKey(c.Name!))
                propertyNameByColumnKey.Add(c.Name!, c.PropertyName!);

            if (!string.IsNullOrEmpty(c.PropertyName) && !propertyNameByColumnKey.ContainsKey(c.PropertyName!))
                propertyNameByColumnKey.Add(c.PropertyName!, c.PropertyName!);
        }

        return propertyNameByColumnKey;
    }

    private static string CsharpString(string value)
    {
        return (@"""" + value
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\r", "\\\r")
                    .Replace("\n", "\\\n")
                    + @"""" );
    }

    private static string ProcessComputedColumn(string computed)
    {
        return "ValueWrapper.From(" + computed + ")";
    }

    public static string StripColumnName(string columnName)
    {
        columnName = columnName.Trim();
        while (columnName.Length > 0 && !Regex.IsMatch(columnName, @"^[a-zA-Z_]") ) columnName = columnName.Remove(0, 1);
        columnName = Regex.Replace(columnName, @"[^a-zA-Z_0-9]", @"");
        return columnName;
    }

    public static string FirstLetterLowerCase(string name)
    {
        if (name.Length == 0) return name;
        return name.Substring(0, 1).ToLowerInvariant() + name.Remove(0, 1);
    }

    public static string ValueToDb(string varName, DalColumn dalCol)
    {
        if (string.IsNullOrEmpty(dalCol.ToDb))
        {
            return varName;
        }
        else
        {
            return string.Format(dalCol.ToDb, varName);
        }
    }

    public static string SnakeCase(string value)
    {
        var values = new List<string>();
        var matches = Regex.Matches(value, @"[^A-Z._-]+|[A-Z\d]+(?![^._-])|[A-Z\d]+(?=[A-Z])|[A-Z][^A-Z._-]*", RegexOptions.ECMAScript);
        foreach (Match match in matches)
            values.Add(match.Value);

        return string.Join("_", values.Select(x => x.ToLowerInvariant()));
    }
}