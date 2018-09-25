using NServiceBus;
using Sfa.Roatp.Events;
using Sfa.Roatp.Events.Types;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.Core.Models;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.NServiceBus;
using ProviderType = Sfa.Roatp.Events.Types.ProviderType;

namespace Sfa.Roatp.Indexer.Infrastructure.Events
{
    public class EventsApiService : IConsumeProviderEvents
    {
        private const string NewRoatpProviderContractType = "ProviderAgreement";
        private const string NewRoatpProviderEvent = "INITIATED";

        private readonly IEventsApi _client;
        private readonly IEventsSettings _eventsApiClientConfiguration;
        private readonly ILog _log;
        private readonly IMessageSession _eventPublisher;

        public EventsApiService(IEventsSettings eventsApiClientConfiguration, ILog log, IMessageSession eventPublisher)
        {
            _eventsApiClientConfiguration = eventsApiClientConfiguration;
            _log = log;
            _eventPublisher = eventPublisher;

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

            PublishRoatpProviderMessage(provider, MessageType.Added);
        }

        public void ProcessChangedProviderEvents(RoatpProviderDocument next, RoatpProviderDocument last)
        {
            _log.Info($"Modified provider", new Dictionary<string, object> { { "ukprn", next.Ukprn } });

            if (_eventsApiClientConfiguration.ApiEnabled && next.RequiresAgreement && last.RequiresAgreement == false)
            {
                _log.Info($"Modified ProviderType", new Dictionary<string, object> { { "Ukprn", next.Ukprn }, { "OldValue", last.ProviderType }, { "NewValue", next.ProviderType } });
                PublishAgreementInitializedEvent(next.Ukprn);
            }

            PublishRoatpProviderMessage(next, MessageType.Modified);
        }

        private void PublishAgreementInitializedEvent(string ukprn)
        {
            var agreementEvent = new AgreementEvent { ContractType = NewRoatpProviderContractType, Event = NewRoatpProviderEvent, ProviderId = ukprn };
            _log.Info($"FCS Provider Event", new Dictionary<string, object> { { "Ukprn", agreementEvent.ProviderId } });
            Task.WaitAll(_client.CreateAgreementEvent(agreementEvent));
        }

        private async void PublishRoatpProviderMessage(RoatpProviderDocument doc, MessageType messageType)
        {
            var roatpProviderMessage = new RoatpProviderMessage()
            {
                MessageType = messageType,
                Ukprn = doc.Ukprn,
                Name = doc.Name,
                StartDate = doc.StartDate,
                EndDate = doc.EndDate,
                ProviderType = (ProviderType)(int)doc.ProviderType,
                ContractedForNonLeviedEmployers = doc.ContractedForNonLeviedEmployers,
                CurrentlyNotStartingNewApprentices = doc.CurrentlyNotStartingNewApprentices,
                NewOrganisationWithoutFinancialTrackRecord = doc.NewOrganisationWithoutFinancialTrackRecord,
                ParentCompanyGuarantee = doc.ParentCompanyGuarantee,
                RequiresAgreement = doc.RequiresAgreement
            };

            _log.Info("Sending Roatp Provider Message", new Dictionary<string, object> { { "Ukprn", roatpProviderMessage.Ukprn }, { "Message Type", roatpProviderMessage.MessageType } });
            await _eventPublisher.Publish(roatpProviderMessage).ConfigureAwait(false);
        }
    }
}