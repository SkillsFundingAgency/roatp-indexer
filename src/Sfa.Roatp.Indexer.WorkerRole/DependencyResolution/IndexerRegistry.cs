using Sfa.Roatp.Indexer.ApplicationServices.Queue;
using Sfa.Roatp.Indexer.Core.Settings;
using Sfa.Roatp.Indexer.Infrastructure.Elasticsearch;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using Sfa.Roatp.Indexer.WorkerRole.Settings;
using Sfa.Roatp.Registry.ApplicationServices;

namespace Sfa.Roatp.Indexer.WorkerRole.DependencyResolution
{
    public class IndexerRegistry : StructureMap.Registry
    {
        public IndexerRegistry()
        {
            For<IIndexerJob>().Use<IndexerJob>();
            For<IGenericControlQueueConsumer>().Use<GenericControlQueueConsumer>();
            For<IElasticsearchClientFactory>().Use<ElasticsearchClientFactory>();
            For<IWorkerRoleSettings>().Use<WorkRoleSettings>();
            For<IProvideSettings>().Use(c => new AppConfigSettingsProvider(new MachineSettings()));
        }
    }
}