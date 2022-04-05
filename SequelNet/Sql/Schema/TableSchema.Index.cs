using System;
using System.Collections.Generic;

namespace SequelNet
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
            public Column[] Columns;

            public Index() { }

            public Index(string name, ClusterMode cluster, IndexMode mode, IndexType type, params object[] columns)
            {
                this.Name = name;
                this.Cluster = cluster;
                this.Mode = mode;
                this.Type = type;

                var generatedColumns = new List<Column>();

                foreach (object obj in columns)
                {
                    if (obj is string || obj is ValueWrapper || obj is IPhrase)
                    {
                        generatedColumns.Add(new Column
                        {
                            Target = obj is string
                            ? ValueWrapper.Column((string)obj)
                            : obj is IPhrase
                            ? ValueWrapper.From((IPhrase)obj)
                            : (ValueWrapper)obj,
                            Length = 0,
                            Sort = SortDirection.ASC
                        });
                    }
                    else if (obj is int)
                    {
                        generatedColumns[generatedColumns.Count - 1].Length = (int)obj;
                    }
                    else if (obj is SortDirection)
                    {
                        generatedColumns[generatedColumns.Count - 1].Sort = (SortDirection)obj;
                    }
                }

                this.Columns = generatedColumns.ToArray();
            }

            public class Column
            {
                public ValueWrapper Target;
                public int? Length;
                public SortDirection Sort;
            }
        }
    }
}
