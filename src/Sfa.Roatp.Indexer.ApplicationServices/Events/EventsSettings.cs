using System.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Settings;
using SFA.DAS.Events.Api.Client.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public class EventsSettings : IEventsApiSettings
    {
        private readonly IProvideSettings _settings;

        public EventsSettings(IProvideSettings settings)
        {
            _settings = settings;
        }

        public bool Enabled => bool.Parse(_settings.GetSetting("FeatureToggle.EventsApiFeature")??"false");

        public string BaseUrl => _settings.GetSetting("EventsApi.BaseUrl");

        public string ClientToken => _settings.GetSetting("EventsApi.ClientToken");
    }
}