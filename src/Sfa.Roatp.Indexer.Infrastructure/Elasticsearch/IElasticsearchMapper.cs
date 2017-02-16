using System.Collections.Generic;
using Sfa.Roatp.Indexer.ApplicationServices.Models;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public interface IElasticsearchMapper
    {
        RoatpProviderDocument CreateRoatpProviderDocument(RoatpProvider roatpProvider);
    }
}