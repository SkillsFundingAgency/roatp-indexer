using System.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using SFA.DAS.Events.Api.Client.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public class EventsConfig : IEventsApiClientConfiguration
    {
        public string BaseUrl => ConfigurationManager.AppSettings["EventsBaseUrl"];

        public string ClientToken => ConfigurationManager.AppSettings["EventsClientToken"];
    }
}