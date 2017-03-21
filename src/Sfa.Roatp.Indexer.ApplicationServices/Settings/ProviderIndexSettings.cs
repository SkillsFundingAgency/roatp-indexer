using System.Configuration;
using Microsoft.Azure;
using Sfa.Roatp.Indexer.Core.Settings;

namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public class ProviderIndexSettings : IIndexSettings<IMaintainProviderIndex>
    {
        private readonly IProvideSettings _settings;

        public ProviderIndexSettings(IProvideSettings settingsProvider)
        {
            _settings = settingsProvider;
        }

        public string IndexesAlias => string.Format(_settings.GetSetting("ElasticSearch.IndexAliasFormat"), _settings.GetSetting("WorkerRole.EnvironmentName")).ToLower();

        public string PauseTime => _settings.GetSetting("WorkerRole.RunFrequencySeconds");
    }
}