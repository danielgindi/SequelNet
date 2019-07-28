namespace dg.Sql.SchemaGenerator
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