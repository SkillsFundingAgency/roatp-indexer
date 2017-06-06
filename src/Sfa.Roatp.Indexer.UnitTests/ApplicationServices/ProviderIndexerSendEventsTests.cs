using NUnit.Framework;
using Moq;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using SFA.DAS.NLog.Logger;
using Sfa.Roatp.Indexer.Core.Models;
using System.Collections.Generic;
using System;

namespace Sfa.Roatp.Indexer.UnitTests.ApplicationServices
{
    [TestFixture]
    public class ProviderIndexerSendEventsTests
    {
        private Mock<IIndexSettings<IMaintainProviderIndex>> _settings;
        private Mock<IMaintainProviderIndex> _indexMaintainer;
        private Mock<IGetRoatpProviders> _providerDataService;
        private Mock<IConsumeProviderEvents> _providerEventConsumer;
        private Mock<ILog> _log;
        private ProviderIndexer _sut;
        private NewProviderStats _newProviderStats;

        private readonly string newproviderukprn = "44444444";
        private readonly string removedproviderukprn = "55555555";
        private readonly string changedproviderukprn = "33333333";
        private readonly DateTime enddate = DateTime.Now.AddDays(-1);

        [SetUp]
        public void TestSetup()
        {
            _settings = new Mock<IIndexSettings<IMaintainProviderIndex>>();
            _indexMaintainer = new Mock<IMaintainProviderIndex>();
            _providerDataService = new Mock<IGetRoatpProviders>();
            _providerEventConsumer = new Mock<IConsumeProviderEvents>();
            _log = new Mock<ILog>();

            _sut = new ProviderIndexer(_settings.Object,_indexMaintainer.Object,_providerDataService.Object,_providerEventConsumer.Object,_log.Object);

            // Arrange
            _indexMaintainer.Setup(m => m.LoadRoatpProvidersFromIndex(It.IsAny<string>())).Returns(LoadNewRoatpProvidersFromIndex);
            _indexMaintainer.Setup(m => m.LoadRoatpProvidersFromAlias()).Returns(LoadoldRoatpProvidersFromIndex);

            // Act 
            _newProviderStats = _sut.SendEvents(It.IsAny<string>());

        }

        [Test]
        public void ShouldIdentityCreationAndModification()
        {

            // Assert
            _providerEventConsumer.Verify(v => v.NewProvider(It.Is<RoatpProviderDocument>(provider => provider.Ukprn == newproviderukprn)));
            _providerEventConsumer.Verify(v => v.ChangedProvider(
                It.Is<RoatpProviderDocument>(p => p.Ukprn == changedproviderukprn && p.ProviderType == ProviderType.EmployerProvider), 
                It.Is<RoatpProviderDocument>(p => p.Ukprn == changedproviderukprn && p.ProviderType == ProviderType.SupportingProvider)));

            _providerEventConsumer.Verify(v => v.ChangedProvider(
                It.Is<RoatpProviderDocument>(p => p.Ukprn == removedproviderukprn && p.EndDate == enddate),
                It.Is<RoatpProviderDocument>(p => p.Ukprn == removedproviderukprn && p.EndDate == null)));
        }


        [Test]
        public void ShouldCallEventNotification()
        {

            // Assert
            _providerEventConsumer.Verify(v => v.NewProvider(It.IsAny<RoatpProviderDocument>()), Times.Once);
            _providerEventConsumer.Verify(v => v.ChangedProvider(It.IsAny<RoatpProviderDocument>(), It.IsAny<RoatpProviderDocument>()), Times.Exactly(2));

        }

        [Test]
        public void ShouldSendCorrectstats()
        {
            var expectedstats = new NewProviderStats
            {
                TotalCount = 1,
                TotalEmployerProviders = 0, TotalMainProviders = 1, TotalSupportProviders = 0
            };
            Assert.True(expectedstats.TotalCount == _newProviderStats.TotalCount &&
                        expectedstats.TotalEmployerProviders == _newProviderStats.TotalEmployerProviders &&
                        expectedstats.TotalMainProviders == _newProviderStats.TotalMainProviders &&
                        expectedstats.TotalSupportProviders == _newProviderStats.TotalSupportProviders);
        }

        private IEnumerable<RoatpProviderDocument> LoadoldRoatpProvidersFromIndex()
        {
            return new List<RoatpProviderDocument>()
            {
                new RoatpProviderDocument
                {
                    Ukprn = "11111111",
                    ProviderType = ProviderType.MainProvider,
                    StartDate = DateTime.Now.AddDays(-10),
                },
                new RoatpProviderDocument
                {
                    Ukprn = "22222222",
                    ProviderType = ProviderType.EmployerProvider,
                    StartDate = DateTime.Now.AddDays(-10),
                },
                new RoatpProviderDocument
                {
                    Ukprn = changedproviderukprn,
                    ProviderType = ProviderType.SupportingProvider,
                    StartDate = DateTime.Now.AddDays(-10),
                },
                new RoatpProviderDocument
                {
                    Ukprn = removedproviderukprn,
                    ProviderType = ProviderType.MainProvider,
                    StartDate = DateTime.Now.AddDays(-10)
                }
            };
        }

        private IEnumerable<RoatpProviderDocument> LoadNewRoatpProvidersFromIndex()
        {
            return new List<RoatpProviderDocument>()
            {
                new RoatpProviderDocument
                {
                    Ukprn = "11111111",
                    ProviderType = ProviderType.MainProvider,
                    StartDate = DateTime.Now.AddDays(-10),
                },
                new RoatpProviderDocument
                {
                    Ukprn = "22222222",
                    ProviderType = ProviderType.EmployerProvider,
                    StartDate = DateTime.Now.AddDays(-10),
                },
                new RoatpProviderDocument
                {
                    Ukprn = changedproviderukprn,
                    ProviderType = ProviderType.EmployerProvider,
                    StartDate = DateTime.Now.AddDays(-10),
                },
                new RoatpProviderDocument
                {
                    Ukprn = newproviderukprn,
                    ProviderType = ProviderType.MainProvider,
                    StartDate = DateTime.Now.AddDays(-10),
                },
                new RoatpProviderDocument
                {
                    Ukprn = removedproviderukprn,
                    ProviderType = ProviderType.MainProvider,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = enddate,
                }
            };
        }
    }
}
