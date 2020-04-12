using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet
{
    public struct AlterTableQueryData
    {
        public AlterTableType Type;
        public TableSchema.Column Column;
        public TableSchema.Index Index;
        public TableSchema.ForeignKey ForeignKey;
        public string OldItemName;
    }
}
