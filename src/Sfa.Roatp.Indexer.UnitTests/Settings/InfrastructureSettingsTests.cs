using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sfa.Roatp.Indexer.Core.Settings;
using Sfa.Roatp.Indexer.Infrastructure.Settings;

namespace Sfa.Roatp.Indexer.UnitTests.Settings
{
    [TestFixture]
    public class InfrastructureSettingsTests
    {
        private Mock<IProvideSettings> _providerSettingsMock;
        private InfrastructureSettings _sut;

        [SetUp]
        public void Init()
        {
            _providerSettingsMock = new Mock<IProvideSettings>();
            _sut = new InfrastructureSettings(_providerSettingsMock.Object);
        }

        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("tru", false)]
        [TestCase("fals", false)]
        public void ShouldHandleIncorrectElk5FeatureSetting(string input, bool expected)
        {
            _providerSettingsMock.Setup(x => x.GetSetting(It.IsAny<string>())).Returns(input);

            var setting = _sut.Elk5Enabled;

            Assert.AreEqual(setting, expected);
        }

        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("tru", false)]
        [TestCase("fals", false)]
        public void ShouldHandleIncorrectIgnoreSslCertificateFeatureSetting(string input, bool expected)
        {
            _providerSettingsMock.Setup(x => x.GetSetting(It.IsAny<string>())).Returns(input);

            var setting = _sut.IgnoreSslCertificateEnabled;

            Assert.AreEqual(setting, expected);
        }

        [TestCase("https://10.10.10.10:9200", 1, "https://10.10.10.10:9200")]
        [TestCase("https://11.10.10.10:9200, https://10.10.10.10:9200", 2, "https://11.10.10.10:9200")]
        [TestCase("https://12.10.10.10, https://10.10.10.10:9200", 2, "https://12.10.10.10")]
        public void ShouldReturnFormattedElasticsearchUrlSetting(string input, int listAmount, string firstUrl)
        {
            _providerSettingsMock.Setup(x => x.GetSetting(It.IsAny<string>())).Returns(input);

            var setting = _sut.ElasticServerUrls.ToList();

            Assert.AreEqual(setting.Count, listAmount);
            Assert.AreEqual(setting.First(), new Uri(firstUrl));
        }
    }
}
