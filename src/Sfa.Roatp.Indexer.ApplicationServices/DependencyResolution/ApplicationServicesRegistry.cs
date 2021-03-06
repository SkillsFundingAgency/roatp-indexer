﻿using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.ApplicationServices.Monitoring;
using Sfa.Roatp.Indexer.ApplicationServices.RoatpClient;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Services;
using SFA.DAS.Events.Api.Client.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.DependencyResolution
{
    public class ApplicationServicesRegistry : StructureMap.Registry
    {
        public ApplicationServicesRegistry()
        {
            For<IGetRoatpProviders>().Use<RoatpProvidersXlsxService>();
            For<IIndexSettings<IMaintainProviderIndex>>().Use<ProviderIndexSettings>();
            For<IMonitoringSettings>().Use<MonitoringSettings>();
            For<IAppServiceSettings>().Use<AppServiceSettings>();
            For<ITokenService>().Use<TokenService>();
            For<IIndexerService<IMaintainProviderIndex>>().Use<IndexerService<IMaintainProviderIndex>>();
            For<IGenericIndexerHelper<IMaintainProviderIndex>>().Use<ProviderIndexer>();
            For<IEventsApiClientConfiguration>().Use<EventsSettings>();
            For<IEventsSettings>().Use<EventsSettings>();
            For<IMonitoringService>().Use<MonitoringService>();
            For<IRoatpApiClient>().Use<RoatpApiClient>();
            For<IRoatpMapper>().Use<RoatpMapper>();
        }
    }
}
