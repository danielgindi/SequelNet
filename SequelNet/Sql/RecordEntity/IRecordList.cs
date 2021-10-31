using SequelNet.Connector;
using System.Threading;
using System.Threading.Tasks;

namespace SequelNet
{
    /// <summary>
    /// Defines an interface for a record collection class - which will few utility functions for operating on a collection
    /// </summary>
    public interface IRecordList
    {
        void SaveAll(ConnectorBase conn = null, bool withTransaction = false);
        Task SaveAllAsync(ConnectorBase conn = null, bool withTransaction = false, CancellationToken? cancellationToken = null);
    }
}
