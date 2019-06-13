using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;
using Sfa.Roatp.Events;
using Sfa.Roatp.Events.Types;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Indexer.Core.Settings;
using Sfa.Roatp.Indexer.Infrastructure.Events;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.Testing;
using ProviderType = Sfa.Roatp.Events.Types.ProviderType;

namespace Sfa.Roatp.Indexer.UnitTests.Infrastructure
{
    [TestFixture]
    public class GivenNewRoatpProviderTests
    {
        private Mock<IEventsSettings> _eventsApiClientConfiguration;
        private Mock<ILog> _log;
        private TestableMessageSession _messageSession;
        private EventsApiService _sut;


        [SetUp]
        public void TestSetup()
        {
            _eventsApiClientConfiguration = new Mock<IEventsSettings>(MockBehavior.Strict);
            _log = new Mock<ILog>();
            _messageSession = new TestableMessageSession();
            
            _eventsApiClientConfiguration.Setup(s => s.ApiEnabled).Returns(false);

            _sut = new EventsApiService(_eventsApiClientConfiguration.Object,_log.Object,_messageSession);
        }

        [Test]
        public void ShouldSendNewRoatpProviderEvent()
        {
            var roatpDocument = new RoatpProviderDocument()
            {
                Ukprn = "12345",
                ContractedForNonLeviedEmployers = true,
                CurrentlyNotStartingNewApprentices = true,
                StartDate = DateTime.Now.AddYears(-2),
                EndDate = null,
                Name = "Test Provider",
                NewOrganisationWithoutFinancialTrackRecord = false,
                ParentCompanyGuarantee = true,
                ProviderType = Sfa.Roatp.Indexer.Core.Models.ProviderType.MainProvider
            };

            var message = new RoatpProviderUpdated()
            {
                
                MessageType = MessageType.Added,
                Ukprn = roatpDocument.Ukprn,
                Name = roatpDocument.Name,
                StartDate = roatpDocument.StartDate,
                EndDate = roatpDocument.EndDate,
                ApplicationDeterminedDate = roatpDocument.ApplicationDeterminedDate,
                ProviderType = (ProviderType)(int) roatpDocument.ProviderType,
                ContractedForNonLeviedEmployers = roatpDocument.ContractedForNonLeviedEmployers,
                CurrentlyNotStartingNewApprentices = roatpDocument.CurrentlyNotStartingNewApprentices,
                NewOrganisationWithoutFinancialTrackRecord = roatpDocument.NewOrganisationWithoutFinancialTrackRecord,
                ParentCompanyGuarantee = roatpDocument.ParentCompanyGuarantee,
                RequiresAgreement = roatpDocument.RequiresAgreement
            };

            _sut.ProcessNewProviderEvents(roatpDocument);

            _messageSession.PublishedMessages.Count().Should().Be(1);
            _messageSession.PublishedMessages.FirstOrDefault()?.Message.Should().BeEquivalentTo(message);

        }
    }
}
