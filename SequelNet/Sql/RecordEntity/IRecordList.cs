using SequelNet.Connector;

namespace SequelNet
{
    public interface IRecordList
    {
        void SaveAll(ConnectorBase conn);
        void SaveAll();
        void SaveAll(ConnectorBase conn, bool withTransaction);
        void SaveAll(bool withTransaction);
    }
}
