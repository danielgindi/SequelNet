using SequelNet.Connector;
using System.Text;

namespace SequelNet
{
    public interface IPhrase
    {
        void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null);
    }
}
