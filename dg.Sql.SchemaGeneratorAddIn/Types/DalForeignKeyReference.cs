using System;

namespace dg.Sql.SchemaGeneratorAddIn
{
	public enum DalForeignKeyReference
	{
		None,
		Restrict,
		Cascade,
		SetNull,
		NoAction
	}
}