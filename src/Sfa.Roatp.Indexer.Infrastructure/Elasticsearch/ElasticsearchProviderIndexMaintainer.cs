﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Nest;
using Sfa.Das.Sas.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Exceptions;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Registry.Core.Logging;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public sealed class ElasticsearchProviderIndexMaintainer : ElasticsearchIndexMaintainerBase, IMaintainProviderIndex
    {
        private readonly IIndexSettings<IMaintainProviderIndex> _settings;
        private readonly ILog _log;
        private readonly IElasticsearchConfiguration _elasticsearchConfiguration;

        public ElasticsearchProviderIndexMaintainer(
            IElasticsearchCustomClient elasticsearchClient,
            IElasticsearchMapper elasticsearchMapper,
            IIndexSettings<IMaintainProviderIndex> settings,
            ILog log,
            IElasticsearchConfiguration elasticsearchConfiguration)
            : base(elasticsearchClient, elasticsearchMapper, log, "RoatpProvider")
        {
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
                        .Map<RoatpProviderDocument>(m => m.AutoMap())));

            if (response.ApiCall.HttpStatusCode != (int)HttpStatusCode.OK)
            {
                throw new ConnectionException($"Received non-200 response when trying to create the Apprenticeship Provider Index, Status Code:{response.ApiCall.HttpStatusCode}");
            }
        }

        public async Task IndexEntries(string indexName, ICollection<RoatpProvider> entries)
        {
            var bulkRoatpProviderTasks = new List<Task<IBulkResponse>>();

            bulkRoatpProviderTasks.AddRange(IndexRoatpProviders(indexName, entries));

            LogResponse(await Task.WhenAll(bulkRoatpProviderTasks), "RoatpProviderDocument");
        }

        public  List<Task<IBulkResponse>> IndexRoatpProviders(string indexName, ICollection<RoatpProvider> indexEntries)
        {
            var bulkProviderLocation = new BulkProviderClient(indexName, Client);
            try
            {
                foreach (var provider in indexEntries)
                {
                    var mappedProvider = ElasticsearchMapper.CreateRoatpProviderDocument(provider);
                    bulkProviderLocation.Create<RoatpProviderDocument>(c => c.Document(mappedProvider));
                }
            }
            catch (Exception ex)
            {
                _log.Error("Something failed indexing roatp provider documents:" + ex.Message);
                throw;
            }

            return bulkProviderLocation.GetTasks();
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
                    .Type(Types.Parse("roatpproviderdocument"))
                    .From(0)
                    .Take(take)
                    .MatchAll());

            return response.Documents;
        }

        private int GetRoatpProvidersAmountFromIndex(string newIndexName)
        {
            var result = Client.Search<RoatpProviderDocument>(s =>
                s.Index(newIndexName)
                    .Type(Types.Parse("roatpproviderdocument"))
                    .From(0)
                    .MatchAll());
            return (int)result.HitsMetaData.Total;
        }
    }
}