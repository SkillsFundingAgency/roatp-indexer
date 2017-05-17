using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.ApplicationServices.FeatureToggles;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Indexer.Core.Services;
using SFA.DAS.Events.Api.Client.Configuration;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public sealed class ProviderIndexer : IGenericIndexerHelper<IMaintainProviderIndex>
    {
        private readonly IMaintainProviderIndex _indexMaintainer;

        private readonly IGetRoatpProviders _providerDataService;
        private readonly IConsumeProviderEvents _providerEventConsumer;

        private readonly IIndexSettings<IMaintainProviderIndex> _settings;
        private readonly ILog _log;

        public ProviderIndexer(
            IIndexSettings<IMaintainProviderIndex> settings,
            IMaintainProviderIndex indexMaintainer,
            IGetRoatpProviders providerDataService,
            IConsumeProviderEvents providerEventConsumer,
            ILog log)
        {
            _settings = settings;
            _providerDataService = providerDataService;
            _providerEventConsumer = providerEventConsumer;
            _indexMaintainer = indexMaintainer;
            _log = log;
        }

        public bool HasRoatpInfoChanged(ICollection<RoatpProvider> roatpProviders)
        {
            var oldProviders = _indexMaintainer.LoadRoatpProvidersFromAlias().ToList();

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

        public NewProviderStats SendNewProviderEvents(string newIndexName)
        {
            var newProviders = CheckNewProviders(newIndexName).ToList();
            foreach (var provider in newProviders)
            {
                try
                {
                    _providerEventConsumer.NewProvider(provider);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message, new Dictionary<string, object> { { "ukprn", provider.Ukprn} });
                }
            }

            return new NewProviderStats
            {
                TotalCount = newProviders.Count,
                TotalMainProviders = newProviders.Count(x => x.ProviderType == ProviderType.MainProvider),
                TotalSupportProviders = newProviders.Count(x => x.ProviderType == ProviderType.SupportingProvider),
                TotalEmployerProviders = newProviders.Count(x => x.ProviderType == ProviderType.EmployerProvider)
            };
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
            _log.Debug("Loading data at RoATP spreadsheet");

            return _providerDataService.GetRoatpData();
        }

        public async Task IndexEntries(string indexName, IEnumerable<RoatpProvider> roatpProviders)
        {
            var bulkApiProviderTasks = new List<Task<IBulkResponse>>();
            roatpProviders = roatpProviders.ToList();

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
    }
}