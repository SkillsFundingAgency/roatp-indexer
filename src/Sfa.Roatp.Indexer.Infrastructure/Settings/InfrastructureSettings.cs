using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Sfa.Roatp.Indexer.Core.Settings;

namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public class InfrastructureSettings : IInfrastructureSettings
    {
        private readonly IProvideSettings _settingsProvider;

        public InfrastructureSettings(IProvideSettings settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public string EnvironmentName => _settingsProvider.GetSetting("WorkerRole.EnvironmentName");

        public IEnumerable<Uri> ElasticServerUrls => GetElasticIPs("ElasticSearch.ServerUrls");

        public string ElasticUsername => _settingsProvider.GetSetting("ElasticSearch.Username");

        public string ElasticPassword => _settingsProvider.GetSetting("ElasticSearch.Password");

        private IEnumerable<Uri> GetElasticIPs(string appSetting)
        {
            return _settingsProvider.GetSetting(appSetting).Split(',').Select(url => new Uri(url));
        }
    }
}