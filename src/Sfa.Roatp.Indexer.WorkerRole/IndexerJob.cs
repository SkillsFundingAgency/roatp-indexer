using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices;

namespace Sfa.Roatp.Indexer.WorkerRole
{
    public class IndexerJob : IIndexerJob
    {
        private readonly IIndexerServiceFactory _indexerServiceFactory;

        public IndexerJob(IIndexerServiceFactory indexerServiceFactory)
        {
            _indexerServiceFactory = indexerServiceFactory;
        }

        public void Run()
        {
            var indexerService = _indexerServiceFactory.GetIndexerService<IMaintainProviderIndex>();
            var tasks = new List<Task>
            {
                indexerService.CreateScheduledIndex(DateTime.Now),
            };

            Task.WaitAll(tasks.ToArray());
        }
    }
}