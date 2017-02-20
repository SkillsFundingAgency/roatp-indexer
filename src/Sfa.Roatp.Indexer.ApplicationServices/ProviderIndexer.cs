using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Indexer.Core.Services;
using Sfa.Roatp.Registry.Core.Logging;
using SFA.DAS.Events.Api.Client.Configuration;
using SFA.DAS.Events.Api.Types;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public sealed class ProviderIndexer : IGenericIndexerHelper<IMaintainProviderIndex>
    {
        private readonly IMaintainProviderIndex _indexMaintainer;

        private readonly IGetRoatpProviders _providerDataService;
        private readonly IEventsApiClientConfiguration _eventsApiClientConfiguration;

        private readonly IIndexSettings<IMaintainProviderIndex> _settings;
        private readonly ILog _log;

        public ProviderIndexer(
            IIndexSettings<IMaintainProviderIndex> settings,
            IMaintainProviderIndex indexMaintainer,
            IGetRoatpProviders providerDataService,
            IEventsApiClientConfiguration eventsApiClientConfiguration,
            ILog log)
        {
            _settings = settings;
            _providerDataService = providerDataService;
            _eventsApiClientConfiguration = eventsApiClientConfiguration;
            _indexMaintainer = indexMaintainer;
            _log = log;
        }

        public bool CreateIndex(string indexName)
        {
            var indexExists = _indexMaintainer.IndexExists(indexName);

            // If it already exists and is empty, let's delete it.
            if (indexExists)
            {
                _log.Warn("Index already exists, deleting and creating a new one");

                _indexMaintainer.DeleteIndex(indexName);
            }

            _indexMaintainer.CreateIndex(indexName);

            return _indexMaintainer.IndexExists(indexName);
        }

        public bool IsIndexCorrectlyCreated(string indexName)
        {
            return _indexMaintainer.IndexContainsDocuments(indexName);
        }

        public void ChangeUnderlyingIndexForAlias(string newIndexName)
        {
            if (!_indexMaintainer.AliasExists(_settings.IndexesAlias))
            {
                _log.Warn("Alias doesn't exist, creating a new one...");

                _indexMaintainer.CreateIndexAlias(_settings.IndexesAlias, newIndexName);
            }
            else
            {
                _indexMaintainer.SwapAliasIndex(_settings.IndexesAlias, newIndexName);
            }
        }

        public bool DeleteOldIndexes(DateTime scheduledRefreshDateTime)
        {
            var today = IndexerHelper.GetIndexNameAndDateExtension(scheduledRefreshDateTime, _settings.IndexesAlias, "yyyy-MM-dd");
            var oneDayAgo = IndexerHelper.GetIndexNameAndDateExtension(scheduledRefreshDateTime.AddDays(-1), _settings.IndexesAlias, "yyyy-MM-dd");

            return _indexMaintainer.DeleteIndexes(x =>
                !(x.StartsWith(today, StringComparison.InvariantCultureIgnoreCase) ||
                    x.StartsWith(oneDayAgo, StringComparison.InvariantCultureIgnoreCase)) &&
                x.StartsWith(_settings.IndexesAlias, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task IndexEntries(string indexName)
        {
            var bulkApiProviderTasks = new List<Task<IBulkResponse>>();

            _log.Debug("Loading data at RoATP provider index");
            var roatpProviders = _providerDataService.GetRoatpData();

            _log.Debug($"Received {roatpProviders.Count} RoATP providers");

            _log.Debug("Indexing " + roatpProviders.Count + " RoATP providers");
            bulkApiProviderTasks.AddRange(_indexMaintainer.IndexRoatpProviders(indexName, roatpProviders));

            _indexMaintainer.LogResponse(await Task.WhenAll(bulkApiProviderTasks), "RoatpProviderDocument");
        }

        public List<RoatpProviderDocument> CheckNewProviders(string newIndexName)
        {
            var actualRoatpProviders = _indexMaintainer.LoadRoatpProvidersFromIndex(newIndexName);
            var oldProviders = _indexMaintainer.LoadRoatpProvidersFromAlias();

            return (from roatpProviderDocument in actualRoatpProviders let roatpProvider = oldProviders.FirstOrDefault(x => x.Ukprn == roatpProviderDocument.Ukprn) where roatpProvider == null select roatpProviderDocument).ToList();
        }

        public void SendNewProviderEvent(List<RoatpProviderDocument> newProviders)
        {
            var roatpProviderEventTasks = new List<Task>();

            foreach (var roatpProviderDocument in newProviders)
            {
                var agreementEvent = new AgreementEvent { ContractType = "ProviderAgreement", Event = "INITIATED", ProviderId = roatpProviderDocument.Ukprn };
                var task = new SFA.DAS.Events.Api.Client.EventsApi(_eventsApiClientConfiguration).CreateAgreementEvent(agreementEvent);
                roatpProviderEventTasks.Add(task);
            }
            Task.WaitAll(roatpProviderEventTasks.ToArray());
        }
    }
}