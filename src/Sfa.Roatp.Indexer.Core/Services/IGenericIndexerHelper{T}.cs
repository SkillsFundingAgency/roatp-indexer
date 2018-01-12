using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.Core.Services
{
    public interface IGenericIndexerHelper<T>
    {
        IEnumerable<RoatpProvider> LoadEntries();

        void IndexEntries(string indexName, List<RoatpProvider> roatpProviders);

        bool IsIndexCorrectlyCreated(string indexName, int documentAmount);

        bool CreateIndex(string indexName);

        void ChangeUnderlyingIndexForAlias(string newIndexName);

        bool HasRoatpInfoChanged(ICollection<RoatpProvider> roatpProviders);
        NewProviderStats SendEvents(string newIndexName);
    }
}