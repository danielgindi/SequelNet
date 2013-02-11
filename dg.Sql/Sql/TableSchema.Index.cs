using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    public partial class TableSchema
    {
        public class IndexList : List<Index>
        {
            public Index Find(string indexName)
            {
                if (this == null) return null;
                foreach (Index index in this)
                {
                    if (index.Name.Equals(indexName, StringComparison.CurrentCultureIgnoreCase)) return index;
                }
                return null;
            }
        }
        public enum IndexMode
        {
            None,
            Unique,
            FullText, // MySQL Only
            Spatial, // MySQL Only
            PrimaryKey
        }
        public enum IndexType // MySQL Only
        {
            None,
            BTREE,
            HASH,
            RTREE
        }
        public enum ClusterMode // T-SQL Only
        {
            None,
            NonClustered,
            Clustered
        }
        public class Index
        {
            public string Name;
            public ClusterMode Cluster;
            public IndexMode Mode;
            public IndexType Type;
            public string[] ColumnNames;
            public int[] ColumnLength;
            public SortDirection[] ColumnSort;

            public Index() { }
            public Index(string Name, ClusterMode Cluster, IndexMode Mode, IndexType Type, params object[] Columns)
            {
                this.Name = Name;
                this.Cluster = Cluster;
                this.Mode = Mode;
                this.Type = Type;
                List<string> ColumnNames = new List<string>();
                List<int> ColumnLength = new List<int>();
                List<SortDirection> ColumnSort = new List<SortDirection>();

                foreach (object obj in Columns)
                {
                    if (obj is string)
                    {
                        if (ColumnLength.Count < ColumnNames.Count) ColumnLength.Add(0);
                        if (ColumnSort.Count < ColumnNames.Count) ColumnSort.Add(SortDirection.ASC);

                        ColumnNames.Add((string)obj);
                    }
                    else if (obj is int)
                    {
                        if (ColumnLength.Count < ColumnNames.Count) ColumnLength.Add((int)obj);
                    }
                    else if (obj is SortDirection)
                    {
                        if (ColumnSort.Count < ColumnNames.Count) ColumnSort.Add((SortDirection)obj);
                    }
                }

                if (ColumnLength.Count < ColumnNames.Count) ColumnLength.Add(0);
                if (ColumnSort.Count < ColumnNames.Count) ColumnSort.Add(SortDirection.ASC);

                this.ColumnNames = ColumnNames.ToArray();
                this.ColumnLength = ColumnLength.ToArray();
                this.ColumnSort = ColumnSort.ToArray();
            }
        }
    }
}
