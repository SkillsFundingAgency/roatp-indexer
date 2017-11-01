using System.Collections.Generic;
using System.Linq;
using Sfa.Roatp.Indexer.Core.Settings;

namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public class ElasticsearchSettings : IElasticsearchSettings
    {
        private readonly IProvideSettings _settings;

        public ElasticsearchSettings(IProvideSettings settingsProvider)
        {
            _settings = settingsProvider;
        }

        public string RoatpProviderIndexShards => GetSetting("ElasticSearch.IndexShards").FirstOrDefault();

        public string RoatpProviderIndexReplicas => GetSetting("ElasticSearch.IndexReplicas").FirstOrDefault();

        private IEnumerable<string> GetSetting(string configName)
        {
            var str = _settings.GetSetting(configName);
            return !string.IsNullOrEmpty(str) ? str.Split('|').Select(element => element.Trim()) : new List<string> {string.Empty};
        }
    }
}