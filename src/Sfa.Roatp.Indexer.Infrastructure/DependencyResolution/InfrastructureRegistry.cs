using Sfa.Roatp.Indexer.ApplicationServices;
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
            For<IIndexerServiceFactory>().Use<IndexerServiceFactory>();
            For<IInfrastructureSettings>().Use<InfrastructureSettings>();
        }
    }
}
