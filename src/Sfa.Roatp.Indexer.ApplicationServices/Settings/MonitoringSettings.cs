using Sfa.Roatp.Indexer.ApplicationServices.Monitoring;
using Sfa.Roatp.Indexer.Core.Settings;

namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public class MonitoringSettings : IMonitoringSettings
    {
        private readonly IProvideSettings _settings;

        public MonitoringSettings(IProvideSettings settingsProvider)
        {
            _settings = settingsProvider;
        }

        public string StatusCakeUrl => _settings.GetSetting("StatusCakeUrl");
    }
}
