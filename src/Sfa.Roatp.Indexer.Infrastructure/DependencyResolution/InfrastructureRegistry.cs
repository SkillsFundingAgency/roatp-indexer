using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Sfa.Das.Sas.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.Infrastructure.Elasticsearch;
using Sfa.Roatp.Indexer.Infrastructure.Elasticsearch.Configuration;
using Sfa.Roatp.Indexer.Infrastructure.Events;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.Infrastructure.DependencyResolution
{
    public class InfrastructureRegistry : StructureMap.Registry
    {
        public InfrastructureRegistry()
        {
            For<ILog>().Use(x => new NLogLogger(x.ParentType, null, GetProperties())).AlwaysUnique();
            For<IInfrastructureSettings>().Use<InfrastructureSettings>();
            For<IMaintainProviderIndex>().Use<ElasticsearchProviderIndexMaintainer>();
            For<IElasticsearchSettings>().Use<ElasticsearchSettings>();
            For<IElasticsearchConfiguration>().Use<ElasticsearchConfiguration>();
            For<IElasticsearchRoatpDocumentMapper>().Use<ElasticsearchRoatpDocumentMapper>();
            For<IElasticsearchCustomClient>().Use<ElasticsearchCustomClient>();
            For<IConsumeProviderEvents>().Use<EventsApiService>();

            if (Debugger.IsAttached)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            }

        }

        private IDictionary<string, object> GetProperties()
        {
            var properties = new Dictionary<string, object>();
            properties.Add("Version", GetVersion());
            return properties;
        }

        private string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}