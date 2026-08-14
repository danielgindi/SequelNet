using SequelNet.SchemaGenerator;
using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests.Parsing;

public class GeographicTypeParsingTests
{
    [Theory]
    [InlineData("GEOGRAPHIC", DalColumnType.TGeographic)]
    [InlineData("GEOGRAPHICCOLLECTION", DalColumnType.TGeographicCollection)]
    [InlineData("GEOGRAPHIC_POINT", DalColumnType.TGeographicPoint)]
    [InlineData("GEOGRAPHIC_LINESTRING", DalColumnType.TGeographicLineString)]
    [InlineData("GEOGRAPHIC_POLYGON", DalColumnType.TGeographicPolygon)]
    [InlineData("GEOGRAPHIC_LINE", DalColumnType.TGeographicLine)]
    [InlineData("GEOGRAPHIC_CURVE", DalColumnType.TGeographicCurve)]
    [InlineData("GEOGRAPHIC_SURFACE", DalColumnType.TGeographicSurface)]
    [InlineData("GEOGRAPHIC_LINEARRING", DalColumnType.TGeographicLinearRing)]
    [InlineData("GEOGRAPHIC_MULTIPOINT", DalColumnType.TGeographicMultiPoint)]
    [InlineData("GEOGRAPHIC_MULTILINESTRING", DalColumnType.TGeographicMultiLineString)]
    [InlineData("GEOGRAPHIC_MULTIPOLYGON", DalColumnType.TGeographicMultiPolygon)]
    [InlineData("GEOGRAPHIC_MULTICURVE", DalColumnType.TGeographicMultiCurve)]
    [InlineData("GEOGRAPHIC_MULTISURFACE", DalColumnType.TGeographicMultiSurface)]
    public void GenerateDalClass_Parses_Correctly_Spelled_Geographic_Types(string typeKeyword, DalColumnType expectedType)
    {
        var result = global::SequelNet.SchemaGenerator.GeneratorCore.GenerateDalClass($"""
            GeoRecord
            geo_records
            Shape: {typeKeyword}; Shape
            """);

        Assert.Equal(expectedType, result.Context.Columns[0].Type);
    }
}
