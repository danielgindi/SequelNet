using System.Collections.Generic;

namespace SequelNet.SchemaGenerator
{
    public class DalForeignKey
    {
        public DalForeignKey()
        {
            this.Columns = new List<string>();
            this.ForeignColumns = new List<string>();
            this.ForeignKeyName = null;
            this.ForeignTable = null;
            this.OnDelete = DalForeignKeyReference.None;
            this.OnUpdate = DalForeignKeyReference.None;
        }

		public string ForeignKeyName;
		public List<string> Columns;
		public string ForeignTable;
		public List<string> ForeignColumns;
		public DalForeignKeyReference OnDelete;
		public DalForeignKeyReference OnUpdate;
	}
}