using NUnit.Framework;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.UnitTests.Core.Models
{
    [TestFixture]
    public class RoatpProviderDocumentTests
    {
        private RoatpProviderDocument original;

        [SetUp]
        public void Setup()
        {
            original = CreateProvider();
        }

        [Test]
        public void ShouldNoticeAChangeInProviderType()
        {
            // Arrange
            var current = CreateProvider();
            current.ProviderType = ProviderType.MainProvider;

            // Act
            var result = current.Equals(original);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldTellThatTwoProvidersAreIdentical()
        {
            // Arrange
            var current = CreateProvider();

            // Act
            var result = current.Equals(original);

            // Assert
            Assert.IsTrue(result);
        }

        private static RoatpProviderDocument CreateProvider()
        {
            return new RoatpProviderDocument
            {
                Name = "Sample",
                Ukprn = "12345678",
                ProviderType = ProviderType.SupportingProvider
            };
        }
    }
}