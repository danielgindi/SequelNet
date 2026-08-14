using System;
using System.Collections.Generic;

namespace SequelNet;

public partial class TableSchema
{
    public enum TableElementType
    {
        Index,
        ForeignKey
    }

    public interface ITableElement
    {
        public TableElementType Type { get; }
    }
}
