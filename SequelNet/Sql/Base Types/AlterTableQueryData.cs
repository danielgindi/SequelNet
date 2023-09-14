namespace SequelNet;

public struct AlterTableQueryData
{
    public AlterTableType Type;
    public TableSchema.Column Column;
    public TableSchema.Index Index;
    public TableSchema.ForeignKey ForeignKey;
    public string OldItemName;
    public bool IgnoreColumnPosition;
}
