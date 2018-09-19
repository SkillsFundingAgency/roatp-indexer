using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.Core.Models;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sfa.Roatp.Indexer.Infrastructure.Events
{
    public class EventsApiService : IConsumeProviderEvents
    {
        private const string NewRoatpProviderContractType = "ProviderAgreement";
        private const string NewRoatpProviderEvent = "INITIATED";

        private readonly EventsApi _client;
        private readonly IEventsSettings _eventsApiClientConfiguration;
        private readonly ILog _log;

        public EventsApiService(IEventsSettings eventsApiClientConfiguration, ILog log)
        {
            _eventsApiClientConfiguration = eventsApiClientConfiguration;
            _log = log;

            if (_eventsApiClientConfiguration.ApiEnabled)
            {
                _client = new EventsApi(eventsApiClientConfiguration);
            }
        }

        public void ProcessNewProviderEvents(RoatpProviderDocument provider)
        {
            _log.Info($"New provider", new Dictionary<string, object> { { "ukprn", provider.Ukprn } });

            if (_eventsApiClientConfiguration.ApiEnabled && provider.RequiresAgreement)
            {
                PublishAgreementInitializedEvent(provider.Ukprn);
            }

        }

        public void ProcessChangedProviderEvents(RoatpProviderDocument next, RoatpProviderDocument last)
        {
            _log.Info($"Modified provider", new Dictionary<string, object> { { "ukprn", next.Ukprn } });

            if (_eventsApiClientConfiguration.ApiEnabled && next.RequiresAgreement && last.RequiresAgreement == false)
            {
                _log.Info($"Modified ProviderType", new Dictionary<string, object> { { "Ukprn", next.Ukprn }, { "OldValue", last.ProviderType }, { "NewValue", next.ProviderType } });
                PublishAgreementInitializedEvent(next.Ukprn);
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