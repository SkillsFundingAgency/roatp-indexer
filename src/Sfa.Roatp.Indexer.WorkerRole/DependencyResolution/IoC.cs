using Sfa.Roatp.Indexer.ApplicationServices.DependencyResolution;
using Sfa.Roatp.Indexer.Infrastructure.DependencyResolution;
using StructureMap;

namespace Sfa.Roatp.Indexer.WorkerRole.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<IndexerRegistry>();
                c.AddRegistry<InfrastructureRegistry>();
                c.AddRegistry<ApplicationServicesRegistry>();
            });
        }
    }
}
