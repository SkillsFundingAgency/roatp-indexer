using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.Core.Services
{
    public interface IGenericIndexerHelper<T>
    {
        Task IndexEntries(string indexName);

        bool DeleteOldIndexes(DateTime scheduledRefreshDateTime);

        bool IsIndexCorrectlyCreated(string indexName);

        bool CreateIndex(string indexName);

        void ChangeUnderlyingIndexForAlias(string newIndexName);

        List<RoatpProviderDocument> CheckNewProviders(string newIndexName);

        void SendNewProviderEvent(List<RoatpProviderDocument> newProviders);
    }
}