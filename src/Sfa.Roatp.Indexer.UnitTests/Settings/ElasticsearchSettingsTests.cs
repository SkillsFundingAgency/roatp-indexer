using Moq;
using NUnit.Framework;
using Sfa.Roatp.Indexer.Core.Settings;
using Sfa.Roatp.Indexer.Infrastructure.Settings;

namespace Sfa.Roatp.Indexer.UnitTests.Settings
{
    [TestFixture]
    public class ElasticsearchSettingsTests
    {
        private Mock<IProvideSettings> _providerSettingsMock;
        private ElasticsearchSettings _sut;

        [SetUp]
        public void Init()
        {
            _providerSettingsMock = new Mock<IProvideSettings>();
            _sut = new ElasticsearchSettings(_providerSettingsMock.Object);
        }

        [TestCase("1|2|3|4", "1")]
        [TestCase("5 | 2 | 3 | 4", "5")]
        [TestCase("4 | 2 | 3 | ", "4")]
        [TestCase("", "")]
        public void ShouldReturnValidShardSetting(string input, string expected)
        {
            _providerSettingsMock.Setup(x => x.GetSetting(It.IsAny<string>())).Returns(input);

            var setting = _sut.RoatpProviderIndexShards;

            Assert.AreEqual(setting, expected);
        }

        [TestCase("1|2|3|4", "1")]
        [TestCase("7 | 2 | 3 | 4", "7")]
        [TestCase("8 | 2 | 3 | ", "8")]
        [TestCase("", "")]
        public void ShouldReturnValidReplicasSetting(string input, string expected)
        {
            _providerSettingsMock.Setup(x => x.GetSetting(It.IsAny<string>())).Returns(input);

            var setting = _sut.RoatpProviderIndexShards;

            Assert.AreEqual(setting, expected);
        }
    }
}
