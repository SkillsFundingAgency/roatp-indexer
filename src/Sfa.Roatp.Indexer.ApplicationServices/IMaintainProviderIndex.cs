using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public interface IMaintainProviderIndex : IMaintainSearchIndexes
    {
        Task IndexEntries(string indexName, ICollection<RoatpProvider> entries);
        List<Task<IBulkResponse>> IndexRoatpProviders(string indexName, ICollection<RoatpProvider> indexEntries);
        void LogResponse(IBulkResponse[] elementIndexResult, string documentType);
    }
}