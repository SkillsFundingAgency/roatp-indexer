using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.ApplicationServices.Queue;

namespace Sfa.Roatp.Indexer.WorkerRole
{
    public class IndexerJob : IIndexerJob
    {
        private readonly IGenericControlQueueConsumer _controlQueueConsumer;

        public IndexerJob(IGenericControlQueueConsumer controlQueueConsumer)
        {
            _controlQueueConsumer = controlQueueConsumer;
        }

        public void Run()
        {
            var tasks = new List<Task>
            {
                _controlQueueConsumer.CheckMessage<IMaintainProviderIndex>(),
            };

            Task.WaitAll(tasks.ToArray());
        }
    }
}