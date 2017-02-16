using Nest;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public interface IElasticsearchClientFactory
    {
        IElasticClient GetElasticClient();
    }
}