using System;
using Sfa.Roatp.Indexer.Core.Settings;

namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public class AppServiceSettings : IAppServiceSettings
    {
        private readonly IProvideSettings _settings;

        public AppServiceSettings(IProvideSettings settingsProvider)
        {
            _settings = settingsProvider;
        }

        public string EventsBaseUrl => _settings.GetSetting("EventsBaseUrl");

        public string EventsClientToken => _settings.GetSetting("EventsClientToken");

        public string EnvironmentName => _settings.GetSetting("EnvironmentName");

        public string VstsRoatpUrl => _settings.GetSetting("VstsRoatpUrl");

        public string GitUsername => _settings.GetSetting("GitUsername");

        public string GitPassword => _settings.GetSetting("GitPassword");

        public string ConnectionString => _settings.GetSetting("StorageConnectionString");

        public string QueueName(Type type)
        {
            var name = $"{type.Name.Replace("IMaintainProviderIndex", "RoatpProvider")}.QueueName";
            return _settings.GetSetting(name).ToLower();
        }
    }
}