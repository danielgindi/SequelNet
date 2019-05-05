using System;

namespace dg.Sql.SchemaGenerator
{
	public enum DalIndexIndexMode
	{
		None,
		Unique,
		FullText,
		Spatial,
		PrimaryKey
	}
}