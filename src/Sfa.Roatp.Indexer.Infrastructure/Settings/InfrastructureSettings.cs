using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Sfa.Roatp.Indexer.Core.Settings;

namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public class InfrastructureSettings : IInfrastructureSettings
    {
        private readonly IProvideSettings _settingsProvider;

        public InfrastructureSettings(IProvideSettings settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public string FrameworkIdFormat => ConfigurationManager.AppSettings["FrameworkIdFormat"];

        public string UkrlpStakeholderId => ConfigurationManager.AppSettings["UkrlpStakeholderId"];

        public string UkrlpProviderStatus => ConfigurationManager.AppSettings["UkrlpProviderStatus"];

        public string UkrlpQueryId => ConfigurationManager.AppSettings["UkrlpQueryId"];

        public string UkrlpServiceEndpointUrl => ConfigurationManager.AppSettings["UKRLP_EndpointUri"];

        public int UkrlpRequestUkprnBatchSize => int.Parse(ConfigurationManager.AppSettings["UkrlpRequestUkprnBatchSize"]);

        public string CourseDirectoryUri => ConfigurationManager.AppSettings["CourseDirectoryUri"];

        public string EnvironmentName => ConfigurationManager.AppSettings["EnvironmentName"];

        public string ApplicationName => ConfigurationManager.AppSettings["ApplicationName"];

        public double HttpClientTimeout => Convert.ToDouble(ConfigurationManager.AppSettings["HttpClient.Timeout"]);

        public string AchievementRateDataBaseConnectionString => _settingsProvider.GetSetting("AchievementRateDataBaseConnectionString");

        public IEnumerable<Uri> ElasticServerUrls => GetElasticIPs("ElasticServerUrls");

        public IEnumerable<Uri> GetElasticIPs(string appSetting)
        {
            var urlsString = _settingsProvider.GetSetting(appSetting).Split(',');

            return urlsString.Select(url => new Uri(url));
        }
    }
}