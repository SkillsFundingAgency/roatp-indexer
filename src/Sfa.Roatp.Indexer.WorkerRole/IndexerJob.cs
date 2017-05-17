using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices;

namespace Sfa.Roatp.Indexer.WorkerRole
{
    public class IndexerJob : IIndexerJob
    {
        private readonly IIndexerService<IMaintainProviderIndex> _indexerService;

        public IndexerJob(IIndexerService<IMaintainProviderIndex> indexerService)
        {
            _indexerService = indexerService;
        }

        public void Run()
        {
            var tasks = new List<Task>
            {
                _indexerService.CheckRoatpAndCreateIndexAndUpdateAlias(DateTime.Now)
            };

            Task.WaitAll(tasks.ToArray());
        }
    }
}