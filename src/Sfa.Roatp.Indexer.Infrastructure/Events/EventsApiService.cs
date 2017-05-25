using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.Core.Models;
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

        public void NewProvider(RoatpProviderDocument provider)
        {
            _log.Info($"New provider", new Dictionary<string, object> { { "ukprn", provider.Ukprn } });

            if (_eventsApiClientConfiguration.Enabled)
            {
                if (provider.ProviderType == ProviderType.EmployerProvider || provider.ProviderType == ProviderType.MainProvider)
                {
                    var agreementEvent = new AgreementEvent {ContractType = NewRoatpProviderContractType, Event = NewRoatpProviderEvent, ProviderId = provider.Ukprn};
                    _log.Info($"FCS Provider Event", new Dictionary<string, object> { { "Ukprn", agreementEvent.ProviderId } });
                    Task.WaitAll(_client.CreateAgreementEvent(agreementEvent));
                }
            }
        }

        public void ChangedProvider(RoatpProviderDocument next, RoatpProviderDocument last)
        {
            if (_eventsApiClientConfiguration.Enabled)
            {
                if (next.ProviderType != last.ProviderType)
                {
                    _log.Info($"Modified ProviderType", new Dictionary<string, object> {{"Ukprn", next.Ukprn}, {"OldValue", last.ProviderType}, {"NewValue", next.ProviderType}});
                    if ((next.ProviderType == ProviderType.EmployerProvider || next.ProviderType == ProviderType.MainProvider) &&
                        (last.ProviderType == ProviderType.SupportingProvider || last.ProviderType == ProviderType.Unknown))
                    {
                        var agreementEvent = new AgreementEvent {ContractType = NewRoatpProviderContractType, Event = NewRoatpProviderEvent, ProviderId = next.Ukprn};
                        _log.Info($"FCS Provider Event", new Dictionary<string, object> { { "Ukprn", agreementEvent.ProviderId } });
                        Task.WaitAll(_client.CreateAgreementEvent(agreementEvent));
                    }
                }
                else
                {
                    _log.Info($"Modified provider", new Dictionary<string, object> { { "ukprn", next.Ukprn } });
                }
            }
        }
    }
}