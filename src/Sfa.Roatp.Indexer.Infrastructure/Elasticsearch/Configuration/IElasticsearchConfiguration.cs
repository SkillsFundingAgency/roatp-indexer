namespace Sfa.Das.Sas.Indexer.Infrastructure.Elasticsearch.Configuration
{
    using Nest;

    public interface IElasticsearchConfiguration
    {
        int RoatpProviderIndexShards();

        int RoatpProviderIndexReplicas();
    }
}