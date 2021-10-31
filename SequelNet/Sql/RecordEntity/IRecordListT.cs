namespace SequelNet
{
    /// <summary>
    /// Defines an interface for a generic record collection class - which will few utility functions for operating on a collection
    /// </summary>
    public interface IRecordList<TListType> : IRecordList
        where TListType : IRecordList<TListType>, new()
    {
        TListType Clone();
    }
}
