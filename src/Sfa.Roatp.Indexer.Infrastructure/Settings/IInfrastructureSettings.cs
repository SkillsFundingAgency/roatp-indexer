using System;
using System.Collections.Generic;

namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public interface IInfrastructureSettings
    {
        string EnvironmentName { get; }

        IEnumerable<Uri> ElasticServerUrls { get; }

        string ApplicationName { get; }
    }
}