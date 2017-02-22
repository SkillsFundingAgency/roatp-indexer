using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public interface IElasticsearchMapper
    {
        RoatpProviderDocument CreateRoatpProviderDocument(RoatpProvider roatpProvider);
    }
}