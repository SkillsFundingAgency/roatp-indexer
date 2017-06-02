using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Indexer.WorkerRole.DependencyResolution;
using System;

namespace Esfa.Roatp.Xslx.IntegrationTests
{
    [TestClass]
    public class RoatpXslxTests
    {
        List<RoatpProvider> results;

        [TestInitialize]
        public void Init()
        {
            // Arrange
            var container = IoC.Initialize();
            container.Configure();
            var sut = container.GetInstance<IGetRoatpProviders>();

            // Act
            results = sut.GetRoatpData().ToList();
            //results = GetRoatpProvider(); To test the tests
        }

        [TestMethod]
        public void RoatpShouldntHaveUnknownProviders()
        {
            // Assert
            var invalidTypes = results.Where(x => x.ProviderType == ProviderType.Unknown).ToList();
            Assert.AreEqual(0, invalidTypes.Count, $"There are {invalidTypes.Count} unknown providers [{string.Join(", ", invalidTypes.Select(x => x.Ukprn))}]" );
        }

        [TestMethod]
        public void RoatpShouldntHaveInvalidUkprns()
        {
            // Assert
            var invalidUkprns = results.Where(x => x.Ukprn.Length != 8).ToList();
            Assert.AreEqual(0, invalidUkprns.Count, $"There are {invalidUkprns.Count} invalid ukprns [{string.Join(", ", invalidUkprns.Select(x => x.Ukprn))}]");
        }

        [TestMethod]
        public void RoatpShouldntHaveAnyDuplicateUkprns()
        {
            var duplicates = results.GroupBy(x => x.Ukprn).Where(g => g.Count() > 1).Select(x => x.Key).ToList();
            Assert.AreEqual(0, duplicates.Count, $"There are {duplicates.Count} duplicate ukprns [{string.Join(", ", duplicates)}]");
        }

        private List<RoatpProvider> GetRoatpProvider()
        {
            return new List<RoatpProvider>
            {
                new RoatpProvider
                {
                    Ukprn = "12345679",
                    ProviderType = ProviderType.EmployerProvider,
                    ContractedForNonLeviedEmployers = true,
                    NewOrganisationWithoutFinancialTrackRecord = true,
                    ParentCompanyGuarantee = true,
                    Name = "Sample",
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = null
                },
                new RoatpProvider
                {
                    Ukprn = "12345678",
                    ProviderType = ProviderType.EmployerProvider,
                    ContractedForNonLeviedEmployers = true,
                    NewOrganisationWithoutFinancialTrackRecord = true,
                    ParentCompanyGuarantee = true,
                    Name = "Sample",
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = null
                },
                 new RoatpProvider
                {
                    Ukprn = "82345678",
                    ProviderType = ProviderType.SupportingProvider,
                    ContractedForNonLeviedEmployers = true,
                    NewOrganisationWithoutFinancialTrackRecord = true,
                    ParentCompanyGuarantee = true,
                    Name = "Sample",
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = null
                }, new RoatpProvider
                {
                    Ukprn = "32345678",
                    ProviderType = ProviderType.MainProvider,
                    ContractedForNonLeviedEmployers = true,
                    NewOrganisationWithoutFinancialTrackRecord = true,
                    ParentCompanyGuarantee = true,
                    Name = "Sample",
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = null
                }
            };
        }
    }
}
