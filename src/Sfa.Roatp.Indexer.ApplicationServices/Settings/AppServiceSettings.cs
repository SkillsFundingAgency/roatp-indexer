using System;
using Microsoft.Azure;
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

        public virtual string EnvironmentName => _settings.GetSetting("WorkerRole.EnvironmentName");

        public string VstsRoatpUrl => string.Format(_settings.GetSetting("Vsts.RoatpUrlFormat"), EnvironmentName);
        
        public string GitUsername => _settings.GetSetting("GitUsername");

        public string GitPassword => _settings.GetSetting("GitPassword");
    }
}