using System;

namespace dg.Sql.SchemaGeneratorAddIn
{
    public class DalColumn
    {
		public bool IsPrimaryKey;
		public bool IsNullable;
		public bool AutoIncrement;
        public bool NoProperty;
        public bool NoSave;
		public DalColumnType Type;
		public string LiteralType;
		public string EnumTypeName;
		public string Name;
		public string NameX;
		public int MaxLength;
		public int Precision;
		public int Scale;
		public string DefaultValue;
		public string ActualDefaultValue;
		public string Comment;
		public string ActualType;
		public string ToDb;
        public string FromDb;
        public string Charset;
        public string Collate;
		public bool Virtual;
	}
}