using System.Collections.Generic;

namespace Sfa.Roatp.Indexer.ApplicationServices.Queue
{
    public interface IMessageQueueService
    {
        /// <summary>
        /// Gets a default number of messages for the given queue
        /// </summary>
        /// <param name="queueName">name fo the queue you want messages from</param>
        /// <returns>A default number of messages from the named queue</returns>
        int GetQueueMessageCount(string queueName);

        /// <summary>
        /// Gets a default number of messages for the given queue
        /// </summary>
        /// <param name="queueName">name fo the queue you want messages from</param>
        /// <returns>A default number of messages from the named queue</returns>
        IEnumerable<IQueueMessage> GetQueueMessages(string queueName);

        /// <summary>
        /// Deletes a single message from the given queue
        /// </summary>
        /// <param name="queueName">Queue name to delete the message from</param>
        /// <param name="message">The message to delete off the queue</param>
        void DeleteQueueMessage(string queueName, IQueueMessage message);

        /// <summary>
        /// Deletes a collection of messages from the given queue
        /// </summary>
        /// <param name="queueName">Name of the queue you want to delete the messages from</param>
        /// <param name="messages">Collection of messages to delete from the named queue</param>
        void DeleteQueueMessages(string queueName, IEnumerable<IQueueMessage> messages);

        /// <summary>
        /// Clear all messages from the given queue
        /// </summary>
        /// <param name="queueName">Name of the queue you want to delete all messages from</param>
        void ClearQueue(string queueName);
    }
}