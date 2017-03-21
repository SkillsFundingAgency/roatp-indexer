using Sfa.Das.Sas.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Infrastructure.Elasticsearch;
using Sfa.Roatp.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.Infrastructure.DependencyResolution
{
    public class InfrastructureRegistry : StructureMap.Registry
    {
        public InfrastructureRegistry()
        {
            For<ILog>().Use(x => new NLogLogger(x.ParentType, null)).AlwaysUnique();
            For<IInfrastructureSettings>().Use<InfrastructureSettings>();
            For<IMaintainProviderIndex>().Use<ElasticsearchProviderIndexMaintainer>();
            For<IElasticsearchSettings>().Use<ElasticsearchSettings>();
            For<IElasticsearchConfiguration>().Use<ElasticsearchConfiguration>();
            For<IElasticsearchRoatpDocumentMapper>().Use<ElasticsearchRoatpDocumentMapper>();
            For<IElasticsearchCustomClient>().Use<ElasticsearchCustomClient>();
        }
    }
}