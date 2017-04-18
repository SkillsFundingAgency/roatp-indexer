using Moq;
using NUnit.Framework;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Core.Models;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.UnitTests.ApplicationServices
{
    [TestFixture]
    public class RoatpProvidersXlsxServiceTests
    {
        [TestCase("main provider", ProviderType.MainProvider)]
        [TestCase("Main Provider", ProviderType.MainProvider)]
        [TestCase("Mian provider", ProviderType.Unknown)]
        [TestCase("Supporting Provider", ProviderType.SupportingProvider)]
        [TestCase("Employer Provider", ProviderType.EmployerProvider)]
        [TestCase(" Employer Provider", ProviderType.EmployerProvider)]
        public void ShouldMatchTheProviderType(string input, ProviderType expected)
        {
            // Arrange
            var sut = new RoatpProvidersXlsxService(null, new Mock<ILog>().Object);

            // Act
            var result = sut.GetProviderType(input);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}