using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Sfa.Roatp.Indexer.ApplicationServices.Queue;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Registry.Core.Logging;

namespace Sfa.Roatp.Indexer.Infrastructure.Azure
{
    public class AzureCloudQueueService : IMessageQueueService
    {
        private readonly IAppServiceSettings _appServiceSettings;
        private readonly ILog _logger;
        private readonly TimeSpan _messageVisibilityTimeout = TimeSpan.FromMinutes(30);

        public AzureCloudQueueService(IAppServiceSettings appServiceSettings, ILog logger)
        {
            _appServiceSettings = appServiceSettings;
            _logger = logger;
        }

        public int DefaultMessageCount { get; set; } = 10;

        /// <summary>
        /// Gets a default number of messages for the given queue
        /// </summary>
        /// <param name="queueName">name fo the queue you want messages from</param>
        /// <returns>A default number of messages from the named queue</returns>
        public IEnumerable<IQueueMessage> GetQueueMessages(string queueName)
        {
            var queue = GetQueueReference(queueName);

            return queue?.GetMessages(DefaultMessageCount, _messageVisibilityTimeout).Select(x => new AzureQueueMessage(x));
        }

        /// <summary>
        /// Gets the number of messages on the current named queue
        /// </summary>
        /// <param name="queueName">Name of the queue to get the message count for</param>
        /// <returns>Number of messages currently on that queue</returns>
        public int GetQueueMessageCount(string queueName)
        {
            _logger.Debug("Getting messages count from queue [" + queueName + "]");
            var queue = GetQueueReference(queueName);
            queue?.FetchAttributes();

            return queue?.ApproximateMessageCount ?? 0;
        }

        /// <summary>
        /// Deletes a single message from the given queue
        /// </summary>
        /// <param name="queueName">Queue name to delete the message from</param>
        /// <param name="message">The message to delete off the queue</param>
        public void DeleteQueueMessage(string queueName, IQueueMessage message)
        {
            var azureQueueMessage = message?.RawMessage as CloudQueueMessage;

            if (azureQueueMessage == null)
            {
                return;
            }

            var queue = GetQueueReference(queueName);
            queue.DeleteMessage(azureQueueMessage);
        }

        /// <summary>
        /// Deletes a collection of messages from the given queue
        /// </summary>
        /// <param name="queueName">Name of the queue you want to delete the messages from</param>
        /// <param name="messages">Collection of messages to delete from the named queue</param>
        public void DeleteQueueMessages(string queueName, IEnumerable<IQueueMessage> messages)
        {
            _logger.Debug("Deleteing messages from queue [" + queueName + "]");

            var queue = GetQueueReference(queueName);

            if (queue == null)
            {
                _logger.Debug("No queue with that name found");
                return;
            }

            var azureCloudMessages = messages.Select(x => x.RawMessage).OfType<CloudQueueMessage>();

            foreach (var message in azureCloudMessages)
            {
                queue.DeleteMessage(message);
            }
        }

        /// <summary>
        /// Clear all messages from the given queue
        /// </summary>
        /// <param name="queueName">Name of the queue you want to delete all messages from</param>
        public void ClearQueue(string queueName)
        {
            _logger.Debug("Clearing messages from queue [" + queueName + "]");
            var queue = GetQueueReference(queueName);
            queue?.Clear();
        }

        private CloudQueue GetQueueReference(string queueName)
        {
            var storageAccount = CloudStorageAccount.Parse(_appServiceSettings.ConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            return queue;
        }
    }
}