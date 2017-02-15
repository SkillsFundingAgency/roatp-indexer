using System;
using System.Collections.Generic;
using Sfa.Roatp.Indexer.Core.Logging.Models;

namespace Sfa.Roatp.Registry.Core.Logging
{
    public interface ILog
    {
        void Debug(object message);

        void Debug(string message, Dictionary<string, object> properties);

        void Debug(string message, ILogEntry entry);

        void Info(string message, Dictionary<string, object> properties);

        void Info(string message, ILogEntry entry);

        void Info(object message);

        void Warn(object message);

        void Warn(string message, Dictionary<string, object> properties);

        void Warn(string message, ILogEntry entry);

        void Warn(Exception exception, object message);

        void Error(Exception exception, object message);

        void Error(string message);

        void Error(string message, ILogEntry entry);

        void Fatal(Exception exception, object message);

        void Fatal(string message, ILogEntry entry);
    }
}