﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Indexer.WorkerRole.DependencyResolution;
using System;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;

namespace Esfa.Roatp.Xslx.IntegrationTests
{
    [TestClass]
    public class RoatpXslxTests
    {
        List<RoatpProvider> results;
        IGetRoatpProviders prodSut;

        private TestContext testContextInstance;


        [TestInitialize]
        public void Init()
        {
            // Arrange
            var container = IoC.Initialize();
            var sut = container.GetInstance<IGetRoatpProviders>();

            using (var prodContainer = container.GetNestedContainer())
            {
                prodContainer.Configure(x =>
                {
                    x.For<IAppServiceSettings>().Use<ProdAppSettings>();
                });
                prodSut = prodContainer.GetInstance<IGetRoatpProviders>();
            }

            // Act
            results = sut.GetRoatpData().ToList();
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

        [TestMethod]
        public void RoatpShouldntRemoveProviders()
        {
            var existing = prodSut.GetRoatpData().ToList();

            // Assert
            var missing = existing.Where(x => !results.Select(y => y.Ukprn).Contains(x.Ukprn)).ToList();
            Assert.AreEqual(0, missing.Count, $"missing [{string.Join(", ", missing.Select(x => x.Ukprn))}]");
        }

        [TestMethod]
        public void GetRoatpProviderCount()
        {
            var activeProviders = results.Where(x => x.EndDate.HasValue == false).Count();
            this.testContextInstance.WriteLine($"{activeProviders} active roatp providers found out of {results.Count} roatp providers");

            var existing = prodSut.GetRoatpData().ToList();
            var added = results.Where(x => !existing.Select(y => y.Ukprn).Contains(x.Ukprn)).ToList();
            this.testContextInstance.WriteLine($"{added.Count()} new roatp providers are added.");
            this.testContextInstance.WriteLine($"{string.Join(Environment.NewLine, added.Select(x => x.Ukprn))}");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
    }
}
