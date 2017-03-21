using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace Sfa.Roatp.Indexer.Infrastructure.Events
{
    public class EventsApiService : IConsumeProviderEvents
    {
        private const string NewRoatpProviderContractType = "ProviderAgreement";
        private const string NewRoatpProviderEvent = "INITIATED";

        private readonly EventsApi _client;
        private readonly IEventsApiSettings _eventsApiClientConfiguration;

        public EventsApiService(IEventsApiSettings eventsApiClientConfiguration)
        {
            _client = new EventsApi(eventsApiClientConfiguration);
            _eventsApiClientConfiguration = eventsApiClientConfiguration;
        }

        public Task NewProvider(string ukprn)
        {
            if (_eventsApiClientConfiguration.Enabled)
            {
                var agreementEvent = new AgreementEvent { ContractType = NewRoatpProviderContractType, Event = NewRoatpProviderEvent, ProviderId = ukprn };
                return _client.CreateAgreementEvent(agreementEvent);
            }

            return Task.FromResult(0);
        }
    }
}