using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureToggle.Core.Fluent;
using Nest;
using Sfa.Roatp.Indexer.ApplicationServices.FeatureToggles;
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

        private const string NewRoatpProviderContractType = "ProviderAgreement";
        private const string NewRoatpProviderEvent = "INITIATED";

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

        public bool HasRoatpInfoChanged(IEnumerable<RoatpProvider> roatpProviders)
        {
            var oldProviders = _indexMaintainer.LoadRoatpProvidersFromAlias();

            if (roatpProviders.Count() != oldProviders.Count())
            {
                return true;
            }

            foreach (var roatpProvider in roatpProviders)
            {
                var oldRoatpProvider = oldProviders.FirstOrDefault(x => x.Ukprn == roatpProvider.Ukprn);

                if (oldRoatpProvider == null)
                {
                    return true;
                }

                if (!roatpProvider.IsEqual(oldRoatpProvider))
                {
                    return true;
                }
            }

            return false;
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

        public IEnumerable<RoatpProvider> LoadEntries()
        {
            _log.Debug("Loading data at RoATP provider index");

            return _providerDataService.GetRoatpData();
        }

        public async Task IndexEntries(string indexName, IEnumerable<RoatpProvider> roatpProviders)
        {
            var bulkApiProviderTasks = new List<Task<IBulkResponse>>();

            _log.Debug("Indexing " + roatpProviders.Count() + " RoATP providers");
            bulkApiProviderTasks.AddRange(_indexMaintainer.IndexRoatpProviders(indexName, roatpProviders));

            _indexMaintainer.LogResponse(await Task.WhenAll(bulkApiProviderTasks), "RoatpProviderDocument");
        }

        public IEnumerable<RoatpProviderDocument> CheckNewProviders(string newIndexName)
        {
            var actualRoatpProviders = _indexMaintainer.LoadRoatpProvidersFromIndex(newIndexName);
            var oldProviders = _indexMaintainer.LoadRoatpProvidersFromAlias();

            return (from roatpProviderDocument in actualRoatpProviders
                    let roatpProvider = oldProviders.FirstOrDefault(x => x.Ukprn == roatpProviderDocument.Ukprn)
                    where roatpProvider == null select roatpProviderDocument);
        }

        public void SendNewProviderEvent(IEnumerable<RoatpProviderDocument> newProviders)
        {
            if (!Is<EventsApiFeature>.Enabled) return;

            var roatpProviderEventTasks = new List<Task>();

            foreach (var roatpProviderDocument in newProviders)
            {
                var agreementEvent = new AgreementEvent { ContractType = NewRoatpProviderContractType, Event = NewRoatpProviderEvent, ProviderId = roatpProviderDocument.Ukprn };
                var task = new SFA.DAS.Events.Api.Client.EventsApi(_eventsApiClientConfiguration).CreateAgreementEvent(agreementEvent);
                roatpProviderEventTasks.Add(task);
            }

            Task.WaitAll(roatpProviderEventTasks.ToArray());
        }
    }
}