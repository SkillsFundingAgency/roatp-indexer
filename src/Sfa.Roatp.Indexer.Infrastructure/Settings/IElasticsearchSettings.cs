namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public interface IElasticsearchSettings
    {
        string RoatpProviderIndexShards { get; }

        string RoatpProviderIndexReplicas { get; }
    }
}