namespace SequelNet.SchemaGenerator;

public class DalColumn
{
	public bool IsPrimaryKey;
	public bool IsNullable;
	public bool AutoIncrement;
        public bool NoProperty;
        public bool NoRead;
        public bool NoSave;
	public DalColumnType Type;
	public string LiteralType;
	public string EnumTypeName;
	public bool HasCustomName;
	public string Name;
        public string PropertyName;
	public int MaxLength;
	public int Precision;
	public int Scale;
	public string DefaultValue;
	public string ActualDefaultValue;
	public bool HasDefault = false;
	public string Comment;
	public string ActualType;
	public bool IsCustomType;
        public string ToDb;
        public string FromDb;
        public string IsMutatedProperty;
        public string Charset;
        public string Collate;
        public int? SRID;
        public bool VirtualProp;
        public string Computed;
        public bool ComputedStored;
}