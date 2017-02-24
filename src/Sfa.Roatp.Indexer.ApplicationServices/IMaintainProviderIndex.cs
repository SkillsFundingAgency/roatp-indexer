using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public interface IMaintainProviderIndex : IMaintainSearchIndexes
    {
        Task IndexEntries(string indexName, IEnumerable<RoatpProvider> entries);
        List<Task<IBulkResponse>> IndexRoatpProviders(string indexName, IEnumerable<RoatpProvider> indexEntries);
        void LogResponse(IBulkResponse[] elementIndexResult, string documentType);
        IEnumerable<RoatpProviderDocument> LoadRoatpProvidersFromIndex(string newIndexName);
        IEnumerable<RoatpProviderDocument> LoadRoatpProvidersFromAlias();
    }
}