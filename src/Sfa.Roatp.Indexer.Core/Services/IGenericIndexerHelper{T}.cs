using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.Core.Services
{
    public interface IGenericIndexerHelper<T>
    {
        List<RoatpProvider> LoadEntries();

        Task IndexEntries(string indexName, List<RoatpProvider> roatpProviders);

        bool IsIndexCorrectlyCreated(string indexName);

        bool CreateIndex(string indexName);

        void ChangeUnderlyingIndexForAlias(string newIndexName);

        List<RoatpProviderDocument> CheckNewProviders(string newIndexName);

        void SendNewProviderEvent(List<RoatpProviderDocument> newProviders);

        bool InfoHasChanged(List<RoatpProvider> roatpProviders);
    }
}