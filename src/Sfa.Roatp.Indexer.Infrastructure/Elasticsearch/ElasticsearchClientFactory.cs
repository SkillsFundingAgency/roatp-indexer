using System.Linq;
using Elasticsearch.Net;
using Nest;
using Sfa.Roatp.Indexer.Infrastructure.Extensions;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public class ElasticsearchClientFactory : IElasticsearchClientFactory
    {
        private readonly IInfrastructureSettings _infrastructureSettings;
        private readonly ILog _logger;

        public ElasticsearchClientFactory(IInfrastructureSettings infrastructureSettings, ILog logger)
        {
            _infrastructureSettings = infrastructureSettings;
            _logger = logger;
        }

        public IElasticClient GetElasticClient()
        {
            if (_infrastructureSettings.IgnoreSslCertificateEnabled)
            {
                using (var settings = new ConnectionSettings(
                    new SingleNodeConnectionPool(_infrastructureSettings.ElasticServerUrls.FirstOrDefault()),
                    new MyCertificateIgnoringHttpConnection()))
                {
                    SetDefaultSettings(settings);

                    return new ElasticClient(settings);
                }
            }

            using (var settings = new ConnectionSettings(new SingleNodeConnectionPool(_infrastructureSettings.ElasticServerUrls.FirstOrDefault())))
            {
                SetDefaultSettings(settings);

                return new ElasticClient(settings);
            }
        }

        private void SetDefaultSettings(ConnectionSettings settings)
        {
            settings
                .OnRequestCompleted(r =>
            {
                _logger.Debug(r.DebugInformation);
            });

            if (_infrastructureSettings.Elk5Enabled)
            {
                settings.BasicAuthentication(_infrastructureSettings.ElasticUsername, _infrastructureSettings.ElasticPassword);
            }
        }
    }
}