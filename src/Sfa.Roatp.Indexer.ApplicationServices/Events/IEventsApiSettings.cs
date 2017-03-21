using SFA.DAS.Events.Api.Client.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public interface IEventsApiSettings : IEventsApiClientConfiguration
    {
        bool Enabled { get; }
    }
}