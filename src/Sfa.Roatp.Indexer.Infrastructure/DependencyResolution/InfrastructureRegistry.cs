using Sfa.Das.Sas.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Infrastructure.Elasticsearch;
using Sfa.Roatp.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.Infrastructure.Services;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using Sfa.Roatp.Registry.Core.Logging;

namespace Sfa.Roatp.Indexer.Infrastructure.DependencyResolution
{
    public class InfrastructureRegistry : StructureMap.Registry
    {
        public InfrastructureRegistry()
        {
            For<ILog>().Use(x => new NLogService(x.ParentType, x.GetInstance<IInfrastructureSettings>())).AlwaysUnique();
            For<IInfrastructureSettings>().Use<InfrastructureSettings>();
            For<IMaintainProviderIndex>().Use<ElasticsearchProviderIndexMaintainer>();
            For<IElasticsearchSettings>().Use<ElasticsearchSettings>(); 
            For<IElasticsearchConfiguration>().Use<ElasticsearchConfiguration>();
            For<IElasticsearchRoatpDocumentMapper>().Use<ElasticsearchRoatpDocumentMapper>();
            For<IElasticsearchCustomClient>().Use<ElasticsearchCustomClient>();
        }
    }
}
