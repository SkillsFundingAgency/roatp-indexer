using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Core.Models;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.UnitTests.ApplicationServices
{
    [TestFixture]
    public class ProviderIndexerTests
    {
        [Test]
        public void ShouldFindANewProviderWithNoExistingProviders()
        {
            // Arrange
            var mockMaintainer = new Mock<IMaintainProviderIndex>();
            var sut = new ProviderIndexer(null, mockMaintainer.Object, null, null, new Mock<ILog>().Object);

            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromAlias()).Returns(new List<RoatpProviderDocument>());
            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromIndex(It.IsAny<string>())).Returns(new List<RoatpProviderDocument>
            {
                new RoatpProviderDocument { Ukprn = "12345678"}
            });
            
            // Act
            var creations = sut.IdentifyCreations(It.IsAny<string>());

            // Assert
            Assert.AreEqual(1, creations.Count());
            mockMaintainer.VerifyAll();
        }

        [Test]
        public void ShouldFindANewProviderWithOneExistingProvider()
        {
            // Arrange
            var ukprn = "12345679";

            var mockMaintainer = new Mock<IMaintainProviderIndex>();
            var sut = new ProviderIndexer(null, mockMaintainer.Object, null, null, new Mock<ILog>().Object);

            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromAlias()).Returns(new List<RoatpProviderDocument>
            {
                new RoatpProviderDocument { Ukprn = "12345678"}
            });
            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromIndex(It.IsAny<string>())).Returns(new List<RoatpProviderDocument>
            {
                new RoatpProviderDocument { Ukprn = "12345678"},
                new RoatpProviderDocument { Ukprn = ukprn}
            });

            // Act
            var creations = sut.IdentifyCreations(It.IsAny<string>()).ToList();

            // Assert
            Assert.AreEqual(1, creations.Count());
            Assert.AreEqual(ukprn, creations.First().Ukprn);
            mockMaintainer.VerifyAll();
        }

        [Test]
        public void ShouldFindAModification()
        {
            // Arrange
            var mockMaintainer = new Mock<IMaintainProviderIndex>();
            var sut = new ProviderIndexer(null, mockMaintainer.Object, null, null, new Mock<ILog>().Object);

            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromAlias()).Returns(new List<RoatpProviderDocument>
            {
                new RoatpProviderDocument { Ukprn = "12345678"}
            });
            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromIndex(It.IsAny<string>())).Returns(new List<RoatpProviderDocument>
            {
                new RoatpProviderDocument { Ukprn = "12345678", EndDate = DateTime.Now }
            });

            // Act
            var modifications = sut.IdentifyModifications(It.IsAny<string>()).ToList();

            // Assert
            Assert.AreEqual(1, modifications.Count);
            mockMaintainer.VerifyAll();
        }

        [Test]
        public void ShouldntFindModificationsIfAProviderIsRemoved()
        {
            // Arrange
            var mockMaintainer = new Mock<IMaintainProviderIndex>();
            var sut = new ProviderIndexer(null, mockMaintainer.Object, null, null, new Mock<ILog>().Object);

            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromAlias()).Returns(new List<RoatpProviderDocument>
            {
                new RoatpProviderDocument { Ukprn = "12345678"}
            });
            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromIndex(It.IsAny<string>())).Returns(new List<RoatpProviderDocument>());

            // Act
            var modifications = sut.IdentifyModifications(It.IsAny<string>()).ToList();

            // Assert
            Assert.AreEqual(0, modifications.Count());
            mockMaintainer.VerifyAll();
        }

        [Test]
        public void ShouldntFindAModificationIfItsJustAnAddition()
        {
            // Arrange
            var ukprn = "12345679";

            var mockMaintainer = new Mock<IMaintainProviderIndex>();
            var sut = new ProviderIndexer(null, mockMaintainer.Object, null, null, new Mock<ILog>().Object);

            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromAlias()).Returns(new List<RoatpProviderDocument>
            {
                new RoatpProviderDocument { Ukprn = "12345678"}
            });
            mockMaintainer.Setup(x => x.LoadRoatpProvidersFromIndex(It.IsAny<string>())).Returns(new List<RoatpProviderDocument>
            {
                new RoatpProviderDocument { Ukprn = "12345678"},
                new RoatpProviderDocument { Ukprn = ukprn}
            });

            // Act
            var result = sut.IdentifyModifications(It.IsAny<string>()).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
            mockMaintainer.VerifyAll();
        }
    }
}
