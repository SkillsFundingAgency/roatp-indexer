using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using SFA.DAS.Events.Api.Client.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public class EventsConfig : IEventsApiClientConfiguration
    {
        private readonly IAppServiceSettings _appServiceSettings;

        public EventsConfig(IAppServiceSettings appServiceSettings)
        {
            _appServiceSettings = appServiceSettings;
        }

        public string BaseUrl
        {
            get { return _appServiceSettings.EventsBaseUrl; }
            set { }
        }

        public string ClientToken
        {
            get { return _appServiceSettings.EventsClientToken; }
            set { }
        }
    }
}