using System;
using System.Linq;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.ApplicationServices.Queue;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Registry.Core.Logging;

namespace Sfa.Roatp.Registry.ApplicationServices
{
    public class GenericControlQueueConsumer : IGenericControlQueueConsumer
    {
        private readonly IAppServiceSettings _appServiceSettings;
        private readonly IMessageQueueService _cloudQueueService;
        private readonly IIndexerServiceFactory _indexerServiceFactory;
        private readonly ILog _log;

        public GenericControlQueueConsumer(
            IAppServiceSettings appServiceSettings,
            IMessageQueueService cloudQueueService,
            IIndexerServiceFactory indexerServiceFactory,
            ILog log)
        {
            _appServiceSettings = appServiceSettings;
            _cloudQueueService = cloudQueueService;
            _indexerServiceFactory = indexerServiceFactory;
            _log = log;
        }

        public async Task CheckMessage<T>()
            where T : IMaintainSearchIndexes
        {
            var indexerService = _indexerServiceFactory.GetIndexerService<T>();

            if (indexerService == null)
            {
                return;
            }

            try
            {
                var queueName = _appServiceSettings.QueueName(typeof(T));

                if (string.IsNullOrEmpty(queueName))
                {
                    return;
                }

                var messages = _cloudQueueService.GetQueueMessages(queueName)?.ToArray();

                if (messages != null && messages.Any())
                {
                    var latestMessage = messages?.FirstOrDefault();

                    var extraMessages = messages?.Where(m => m != latestMessage).ToList();

                    // Delete all the messages except the first as they are not needed
                    _cloudQueueService.DeleteQueueMessages(queueName, extraMessages);

                    var indexTime = latestMessage?.InsertionTime ?? DateTime.Now;

                    await indexerService.CreateScheduledIndex(indexTime).ConfigureAwait(false);

                    _cloudQueueService.DeleteQueueMessage(queueName, latestMessage);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Something failed creating index: {typeof(T)}");
            }
        }
    }
}