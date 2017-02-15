using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Infrastructure.Services;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using Sfa.Roatp.Registry.Core.Logging;
using StructureMap;

namespace Sfa.Roatp.Indexer.Infrastructure
{
    public class IndexerServiceFactory : IIndexerServiceFactory
    {
        private readonly IContainer _container;

        public IndexerServiceFactory(IContainer container)
        {
            _container = container;
        }

        public IIndexerService<T> GetIndexerService<T>()
        {
            using (var nested = _container.GetNestedContainer())
            {
                nested.Configure(_ =>
                {
                    _.For<ILog>().Use(x => new NLogService<T>(x.ParentType, x.GetInstance<IInfrastructureSettings>()));
                });

                return nested.GetInstance<IIndexerService<T>>();
            }
        }
    }
}