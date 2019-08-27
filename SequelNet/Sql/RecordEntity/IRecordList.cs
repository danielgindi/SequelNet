using SequelNet.Connector;
using System.Threading;
using System.Threading.Tasks;

namespace SequelNet
{
    public interface IRecordList
    {
        void SaveAll(ConnectorBase conn = null, bool withTransaction = false);
        Task SaveAllAsync(ConnectorBase conn = null, bool withTransaction = false, CancellationToken? cancellationToken = null);
    }
}
