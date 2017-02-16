using System;
using Microsoft.WindowsAzure.Storage.Queue;
using Sfa.Roatp.Indexer.ApplicationServices.Queue;

namespace Sfa.Roatp.Indexer.Infrastructure.Azure
{
    public class AzureQueueMessage : IQueueMessage
    {
        public AzureQueueMessage(CloudQueueMessage message)
        {
            RawMessage = message;
            Message = message?.AsString;
            InsertionTime = message?.InsertionTime?.DateTime;
        }

        /// <summary>
        /// Gets message of the queue message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the time the message was added to the message queue
        /// </summary>
        public DateTime? InsertionTime { get; }

        /// <summary>
        /// Gets the raw message object that is specific to the queue itself (i.e. Azure's CloudQueueMessage)
        /// </summary>
        public object RawMessage { get; }
    }
}
