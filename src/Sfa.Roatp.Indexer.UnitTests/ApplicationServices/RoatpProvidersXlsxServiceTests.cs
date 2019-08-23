using Moq;
using NUnit.Framework;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Core.Models;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;

namespace Sfa.Roatp.Indexer.UnitTests.ApplicationServices
{
    [TestFixture]
    public class RoatpProvidersXlsxServiceTests
    {
        [TestCase("main provider", ProviderType.MainProvider)]
        [TestCase("Main Provider", ProviderType.MainProvider)]
        [TestCase("Mian provider", ProviderType.Unknown)]
        [TestCase("MainProvider", ProviderType.Unknown)]
        [TestCase(" ", ProviderType.Unknown)]
        [TestCase("", ProviderType.Unknown)]
        [TestCase(null, ProviderType.Unknown)]
        [TestCase("Supporting Provider", ProviderType.SupportingProvider)]
        [TestCase("Employer Provider", ProviderType.EmployerProvider)]
        [TestCase(" Employer Provider", ProviderType.EmployerProvider)]
        [TestCase("Employer Provider ", ProviderType.EmployerProvider)]
        public void ShouldMatchTheProviderType(string input, ProviderType expected)
        {
            // Arrange
            var sut = new RoatpProvidersXlsxService(null, new Mock<ILog>().Object,null);

            // Act
            var result = sut.GetProviderType(input, null, 1);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestCase("Mian provider")]
        [TestCase("MainProvider")]
        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldLogAWarningMessageForUnKnownProvider(string input)
        {
            // Arrange
            var logObject = new Mock<ILog>();
            var sut = new RoatpProvidersXlsxService(null, logObject.Object,null);

            // Act
            var result = sut.GetProviderType(input, null, 1);

            // Assert
            logObject.Verify(x => x.Warn(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()), Times.Once());
        }
    }
}