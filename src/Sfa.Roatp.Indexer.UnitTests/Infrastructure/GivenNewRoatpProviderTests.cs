﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
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
        private Mock<IMessageSession> _eventPublisher;
        private EventsApiService _sut;


        [SetUp]
        public void TestSetup()
        {
            _eventsApiClientConfiguration = new Mock<IEventsSettings>(MockBehavior.Strict);
            _log = new Mock<ILog>();
            _eventPublisher = new Mock<IMessageSession>();
            
            _eventsApiClientConfiguration.Setup(s => s.ApiEnabled).Returns(false);

            _sut = new EventsApiService(_eventsApiClientConfiguration.Object,_log.Object,_eventPublisher.Object);
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

            var message = new RoatpProviderMessage()
            {
                
                MessageType = MessageType.Added,
                Ukprn = roatpDocument.Ukprn,
                Name = roatpDocument.Name,
                StartDate = roatpDocument.StartDate,
                EndDate = roatpDocument.EndDate,
                ProviderType = (ProviderType)(int) roatpDocument.ProviderType,
                ContractedForNonLeviedEmployers = roatpDocument.ContractedForNonLeviedEmployers,
                CurrentlyNotStartingNewApprentices = roatpDocument.CurrentlyNotStartingNewApprentices,
                NewOrganisationWithoutFinancialTrackRecord = roatpDocument.NewOrganisationWithoutFinancialTrackRecord,
                ParentCompanyGuarantee = roatpDocument.ParentCompanyGuarantee,
                RequiresAgreement = roatpDocument.RequiresAgreement
            };

            _sut.ProcessNewProviderEvents(roatpDocument);

            _eventPublisher.Verify(s => s.Publish(It.Is<RoatpProviderMessage>(p => p == message)));

            //_eventPublisher.Events.Count().Should().Be(1);
            //_eventPublisher.Events.FirstOrDefault().Should().BeEquivalentTo(message);

        }
    }
}
