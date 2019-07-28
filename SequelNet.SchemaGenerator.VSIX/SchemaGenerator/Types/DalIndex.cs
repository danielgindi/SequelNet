using System.Collections.Generic;

namespace SequelNet.SchemaGenerator
{
    public class DalIndex
    {
        public DalIndex()
        {
            this.Columns = new List<DalIndexColumn>();
            this.IndexName = null;
            this.ClusterMode = DalIndexClusterMode.None;
            this.IndexType = DalIndexIndexType.None;
            this.IndexMode = DalIndexIndexMode.None;
        }

		public string IndexName;
		public DalIndexClusterMode ClusterMode;
		public DalIndexIndexType IndexType;
		public DalIndexIndexMode IndexMode;
        public List<DalIndexColumn> Columns;
	}
}