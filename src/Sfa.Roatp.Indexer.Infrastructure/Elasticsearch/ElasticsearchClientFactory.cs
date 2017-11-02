using Elasticsearch.Net;
using Nest;
using Sfa.Roatp.Indexer.Infrastructure.Extensions;
using Sfa.Roatp.Indexer.Infrastructure.Settings;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public class ElasticsearchClientFactory : IElasticsearchClientFactory
    {
        private readonly IInfrastructureSettings _infrastructureSettings;

        public ElasticsearchClientFactory(IInfrastructureSettings infrastructureSettings)
        {
            _infrastructureSettings = infrastructureSettings;
        }

        public IElasticClient GetElasticClient()
        {
            if (_infrastructureSettings.IgnoreSslCertificateEnabled)
            {
                using (var settings = new ConnectionSettings(
                    new StaticConnectionPool(_infrastructureSettings.ElasticServerUrls),
                    new MyCertificateIgnoringHttpConnection()))
                {
                    SetDefaultSettings(settings);

                    return new ElasticClient(settings);
                }
            }

            using (var settings = new ConnectionSettings(new StaticConnectionPool(_infrastructureSettings.ElasticServerUrls)))
            {
                SetDefaultSettings(settings);

                return new ElasticClient(settings);
            }
        }

        private void SetDefaultSettings(ConnectionSettings settings)
        {
            if (_infrastructureSettings.Elk5Enabled)
            {
                settings.BasicAuthentication(_infrastructureSettings.ElasticUsername, _infrastructureSettings.ElasticPassword);
            }
        }
    }
}