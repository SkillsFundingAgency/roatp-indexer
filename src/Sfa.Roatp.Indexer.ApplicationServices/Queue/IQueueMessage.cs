using System;

namespace Sfa.Roatp.Indexer.ApplicationServices.Queue
{
    /// <summary>
    /// Generic interface that all queue message wrappers should derive from for internal application use
    /// </summary>
    public interface IQueueMessage
    {
        /// <summary>
        /// Gets message of the queue message
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the time the message was added to the message queue
        /// </summary>
        DateTime? InsertionTime { get; }

        object RawMessage { get; }
    }
}