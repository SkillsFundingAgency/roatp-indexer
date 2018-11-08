using SFA.DAS.Events.Api.Client.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public interface IEventsSettings : IEventsApiClientConfiguration
    {
        bool ApiEnabled { get; }
        string ServiceBusConnectionString { get; }
    }
}