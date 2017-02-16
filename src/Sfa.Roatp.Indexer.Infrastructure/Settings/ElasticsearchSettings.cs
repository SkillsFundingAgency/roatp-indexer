using System.Configuration;
using System.Linq;

namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public class ElasticsearchSettings : IElasticsearchSettings
    {
        public string[] StopWords => GetSetting("StopWords");

        public string[] StopWordsExtended => GetSetting("StopWordsExtended");

        public string[] Synonyms => GetSetting("Synonyms");

        public string ApprenticeshipIndexShards => GetSetting("ApprenticeshipIndexShards").FirstOrDefault();

        public string ApprenticeshipIndexReplicas => GetSetting("ApprenticeshipIndexReplicas").FirstOrDefault();

        public string RoatpProviderIndexShards => GetSetting("RoatpProviderIndexShards").FirstOrDefault();

        public string RoatpProviderIndexReplicas => GetSetting("RoatpProviderIndexReplicas").FirstOrDefault();

        public string LarsIndexShards => GetSetting("LarsIndexShards").FirstOrDefault();

        public string LarsIndexReplicas => GetSetting("LarsIndexReplicas").FirstOrDefault();

        private string[] GetSetting(string configName)
        {
            var str = ConfigurationManager.AppSettings[configName];
            return !string.IsNullOrEmpty(str) ? str.Split('|') : new[] { string.Empty };
        }
    }
}