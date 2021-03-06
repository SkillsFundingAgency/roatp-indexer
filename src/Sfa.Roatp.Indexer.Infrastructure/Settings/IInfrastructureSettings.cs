﻿using System;
using System.Collections.Generic;

namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public interface IInfrastructureSettings
    {
        string EnvironmentName { get; }
        
        IEnumerable<Uri> ElasticServerUrls { get; }

        string ElasticUsername { get; }

        string ElasticPassword { get; }

        bool Elk5Enabled { get; }

        bool IgnoreSslCertificateEnabled { get; }
    }
}