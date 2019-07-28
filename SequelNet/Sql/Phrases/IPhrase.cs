namespace SequelNet
{
    public interface IPhrase
    {
        string BuildPhrase(Connector.ConnectorBase connection, Query relatedQuery = null);
    }
}
