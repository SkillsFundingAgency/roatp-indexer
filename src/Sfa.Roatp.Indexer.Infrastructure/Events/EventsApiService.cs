using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.Infrastructure.Events
{
    public class EventsApiService : IConsumeProviderEvents
    {
        private const string NewRoatpProviderContractType = "ProviderAgreement";
        private const string NewRoatpProviderEvent = "INITIATED";

        private readonly EventsApi _client;
        private readonly IEventsApiSettings _eventsApiClientConfiguration;
        private readonly ILog _log;

        public EventsApiService(IEventsApiSettings eventsApiClientConfiguration, ILog log)
        {
            _client = new EventsApi(eventsApiClientConfiguration);
            _eventsApiClientConfiguration = eventsApiClientConfiguration;
            _log = log;
        }

        public Task NewProvider(string ukprn)
        {
            _log.Info($"New provider {ukprn}");

            if (_eventsApiClientConfiguration.Enabled)
            {
                var agreementEvent = new AgreementEvent { ContractType = NewRoatpProviderContractType, Event = NewRoatpProviderEvent, ProviderId = ukprn };
                return _client.CreateAgreementEvent(agreementEvent);
            }

            return Task.FromResult(0);
        }
    }
}