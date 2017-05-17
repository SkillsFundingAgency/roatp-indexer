﻿using System.Collections.Generic;
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
        List<RoatpProvider> results;

        [TestInitialize]
        public void Init()
        {
            // Arrange
            var container = IoC.Initialize();
            var sut = container.GetInstance<IGetRoatpProviders>();

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
    }
}