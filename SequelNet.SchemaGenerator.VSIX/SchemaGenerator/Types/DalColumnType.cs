namespace SequelNet.SchemaGenerator
{
    public enum DalColumnType
	{
		TLiteral,

		TInt,
		TInt8,
		TInt16,
		TInt32,
		TInt64,
		TUInt8,
		TUInt16,
		TUInt32,
		TUInt64,

		TString,
		TFixedString,
		TText,
		TLongText,
		TMediumText,

		TBool,
		TGuid,

		TDateTime,
		TDateTimeUtc,
		TDateTimeLocal,
		TDate,
		TTime,

		TDecimal,
		TMoney,
		TDouble,
		TFloat,

        TJson,
        TJsonBinary,

        TGeometry,
		TGeometryCollection,
		TPoint,
		TLineString,
		TPolygon,
		TLine,
		TCurve,
		TSurface,
		TLinearRing,

		TMultiPoint,
		TMultiLineString,
		TMultiPolygon,
		TMultiCurve,
		TMultiSurface,

		TGeographic,
		TGeographicCollection,
		TGeographicPoint,
		TGeographicLineString,
		TGeographicPolygon,
		TGeographicLine,
		TGeographicCurve,
		TGeographicSurface,
		TGeographicLinearRing,
		TGeographicMultiPoint,
		TGeographicMultiLineString,
		TGeographicMultiPolygon,
		TGeographicMultiCurve,
		TGeographicMultiSurface
	}
}