using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Indexer.WorkerRole.DependencyResolution;

namespace Esfa.Roatp.Xslx.IntegrationTests
{
    [TestClass]
    public class RoatpXslxTests
    {
        [TestMethod]
        public void ShouldLoadValidSpreadsheet()
        {
            // Arrange
            var container = IoC.Initialize();
            var sut = container.GetInstance<IGetRoatpProviders>();

            // Act
            var results = sut.GetRoatpData().ToList();

            // Assert
            var invalidTypes = results.Where(x => x.ProviderType == ProviderType.Unknown).ToList();
            Assert.AreEqual(0, invalidTypes.Count, $"There are {invalidTypes.Count} unknown providers [{string.Join(", ", invalidTypes.Select(x => x.Ukprn))}]" );
            var invalidUkprns = results.Where(x => x.Ukprn.Length != 8).ToList();
            Assert.AreEqual(0, invalidUkprns.Count, $"There are {invalidUkprns.Count} invalid ukprns [{string.Join(", ", invalidUkprns.Select(x => x.Ukprn))}]");
        }
    }
}
