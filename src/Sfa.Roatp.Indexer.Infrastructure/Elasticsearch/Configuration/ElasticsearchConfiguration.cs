using Sfa.Das.Sas.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.Infrastructure.Settings;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch.Configuration
{
    public class ElasticsearchConfiguration : IElasticsearchConfiguration
    {
        private readonly IElasticsearchSettings _elasticsearchSettings;

        public ElasticsearchConfiguration(IElasticsearchSettings elasticsearchSettings)
        {
            _elasticsearchSettings = elasticsearchSettings;
        }

        public int RoatpProviderIndexShards() => !string.IsNullOrEmpty(_elasticsearchSettings.RoatpProviderIndexShards) ? int.Parse(_elasticsearchSettings.RoatpProviderIndexShards) : 1;

        public int RoatpProviderIndexReplicas() => !string.IsNullOrEmpty(_elasticsearchSettings.RoatpProviderIndexReplicas) ? int.Parse(_elasticsearchSettings.RoatpProviderIndexReplicas) : 0;
    }
}
