using System.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using SFA.DAS.Events.Api.Client.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public class EventsSettings : IEventsApiClientConfiguration
    {


        public string BaseUrl => ConfigurationManager.AppSettings["EventsApi.BaseUrl"];

        public string ClientToken => ConfigurationManager.AppSettings["EventsApi.ClientToken"];
    }
}