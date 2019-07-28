namespace SequelNet.SchemaGenerator
{
    public class DalIndexColumn
    {
        public DalIndexColumn()
        {
        }

        public DalIndexColumn(string name, string sortDirection)
        {
            this.Name = name;
            this.SortDirection = sortDirection;
        }

        public DalIndexColumn(string name)
        {
            this.Name = name;
        }

        public string Name;
		public string SortDirection;
	}
}