namespace SequelNet
{
    public interface IRecordList<TListType> : IRecordList
        where TListType : IRecordList<TListType>, new()
    {
        TListType Clone();
    }
}
