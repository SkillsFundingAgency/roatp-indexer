using Sfa.Roatp.Indexer.Core.Settings;
using Sfa.Roatp.Indexer.Infrastructure.Elasticsearch;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using Sfa.Roatp.Indexer.WorkerRole.Settings;

namespace Sfa.Roatp.Indexer.WorkerRole.DependencyResolution
{
    public class IndexerRegistry : StructureMap.Registry
    {
        public IndexerRegistry()
        {
            For<IIndexerJob>().Use<IndexerJob>();
            For<IElasticsearchClientFactory>().Use<ElasticsearchClientFactory>();
            For<IWorkerRoleSettings>().Use<WorkRoleSettings>();
            For<IProvideSettings>().Use(c => new AppConfigSettingsProvider(new MachineSettings()));
        }
    }
}