using System.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Settings;
using SFA.DAS.Events.Api.Client.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public class EventsSettings : IEventsSettings
    {
        private readonly IProvideSettings _settings;

        public EventsSettings(IProvideSettings settings)
        {
            _settings = settings;
        }

        public bool ApiEnabled => bool.Parse(_settings.GetSetting("FeatureToggle.EventsApiFeature")??"false");
        public string ServiceBusConnectionString => _settings.GetSetting("Events.NServiceBus.ConnectionString");

        public string BaseUrl => _settings.GetSetting("EventsApi.BaseUrl");

        public string ClientToken => _settings.GetSetting("EventsApi.ClientToken");
    }
}