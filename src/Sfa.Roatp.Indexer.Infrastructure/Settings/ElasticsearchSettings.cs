using System.Configuration;
using System.Linq;

namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public class ElasticsearchSettings : IElasticsearchSettings
    {
        public string RoatpProviderIndexShards => GetSetting("RoatpProviderIndexShards").FirstOrDefault();

        public string RoatpProviderIndexReplicas => GetSetting("RoatpProviderIndexReplicas").FirstOrDefault();

        private string[] GetSetting(string configName)
        {
            var str = ConfigurationManager.AppSettings[configName];
            return !string.IsNullOrEmpty(str) ? str.Split('|') : new[] { string.Empty };
        }
    }
}