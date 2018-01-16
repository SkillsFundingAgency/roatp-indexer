using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nest;
using Sfa.Das.Sas.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Exceptions;
using Sfa.Roatp.Indexer.Core.Models;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public sealed class ElasticsearchProviderIndexMaintainer : ElasticsearchIndexMaintainerBase, IMaintainProviderIndex
    {
        private readonly IElasticsearchCustomClient _elasticsearchClient;
        private readonly IIndexSettings<IMaintainProviderIndex> _settings;
        private readonly ILog _log;
        private readonly IElasticsearchConfiguration _elasticsearchConfiguration;

        public ElasticsearchProviderIndexMaintainer(
            IElasticsearchCustomClient elasticsearchClient,
            IElasticsearchRoatpDocumentMapper elasticsearchRoatpDocumentMapper,
            IIndexSettings<IMaintainProviderIndex> settings,
            ILog log,
            IElasticsearchConfiguration elasticsearchConfiguration)
            : base(elasticsearchClient, elasticsearchRoatpDocumentMapper, log, "RoatpProvider")
        {
            _elasticsearchClient = elasticsearchClient;
            _settings = settings;
            _log = log;
            _elasticsearchConfiguration = elasticsearchConfiguration;
        }

        public override void CreateIndex(string indexName)
        {
            var response = Client.CreateIndex(
                indexName,
                i => i
                    .Settings(settings => settings
                        .NumberOfShards(_elasticsearchConfiguration.RoatpProviderIndexShards())
                        .NumberOfReplicas(_elasticsearchConfiguration.RoatpProviderIndexReplicas()))
                    .Mappings(ms => ms
                        .Map<RoatpProviderDocument>(m => m
                            .AutoMap()
                            .Properties(p => p
                                .Text(t => t
                                    .Name(n => n.Ukprn).Fielddata()))
                        ) ));

            if (response.ApiCall.HttpStatusCode != (int)HttpStatusCode.OK)
            {
                throw new ConnectionException($"Received non-200 response when trying to create the Apprenticeship Provider Index, Status Code:{response.ApiCall.HttpStatusCode}");
            }
        }

        public void IndexRoatpProviders(string indexName, IEnumerable<RoatpProvider> indexEntries)
        {
            var documents = indexEntries.Select(roatpProvider => ElasticsearchRoatpDocumentMapper.CreateRoatpProviderDocument(roatpProvider)).ToList();

            _elasticsearchClient.BulkAll(documents, indexName);
        }

        public IEnumerable<RoatpProviderDocument> LoadRoatpProvidersFromAlias()
        {
            return LoadRoatpProvidersFromIndex(_settings.IndexesAlias);
        }

        public IEnumerable<RoatpProviderDocument> LoadRoatpProvidersFromIndex(string newIndexName)
        {
            var take = GetRoatpProvidersAmountFromIndex(newIndexName);

            var response = Client.Search<RoatpProviderDocument>(s =>
                s.Index(newIndexName)
                    .Type(Types.Type<RoatpProviderDocument>())
                    .From(0)
                    .Take(take)
                    .MatchAll());

            return response.Documents;
        }

        private int GetRoatpProvidersAmountFromIndex(string newIndexName)
        {
            var result = Client.Search<RoatpProviderDocument>(s =>
                s.Index(newIndexName)
                    .Type(Types.Type<RoatpProviderDocument>())
                    .From(0)
                    .MatchAll());


            if (result.HitsMetaData!= null)
            {
                return (int) result.HitsMetaData.Total;
            }

            _log.Error(new ApplicationException("Elasticsearch response was null, the box is down"), "Elasticsearch box is down trying to check the RoATP index data");
            throw new ApplicationException("Elasticsearch response was null, the box is down");
        }
    }
}