using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public void ProcessNewProviderEvents(RoatpProviderDocument provider)
        {
            _log.Info($"New provider", new Dictionary<string, object> { { "ukprn", provider.Ukprn } });

            if (_eventsApiClientConfiguration.Enabled)
            {
                if (provider.RequiresAgreement)
                {
                    PublishAgreementInitializedEvent(provider.Ukprn);
                }
            }
        }

        public void ProcessChangedProviderEvents(RoatpProviderDocument next, RoatpProviderDocument last)
        {
            if (_eventsApiClientConfiguration.Enabled)
            {

                if (next.RequiresAgreement && last.RequiresAgreement == false)
                {
                    _log.Info($"Modified ProviderType", new Dictionary<string, object> { { "Ukprn", next.Ukprn }, { "OldValue", last.ProviderType }, { "NewValue", next.ProviderType } });
                    PublishAgreementInitializedEvent(next.Ukprn);
                }

                _log.Info($"Modified provider", new Dictionary<string, object> { { "ukprn", next.Ukprn } });

            }
        }

        private void PublishAgreementInitializedEvent(string ukprn)
        {
            var agreementEvent = new AgreementEvent { ContractType = NewRoatpProviderContractType, Event = NewRoatpProviderEvent, ProviderId = ukprn };
            _log.Info($"FCS Provider Event", new Dictionary<string, object> { { "Ukprn", agreementEvent.ProviderId } });
            Task.WaitAll(_client.CreateAgreementEvent(agreementEvent));
        }
    }
}