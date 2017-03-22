using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.Core.Services
{
    public interface IGenericIndexerHelper<T>
    {
        IEnumerable<RoatpProvider> LoadEntries();

        Task IndexEntries(string indexName, IEnumerable<RoatpProvider> roatpProviders);

        bool IsIndexCorrectlyCreated(string indexName);

        bool CreateIndex(string indexName);

        void ChangeUnderlyingIndexForAlias(string newIndexName);

        bool HasRoatpInfoChanged(ICollection<RoatpProvider> roatpProviders);
        void SendNewProviderEvents(string newIndexName);
    }
}